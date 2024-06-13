namespace ReportService.Api.RabbitMq
{
    public interface IMessageProducer
    {
        public void SendingMessage<T>(T message);
    }
}
