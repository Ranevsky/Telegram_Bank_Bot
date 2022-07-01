using System.ComponentModel.DataAnnotations;

using TelegramBankBot.Model.Interfaces;

namespace TelegramBankBot.Model;

[Serializable]
public class User : IEntityId
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}
