using AutoMapper;
using Fun_Funding.Application.ViewModel.CartDTO;
using Fun_Funding.Application.ViewModel.MarketplaceProjectDTO;
using Fun_Funding.Domain.Entity;
using Fun_Funding.Domain.Entity.NoSqlEntities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Application.Resolver
{
    public class CartItemResolver : IValueResolver<Cart, CartInfoResponse, List<ItemInfoResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CartItemResolver(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<ItemInfoResponse> Resolve(Cart source, CartInfoResponse destination, List<ItemInfoResponse> destMember, ResolutionContext context)
        {
            var items = new List<ItemInfoResponse>();

            foreach (var bsonItem in source.Items)
            {
                if (bsonItem.TryGetValue("marketplaceProjectId", out BsonValue projectIdValue) && projectIdValue.IsGuid)
                {
                    Guid projectId = projectIdValue.AsGuid;

                    MarketplaceProject projectInfo = _unitOfWork.MarketplaceRepository.GetById(projectId);

                    if (projectInfo != null)
                    {
                        items.Add(new ItemInfoResponse
                        {
                            MarketplaceProject = _mapper.Map<MarketplaceProjectInfoResponse>(projectInfo),
                            CreatedDate = bsonItem.TryGetValue("createdDate", out BsonValue createdDateValue) && createdDateValue.IsValidDateTime
                                ? createdDateValue.ToLocalTime()
                                : null,
                        });
                    }
                }
            }

            return items;
        }
    }

}
