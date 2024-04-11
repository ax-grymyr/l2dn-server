using BuildDataPackDb.Db;
using L2Dn.Packages.Textures;
using L2Dn.Packages.Unreal;
using NLog;
using SixLabors.ImageSharp;

namespace BuildDataPackDb.Services;

public class IconService
{
    private readonly Logger _logger = LogManager.GetLogger(nameof(DatabaseBuilder));
    private readonly FileLocationService _fileLocationService;
    private readonly DatabaseService _databaseService;

    private readonly Dictionary<string, Dictionary<IconKey, int>> _iconPackages =
        new(StringComparer.InvariantCultureIgnoreCase);
    
    public IconService(FileLocationService fileLocationService, DatabaseService databaseService)
    {
        _fileLocationService = fileLocationService;
        _databaseService = databaseService;
    }

    public void LoadIconPackage()
    {
        using DataPackDbContext ctx = _databaseService.CreateContext();
        string iconPath = Path.Combine(_fileLocationService.ClientPath, "SysTextures", "Icon.utx"); 
        LoadIconPackage(ctx, iconPath);
    }
    
    public int? GetIconId(string iconName, IconType iconType, int itemId)
    {
        if (string.Equals(iconName, "None", StringComparison.InvariantCultureIgnoreCase))
            return null;

        string[] parts = iconName.Split('.');
        string packageName = parts[0];
        if (!_iconPackages.TryGetValue(packageName, out Dictionary<IconKey, int>? icons))
        {
            string packageFileName = packageName + ".utx";
            string? packagePath = _fileLocationService.SearchClientFilePath(packageFileName);
            if (packagePath is null)
            {
                _logger.Warn($"Client package '{packageFileName}' not found, icon '{iconName}', item {itemId}");
                return null;
            }
            
            using DataPackDbContext ctx = _databaseService.CreateContext();
            icons = LoadIconPackage(ctx, _fileLocationService.GetClientFilePath(packageFileName));
        }
        
        bool isIconPackage = string.Equals(packageName, "icon", StringComparison.InvariantCultureIgnoreCase);
        if (!isIconPackage)
            iconType = IconType.None;

        if (icons.TryGetValue(new IconKey(iconName, iconType), out int iconId))
            return iconId;

        if (isIconPackage)
        {
            _logger.Trace($"No icon '{iconName}', type={iconType} found for item {itemId}, searching another types");
            if (SearchIcon(iconName, icons, itemId, out iconId))
                return iconId;
        }
        
        _logger.Warn($"Icon '{iconName}' not found for item {itemId}");
        return null;
    }

    private bool SearchIcon(string iconName, Dictionary<IconKey, int> icons, int itemId, out int iconId)
    {
        if (icons.TryGetValue(new IconKey(iconName, IconType.None), out iconId))
            return true;

        List<KeyValuePair<IconKey, int>> possibleIcons = icons.Where(pair =>
                string.Equals(pair.Key.Name, iconName, StringComparison.InvariantCultureIgnoreCase))
            .OrderBy(x => x.Key.Type).ToList();

        if (possibleIcons.Count == 1)
        {
            iconId = possibleIcons[0].Value;
            return true;
        }

        if (possibleIcons.Count > 1)
        {
            string names = string.Join(", ", possibleIcons.Select(x => x.Key.Name));
            _logger.Trace($"Possible icon matches with '{iconName}' for item {itemId}: {names}");
            iconId = possibleIcons[0].Value;
            return true;
        }

        return false;
    }
    
    private Dictionary<IconKey, int> LoadIconPackage(DataPackDbContext ctx, string packagePath)
    {
        _logger.Info($"Loading icons from '{packagePath}'...");

        string packageName = Path.GetFileNameWithoutExtension(packagePath);
        Dictionary<IconKey, int> icons = new Dictionary<IconKey, int>();
        _iconPackages.Add(packageName, icons);

        UPackageManager packageManager = new();
        UPackage package = UPackage.LoadFrom(packageManager, packagePath);
        IEnumerable<UTexture> textures = package.Exports.Where(x => x.Class?.Name == "Texture")
            .Select(x => (UTexture)x.Object);

        int counter = 0;
        Dictionary<IconKey, string> names = new Dictionary<IconKey, string>();
        List<DbIcon> dbIcons = new();
        using MemoryStream stream = new();

        foreach (UTexture texture in textures)
        {
            if (string.IsNullOrEmpty(texture.Lineage2Name))
                continue;

            string textureName = texture.Lineage2Name;
            string[] parts = textureName.Split('.');
            string iconPackageName = parts[0];
            IconType iconType = IconType.None;
            bool isIconPackage = string.Equals(iconPackageName, "icon", StringComparison.InvariantCultureIgnoreCase); 
            if (isIconPackage && parts.Length == 3)
            {
                // Remove middle part
                textureName = parts[0] + "." + parts[2];
                string iconTypeString = parts[1];
                iconType = ParseIconType(iconTypeString);
            }

            UBitmap? bitmap =
                texture.Bitmaps.SingleOrDefault(b => b is { Width: 64, Height: 64 }) ??
                texture.Bitmaps.OrderByDescending(b => b.Width).FirstOrDefault(b => b is { Width: <= 64, Height: <= 64 }) ??
                texture.Bitmaps.OrderBy(b => b.Width).FirstOrDefault(b => b is { Width: >= 64, Height: >= 64 });

            if (bitmap != null)
            {
                IconKey key = new IconKey(textureName, iconType);
                if (!names.TryAdd(key, texture.Lineage2Name))
                {
                    _logger.Warn($"Duplicated icon name '{textureName}' ('{texture.Lineage2Name}', " +
                                 $"previous '{names[key]}') in package '{packagePath}'");
                    
                    continue;
                }

                stream.SetLength(0);
                bitmap.Image?.SaveAsPng(stream);

                DbIcon dbIcon = new DbIcon()
                {
                    Name = textureName,
                    Type = iconType,
                    Extension = "png",
                    Height = bitmap.Height,
                    Width = bitmap.Width,
                    Bitmap = stream.ToArray(),
                };
                
                dbIcons.Add(dbIcon);
                ctx.Icons.Add(dbIcon);

                counter++;
            }
        }

        ctx.SaveChanges();
        _logger.Info($"{counter} icons saved");

        foreach (DbIcon dbIcon in dbIcons)
            icons.Add(new IconKey(dbIcon.Name, dbIcon.Type), dbIcon.IconId);
        
        return icons;
    }

    private static IconType ParseIconType(string middlePart) =>
        middlePart.ToLowerInvariant() switch
        {
            "accessary_i" => IconType.Accessary,
            "boots_i" => IconType.Boots,
            "etc_i" => IconType.Etc,
            "glove_i" => IconType.Gloves,
            "helmet_i" => IconType.Helmet,
            "lowbody_i" => IconType.LowerBody,
            "onepiece" => IconType.OnePiece,
            "shield_i" => IconType.Shield,
            "upbody_i" => IconType.UpperBody,
            "weapon_i" => IconType.Weapon,
            
            "action_i" => IconType.Action,
            "skill_i" => IconType.Skill,
            "wedding_i" => IconType.Wedding,
            
            "time_weapon" => IconType.TimeWeapon,
            
            "icon_panel" => IconType.IconPanel,
            "macrownd" => IconType.MacroWnd,
            "mticket" => IconType.MTicket,
            "quest" => IconType.Quest,
            _ => throw new ArgumentException("Invalid middle part of icon name")
        };

    private readonly struct IconKey(string name, IconType type): IEquatable<IconKey>
    {
        public string Name { get; } = name;
        public IconType Type { get; } = type;

        public bool Equals(IconKey other) =>
            StringComparer.InvariantCultureIgnoreCase.Equals(Name, other.Name) && Type == other.Type;

        public override bool Equals(object? obj) => obj is IconKey other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name), (int)Type);
    }
}