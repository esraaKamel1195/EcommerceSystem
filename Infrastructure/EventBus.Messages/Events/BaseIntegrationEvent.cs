
namespace EventBus.Messages.Events
{
    public class BaseIntegrationEvent
    {
        public string CorrelationId { get; set; }

        public DateTime CreatedAt;

        public BaseIntegrationEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
        }

        public BaseIntegrationEvent(string correlationId, DateTime createdAt)
        {
            CorrelationId = correlationId;
            CreatedAt = createdAt;
        }
    }
}
