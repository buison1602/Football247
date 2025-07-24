using AutoMapper;
using Football247.Models.DTOs.Comment;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services;
using Football247.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Football247.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRealtimeService _realtimeService;

        public CommentController(IUnitOfWork unitOfWork, IMapper mapper, IRealtimeService realtimeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _realtimeService = realtimeService;
        }

        [HttpGet("{articleId:guid}")]
        public async Task<IActionResult> GetCommentsByArticleId(Guid articleId)
        {
            var comments = await _unitOfWork.CommentRepository.GetCommentsByArticleIdAsync(articleId);

            var commentDtos = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CreatorId = c.CreatorId,
                CreatorName = c.Creator?.UserName ?? "Unknown",
                ArticleId = c.ArticleId
            });

            return Ok(commentDtos);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] AddCommentRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var comment = _mapper.Map<Comment>(request);
            comment.Id = Guid.NewGuid();
            comment.CreatedAt = DateTime.UtcNow;
            comment.CreatorId = userId;

            await _unitOfWork.CommentRepository.CreateAsync(comment);
            await _unitOfWork.SaveAsync();

            var savedComment = await _unitOfWork.CommentRepository
                .GetCommentsByArticleIdAsync(comment.ArticleId);

            var result = savedComment
                .FirstOrDefault(c => c.Id == comment.Id);

            var commentDto = _mapper.Map<CommentDto>(result);

            await _realtimeService.NotifyAllAsync("ReceiveComment", commentDto);

            return Ok(commentDto);
        }
    }
}
