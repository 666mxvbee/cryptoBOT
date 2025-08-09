using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("TELEGRAM_TOKEN не задан. Установи переменную окружения и перезапусти.");
    return;
}

var bot = new TelegramBotClient(token);

var me = await bot.GetMe();
Console.WriteLine($"Бот @{me.Username} запущен. Нажми Ctrl+C для выхода.");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

bot.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cts.Token
);

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await Task.Delay(Timeout.Infinite, cts.Token);

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    if (update.Message is not { Text: { } text } msg)
        return;

    await bot.SendMessage(
        chatId: msg.Chat,
        text: $"Эхо: {text}",
        cancellationToken: ct
    );
}

Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine($"[ERR] {ex.Message}");
    return Task.CompletedTask;
}