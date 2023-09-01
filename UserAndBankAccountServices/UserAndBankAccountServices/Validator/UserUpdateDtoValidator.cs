using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using UserAndBankAccountServices.Models.Dtos;

namespace UserAndBankAccountServices.Validator
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(user => user.Address)
                .NotEmpty().WithMessage("Address is required")
                .NotNull().WithMessage("Address is required")
                .MaximumLength(60).WithMessage("Address is to long");
        }
    }
}
