using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;



namespace ServerTest
{
    class Send
    {
        private readonly static Configuration conf = new Configuration();
        /// <summary>
        ///Имя очереди для отправки
        ///</summary>
        private readonly static string nameCall = conf.QueueName("send");
        /// <summary>
        /// Имя очереди для получения
        /// </summary>
        private readonly static string nameResev = conf.QueueName("receive");
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

        /*
        /// <summary>
        /// Отправка сообщения в Брокер
        /// </summary>
        /// <param name="qname">Имя очереди</param>
        /// <param name="brokerHost">DNS брокера</param>
        /// <param name="number">число для подсчета интегралла</param>
        /// <returns></returns>
        public async Task SendAcync(string qname, string brokerHost, long number )
        {
            while (true)
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = brokerHost };
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: qname,
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                            string message = number.ToString();
                            // переводим в байты для брокера
                            var body = Encoding.UTF8.GetBytes(message);


                            channel.BasicPublish(exchange: "",
                                         routingKey: qname,
                                         basicProperties: null,
                                         body: body);


                            channel.Close();
                            connection.Close();
                        }
                    }
                    Console.WriteLine("Число");
                    await Task.Delay(600);

                }
                catch (Exception ex) 
                {
                    Console.WriteLine("Ошибка {0}",ex);
                }
            }
        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
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
        /// 
        /// </summary>
        public void Close()
        {
            connection.Close();
        }
    }
}
