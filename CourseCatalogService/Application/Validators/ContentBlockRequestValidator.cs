using CourseCatalogService.Application.Models.DTOs;
using CourseCatalogService.Infrastructure.Entities;
using FluentValidation;

namespace CourseCatalogService.Application.Validators
{
    public class ContentBlockRequestValidator : AbstractValidator<ContentBlockRequestDto>
    {
        public ContentBlockRequestValidator()
        {
            RuleFor(x => x.VideoUrl)
                .NotEmpty()
                .When(x => x.Type == ContentBlockType.Video)
                .WithMessage("A Video block must have a VideoUrl.");

            RuleFor(x => x.FileUrl)
                .NotEmpty()
                .When(x => x.Type == ContentBlockType.File)
                .WithMessage("A File block must have a FileUrl.");
        }
    }
}
