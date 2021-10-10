using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using System.Threading;


namespace Worker
{
    public class Receive
    {

        private readonly static Configuration conf = new Configuration();
        /// <summary>
        ///Имя очереди для отправки
        ///</summary>
        private readonly static string nameCall = conf.QueueName("send");
        /// <summary>
        /// Имя очереди для получения
        /// </summary>
        private readonly static string nameReceiv = conf.QueueName("receive");
        /// <summary>
        /// Строка для подключения к брокеру
        /// </summary>
        private readonly static ConnectionFactory factory = new ConnectionFactory() { HostName = conf.BrokerHost(), DispatchConsumersAsync = true };
        /// <summary>
        /// Соединение с брокером
        /// </summary>
        private readonly static IConnection connection = factory.CreateConnection();
        /// <summary>
        /// Канал работы с брокером
        /// </summary>
        private readonly static IModel channel = connection.CreateModel();

        /// <summary>
        /// Получение сообщений из очереди qname и преобразование его в long
        /// </summary>
        public static void Calculate()
        {
            Logger.LInfo("brokerHost = {0}", conf.BrokerHost());
            Logger.LInfo("queueName = {0}", nameReceiv);
            try
            {
                channel.QueueDeclare(queue: nameReceiv,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new AsyncEventingBasicConsumer(channel);
                channel.BasicConsume(queue: nameReceiv, autoAck: false, consumer: consumer);
                consumer.Received += consumerResivedAsync;
                Logger.LInfo("Start listening for {0}", nameReceiv);
                while (Console.ReadLine() != "exit")
                {
                    Console.WriteLine("Для выхода введите: exit");
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Logger.LFattal("При подключении к брокеру ошибка: {0}", ex.Message);
            }
            finally
            {

                channel.Close();
                connection.Close();
            }
        }

        /// <summary>
        /// обработка полученных сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        private static async Task consumerResivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            string response = null;

            var body = @event.Body.ToArray();
            var props = @event.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                Logger.LDebug("Получено число: {0}", message);
                long number;
                number = (long)Convert.ToUInt64(message);
                // вычисляем факториал
                number = new Calculate().calculate(number);
                response = number.ToString();
                Logger.LInfo("Факториал числа {0} = {1}", message, response);

            }
            catch (Exception ex)
            {
                Logger.LError("При обработке сообщения ошибка: {0}  ", ex.Message);
                response = "0";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                  basicProperties: replyProps, body: responseBytes);
                channel.BasicAck(deliveryTag: @event.DeliveryTag,
                  multiple: false);
            }
            //сообщаем об успешном нахождении факториала для удаления числа.
            //channel.BasicAck(@event.DeliveryTag, true);
            //Thread.Sleep(1000);
            await Task.Yield();
        }

    }
}
