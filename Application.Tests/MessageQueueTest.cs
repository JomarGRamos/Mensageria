using Application.Services;
using Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace Application.Tests
{

    [TestClass]
    public class MessageQueueTest
    {
        string hostname = "b-68f71a27-6ffc-4e9b-b17e-1e410f7a518d.mq.us-east-1.amazonaws.com";
        string endpoint = "amqps://b-68f71a27-6ffc-4e9b-b17e-1e410f7a518d.mq.us-east-1.amazonaws.com:5671";
        public Tuple<IConnection, IModel> Connect(string hostname, string user, string pass, string endpoint)
        {
            var factory = new ConnectionFactory() { HostName = hostname, UserName = user, Password = pass };
            factory.Uri = new System.Uri(endpoint);
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                                exchange: "logs",
                                  routingKey: "");
            return new Tuple<IConnection, IModel>(connection, channel);
        }

        [TestMethod]
        public void TestConnectionSuccess()
        {

            var result = Connect(hostname, "Jzinho", "guest!", endpoint);

            Assert.IsTrue(result.Item1.IsOpen);
            Assert.IsTrue(result.Item2.IsOpen);
            result.Item2.Close();
            result.Item1.Close();
        }

        [TestMethod]
        public void TestConnectionFail()
        {
            Tuple<IConnection, IModel> result = null;
            try
            {
                result = Connect(hostname, "User", "Pass!", endpoint);
            }
            catch
            {
                Assert.IsNull(result);
            }

        }

    }
}
