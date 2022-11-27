# Rzd.ChatBot

![](https://github.com/lenchq/Chatbot-public/actions/workflows/dotnet.yml/badge.svg)
![](https://github.com/lenchq/Chatbot-public/actions/workflows/docker-image.yml/badge.svg)

<hr>

Основаня логика находится в абстрактном классе [BotWorker](Rzd.ChatBot/Types/BotWorker.cs), который представляет собой фоновый процесс взаимодействующий с API.

Для того чтоб подключить бота к конкретному мессенджеру нужно создать новый класс WeChatBotWorker реализовать в нем метод SendMessage() и при новом сообщении вызывать метод HandleMessage().

Реализации класса - [TelegramBotWorker](Rzd.ChatBot/TelegramBotWorker.cs) и [VkBotWorker](Rzd.ChatBot/VkBotWorker.cs) всего лишь реализуют интерфейс взаимодействия с определенным API через абстрактные методы (например абстрактный метод SendMessage, который необходимо реализовать в каждом классе по-своему (абстрактные методы - методы в абстрактном классе А, которые при наследовании от А обязательны к реализации)).

Весь функционал бота реализуется в классах, наследующих от абстрактного класса Dialogue. На данный момент есть два вида Dialogue - это [ActionDialogue](Rzd.ChatBot/Types/ActionDialogue.cs) (нужно нажать на какую-то опцию в меню у бота) и [InputDialogue](Rzd.ChatBot/Types/InputDialogue.cs) (нужно ввести произвольный текст который впоследствии будет передан в метод Validate проверяющий корректность ввода и обрабатывающий его). В разработке находятся методы приема у пользователя фотографий, геолокации, стикеров и файлов.

Локализация подтягивается из [файла локализации](Rzd.ChatBot/localization.yml) в формате YAML, там же задается и цвет кнопок (допустимо только для ВКонтакте). Файл локализации можно изменять во время работы программы и изменения отобразятся на работе.

Также проект имеет Jenkins CI Pipeline (WIP)
