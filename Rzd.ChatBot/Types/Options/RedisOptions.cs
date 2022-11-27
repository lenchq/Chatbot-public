﻿namespace Rzd.ChatBot.Types.Options;

public record RedisOptions
{
    public string Url { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}