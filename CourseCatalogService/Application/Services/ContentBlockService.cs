using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;

namespace CourseCatalogService.Application.Services
{
    public class ContentBlockService(IContentBlockRepository repository, IBlobStorageService blobStorage) : IContentBlockService
    {
        private readonly IContentBlockRepository _repository = repository;
        private readonly IBlobStorageService _blobStorage = blobStorage;

        public async Task<IEnumerable<ContentBlockResponseDto>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            var blocks = await _repository.GetByLessonIdAsync(lessonId, cancellationToken);
            return blocks.Select(WithFreshSas);
        }

        public async Task<ContentBlockResponseDto> AddAsync(int lessonId, ContentBlockRequestDto dto, CancellationToken cancellationToken = default)
        {
            Validate(dto);

            var block = new ContentBlock
            {
                LessonId = lessonId,
                Type = dto.Type,
                Order = dto.Order,
                TextContent = dto.TextContent,
                VideoUrl = dto.VideoUrl,
                FileUrl = dto.FileUrl,
                Metadata = dto.Metadata
            };

            await _repository.AddAsync(block, cancellationToken);
            return ToDto(block);
        }

        public async Task<ContentBlockResponseDto?> UpdateAsync(int id, ContentBlockRequestDto dto, CancellationToken cancellationToken = default)
        {
            var block = await _repository.GetByIdAsync(id, cancellationToken);
            if (block == null) return null;

            Validate(dto);

            block.Type = dto.Type;
            block.Order = dto.Order;
            block.TextContent = dto.TextContent;
            block.VideoUrl = dto.VideoUrl;
            block.FileUrl = dto.FileUrl;
            block.Metadata = dto.Metadata;

            await _repository.UpdateAsync(block, cancellationToken);
            return ToDto(block);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var block = await _repository.GetByIdAsync(id, cancellationToken);
            if (block == null) return false;

            await _repository.DeleteAsync(block, cancellationToken);
            return true;
        }

        public async Task ReorderAsync(int lessonId, List<int> orderedBlockIds, CancellationToken cancellationToken = default)
        {
            var blocks = (await _repository.GetByLessonIdAsync(lessonId, cancellationToken)).ToList();

            for (int i = 0; i < orderedBlockIds.Count; i++)
            {
                var block = blocks.FirstOrDefault(b => b.Id == orderedBlockIds[i]);
                if (block != null)
                    block.Order = i;
            }

            await _repository.UpdateRangeAsync(blocks, cancellationToken);
        }

        private ContentBlockResponseDto WithFreshSas(ContentBlock cb) => new()
        {
            Id = cb.Id,
            LessonId = cb.LessonId,
            Type = cb.Type,
            Order = cb.Order,
            TextContent = cb.TextContent,
            VideoUrl = cb.VideoUrl != null ? _blobStorage.RefreshSasUrl(cb.VideoUrl, "videos") : null,
            FileUrl = cb.FileUrl != null ? _blobStorage.RefreshSasUrl(cb.FileUrl, "documents") : null,
            Metadata = cb.Metadata
        };

        private static void Validate(ContentBlockRequestDto dto)
        {
            if (dto.Type == ContentBlockType.Video && dto.VideoUrl == null)
                throw new ArgumentException("A Video block must have a VideoUrl.");
            if (dto.Type == ContentBlockType.File && dto.FileUrl == null)
                throw new ArgumentException("A File block must have a FileUrl.");
        }

        private static ContentBlockResponseDto ToDto(ContentBlock cb) => new()
        {
            Id = cb.Id,
            LessonId = cb.LessonId,
            Type = cb.Type,
            Order = cb.Order,
            TextContent = cb.TextContent,
            VideoUrl = cb.VideoUrl,
            FileUrl = cb.FileUrl,
            Metadata = cb.Metadata
        };
    }
}
