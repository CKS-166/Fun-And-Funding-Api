using AutoMapper;
using Fun_Funding.Application.ViewModel.BankAccountDTO;
using Fun_Funding.Application.ViewModel.CategoryDTO;
using Fun_Funding.Application.ViewModel.FundingFileDTO;
using Fun_Funding.Application.ViewModel.FundingProjectDTO;
using Fun_Funding.Application.ViewModel.PackageDTO;
using Fun_Funding.Application.ViewModel.RewardItemDTO;
using Fun_Funding.Domain.Entity;

namespace Fun_Funding.Infrastructure.Mapper
{
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {
            MappingFunfingProject();
        }
        public void MappingFunfingProject()
        {
            CreateMap<FundingFileRequest, FundingFile>().ReverseMap();

            CreateMap<ItemAddRequest, RewardItem>().ReverseMap();

            CreateMap<PackageAddRequest, Package>()
                .ForMember(des => des.RewardItems, src => src.MapFrom(x => x.RewardItems)).ReverseMap();

            CreateMap<BankAccountRequest, BankAccount>();

            CreateMap<FundingProjectAddRequest, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages)).ReverseMap();

            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<CategoryRequest, Category>().ReverseMap();
        }
    }


}
