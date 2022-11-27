using VkMessage = VkNet.Model.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Rzd.ChatBot.Types;

public class Message
{
    public long ChatId { get; private set; }
    public string Text { get; private set; }
    public UserInfo From { get; private set; }


    public static Message FromTelegramMessage(TelegramMessage message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message.Text);
        return new Message
        {
            ChatId = message.Chat.Id,
            Text = message.Text,
            From = new UserInfo
            {
                FirstName = message.From?.FirstName,
                LastName = message.From?.LastName,
                Username = message.From?.Username,
            }
        };
    }

    public static Message FromVk(VkMessage message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message.Text);
        return new Message
        {
            ChatId = message.FromId ??
                     throw new ArgumentNullException(nameof(message.FromId)),
            Text = message.Text,
            From = new UserInfo
            {
                //TODO: sdelati plz
            }
        };
    }
}

public class UserInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
}