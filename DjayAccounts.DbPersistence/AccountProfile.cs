using AutoMapper;
using DjayAccounts.DbPersistence.ObjectModels;
using DjayAccounts.EntityFramework.Entities;

namespace DjayAccounts.DbPersistence;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountModel>()
            .Include<Account, CurrentAccountModel>()
            .Include<Account, SavingsAccountModel>()
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => Enum.Parse<AccountType>(src.AccountType, true)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<AccountStatus>(src.Status, true)));

#warning do not use ?? 0m
        CreateMap<Account, CurrentAccountModel>()
            .ForMember(dest => dest.OverdraftLimit, opt => opt.MapFrom(src => src.OverdraftLimit ?? 0m));

        CreateMap<Account, SavingsAccountModel>()
            .ForMember(dest => dest.InterestRate, opt => opt.MapFrom(src => src.InterestRate ?? 0m));

        CreateMap<Account, AccountModel>().ConvertUsing((src, dest, context) =>
        {
            var accountType = Enum.Parse<AccountType>(src.AccountType, true);
            return accountType switch
            {
                AccountType.Current => context.Mapper.Map<CurrentAccountModel>(src),
                AccountType.Savings => context.Mapper.Map<SavingsAccountModel>(src),
                _ => throw new ArgumentOutOfRangeException($"Unknown account type: {src.AccountType}")
            };
        });
    }
}