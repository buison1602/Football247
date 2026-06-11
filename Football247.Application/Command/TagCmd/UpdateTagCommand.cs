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
    public class UpdateTagCommand : UpdateTagCommandModel, IRequest<MethodResult<TagDto>>
    {
    }

    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, MethodResult<TagDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTagCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<TagDto>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
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

            var category = await _unitOfWork.TagRepository.GetByIdAsync(request.Id);

            if (category == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }
            #endregion

            var tagDomain = _mapper.Map<Tag>(request);
            await _unitOfWork.TagRepository.UpdateAsync(request.Id, tagDomain);
            await _unitOfWork.SaveAsync();

            methodResult.Result = _mapper.Map<TagDto>(tagDomain);
            return methodResult;
        }
    }
}
