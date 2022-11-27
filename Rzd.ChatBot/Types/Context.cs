using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Types;


//TODO: rename
public class Context
{
    public UserContext UserContext { get; set; }
    public Message Message { get; set; }
}