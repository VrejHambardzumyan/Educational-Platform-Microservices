using CourseCatalogService.Application.Interfaces;
using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using CourseCatalogService.Infrastructure.Interfaces;

namespace CourseCatalogService.Application.Services
{
    public class RatingService(IRatingRepository ratingRepo, ICourseRepository courseRepo) : IRatingService
    {
        public async Task<RatingResponseDto> UpsertRatingAsync(int courseId, int userId, RatingRequestDto dto)
        {
            var existing = await ratingRepo.GetByUserAndCourseAsync(userId, courseId);

            if (existing is not null)
            {
                existing.Rating = dto.Rating;
                existing.Feedback = dto.Feedback;
                await ratingRepo.UpdateAsync(existing);
            }
            else
            {
                existing = new CourseRating
                {
                    CourseId = courseId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Feedback = dto.Feedback
                };
                await ratingRepo.AddAsync(existing);
            }

            var (average, count) = await ratingRepo.RecalculateAsync(courseId);
            var course = await courseRepo.GetCourseByIdAsync(courseId, default);
            if (course is not null)
            {
                course.AverageRating = average;
                course.RatingCount = count;
                await courseRepo.UpdateAsync(course);
            }

            return MapToDto(existing);
        }

        public async Task<CourseRatingSummaryDto?> GetCourseRatingsAsync(int courseId)
        {
            var course = await courseRepo.GetCourseByIdAsync(courseId, default);
            if (course is null) return null;

            var ratings = await ratingRepo.GetByCourseAsync(courseId);
            return new CourseRatingSummaryDto
            {
                AverageRating = course.AverageRating,
                RatingCount = course.RatingCount,
                Ratings = ratings.Select(MapToDto)
            };
        }

        public async Task<RatingResponseDto?> GetMyRatingAsync(int courseId, int userId)
        {
            var rating = await ratingRepo.GetByUserAndCourseAsync(userId, courseId);
            return rating is null ? null : MapToDto(rating);
        }

        private static RatingResponseDto MapToDto(CourseRating r) => new()
        {
            Id = r.Id,
            CourseId = r.CourseId,
            UserId = r.UserId,
            Rating = r.Rating,
            Feedback = r.Feedback,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}
