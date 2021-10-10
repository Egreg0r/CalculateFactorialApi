using System;
using System.IO;

namespace Worker
{

    class FactorialCalculator
    {
        static void Main(string[] args)
        {
            try
            {
                Logger.InitLogger();
            }
            catch
            {
                return;
            }
            Logger.LInfo("***********Запуск Worker***********");
            //проверка наличия файла настроек Xml.config
            try
            {
                fileExist(@"Config/Xml.config");
            }
            catch (Exception ex)
            {
                Logger.LError("не удалось загрузить конфигурации xml.config");
                Logger.LFattal("***********Неожиданная остановка Worker********** Ошибка: {0}", ex.Message);
                return;
            }

            Receive.Calculate();
            
            Logger.LInfo("***********Остановка Worker***********");
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