using FluentValidation;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Validator
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(product => product.Name)
                .MaximumLength(20).WithMessage("Name to long");

            RuleFor(product => product.Description)
                .MaximumLength(120).WithMessage("Description to long");

            RuleFor(product => product.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount should be greater or equal than 0")
                .LessThanOrEqualTo(100).WithMessage("Discount can not be greater than 100");

            RuleFor(product => product)
                .Must(IsProductUpdateDtoValid)
                .WithMessage("One field should be set");
        }

        private bool IsProductUpdateDtoValid(ProductUpdateDto product)
        {
            if (product.Name == null && product.Description == null && product.Discount == null)
            {
                return false;
            }

            return true;
        }
    }
}
