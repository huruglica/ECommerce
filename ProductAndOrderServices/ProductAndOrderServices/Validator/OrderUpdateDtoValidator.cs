using FluentValidation;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Validator
{
    public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
    {
        public OrderUpdateDtoValidator()
        {
            RuleFor(order => order.Address)
                .NotEmpty().WithMessage("Address is required")
                .NotNull().WithMessage("Address is required")
                .MaximumLength(60).WithMessage("Address is to long");
        }
    }
}
