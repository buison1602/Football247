using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Category;
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

namespace Football247.Application.Query.CategoryQuery
{
    public class GetAllCategoryQueryModel : BaseQueryModel
    {
    }

    public class GetAllCategoryQuery : GetAllCategoryQueryModel, IRequest<MethodResult<PagingItemsModel<CategoryDto>>>
    {
    }

    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, MethodResult<PagingItemsModel<CategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCategoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<PagingItemsModel<CategoryDto>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<CategoryDto>>();
            var query = _unitOfWork.CategoryRepository.ReadQueryable
                .OrderByDescending(c => c.CreatedDate);
            var totalItems = await query.CountAsync(cancellationToken);
            var categoryDtos = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                })
                .ToListAsync(cancellationToken);

            methodResult.Result = new PagingItemsModel<CategoryDto>(categoryDtos, request, totalItems);
            return methodResult;
        }
    }
}