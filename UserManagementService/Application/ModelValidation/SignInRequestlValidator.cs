using FluentValidation;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Application.ModelValidation
{
    public class SignInRequestlValidator : AbstractValidator<SignInRequestDto>
    {
        public SignInRequestlValidator() 
        {
            RuleFor(signInRequest => signInRequest.UserName)
                .NotEmpty()
                .WithMessage("Invalid username");

            RuleFor(signInRequest => signInRequest.Password)
            .NotEmpty()
            .WithMessage("Invalid password");

        }
    }
}
