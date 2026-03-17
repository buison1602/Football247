using Football247.Authorization;
using Football247.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkFlowController : ControllerBase
    {
        private readonly ILogger<WorkFlowController> _logger;

        private readonly IWorkFlowService _workFlowService;

        public WorkFlowController(ILogger<WorkFlowController> logger, IWorkFlowService workFlowService)
        {
            _logger = logger;
            _workFlowService = workFlowService;
        }

        [HttpPut]
        [Authorize(Policy = Permissions.Articles.Approve)]
        public async Task<IActionResult> ApproveArticle(Guid articleId)
        {
            _logger.LogInformation($"Approving article with ID: {articleId}");

            try 
            {
                var articleDto = await _workFlowService.ApproveArticleAsync(articleId);
                if (articleDto == null)
                {
                    _logger.LogWarning($"Article with ID: {articleId} not found for approval.");
                    return NotFound($"Article with ID: {articleId} not found.");
                }

                return Ok($"Article {articleId} has been approved.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving article with ID: {articleId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the article.");
            }

        }
    }
}
