using AutoMapper;
using Financial.Api.Models;
using Financial.Domain.Entities;

namespace Financial.Api.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<UserInputModel, User>();

                config.CreateMap<WalletInputModel, Wallet>();

                config.CreateMap<TransactionInputModel, Transaction>()
                    .ForSourceMember(t => t.CategoryId, opt => opt.Ignore())
                    .ForSourceMember(t => t.CategoryName, opt => opt.Ignore());

                config.CreateMap<TransactionCategoryInputModel, TransactionCategory>();
            });
        }
    }
}