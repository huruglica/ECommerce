using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace UserAndBankAccountServicesTest.Helpers
{
    public class AutoMapperFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; }

        public AutoMapperFixture()
        {
            var services = new ServiceCollection();

            var mapperConfiguration = new MapperConfiguration(
                mc => mc.AddProfile(new UserAndBankAccountServices.Helpers.AutoMapper()));

            IMapper mapper = mapperConfiguration.CreateMapper();

            services.AddSingleton(mapper);

            ServiceProvider = services.BuildServiceProvider();
        }
        public void Dispose()
        {
            ServiceProvider.Dispose();
        }
    }
}
