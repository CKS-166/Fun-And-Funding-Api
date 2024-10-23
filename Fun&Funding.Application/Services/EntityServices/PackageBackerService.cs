using AutoMapper;
using Fun_Funding.Application.IService;
using Fun_Funding.Application.ViewModel;
using Fun_Funding.Application.ViewModel.PackageBackerDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Services.EntityServices
{
    public class PackageBackerService : IPackageBackerService
    {
        private IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;
        private IMapper _mapper;

        public PackageBackerService(IUnitOfWork unitOfWork, ITransactionService transactionService)
        {
            _unitOfWork = unitOfWork;
            _transactionService = transactionService;
        }
        public async Task<ResultDTO<PackageBackerResponse>> DonateFundingProject(PackageBackerRequest packageBackerRequest)
        {
            if (packageBackerRequest == null)
                return ResultDTO<PackageBackerResponse>.Fail("Invalid request data");

            try
            {
                // Validations
                var user = await _unitOfWork.UserRepository.GetByIdAsync(packageBackerRequest.UserId);
                if (user == null)
                    return ResultDTO<PackageBackerResponse>.Fail("User not found!");

                var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageBackerRequest.PackageId);
                if (package == null)
                    return ResultDTO<PackageBackerResponse>.Fail("Package not found!");

                var wallet = await _unitOfWork.WalletRepository.GetAsync(w => w.Backer.Id == packageBackerRequest.UserId);
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

                // add donation
                var packageBacker = new PackageBacker
                {
                    UserId = packageBackerRequest.UserId,
                    PackageId = packageBackerRequest.PackageId,
                    User = user,
                    Package = package,
                    DonateAmount = packageBackerRequest.DonateAmount,
                    IsHidden = false
                };

                Package donatedPack = _unitOfWork.PackageRepository.GetById(packageBackerRequest.PackageId);
                donatedPack.LimitQuantity -= 1;
                await _unitOfWork.PackageBackerRepository.AddAsync(packageBacker);
                _unitOfWork.PackageRepository.Update(donatedPack);
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
