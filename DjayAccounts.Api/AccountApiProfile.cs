using AutoMapper;
using DjayAccounts.Api.Models;
using DjayAccounts.DbPersistence.ObjectModels;

namespace DjayAccounts.Api;

public class AccountApiProfile : Profile
{
    public AccountApiProfile()
    {
        CreateMap<PaginatedResult<CustomerModel>, PaginatedResult<CustomerDto>>();
        CreateMap<CustomerModel, CustomerDto>();

        CreateMap<AccountModel, AccountDto>()
            .Include<AccountModel, CurrentAccountDto>()
            .Include<AccountModel, SavingsAccountDto>();

        CreateMap<AccountModel, CurrentAccountDto>();
        CreateMap<AccountModel, SavingsAccountDto>();
    }
}