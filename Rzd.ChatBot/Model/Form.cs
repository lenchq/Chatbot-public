namespace Rzd.ChatBot.Model;

public class Form
{
    public int Age { get; set; }
    public string Name { get; set; }
    //TODO: wtf is city
    public string City { get; set; }
    /// <summary>
    /// Comma-separated paths of photos
    /// </summary>
    public string[] Photos { get; set; }

    public string About { get; set; }
}