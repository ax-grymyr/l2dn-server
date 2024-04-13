using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.DataPack.Db;

public class DbIcon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IconId { get; set; }

    [MaxLength(400)]
    public string Name { get; set; } = string.Empty;
 
    public IconType Type { get; set; }
    
    public int Width { get; set; }
    public int Height { get; set; }

    [MaxLength(10)]
    public string Extension { get; set; } = string.Empty;
    
    public byte[] Bitmap { get; set; } = [];
}

public enum IconType
{
    None,
    
    Accessary, // accessary_i
    Boots, // boots_i
    Etc, // etc_i
    Gloves, // glove_i
    Helmet, // helmet_i
    LowerBody, // lowbody_i
    OnePiece, // onepiece
    Shield, // shield_i
    UpperBody, // upbody_i
    Weapon, // weapon_i
    
    Action, // action_i
    Skill, // skill_i
    Wedding, // wedding_i

    TimeWeapon, // time_weapon
    
    IconPanel, // icon_panel
    MacroWnd, // MacroWnd
    MTicket, // mticket
    Quest, // Quest
}