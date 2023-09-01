using FluentValidation;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Validator
{
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(order => order.Address)
                .NotEmpty().WithMessage("You must set an address")
                .NotNull().WithMessage("You must set an address")
                .MaximumLength(60).WithMessage("Address is to long");

            RuleFor(order => order.ProductsCreateDto)
                .NotEmpty().WithMessage("You must select one product")
                .NotNull().WithMessage("You must select one product");
        }
    }
}
