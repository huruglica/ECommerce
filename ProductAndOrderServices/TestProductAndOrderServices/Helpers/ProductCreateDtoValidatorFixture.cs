using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Validator;

namespace TestProductAndOrderServices.Helpers
{
    public class ProductCreateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public ProductCreateDtoValidatorFixture()
        {
            var services = new ServiceCollection();

            services.AddTransient<IValidator<ProductCreateDto>, ProductCreateDtoValidator>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
