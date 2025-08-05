using AutoMapper;
using DjayAccounts.Api.Models;
using DjayAccounts.DbPersistence.ObjectModels;
using DjayAccounts.EntityFramework.Entities;

namespace DjayAccounts.Api;

public class AccountApiProfile : Profile
{
    public AccountApiProfile()
    {
        CreateMap<CustomerModel, CustomerDto>();

        CreateMap<AccountModel, AccountDto>()
            .Include<AccountModel, CurrentAccountDto>()
            .Include<AccountModel, SavingsAccountDto>();

        CreateMap<AccountModel, CurrentAccountDto>();
        CreateMap<AccountModel, SavingsAccountModel>();
    }
}