using Bank.Application.Models;
using Bank.Domain.Entities;
using Base.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetDepartmentWithLocationAndCurrencyAsync(int id);
    Task<Department?> GetDepartmentWithLocationAndCurrencyAndBankAsync(int id);

    Task<DepartmentsCounter> GetOrderedDepartments(
        string cityName,
        string currName,
        Location location,
        Func<IEnumerable<DepartmentByDistance>, IOrderedEnumerable<DepartmentByDistance>> order,
        int page,
        int take = 10,
        bool onUpdate = true);
}