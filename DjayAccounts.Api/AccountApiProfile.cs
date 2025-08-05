using AutoMapper;
using DjayAccounts.Api.Models;
using DjayAccounts.DbPersistence.ObjectModels;

namespace DjayAccounts.Api;

/// <summary>
/// AutoMapper profile for AccountApi.
/// </summary>
public class AccountApiProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountApiProfile"/> class.
    /// </summary>
    public AccountApiProfile()
    {
        CreateMap<CustomerModel, CustomerDto>();

        CreateMap<AccountModel, AccountDto>()
            .Include<CurrentAccountModel, CurrentAccountDto>()
            .Include<SavingsAccountModel, SavingsAccountDto>();

        CreateMap<CurrentAccountModel, CurrentAccountDto>();
        CreateMap<SavingsAccountModel, SavingsAccountDto>();

        CreateMap(typeof(PaginatedResult<>), typeof(PaginatedResult<>));
    }
}