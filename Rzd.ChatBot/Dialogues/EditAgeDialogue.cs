using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class EditAgeDialogue : InputDialogue
{
    public override State State { get; set; } = State.EditAge;

    public EditAgeDialogue()
        : base(nameof(EditAgeDialogue))
    {
        
    }
    
    public override State ProceedInput(Context ctx)
    {
        ctx.UserContext.Age = int.Parse(ctx.Message.Text);
        return State.ConfirmAgeDialogue;
    }

    public override bool Validate(Message msg)
    {
        return (
            int.TryParse(msg.Text, out var age)
            && age is > 12 and < 99
        );
    }
}