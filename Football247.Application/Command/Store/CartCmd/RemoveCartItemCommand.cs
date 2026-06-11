using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Command.Store.CartCmd
{
    public class RemoveCartItemCommand : IRequest<MethodResult<bool>>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }

    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveCartItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<bool>();

            // Join bảng để chắc chắn CartItem này thuộc về đúng User đang login (Tránh lỗi bảo mật IDOR)
            var cartItem = await _unitOfWork.CartItemRepository.Queryable
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == request.UserId && ci.ProductId == request.ProductId, cancellationToken);

            if (cartItem != null)
            {
                // ✅ XÓA MỀM THAY VÌ DELETE THẬT
                //cartItem.IsDeleted = true;
                await _unitOfWork.CartItemRepository.DeleteAsync(cartItem.Id);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataNotExist), nameof(cartItem), request.ProductId);
            }

                methodResult.Result = true;
            return methodResult;
        }
    }
}
