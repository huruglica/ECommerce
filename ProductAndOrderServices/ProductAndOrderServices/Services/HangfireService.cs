using MassTransit;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Services.IServices;

namespace ProductAndOrderServices.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly IOrderService _orderService;
        private readonly IBus _bus;

        public HangfireService(IOrderService orderService, IBus bus)
        {
            _orderService = orderService;
            _bus = bus;
        }

        public async Task GetUserSpendMost()
        {
            var orders = await _orderService.GetAll();

            var todayOrders = orders.Where(x => x.BoughtTime.Date == DateTime.Now.AddDays(-1).Date
                                           && x.IsBought);

            var dictionary = todayOrders.GroupBy(x => x.UserId)
                .ToDictionary(x => x.Key, y => y.ToList().Select(z => z.Price).Sum());

            if (dictionary.Count == 0)
            {
                return;
            }

            var userInfo = new MostSpentUserInfo
            {
                UserId = dictionary.MaxBy(x => x.Value).Key,
                Amount = Math.Round(dictionary.MaxBy(x => x.Value).Value * 0.1, 2)
            };

            await SetToRabbitMQ(userInfo);
        }

        private async Task SetToRabbitMQ(MostSpentUserInfo mostSpentUserInfo)
        {
            var queueToPublishTo = new Uri("rabbitmq://localhost/user-spent-most");
            var endPoint = await _bus.GetSendEndpoint(queueToPublishTo);
            await endPoint.Send(mostSpentUserInfo);
        }
    }
}
