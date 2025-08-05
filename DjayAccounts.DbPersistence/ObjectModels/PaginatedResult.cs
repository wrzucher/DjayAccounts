namespace DjayAccounts.DbPersistence.ObjectModels;

/// <summary>
/// Represents a paginated response with metadata about the result set.
/// </summary>
/// <param name="Page"> Gets or sets the current page number (1-based).</param>
/// <param name="PageSize"> Gets or sets the number of items per page.</param>
/// <param name="TotalCount"> Gets or sets the total number of items matching the query.</param>
/// <param name="Items"> Gets or sets the items contained in the current page.</param>
/// <typeparam name="T">The type of items contained in the result set.</typeparam>
public record PaginatedResult<T>(int Page, int PageSize, int TotalCount, IEnumerable<T> Items);
