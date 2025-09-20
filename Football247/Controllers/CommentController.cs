using Football247.Models.DTOs.Comment;
using Football247.Models.Entities;
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
        private readonly ICommentService _commentService;

        public CommentController(IRealtimeService realtimeService, ICommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpGet("{articleId:guid}")]
        public async Task<IActionResult> GetCommentsByArticleId(Guid articleId)
        {
            List<CommentDto> commentsDto = await _commentService.GetCommentsByArticleIdAsync(articleId);

            return Ok(commentsDto);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] AddCommentRequestDto addCommentRequestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            CommentDto createdCommentDto = await _commentService.PostCommentAsync(addCommentRequestDto, userId);

            return Ok(createdCommentDto);
        }
    }
}
