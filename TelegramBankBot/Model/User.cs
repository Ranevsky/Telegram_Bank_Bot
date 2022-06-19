using System.ComponentModel.DataAnnotations;

namespace TelegramBankBot.Model;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
