using CourseCatalogService.Application.Models.DTOs;
using FluentValidation;

namespace CourseCatalogService.Application.ModelValidation
{
    public class CourseRequestValidator : AbstractValidator<CourseRequestDto>
    {
        public CourseRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters long")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters long")
                .Matches(@"^[a-zA-Z][a-zA-Z0-9_ ]*$")
                .WithMessage("Title must start with a letter and contain only letters, numbers, underscores, or spaces.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description must be at most 2000 characters long");

            RuleFor(x => x.DurationInMonth)
                .GreaterThan(0).WithMessage("Duration must be at least 1 month")
                .LessThanOrEqualTo(24).WithMessage("Duration can't exceed 2 years");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Price too high");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Category must be at most 100 characters")
                .When(x => x.Category != null);
        }
    }
}
