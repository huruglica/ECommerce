using ProductAndOrderServices.Model;
using static EmailService.EmailService;
using EmailService;
using static UserService.UserService;
using UserService;
using MassTransit;
using static BankAccountService.BankAccountService;
using BankAccountService;

namespace ProductAndOrderServices.Consumers
{
    public class MostSpentUserInfoConsumer : BackgroundService, IConsumer<MostSpentUserInfo>
    {
        private readonly EmailServiceClient _emailServiceClient;
        private readonly UserServiceClient _userServiceClient;
        private readonly BankAccountServiceClient _bankAccountServiceClient;
        private static string eCommerceBankAccountId = "64f1bf6c2c45efd1d18f86c7";

        public MostSpentUserInfoConsumer(EmailServiceClient emailServiceClient, UserServiceClient userServiceClient, BankAccountServiceClient bankAccountServiceClient)
        {
            _emailServiceClient = emailServiceClient;
            _userServiceClient = userServiceClient;
            _bankAccountServiceClient = bankAccountServiceClient;
        }

        public async Task Consume(ConsumeContext<MostSpentUserInfo> context)
        {
            await SendEmailAndTransferMoney(context.Message);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            return Task.CompletedTask;
        }

        private async Task SendEmailAndTransferMoney(MostSpentUserInfo mostSpentUserInfo)
        {
            var userIdRequest = new UserIdRequest()
            {
                UserId = mostSpentUserInfo.UserId,
            };

            var response = await _userServiceClient.GetUserInfoAsync(userIdRequest);

            await SendEmail(response.Name, response.Surname, response.Email, mostSpentUserInfo.Amount);
            await TranferMoney(response.BankAccountId, mostSpentUserInfo.Amount);
        }

        private async Task SendEmail(string name, string surname, string email, double amount)
        {
            var request = new Request
            {
                Name = name + " " + surname,
                Email = email,
                Amount = amount
            };

            await _emailServiceClient.SendEmailAsync(request);
        }

        private async Task TranferMoney(string bankAccountId, double amount)
        {
            var requestList = new TransferRequestList();
            var request = new TransferRequest
            {
                SenderBankAccountId = eCommerceBankAccountId,
                RecieverBankAccountId = bankAccountId,
                Amount = amount
            };

            requestList.List.Add(request);

            await _bankAccountServiceClient.TransferAsync(requestList);
        }
    }
}
