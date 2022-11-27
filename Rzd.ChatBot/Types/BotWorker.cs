using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class BotWorker : BackgroundService
{
    protected readonly BotDialogues Dialogues;
    protected readonly IUserContextRepository ContextRepository;
    protected readonly AppLocalization Localization;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger<BotWorker> Logger;


    protected BotWorker(
        string redisBotPrefix,
        ILogger<BotWorker> logger,
        IUserContextRepository contextRepository,
        AppLocalization localization,
        BotDialogues dialogues,
        IServiceProvider serviceProvider
        )
    {
        Logger = logger;
        ContextRepository = contextRepository;
        Localization = localization;
        Dialogues = dialogues;
        ServiceProvider = serviceProvider;
        ContextRepository.Initialize(redisBotPrefix);
    }

    protected async Task HandleMessageAsync(Context ctx)
    {
        var commandsCts = new CancellationTokenSource();
        await HandleCommands(ctx,
            commandsCts);
        if (commandsCts.IsCancellationRequested)
            return;

        var userCtx = ctx.UserContext;
        var msg = ctx.Message;
        var chatId = userCtx.Id;
        
        var dialogue = Dialogues.GetDialogueByState(userCtx.State);
        
        // TODO: resolve contextual buttons
        // TODO: extract method ResolveOption() 
        if (userCtx.InputType == InputType.Option)
        {
            var actionDialogue = (ActionDialogue) dialogue;
            actionDialogue.Actions.TryGetValue(msg.Text, out var action);
            if (int.TryParse(msg.Text, out var index)
                && index > 0
                && index - 1 < actionDialogue.Actions.Count)
            {
                action ??= actionDialogue.Actions
                    .Values
                    .ToArray()[index - 1];
            }

            if (action is not null)
            {
                var nextState = action.Invoke(ctx);
                userCtx.State = nextState;
                
                await Finalize(nextState);
            }
            // exclude commands
            else if (!msg.Text.StartsWith('/'))
            {
                //var options = GetOptions(actionDialogue);
                var options = actionDialogue.GetOptions();
                await SendTextMessage(chatId,
                    actionDialogue.WrongAnswerText(ctx),
                    options);
            }
        }
        //TODO extract method ResolveTextInput()
        else if (userCtx.InputType == InputType.Text)
        {
            var inputDialogue = (InputDialogue) dialogue;
            if (!inputDialogue.Validate(ctx.Message))
            {
                await SendTextMessage(chatId,
                    inputDialogue.GetErrorText(ctx));
                return;
            }

            var nextState = inputDialogue.ProceedInput(ctx);
            userCtx.State = nextState;

            
            await Finalize(nextState);
        }
        else if (userCtx.InputType == InputType.Photo)
        {
            //TODO Photo InputType
        }

        if (userCtx.Modified)
        {
            ContextRepository.SetContext(userCtx);
        }

        // ReSharper disable once LocalFunctionHidesMethod
        async Task Finalize(State nextState)
        {
            if (Dialogues.GetDialogueByState(nextState) is { } nextDialogue)
            {
                OptionsProvider? options = null;
                if (nextDialogue.InputType == InputType.Option)
                {
                    options = ((ActionDialogue) nextDialogue).GetOptions();
                }

                await SendTextMessage(chatId,
                    nextDialogue.GetText(ctx), options);
                userCtx.InputType = nextDialogue.InputType;
            }
        }
    }

    

    protected UserContext GetUserContext(long msgFromId)
    {
        var success = ContextRepository.TryGetContext(msgFromId, out var userCtx);
        if (!success)
        {
            var newContext = new UserContext
            {
                Id = msgFromId,
                Modified = true
            };
            userCtx = newContext;
            //ContextRepository.SetContext(newContext);
        }

        return userCtx;
    }
    
    

    protected async Task HandleCommands(Context ctx, CancellationTokenSource cts)
    {
        if (ctx.Message.Text == "/start")
        {
            ContextRepository.DeleteContext(ctx.UserContext.Id);
            
            var starting = new StartDialogue();
            starting.DependencyInjection(ServiceProvider);
            await SendTextMessage(ctx.UserContext.Id,
                starting.GetText(ctx), starting.GetOptions());
            cts.Cancel();
        }
        //TODO: another commands, /help, /about....et....c..
    }
    
    protected async Task SendTextMessage(long chatId, string text)
    {
        await this.SendTextMessage(chatId, text, null);
    }

    
    // protected async Task SendTextMessage(long chatId, string text, IEnumerable<string>? flatOptions = null)
    // {
    //     await this.SendTextMessage(chatId, text,
    //         flatOptions is null ? null : new[] {flatOptions});
    // }
    protected abstract Task SendTextMessage(long chatId, string text, OptionsProvider? prov);
    // protected virtual IEnumerable<string> GetOptions(ActionDialogue actionDialogue)
    //     => actionDialogue.Actions.Keys;
}