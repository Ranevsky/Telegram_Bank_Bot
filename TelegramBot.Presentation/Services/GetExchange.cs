using AutoMapper;
using Bank.Application.Models;
using Base.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Models.Args;
using TelegramBot.Presentation.Services.Handlers.CallbackQueries;

namespace TelegramBot.Presentation.Services;

// Todo: move new directories
public class GetExchange
{
    private readonly ITelegramBotClient _bot;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public GetExchange(ITelegramBotClient bot, IUnitOfWork uow, IMapper mapper)
    {
        _bot = bot;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task GetFormsAsync(
        MessageInfo args,
        Func<IEnumerable<DepartmentByDistance>, IOrderedEnumerable<DepartmentByDistance>> order,
        string callbackDataName,
        int page,
        int take)
    {
        async Task NotFoundSettingsAsync(long userId, List<string> message)
        {
            var keyboard = SettingsCallback.GetKeyboard();
            var mainText = await MainCallback.GetTextAsync(_uow, userId);
            // Todo: maybe have exception
            var text = mainText +
                       "\n=== Error ===\n" +
                       string.Join("\n", message);

            await _bot.EditMessageTextAsync(args.ChatId, args.MessageId, text, replyMarkup: keyboard);
        }

        var id = args.From.Id;
        var user = await _uow.Users.GetWithLocationAndCurrencyAndNearCityAsync(id);

        var msg = new List<string>();
        if (user.Location is null || user.NearCity is null)
        {
            msg.Add("Set your location");
        }

        if (user.SelectedCurrency is null || user.IsBuyOperation is null)
        {
            msg.Add("Set your currency");
        }

        if (msg.Count > 0)
        {
            await NotFoundSettingsAsync(id, msg);
            return;
        }

        var location = _mapper.Map<Location>(user.Location);

        var departments = await _uow.Departments.GetOrderedDepartments(
            user.NearCity!.Name,
            user.SelectedCurrency!.Name,
            location,
            order,
            page,
            take);

        var format =
            "Format:\n" +
            "===============================\n" +
            "Distance Bank_Name\n" +
            "Exchange_Rate [Buy | Sell]\n" +
            "===============================\n" +
            $"{user.SelectedCurrency.Name} - {(user.IsBuyOperation == true ? "Buy" : "Sell")}";

        InlineKeyboardMarkup markup;
        InlineKeyboardButton[] backToMenu =
        {
            InlineKeyboardButton.WithCallbackData("Menu", $"{MainCallback.Name}")
        };
        if (departments.Count == 0)
        {
            const string text = "Empty";
            markup = new InlineKeyboardMarkup(backToMenu);
            await _bot.EditMessageTextAsync(args.ChatId, args.MessageId, text, replyMarkup: markup);
            return;
        }

        var buttons = new List<InlineKeyboardButton[]>();
        foreach (var department in departments.Departments)
        {
            var currency = department.CurrencyExchange;
            var text =
                $"{department.Distance * 1000:f0} {department.Department.Bank.Name}\n{currency.Buy.ToString("F")} | {currency.Sell.ToString("F")}";
            // \n{department.Department.Street}

            buttons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text, $"{DepartmentCallback.Name}.{department.Department.Id}")
            });
        }

        Stack<InlineKeyboardButton> pagingButtons = new();

        if (departments.Count - page * take > take)
        {
            pagingButtons.Push(InlineKeyboardButton.WithCallbackData("Next",
                $"{BankCallback.Name}.{callbackDataName}.{page + 1}"));
        }

        if (page != 0)
        {
            pagingButtons.Push(InlineKeyboardButton.WithCallbackData("Back",
                $"{BankCallback.Name}.{callbackDataName}.{page - 1}"));
        }

        buttons.Add(pagingButtons.ToArray());
        buttons.Add(backToMenu);
        markup = new InlineKeyboardMarkup(buttons);

        if (page == 0)
        {
            await _bot.EditMessageTextAsync(args.ChatId, args.MessageId, format, replyMarkup: markup);
        }
        else
        {
            await _bot.EditMessageReplyMarkupAsync(args.ChatId, args.MessageId, markup);
        }
    }
}