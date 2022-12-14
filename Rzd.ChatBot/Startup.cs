using Rzd.ChatBot.Data;
using Rzd.ChatBot.Data.Interfaces;
using Rzd.ChatBot.Dialogues;
using Rzd.ChatBot.Extensions;
using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Options;

namespace Rzd.ChatBot;

internal sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        

        services.Configure<TelegramBotOptions>(
            _configuration.GetSection("Bot:Telegram")
        );
        services.Configure<VkBotOptions>(
            _configuration.GetSection("Bot:Vk")
        );
        services.Configure<RedisOptions>(
            _configuration.GetSection("Data:Redis")
        );
        services.Configure<DbOptions>(
            _configuration.GetSection("Data:Database")
        );
        //var localization = new AppLocalization(_configuration);
        services.AddTransient<AppLocalization>();

        services.AddSingleton<IMemoryCache<UserContext>, RedisCache<UserContext>>();
        services.AddTransient<IUserContextRepository, UserContextRepository>();
        
        services.AddSingleton<BotDialogues>();
        
        services.AddHostedService<TelegramBotWorker>();
        services.AddHostedService<VkBotWorker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //StaticServiceProvider.Instance = app.ApplicationServices;
    }
}