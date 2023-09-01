using MassTransit;
using ProductAndOrderServices.Consumers;

namespace ProductAndOrderServices.Helpers
{
    public static class StartupHelper
    {
        public static void AddRabbitMQMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MostSpentUserInfoConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("user-spent-most", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<MostSpentUserInfoConsumer>(context);
                    });
                });
            });
        }
    }
}
