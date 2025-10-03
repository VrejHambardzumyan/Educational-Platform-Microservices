using FluentValidation;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Application.ModelValidation
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequestDto>
    {
        public SignUpRequestValidator()
        {
            RuleFor(signUpRequest => signUpRequest.UserName)
                   .NotEmpty().WithMessage("Username is required")
                   .Matches("^[a-zA-Z][a-zA-Z0-9_]{2,29}$")
                   .WithMessage("Username must be 3–30 chars, start with a letter, and contain only letters, numbers, or underscores.");

            RuleFor(signUpRequest => signUpRequest.Password)
                   .NotEmpty().WithMessage("Username is required")
                   .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                   .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                   .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                   .Matches("[0-9]").WithMessage("Password must contain at least one number")
                   .Matches("[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*)");

            RuleFor(signUpRequest => signUpRequest.Email)
                   .NotEmpty().WithMessage("Email is required")
                   .EmailAddress().WithMessage("Email must be a valid email address")
                   .MaximumLength(150).WithMessage("Email must be at most 150 characters long");
        }
    }
}
