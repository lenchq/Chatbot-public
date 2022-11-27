using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class ConfirmAgeDialogue : ActionDialogue
{
    public override State State { get; set; } = State.ConfirmAgeDialogue;


    public ConfirmAgeDialogue()
        : base(nameof(ConfirmAgeDialogue))
    {
    }

    protected override void Initialized()
    {
        Actions = new Dictionary<string, AsyncBotAction>()
        {
            {LocalizationWrapper[0], AgreeOption},
            {LocalizationWrapper[1], DisagreeOption}
        };
    }

    private State AgreeOption(Context ctx)
        => State.Starting;

    private State DisagreeOption(Context ctx)
        => State.EditAge;
}