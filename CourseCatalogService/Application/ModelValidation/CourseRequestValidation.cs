using CourseCatalogService.Application.Models.DTOs;
using FluentValidation;

namespace CourseCatalogService.Application.ModelValidation
{
    public class CourseRequestValidator: AbstractValidator<CourseRequestDto>
    {
        public CourseRequestValidator() 
        { 
            RuleFor(CourseRequest => CourseRequest.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters long")
                .Matches("[a-zA-Z][a-zA-Z0-9_]")
                .WithMessage("Username must contain only letters, numbers, or underscores.");

            RuleFor(CourseRequest => CourseRequest.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(150).WithMessage("Description must be at most 150 characters long")
                .WithMessage("Description must contain only letters, numbers, or underscores.");

            RuleFor(CourseRequest => CourseRequest.DurationInMonth)
                .NotEmpty().WithMessage("Duration must be specified")
                .GreaterThan(0).WithMessage("Invalid duration, must be at least 1 month")
                .LessThanOrEqualTo(12).WithMessage("Duration can'texceed 1 year");

            RuleFor(CourseRequest => CourseRequest.Price)
                .NotEmpty().WithMessage("Price must be specified")
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Price too high");
        }
    }
}
