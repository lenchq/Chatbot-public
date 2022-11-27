﻿namespace Rzd.ChatBot.Types.Options;

public record DbOptions
{
    public string Url { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}