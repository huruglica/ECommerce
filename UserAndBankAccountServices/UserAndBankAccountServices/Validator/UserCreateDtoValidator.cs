using FluentValidation;
using FluentValidation.Validators;
using System.ComponentModel.DataAnnotations;
using UserAndBankAccountServices.Models.Dtos;

namespace UserAndBankAccountServices.Validator
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name is required")
                .NotNull().WithMessage("Name is required")
                .MaximumLength(15).WithMessage("Name is to long");

            RuleFor(user => user.Surname)
                .NotEmpty().WithMessage("Surname is required")
                .NotNull().WithMessage("Surname is required")
                .MaximumLength(15).WithMessage("Surname is to long");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .NotNull().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password is to short");

            RuleFor(user => user.Address)
                .NotEmpty().WithMessage("You must set an address")
                .NotNull().WithMessage("You must set an address")
                .MaximumLength(60).WithMessage("Address is to long");

            RuleFor(user => user.Email)
                .EmailAddress(EmailValidationMode.Net4xRegex)
                .NotEmpty().WithMessage("You must set an email address")
                .NotNull().WithMessage("You must set an email address")
                .MaximumLength(50).WithMessage("Email address is to long");
        }
    }
}
