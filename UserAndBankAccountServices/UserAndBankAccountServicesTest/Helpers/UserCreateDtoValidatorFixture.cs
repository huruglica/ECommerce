using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Validator;

namespace UserAndBankAccountServicesTest.Helpers
{
    public class UserCreateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public UserCreateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
