using AutoMapper;
using Football247.Models.DTOs.Comment;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
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
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "commentes";
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, IRealtimeService realtimeService,
             IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _realtimeService = realtimeService;
            _memoryCache = memoryCache;
        }
        public async Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId)
        {
            List<Comment>? comments;

            string cacheKeyForArticle = $"{CacheKey}-{articleId}";

            if (_memoryCache.TryGetValue(cacheKeyForArticle, out List<Comment>? data))
            {
                comments = data;
            }
            else
            {
                comments = await _unitOfWork.CommentRepository.GetCommentsByArticleIdAsync(articleId);
                if (comments == null || !comments.Any())
                {
                    comments = new List<Comment>();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                _memoryCache.Set(CacheKey, comments, cacheEntryOptions);
            }

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
            _memoryCache.Remove(cacheKeyForArticle);

            await _unitOfWork.SaveAsync();

            var commentDto = _mapper.Map<CommentDto>(commentDomain);

            await _realtimeService.NotifyAllAsync("ReceiveComment", commentDto);

            return commentDto;
        }
    }
}
