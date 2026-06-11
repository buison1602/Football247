using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.Store.OrderCmd
{
    public class UpdateOrderStatusCommand : IRequest<MethodResult<bool>>
    {
        public Guid OrderId { get; set; }
        public EnumOrderStatuss Status { get; set; }
    }

    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<bool>();

            var order = await _unitOfWork.OrderRepository.Queryable
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, "NotFound", "Không tìm thấy đơn hàng");
                return methodResult;
            }

            order.Status = request.Status;
            await _unitOfWork.SaveAsync();

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
