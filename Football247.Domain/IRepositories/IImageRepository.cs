using Football247.Domain.Models.EntityModels.DTOs.Image;
using Football247.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace Football247.Repositories.IRepository
{
    public interface IImageRepository : IRepository<Image>
    {
        Task<Image> UploadAsync(Image image, string ArticleSlug);

        Task<List<ImageDto>> CreateImageDto(List<IFormFile> BgrImgs, List<string> Captions,
            Guid ArticleDomainId, string ArticleDomainSlug);

        void ValidateFileUpload(List<IFormFile> BgrImgs, List<string> Captions);
    }
}
