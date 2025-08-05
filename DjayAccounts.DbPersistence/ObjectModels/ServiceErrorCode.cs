namespace DjayAccounts.DbPersistence.ObjectModels;

/// <summary>
/// Represents possible outcomes of business operations.
/// </summary>
public enum ServiceErrorCode
{
    Ok = 0,

    // Generic
    UnknownError = 1,
    ValidationFailed = 2,

    // Customer related
    CustomerNotFound = 10,
    CustomerAlreadyExists = 11,

    // Account related
    AccountNotFound = 20,
    AccountAlreadyExists = 21,
    AccountTypeNotAllowed = 22,
    AccountAlreadyFrozen = 23,
    AccountNotFrozen = 24,
    AccountClosed = 25,

    // Business rules
    InsufficientFunds = 30,
    OverdraftNotAllowed = 31,
    CurrencyMismatch = 32
}