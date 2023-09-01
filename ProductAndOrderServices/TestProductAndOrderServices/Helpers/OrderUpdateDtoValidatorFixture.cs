using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductAndOrderServices.Model.Dtos;
using ProductAndOrderServices.Validator;

namespace TestProductAndOrderServices.Helpers
{
    public class OrderUpdateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public OrderUpdateDtoValidatorFixture()
        {
            var services = new ServiceCollection();

            services.AddTransient<IValidator<OrderUpdateDto>, OrderUpdateDtoValidator>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
