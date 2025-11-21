using AutoMapper;
using Wasalnyy.BLL.DTO.Update;
using Wasalnyy.BLL.DTO.Wallet;

namespace Wasalnyy.BLL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
         CreateMap<UpdateRider,Rider>().ReverseMap();
            CreateMap<UpdateDriver, Driver>().ReverseMap();

            // Wallet mappings
            CreateMap<Wallet, WalletDto>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionDto>()
                .ForMember(dest => dest.TransactionType,
                           opt => opt.MapFrom(src => src.TransactionType.ToString()));



        }
    }
}
