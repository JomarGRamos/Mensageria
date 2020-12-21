using System;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace Application.Services
{
    public class MessageQueueManager : IMessageQueue
    {
        readonly ILogger _logger;
        IConnection connection;
        IModel channel;
        string queueName;
        public MessageQueueManager(ILogger logger)
        {
            _logger = logger;
            Connect();
        }
        private void Connect()
        {
            var factory = new ConnectionFactory() { HostName = "b-68f71a27-6ffc-4e9b-b17e-1e410f7a518d.mq.us-east-1.amazonaws.com", UserName = "Jzinho", Password = "guest!" };
            var endpoint = "amqps://b-68f71a27-6ffc-4e9b-b17e-1e410f7a518d.mq.us-east-1.amazonaws.com:5671";
            factory.Uri = new System.Uri(endpoint);
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                                exchange: "logs",
                                  routingKey: "");
        }

        public async Task Consumer(int id)
        {
            try
            {
                var result = string.Empty;

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Message objMsg = JsonConvert.DeserializeObject<Message>(message);

                    if (objMsg.IdService == id)
                    {
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                    else
                    {
                        channel.BasicAck(ea.DeliveryTag, false);
                        result = objMsg.MessageToString();
                        Console.WriteLine(result);
                    }

                };

                string consumerTag = channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        public async Task Publisher(int id)
        {
            try
            {

                string jsonString = JsonConvert.SerializeObject(BuildMessage(id));
                string msgSend = jsonString;
                var body = Encoding.UTF8.GetBytes(msgSend);
                channel.ConfirmSelect();

                channel.BasicPublish(exchange: "logs",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
                channel.WaitForConfirmsOrDie();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        private Message BuildMessage(int id)
        {
            Random random = new Random();
            int intRandom = random.Next(10000, 99999);
            var timestamp = DateTime.Now.ToFileTime();

            return new Message("Hello World", id, timestamp, intRandom);
        }
    }
}
