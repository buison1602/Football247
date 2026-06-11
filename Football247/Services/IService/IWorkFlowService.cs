using Football247.Domain.Models.EntityModels.DTOs.Article;

namespace Football247.Services.IService
{
    public interface IWorkFlowService
    {
        Task<ArticleDto> ApproveArticleAsync(Guid articleId);
    }
}
