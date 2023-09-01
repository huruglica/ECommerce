using FluentValidation;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Validator
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("Name is required")
                .NotNull().WithMessage("Name is required")
                .MaximumLength(20).WithMessage("Name to long");

            RuleFor(product => product.Stock)
                .NotEmpty().WithMessage("Stock is required")
                .NotNull().WithMessage("Stock is required")
                .GreaterThan(0).WithMessage("Stock must be greater than 0");

            RuleFor(product => product.BasePrice)
                .NotEmpty().WithMessage("BasePrice is required")
                .NotNull().WithMessage("BasePrice is required")
                .GreaterThan(0).WithMessage("BasePrice must be greater than 0");
        }
    }
}
