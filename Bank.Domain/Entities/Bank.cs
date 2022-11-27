namespace Bank.Domain.Entities;

public class Bank : BaseBank
{
    public List<Department> Departments { get; set; } = new();
}