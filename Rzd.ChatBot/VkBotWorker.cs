using Microsoft.Extensions.Options;
using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Options;
using VkNet;
using VkNet.Enums.Filters;
using Rzd.ChatBot.Types;
using VkNet.Enums.SafetyEnums;
using VkMessage = VkNet.Model.Message;
using VkNet.Model;
using VkNet.Model.Keyboard;
using Message = Rzd.ChatBot.Types.Message;

namespace Rzd.ChatBot;

public sealed class VkBotWorker : BotWorker
{
    private const string RedisPrefix = "vk:";
    
    private ulong _lastTs;
    private VkApi _client;
    private readonly Random _random;
    private const string TsFilename = "vk_ts";

    public VkBotWorker(
        ILogger<VkBotWorker> logger,
        IOptions<VkBotOptions> options,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        BotDialogues dialogues,
        IServiceProvider provider
    )
    : base(RedisPrefix, logger, contextRepository, localization, dialogues, provider)
    {
        _client = new VkApi();
        _random = new Random();
        _client.Authorize(new()
        {
            AccessToken = options.Value.Token,
        });
        _lastTs = LoadLastTs() ?? ulong.Parse(_client.Messages.GetLongPollServer().Ts);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var poll = _client.Messages.GetLongPollHistory(new()
            {
                Ts = _lastTs,
            });
            if (poll.UnreadMessages == 0)
            {
                await Task.Delay(100, stoppingToken);
                continue;
            };

            for (var i = 0; i < poll.Messages.Count; i++)
            {
                var msg = poll.Messages[i];

                ArgumentNullException.ThrowIfNull(msg.FromId);
                var userCtx = GetUserContext(msg.FromId.Value);

                var context = ContextFactory.FromVk(msg, userCtx);
                
                await HandleMessageAsync(context);
            }

            _lastTs = ulong.Parse(_client.Messages.GetLongPollServer().Ts);;
        }
    }

    

    protected override async Task SendTextMessage(long chatId, string text, OptionsProvider? provider)
    {
        MessageKeyboard? keyboard = null; 
        if (provider?.Options is not null)
        {
            
            keyboard = new MessageKeyboard
            {
                Buttons = provider.Options.Select((row, i) =>
                {
                    return row.Select((action, j) => new MessageKeyboardButton
                    {
                        Action = new MessageKeyboardButtonAction
                        {
                            Type = KeyboardButtonActionType.Text,
                            Label = action
                        },
                        Color = SafetyEnum<KeyboardButtonColor>.FromJsonString(provider.Colors[i][j]) ??
                                KeyboardButtonColor.Default
                    }).ToArray();
                }),
                Inline = false,
                OneTime = true,
            };
        }
        
        
        
        await _client.Messages.SendToUserIdsAsync(new()
        {
            RandomId = _random.Next(),
            UserId = chatId,
            Message = text,
            Keyboard = keyboard,
        });
        //_logger.LogDebug("SENT MESSAGE TO {0}", chatId);
    }

    public override void Dispose()
    {
        SaveLastTs();
        base.Dispose();
    }

    private void SaveLastTs()
    {
        File.WriteAllText(TsFilename, _lastTs.ToString());
    }
    private ulong? LoadLastTs()
    {
        if (!File.Exists(TsFilename))
            return null;
        
        return ulong.Parse(File.ReadAllText(TsFilename));
    }
}