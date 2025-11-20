using AutoMapper;
using Football247.Models.DTOs.Comment;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.Caching;
using Football247.Services.IService;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Football247.Services
{
    public class CommentService : ICommentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRealtimeService _realtimeService;
        private readonly IRedisCacheService _redisCacheService;
        private const string CacheKey = "commentes";
        public CommentService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IRealtimeService realtimeService,
            IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _realtimeService = realtimeService;
            _redisCacheService = redisCacheService;
        }
        public async Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId)
        {
            string cacheKeyForArticle = $"{CacheKey}-{articleId}";

            List<Comment>? comments = await _redisCacheService.GetDataAsync<List<Comment>>(cacheKeyForArticle);

            if (comments != null)
            {
                return _mapper.Map<List<CommentDto>>(comments);
            }

            comments = await _unitOfWork.CommentRepository.GetCommentsByArticleIdAsync(articleId);
            comments ??= new List<Comment>();

            await _redisCacheService.SetDataAsync(cacheKeyForArticle, comments);
            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<CommentDto> PostCommentAsync(AddCommentRequestDto addCommentRequestDto, string userId)
        {
            Comment commentDomain = _mapper.Map<Comment>(addCommentRequestDto);
            commentDomain.Id = Guid.NewGuid();
            commentDomain.CreatedAt = DateTime.UtcNow;
            commentDomain.CreatorId = userId;

            commentDomain = await _unitOfWork.CommentRepository.CreateAsync(commentDomain);

            if (commentDomain == null)
            {
                throw new InvalidOperationException("Failed to create the comment in the database.");
            }

            string cacheKeyForArticle = $"{CacheKey}-{addCommentRequestDto.ArticleId}";
            await _redisCacheService.RemoveDataAsync(cacheKeyForArticle);

            await _unitOfWork.SaveAsync();

            var commentDto = _mapper.Map<CommentDto>(commentDomain);

            await _realtimeService.NotifyAllAsync("ReceiveComment", commentDto);

            return commentDto;
        }
    }
}
