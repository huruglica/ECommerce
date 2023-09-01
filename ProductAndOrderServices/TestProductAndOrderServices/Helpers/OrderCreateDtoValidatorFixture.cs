using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Validator;

namespace TestProductAndOrderServices.Helpers
{
    public class OrderCreateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public OrderCreateDtoValidatorFixture()
        {
            var services = new ServiceCollection();

            services.AddTransient<IValidator<OrderCreateDto>, OrderCreateDtoValidator>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
