namespace DjayAccounts.DbPersistence.ObjectModels;

/// <summary>
/// Represents a savings account in the business layer.
/// </summary>
public class SavingsAccountModel : AccountModel
{
    /// <summary>
    /// Gets or sets a value that defines the interest rate for this savings account.
    /// </summary>
    public decimal InterestRate { get; set; }
}