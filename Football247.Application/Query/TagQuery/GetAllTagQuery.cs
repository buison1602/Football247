using Football247.Domain.Models.EntityModels.DTOs.Tag;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.TagQuery
{
    public class GetAllTagQueryModel : BaseQueryModel
    {
    }

    public class GetAllTagQuery : GetAllTagQueryModel, IRequest<MethodResult<PagingItemsModel<TagDto>>>
    {
    }

    public class GetAllTagQueryHandler : IRequestHandler<GetAllTagQuery, MethodResult<PagingItemsModel<TagDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllTagQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<PagingItemsModel<TagDto>>> Handle(GetAllTagQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<TagDto>>();
            var query = _unitOfWork.TagRepository.ReadQueryable
                .OrderByDescending(c => c.CreatedDate);

            var totalItems = await query.CountAsync(cancellationToken);

            var tagDtos = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new TagDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                })
                .ToListAsync(cancellationToken);

            methodResult.Result = new PagingItemsModel<TagDto>(tagDtos, request, totalItems);
            return methodResult;
        }
    }
}
