using Football247.Domain.Entities.Stores;
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
    public class AddCartItemCommand : IRequest<MethodResult<bool>>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } // Số lượng thêm vào (vd: +1, +2)
    }

    public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCartItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<bool>();

            if (request.Quantity <= 0)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, "InvalidQuantity", "Số lượng phải lớn hơn 0");
                return methodResult;
            }

            // 1. Kiểm tra sản phẩm còn bán không và tồn kho thực tế
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
            if (product == null || !product.IsActive)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, "ProductNotAvailable", "Sản phẩm không tồn tại hoặc đã ngừng bán.");
                return methodResult;
            }

            // Tính giá hiện tại (Ưu tiên SalePrice)
            var currentPrice = product.SalePrice ?? product.Price;

            // 2. Tìm Giỏ hàng của User. Nếu chưa có -> Tạo mới (Lazy Creation)
            var cart = await _unitOfWork.CartRepository.ReadQueryable
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

            if (cart == null)
            {
                cart = new Cart { UserId = request.UserId };
                cart = await _unitOfWork.CartRepository.CreateAsync(cart);
            }

            // 3. Xử lý CartItem (Bao gồm case hồi sinh Xóa mềm)
            var existingItem = await _unitOfWork.CartItemRepository.ReadQueryable
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == request.ProductId, cancellationToken);

            if (existingItem != null)
            {
                if (existingItem.IsDeleted == false)
                {
                    // Case: Đang có trong giỏ -> CỘNG DỒN
                    var newQuantity = existingItem.Quantity + request.Quantity;
                    if (newQuantity > product.Stock)
                    {
                        methodResult.AddError(StatusCodes.Status400BadRequest, "OutOfStock", $"Bạn đã có {existingItem.Quantity} sản phẩm trong giỏ. Kho chỉ còn {product.Stock} chiếc, không thể thêm nữa.");
                        return methodResult;
                    }
                    existingItem.Quantity = newQuantity;
                    // Tùy nghiệp vụ: Có cập nhật lại PriceAtTime khi khách +1 không? Thường là CÓ để khách nhận giá mới.
                    existingItem.PriceAtTime = currentPrice;
                }
                await _unitOfWork.CartItemRepository.UpdateAsync(existingItem.Id, existingItem);
            }
            else
            {
                // Case: Thêm mới hoàn toàn
                if (request.Quantity > product.Stock)
                {
                    methodResult.AddError(StatusCodes.Status400BadRequest, "OutOfStock", $"Chỉ còn {product.Stock} sản phẩm trong kho.");
                    return methodResult;
                }

                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    PriceAtTime = currentPrice
                };
                await _unitOfWork.CartItemRepository.CreateAsync(newItem);
            }

            await _unitOfWork.SaveAsync();
            methodResult.Result = true;
            return methodResult;
        }
    }
}
