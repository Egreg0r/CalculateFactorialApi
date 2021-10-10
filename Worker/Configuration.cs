using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;

// ============================================================
//         Методы запроса конфигураций из файла xml.config
// ===============================================================


namespace Worker
{
    public class Configuration
    {
        private static IConfiguration XmlConfig { get; set; }
        public Configuration()
        {
            var builder = new ConfigurationBuilder()
                .AddXmlFile(@"Config/Xml.config",
                            optional: true,
                            reloadOnChange: true);
            XmlConfig = builder.Build();
        }

        /// <summary>
        /// Исползуемый брокер
        /// </summary>
        /// <returns>Element value "useBroker"</returns>
        public string UseBroker()
        {
            return XmlConfig["useBroker"];
        }

        /// <summary>
        /// Get host name in xml.config from brokers -> dnsName
        /// </summary>
        /// <returns>Element value "dnsName" Dns broker</returns>
        public string BrokerHost()
        {
            string st = "brokers:" + UseBroker();
            return XmlConfig[st + ":dnsName"];
        }

        /// <summary>
        /// Получение имени указаной очереди в структуре brokers -> имя брокера -> queueName
        /// </summary>
        /// <param name="mode"> Выбор режима работы с брокером (send/receive) </param>
        /// <returns>Element value "mode" (send/receive)</returns>
        public string QueueName (string mode)
        {
            string st =string.Format("brokers:{0}:queueName:{1}", UseBroker(), mode);
            return XmlConfig[st];
        }

        /// <summary>
        /// Use to get list value for children element from xml.config 
        /// </summary>
        /// <param name="value">name element with children</param>
        /// <returns>dict(key, value)</returns>
        public Dictionary<string, string> GetChildElemInDict(string value)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var count = XmlConfig.GetSection(value).GetChildren();
            foreach (var p in count)
            {
                dict.Add(p.Key ,p.Value);
            }
            return dict;
        }

    }


}
