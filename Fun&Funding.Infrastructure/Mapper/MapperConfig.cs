using AutoMapper;
using Fun_Funding.Application.ViewModel.BankAccountDTO;
using Fun_Funding.Application.ViewModel.CategoryDTO;
using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using Fun_Funding.Application.ViewModel.PackageDTO;
using Fun_Funding.Application.ViewModel.RewardItemDTO;
using Fun_Funding.Application.ViewModel.UserDTO;
using Fun_Funding.Application.ViewModel.WalletDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;

namespace Fun_Funding.Infrastructure.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig() {
            MappingFundingProject();
            MappingUser();
            MappingWallet();
        }
        public void MappingFundingProject()
        {
            CreateMap<FundingFileRequest, FundingFile>().ReverseMap();
            CreateMap<ItemAddRequest, RewardItem>().ReverseMap();
            CreateMap<PackageAddRequest, Package>()
                .ForMember(des => des.RewardItems, src => src.MapFrom(x => x.RewardItems)).ReverseMap();
            CreateMap<BankAccountRequest, BankAccount>().ReverseMap();
            CreateMap<PackageUpdateRequest, Package>().ReverseMap();
            CreateMap<ItemUpdateRequest, RewardItem>().ReverseMap();
            CreateMap<FundingFileResponse, FundingFile>().ReverseMap();
            CreateMap<FundingFileUpdateRequest, FundingFile>().ReverseMap();
            CreateMap<ItemResponse,RewardItem>().ReverseMap();
            CreateMap<PackageResponse, Package>().ReverseMap();
            CreateMap<FundingProjectAddRequest, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages)).ReverseMap();
            CreateMap<FundingProjectResponse, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))

                .ForMember(des =>des.SourceFiles, src => src.MapFrom(x => x.FundingFiles))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages)).ReverseMap();            
            CreateMap<FundingProjectUpdateRequest, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages))
                .ReverseMap(); ;

            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<CategoryRequest, Category>().ReverseMap();
        }
        
        public void MappingUser()
        {
            // UserUpdateRequest -> User
            CreateMap<UserUpdateRequest, User>()
                .ReverseMap();

            // UserInfoResponse -> User
            CreateMap<UserInfoResponse, User>()
                .ForPath(des => des.File.URL, src => src.MapFrom(x => x.Avatar))
                .ForMember(des => des.Wallet, src => src.MapFrom(x => x.Wallet))
                .ReverseMap();
        }

        public void MappingWallet()
        {
            //WalletInfoResponse -> Wallet
            CreateMap<WalletInfoResponse, Wallet>()
                .ReverseMap();
        }
    }
}
