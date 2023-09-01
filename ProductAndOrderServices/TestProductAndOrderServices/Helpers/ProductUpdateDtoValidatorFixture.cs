using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Validator;

namespace TestProductAndOrderServices.Helpers
{
    public class ProductUpdateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public ProductUpdateDtoValidatorFixture()
        {
            var services = new ServiceCollection();
            services.AddTransient<IValidator<ProductUpdateDto>, ProductUpdateDtoValidator>();

            ServiceProvider = services.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
