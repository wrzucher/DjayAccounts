using AutoMapper;
using DjayAccounts.Api.Models;
using DjayAccounts.DbPersistence.ObjectModels;

namespace DjayAccounts.Api;

public class AccountApiProfile : Profile
{
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