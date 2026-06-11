using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Repositories.IRepository;

namespace Football247.Services
{
    public class WorkFlowService
    {
        private readonly IUnitOfWork _unitOfWork;   
        private readonly IMapper _mapper;

        public WorkFlowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ArticleDto> ApproveArticleAsync(Guid articleId)
        {
            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(articleId);

            if (article == null) 
            {
                throw new Exception("Article not found");
            }

            article.IsApproved = true;
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ArticleDto>(article);
        }
    }
}
