using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.DbModel;

public class AuthData
{
    public int ServerId { get; set; }
    public int AccountId { get; set; }

    [ForeignKey(nameof(ServerId))]
    public virtual GameServer? Server { get; set; }

    [ForeignKey(nameof(AccountId))]
    public virtual Account? Account { get; set; }
    
    public int LoginKey1 { get; set; }
    public int LoginKey2 { get; set; }
    public int PlayKey1 { get; set; }
    public int PlayKey2 { get; set; }
}
