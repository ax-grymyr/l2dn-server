using System.Text.Json;
using L2Dn.Collections;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Variables;

public abstract class AbstractVariables<T>: IStorable, IDeletable
    where T: DbVariable
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AbstractVariables<T>));
    private readonly PropertyDictionary _values = new(StringComparer.OrdinalIgnoreCase);

    public void Set<TValue>(string name, TValue value) => _values.Set(name, value);

    public TValue? Get<TValue>(string name) => _values.Get<TValue>(name);

    public TValue Get<TValue>(string name, TValue defaultValue) => _values.Get(name, defaultValue);

    public bool Remove(string name) => _values.Remove(name);

    public void RemoveAll(string namePrefix) => _values.RemoveAll(namePrefix);

    /**
     * Return true if there exists a record for the variable name.
     * @param name
     * @return
     */
    public bool ContainsKey(string name) => _values.ContainsKey(name);

    /**
     * @return {@code true} if changes are made since last load/save.
     */
    public bool HasChanges => _values.Changed;

    /**
     * Delete all entries for an requested var
     * @param var
     * @return success
     */
    public static bool DeleteVariable(string name)
    {
        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

            // Clear previous entries.
            ctx.Set<T>().Where(r => r.Name == name).ExecuteDelete();
        }
        catch (Exception e)
        {
            _logger.Error("AccountVariables: Couldn't delete variables: " + e);
            return false;
        }

        return true;
    }

    public bool deleteMe()
    {
        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

            // Clear previous entries.
            GetQuery(ctx).ExecuteDelete();

            // Clear all entries
            _values.Clear();
        }
        catch (Exception e)
        {
            _logger.Warn(GetType().Name + ": Couldn't delete variables: " + e);
            return false;
        }

        return true;
    }

    public bool storeMe()
    {
        // No changes, nothing to store.
        try
        {
            List<(string Name, string Text, PropertyState State)> itemsToStore = _values.ResetChanges();
            if (itemsToStore.Count == 0)
                return true;

            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            foreach ((string name, string text, PropertyState state) in itemsToStore)
            {
                switch (state)
                {
                    case PropertyState.New:
                    {
                        T variable = CreateVar();
                        variable.Name = name;
                        variable.Value = text;
                        ctx.Add(variable);
                        break;
                    }

                    case PropertyState.Modified:
                    {
                        T variable = CreateVar();
                        variable.Name = name;
                        variable.Value = text;
                        ctx.Update(variable);
                        break;
                    }

                    case PropertyState.Deleted:
                    {
                        GetQuery(ctx).Where(v => v.Name == name).ExecuteDelete();
                        break;
                    }
                }
            }

            ctx.SaveChanges();
        }
        catch (Exception exception)
        {
            _logger.Warn(GetType().Name + ": Couldn't update variables: " + exception);
            return false;
        }

        return true;
    }

    protected abstract IQueryable<T> GetQuery(GameServerDbContext ctx);
    protected abstract T CreateVar();

    protected void Restore()
    {
        // Restore previous variables.
        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            foreach (T record in GetQuery(ctx))
                _values.Set(record.Name, record.Value, PropertyState.Unchanged);
        }
        catch (Exception exception)
        {
            _logger.Error(GetType().Name + ": Couldn't restore variables: " + exception);
        }
    }
}