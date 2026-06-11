using Football247.Domain.Models.EntityModels.DTOs.Cart;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StoreQuery.CartQuery
{
    public class GetCartByUserIdQuery : IRequest<MethodResult<CartDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetCartByUserIdQueryHandler : IRequestHandler<GetCartByUserIdQuery, MethodResult<CartDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCartByUserIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<CartDto>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
        {
            // Luôn khởi tạo sẵn đối tượng rỗng để nếu giỏ trống, FE không bị crash khi map data
            var methodResult = new MethodResult<CartDto> { Result = new CartDto() };

            // 1. Kéo dữ liệu Giỏ hàng kèm theo Item và Product.
            // EF Core sẽ tự động áp dụng Global Query Filter (IsDeleted = false) nên ta không lo bốc nhầm đồ đã bị xóa mềm.
            var cart = await _unitOfWork.CartRepository.ReadQueryable
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId && !c.IsDeleted, cancellationToken);

            // Nếu khách chưa có giỏ hoặc giỏ trống -> Trả về DTO rỗng, FE sẽ hiện UI "Giỏ hàng trống"
            if (cart == null || !cart.CartItems.Any())
            {
                return methodResult;
            }

            methodResult.Result.Id = cart.Id;

            // 2. Tính toán và Map dữ liệu
            foreach (var item in cart.CartItems)
            {
                // Kiểm tra xem sản phẩm còn đang được bán không (IsActive = true)
                bool isProductActive = item.Product != null && item.Product.IsActive;

                var itemDto = new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Sản phẩm không khả dụng",
                    ProductThumbnail = item.Product?.ThumbnailUrl,
                    Price = item.Product?.Price ?? 0,
                    SalePrice = item.Product?.SalePrice,
                    PriceAtTime = item.PriceAtTime,
                    Quantity = item.Quantity,

                    // Tính thành tiền dựa trên giá lúc thêm vào. 
                    // Tùy nghiệp vụ: Nếu công ty muốn ép khách mua theo giá mới nhất thì thay bằng SalePrice/Price
                    SubTotal = item.PriceAtTime * item.Quantity,

                    // Nếu sản phẩm ngừng kinh doanh -> Báo tồn kho = 0 để FE khóa nút Thanh toán
                    StockAvailable = isProductActive ? item.Product!.Stock : 0
                };

                methodResult.Result.Items.Add(itemDto);

                // Cộng dồn tổng hóa đơn
                methodResult.Result.TotalItems += item.Quantity;
                methodResult.Result.TotalAmount += itemDto.SubTotal;
            }

            // Sắp xếp lại danh sách sản phẩm theo thứ tự mới thêm lên đầu (Tùy chọn)
            methodResult.Result.Items = methodResult.Result.Items.OrderByDescending(x => x.Id).ToList();

            return methodResult;
        }
    }
}
