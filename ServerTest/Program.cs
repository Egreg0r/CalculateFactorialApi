using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerHost; // DNS Имя хоста
            string queueName; // имя очереди

            //проверка настроек
            try
            {
                fileExist(@"Config/Xml.config");
                Configuration conf = new Configuration();
                brokerHost = conf.BrokerHost();
                Console.WriteLine("brokerHost={0}", 
                    brokerHost);
                queueName = conf.QueueName("send");
                Console.WriteLine("queueName= " + queueName);
            }
            catch
            {
                Console.WriteLine("не удалось загрузить конфигурации xml.config");
                Console.WriteLine("***********Неожиданная остановка Worker**********");
                return;
            }
            Console.WriteLine("Загрузка Брокера");
            // отправляем число в очередь для вычисления.
            //for (int i=0; i < 10; i++)
            //{
            //   _ = Send.SendAcync(queueName, brokerHost, new Random().Next(2, 20));
            //} 
            var sendClient = new Send();

            do
            {
                    var mes = new Random().Next(0, 20).ToString();
                    Console.WriteLine(" [x] Requesting nomber =  {0}", mes);
                    var response = sendClient.Call(mes);
                    Console.WriteLine(" [.] Got '{0}'", response);
                    Console.WriteLine("Для выхода введите: exit");
                    Console.ReadLine(); 

            } while (Console.ReadLine() != "exit");

            sendClient.Close();
        }
    

        /// <summary>
        /// Проверка на существование указанного файла
        /// </summary>
        /// <param name="dir">полный путь в папке с программой</param>
        private static void fileExist(string dir)
        {
            bool exist = File.Exists(dir);
            if (exist == false)
            {
                Console.WriteLine("Нет файла конфигурации");
            }
            else Console.WriteLine("Файл конфигурации найден");
        }

    }
}
