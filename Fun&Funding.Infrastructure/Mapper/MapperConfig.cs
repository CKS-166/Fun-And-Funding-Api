﻿using AutoMapper;
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
            MappingFundingProject();
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
            CreateMap<FundingProjectAddRequest, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages)).ReverseMap();
            CreateMap<FundingProjectResponse, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages)).ReverseMap();
            CreateMap<FundingProject, FundingProjectResponse>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages))
                .ForMember(des => des.Email, src => src.MapFrom(x => x.User.Email))
                .ForMember(des => des.FundingFiles, src => src.MapFrom(x => x.SourceFiles)).ReverseMap();

            CreateMap<FundingProjectUpdateRequest, FundingProject>()
                .ForMember(des => des.BankAccount, src => src.MapFrom(x => x.BankAccount))
                .ForMember(des => des.Packages, src => src.MapFrom(x => x.Packages)).ReverseMap();

            CreateMap<Category, CategoryResponse>().ReverseMap();
            CreateMap<CategoryRequest, Category>().ReverseMap();
        }
    }


}
