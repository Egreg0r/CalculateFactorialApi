using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using System.Collections.Concurrent;



namespace WebServer
{
    /// <summary>
    /// Класс обмена сообщениями с брокером
    /// </summary>
    class Send
    {
        private readonly static Configuration conf = new Configuration();
        /// <summary>
        ///Имя очереди для отправки
        ///</summary>
        private readonly static string nameCall = conf.QueueName("send");
        /// <summary>
        /// Соединение с брокером
        /// </summary>
        private readonly IConnection connection;
        /// <summary>
        /// Канал работы с брокером
        /// </summary>
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        /// <summary>
        /// Получение сообщения от брокера
        /// </summary>
        public Send()
        {
            var factory = new ConnectionFactory() { HostName = conf.BrokerHost() };
            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                replyQueueName = channel.QueueDeclare().QueueName;
                consumer = new EventingBasicConsumer(channel);

                props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        respQueue.Add(response);
                    }
                };

                channel.BasicConsume(
                    consumer: consumer,
                    queue: replyQueueName,
                    autoAck: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption", ex);
            }
        }


        /// <summary>
        /// Отпарвление сообшения в брокер
        /// </summary>
        /// <param name="message">строка сообщения</param>
        /// <returns></returns>
        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: nameCall,
                basicProperties: props,
                body: messageBytes);

            return respQueue.Take();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            connection.Close();
        }
    }
}
