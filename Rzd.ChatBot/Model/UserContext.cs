using Newtonsoft.Json;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Model;

public record UserContext
{
    /// <summary>
    /// Represents telegram chat id
    /// </summary>
    public long Id { get; init; }
    
    
    private int _age = default;
    public int Age
    {
        get => _age;
        set => SetField(ref _age, value);
    }
    
    private InputType _inputType = InputType.Option;
    public InputType InputType
    {
        get => _inputType;
        set => SetField(ref _inputType, value);
    }
    
    private State _state;
    public State State
    {
        get => _state;
        set => SetField(ref _state, value);
    }

    [JsonIgnore]
    public bool Modified { get; set; } = false;
    
    private void SetField<T>(ref T field, T value)
    {
        Modified = true;
        field = value;
    }
}