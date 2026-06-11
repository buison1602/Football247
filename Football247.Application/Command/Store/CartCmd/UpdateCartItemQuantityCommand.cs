using Football247.Repositories.IRepository;
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
    public class UpdateCartItemQuantityCommand : IRequest<MethodResult<bool>>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } // Số lượng chốt cuối cùng mà khách muốn (VD: 3, 4, hoặc 0)
    }

    public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCartItemQuantityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<bool>();

            // 1. Join bảng Cart và Product để lấy dữ liệu kiểm tra
            var cartItem = await _unitOfWork.CartItemRepository.ReadQueryable
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == request.UserId && ci.ProductId == request.ProductId, cancellationToken);

            if (cartItem == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, "ItemNotFound", "Sản phẩm không có trong giỏ hàng.");
                return methodResult;
            }

            // 2. Xử lý bẫy giảm số lượng về 0 hoặc âm -> Tự động chuyển thành Xóa mềm
            if (request.Quantity <= 0)
            {
                cartItem.IsDeleted = true;
                await _unitOfWork.CartItemRepository.UpdateAsync(cartItem.Id, cartItem);
                await _unitOfWork.SaveAsync();

                methodResult.Result = true;
                return methodResult;
            }

            // 3. Xử lý bẫy tồn kho: Số lượng khách muốn lớn hơn số lượng trong kho
            if (cartItem.Product != null && request.Quantity > cartItem.Product.Stock)
            {
                // Trả về lỗi để FE giật ngược số lượng lại mức tối đa cho phép
                methodResult.AddError(StatusCodes.Status400BadRequest, "OutOfStock", $"Kho chỉ còn {cartItem.Product.Stock} sản phẩm, không thể tăng thêm.");
                return methodResult;
            }

            // 4. Update số lượng thành công (Có thể cập nhật luôn giá mới nhất nếu muốn)
            cartItem.Quantity = request.Quantity;

            // Nếu nghiệp vụ yêu cầu đổi giá về giá mới nhất khi tăng/giảm thì mở comment dòng dưới:
            // cartItem.PriceAtTime = cartItem.Product?.SalePrice ?? cartItem.Product?.Price ?? cartItem.PriceAtTime;

            await _unitOfWork.CartItemRepository.UpdateAsync(cartItem.Id, cartItem);
            await _unitOfWork.SaveAsync();

            methodResult.Result = true;
            return methodResult;
        }
    }
}
