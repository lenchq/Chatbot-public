using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class BotDialogues
{
    public Dialogue[] Dialogues { get; private set; }

    private readonly IServiceProvider _provider;
    
    public BotDialogues(IServiceProvider provider)
    {
        _provider = provider;
        Dialogues = new Dialogue[]
        {
            new StartDialogue(),
            new EditAgeDialogue(),
            new ConfirmAgeDialogue()
        };
        InitializeDependencies();
    }

    public Dialogue GetDialogueByState(State state)
    {
        foreach (var dialogue in Dialogues)
        {
            if (dialogue.State == state)
            {
                return dialogue;
            }
        }

        throw new KeyNotFoundException("Dialogue with provided state not found");
    }

    private void InitializeDependencies()
    {
        foreach(var dialogue in Dialogues)
        {
            dialogue.DependencyInjection(_provider);
        }
    }
    
    
}