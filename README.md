# CalculateFactorialApi
Установить Erlang 24.1 или выше
Установить RabbitMQ 3.9.7 или выше
.Net 5.0 или выше
В проектах WebServer и Worker указать настройки подключения к RabbitMQ файл XML.Config
Имя (value Element) очереди Webserver:send = Broker:receive
ServerTest - тестовое консольное приложение для отправки чисел в очередь
log4net.config - файл настройки формирования логов. По умолчанию логи хронятся в каталоге Logs
