using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class StartDialogue : ActionDialogue
{
    //public override Dictionary<string, AsyncBotAction> Actions { get; set; }
    public override State State { get; set; } = State.Starting;

    public override string GetText(Context ctx)
        => LocalizationWrapper.GetText().Format(ctx.Message.ChatId, ctx.UserContext.State, ctx.UserContext.Age);
    
    public StartDialogue()
        : base(nameof(StartDialogue))
    {
    }

    protected override void Initialized()
    {
        Actions = new Dictionary<string, AsyncBotAction>
        {
            {LocalizationWrapper.GetOption(0), OkAction}
        };
    }


    public override void WrongAnswer(Context ctx)
    {
        //TODO:
    }

    public State OkAction(Context ctx)
        => State.EditAge;
}