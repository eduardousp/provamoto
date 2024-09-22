using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly string _hostname;
    private readonly string _queueName;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQPublisher(string hostname, string queueName)
    {
        _hostname = hostname;
        _queueName = queueName;

        var factory = new ConnectionFactory { HostName = _hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declara a fila no RabbitMQ
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishMotoCadastrada(Moto moto)
    {
        var message = JsonSerializer.Serialize(moto);
        var body = Encoding.UTF8.GetBytes(message);

        // Publica a mensagem na fila
        _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        Console.WriteLine($"[x] Moto cadastrada: {message}");
    }

    // Libera os recursos quando o publisher for descartado
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
