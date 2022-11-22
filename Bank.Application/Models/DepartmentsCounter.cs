namespace Bank.Application.Models;

public class DepartmentsCounter
{
    public IEnumerable<DepartmentByDistance> Departments { get; set; } = null!;
    public int Count { get; set; }
}