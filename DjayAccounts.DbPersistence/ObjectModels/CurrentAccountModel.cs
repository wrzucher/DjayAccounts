namespace DjayAccounts.DbPersistence.ObjectModels;

/// <summary>
/// Represents a current account in the business layer.
/// </summary>
public class CurrentAccountModel : AccountModel
{
    /// <summary>
    /// Gets or sets a value that defines the overdraft limit for this current account.
    /// </summary>
    public decimal OverdraftLimit { get; set; }
}