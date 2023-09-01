using FluentValidation;
using UserAndBankAccountServices.Model.Dtos;

namespace BankAccountService.Validator
{
    public class BankAccountDtoValidator : AbstractValidator<BankAccountDto>
    {
        public BankAccountDtoValidator()
        {
            RuleFor(bankAccount => bankAccount.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("There is no negative amount");
        }
    }
}
