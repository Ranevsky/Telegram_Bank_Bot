using Base.Domain.Entities;
using Telegram.Bot;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.CallbackQueries;

public class DepartmentCallback : Handler<CallbackArgs>
{
    public const string Name = "DEPARTMENT";

    private readonly ITelegramBotClient _bot;
    private readonly IUnitOfWork _uow;

    public DepartmentCallback(ITelegramBotClient bot, IUnitOfWork uow)
    {
        _bot = bot;
        _uow = uow;
    }

    public override async Task HandleAsync(CallbackArgs args)
    {
        if (args.Args.Length - args.ArgsIteration >= 2 && args.GetArg() == Name)
        {
            var departmentId = int.Parse(args.Args[args.ArgsIteration + 1]);
            args.ArgsIteration += 2;

            var department = await _uow.Departments.GetDepartmentWithLocationAndCurrencyAndBankAsync(departmentId);
            var userDb = await _uow.Users.GetWithLocationAndCurrencyAsync(args.From.Id);

            if (department?.Location is null || userDb.Location is null)
            {
                return;
            }

            var departmentLocation = department.Location;
            // street 
            // bankName
            // Distance
            // current curr
            var curr =
                department.Currencies.FirstOrDefault(c =>
                    c.Currency.Name.ToUpper() == userDb.SelectedCurrency!.Name.ToUpper())!;

            Location userLocation = new()
                { Longitude = userDb.Location.Longitude, Latitude = userDb.Location.Latitude };
            var distance = Location.Distance(userLocation, department.Location);
            var text =
                $"Distance: {distance * 1000:f0}\n" +
                // Todo: Check department.Bank
                $"Bank: {department.Bank.Name}\n" +
                $"Street: {department.Street}\n" +
                //"Format: Currency: Buy | Sell\n" +
                //$"{curr.Name}: {curr.Buy} | {curr.Sell}";
                $"Currency: {curr.Currency.Name}\n" +
                $"Buy: {curr.Buy}\n" +
                $"Sell: {curr.Sell}";

            var msg = await _bot.SendTextMessageAsync(args.ChatId, text);
            await _bot.SendLocationAsync(args.ChatId, departmentLocation.Latitude, departmentLocation.Longitude,
                replyToMessageId: msg.MessageId);
        }
        else
        {
            await Successor.HandleAsync(args);
        }
    }
}