using Microsoft.VisualBasic;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Telegram.Bot.Types;

namespace Rzd.ChatBot.Types;

public abstract class InputDialogue : Dialogue
{
    public override InputType InputType { get; } = InputType.Text;

    protected InputDialogue(string localizationName) : base(localizationName)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns>Next state</returns>
    public abstract State ProceedInput(Context ctx);
    public abstract bool Validate(Message msg);

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.GetText("validationError");
}