using BankAccountService.Validator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserAndBankAccountServices.Model.Dtos;

namespace UserAndBankAccountServicesTest.Helpers
{
    public class BankAccountDtoValidatorFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public BankAccountDtoValidatorFixture()
        {
            var service = new ServiceCollection();

            service.AddScoped<IValidator<BankAccountDto>, BankAccountDtoValidator>();

            ServiceProvider = service.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
