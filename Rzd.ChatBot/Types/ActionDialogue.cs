using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Telegram.Bot.Types;

namespace Rzd.ChatBot.Types;

public abstract class ActionDialogue : Dialogue
{
    public override InputType InputType { get;} = InputType.Option;
    public virtual Dictionary<string, AsyncBotAction> Actions { get; set; }
    protected ActionDialogue(string localizationName) : base(localizationName)
    {
        
    }

    public virtual void WrongAnswer(Context ctx)
    {
        
    }

    public virtual string WrongAnswerText(Context ctx)
        => LocalizationWrapper.GetText("wrongOption") ?? LocalizationWrapper.GetRaw("wrongOption")!;

    // public IEnumerable<IEnumerable<string>> GetOptions()
    // {
    //     return LocalizationWrapper.GetOptions();
    // }
    public OptionsProvider GetOptions()
    {
        return new OptionsProvider
        {
            Options = LocalizationWrapper.GetOptions(),
            Colors = LocalizationWrapper.GetColors(),
        };
    }
}