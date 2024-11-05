using AutoMapper;
using Fun_Funding.Application.ExceptionHandler;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class PackageBackerService : IPackageBackerService
    {
        private IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;
        private IMapper _mapper;
        private readonly IUserService _userService;
        public PackageBackerService(IUnitOfWork unitOfWork, ITransactionService transactionService, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _transactionService = transactionService;
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<ResultDTO<PackageBackerResponse>> DonateFundingProject(PackageBackerRequest packageBackerRequest)
        {
            if (packageBackerRequest == null)
                return ResultDTO<PackageBackerResponse>.Fail("Invalid request data");

            try
            {
               var authorUser = _userService.GetUserInfo().Result;
                User mapUser = _mapper.Map<User>(authorUser._data);
                
                User user = _unitOfWork.UserRepository.GetById(mapUser.Id);
                if (authorUser is null)
                    return ResultDTO<PackageBackerResponse>.Fail("can not found user");
                
                var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageBackerRequest.PackageId);
                if (package == null)
                    return ResultDTO<PackageBackerResponse>.Fail("Package not found!");

                var wallet = await _unitOfWork.WalletRepository.GetAsync(w => w.Backer.Id == user.Id);
                if (wallet == null)
                    return ResultDTO<PackageBackerResponse>.Fail("Wallet not found");

                if (package.LimitQuantity == 0 && package.PackageTypes.Equals(PackageType.FixedPackage))
                    return ResultDTO<PackageBackerResponse>.Fail("Package is currently out of quantity!");

                if (_unitOfWork.FundingProjectRepository.GetById(package.ProjectId).Status != ProjectStatus.Processing)
                    return ResultDTO<PackageBackerResponse>.Fail("Project is currently cannot be donated to!");

                if (package.PackageTypes.Equals(PackageType.FixedPackage))
                    packageBackerRequest.DonateAmount = package.RequiredAmount;
                else
                {
                    if (packageBackerRequest.DonateAmount <= 0)
                        return ResultDTO<PackageBackerResponse>.Fail("Invalid donate amount");
                }
                //add project balance
                var project = _unitOfWork.FundingProjectRepository.GetById(package.ProjectId);
                project.Balance += packageBackerRequest.DonateAmount;
                _unitOfWork.FundingProjectRepository.Update(project);
                // add donation
                var packageBacker = new PackageBacker
                {
                    UserId = user.Id,
                    PackageId = packageBackerRequest.PackageId,
                    User = user,
                    Package = package,
                    DonateAmount = packageBackerRequest.DonateAmount,
                    IsHidden = false,
                    CreatedDate = DateTime.Now
                };
                // add donation amount to project wallet
                var projectWallet = _unitOfWork.WalletRepository.GetQueryable()
                    .Include(w => w.FundingProject)
                    .FirstOrDefault(w => w.FundingProject.Id == package.ProjectId);

                projectWallet.Balance += packageBackerRequest.DonateAmount;
                if (wallet.Balance > 0)
                {
                    wallet.Balance -= packageBackerRequest.DonateAmount;
                }
                else
                {
                    throw new ExceptionError((int)HttpStatusCode.BadRequest, "Backer wallet is not enough money for donation! Please charge more");
                }
               
                Package donatedPack = _unitOfWork.PackageRepository.GetById(packageBackerRequest.PackageId);
                donatedPack.LimitQuantity -= 1;
                await _unitOfWork.PackageBackerRepository.AddAsync(packageBacker);
                _unitOfWork.PackageRepository.Update(donatedPack);
                _unitOfWork.WalletRepository.Update(projectWallet);
                _unitOfWork.WalletRepository.Update(wallet);
                // add transaction
                var description = $"Donation to package: {package.Name}";
                await _transactionService.CreateTransactionAsync(
                    totalAmount: packageBackerRequest.DonateAmount,
                    description: description,
                    transactionType: TransactionTypes.PackageDonation,
                    packageId: packageBackerRequest.PackageId,
                    walletId: user.Wallet.Id
                );

                await _unitOfWork.CommitAsync();

                //var response = new PackageBackerResponse
                //{

                //};

                return ResultDTO<PackageBackerResponse>.Success(null, "Donation successfully added!");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ResultDTO<PackageBackerResponse>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ResultDTO<List<DonationResponse>>> ViewDonationById(Guid id)
        {
            try
            {
                var listById = _unitOfWork.PackageBackerRepository.GetQueryable()
                    .Include(x => x.Package)
                    .Include(x => x.User)
                    .Where(x => x.UserId == id)
                    .ToList();
                if (listById is null)
                {
                    return ResultDTO<List<DonationResponse>>.Fail("There are no donation found with this id");
                }

                var response = listById.Select(x => new DonationResponse
                {
                    UserName = x.User.FullName,
                    CreateDate = x.CreatedDate,
                    DonateAmount = x.DonateAmount,
                    PackageName = x.Package.Name,
                    Types = x.Package.PackageTypes,

                }).ToList();

                return ResultDTO<List<DonationResponse>>.Success(response, "donation by id");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
