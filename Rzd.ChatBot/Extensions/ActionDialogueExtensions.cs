using Rzd.ChatBot.Types;

namespace Rzd.ChatBot.Extensions;

public static class ActionDialogueExtensions
{
    public static IEnumerable<string> GetOptions(this ActionDialogue dialogue)
        => dialogue.Actions.Keys;
}