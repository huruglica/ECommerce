using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserAndBankAccountServices.Models.Dtos;
using UserAndBankAccountServices.Validator;

namespace UserAndBankAccountServicesTest.Helpers
{
    public class UserUpdateDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public UserUpdateDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
