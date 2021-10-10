using System;
using System.Threading.Tasks;

namespace WebServer
{
    public interface IFactorialCalculator
    {
        Task<long> CalculateAsync(long number);
    }

    public class ApiMetod : IFactorialCalculator
    {

        /// <summary>
        /// Асинхронный бработчик для отправки числа  и получения Факториала
        /// </summary>
        /// <param name="number">Полученное от пользователя число</param>
        /// <returns>Факториал числа формата long. Если 0 - ошибка вычисления</returns>
        public Task<long> CalculateAsync(long number)
        {
            string brokerHost; // DNS Имя хоста
            string queueName; // имя очереди

            //проверка настроек
            try
            {
                Configuration conf = new Configuration();
                brokerHost = conf.BrokerHost();
                queueName = conf.QueueName("send");
            }
            catch
            {
                return Task.FromResult((long)0);
            }
            var sendClient = new Send();
            var mes = number.ToString();
            var response = sendClient.Call(mes);
            sendClient.Close();
            // проверяем на long на переполнение. Если true то возвращаем 0
            try
            {
                var p = (long)Convert.ToUInt64(response);
                return Task.FromResult((long)p);
            }
            catch 
            {
                return Task.FromResult((long)0);
            }

        }

    }
}
