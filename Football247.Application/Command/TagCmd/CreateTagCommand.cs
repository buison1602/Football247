using AutoMapper;
using Football247.Domain.Models.CommandModels.TagCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Domain.Models.EntityModels.DTOs.Tag;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.TagCmd
{
    public class CreateTagCommand : CreateTagCommandModel, IRequest<MethodResult<TagDto>>
    {
    }

    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, MethodResult<TagDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;   

        public CreateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<TagDto>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<TagDto>();

            #region Validation
            if (string.IsNullOrEmpty(request.Name))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Name));
                return methodResult;
            }

            if (string.IsNullOrEmpty(request.Slug))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Slug));
                return methodResult;
            }

            var tagExist = await _unitOfWork.TagRepository.GetBySlugAsync(request.Slug);
            if (tagExist is not null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Slug), request.Slug);
                return methodResult;
            }
            #endregion

            var tag = _mapper.Map<Tag>(request);
            if (tag == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(tag));
                return methodResult;
            }
            var tagDomain = await _unitOfWork.TagRepository.CreateAsync(tag);
            if (tagDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(tag));
                return methodResult;
            }

            methodResult.StatusCode = StatusCodes.Status200OK;
            methodResult.Result = _mapper.Map<TagDto>(tagDomain);
            return methodResult;
        }
    }
}
