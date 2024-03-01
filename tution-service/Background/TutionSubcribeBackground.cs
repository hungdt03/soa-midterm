using Azure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using tution_service.AsyncDataServices;
using tution_service.Data;
using tution_service.Models;

namespace tution_service.Background
{
    public class TutionSubcribeBackground : IHostedService
    {
        private readonly ISubscriber subscriber;
        private readonly IServiceScopeFactory _scopeFactory;
        public TutionSubcribeBackground(ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            this.subscriber = subscriber;
            _scopeFactory = scopeFactory;
            //this.dbContext = dbContext;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            subscriber.Subscribe(ProcessMessage);
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TutionDbContext>();
                TransactionEvent transactionEvent = JsonConvert.DeserializeObject<TransactionEvent>(message)!;
                Tution tution = dbContext.Tutions.SingleOrDefault(x => x.TransactionId == transactionEvent.Id);
                tution.Status = Enums.TutionStatus.PAID;
                dbContext.SaveChanges();
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
