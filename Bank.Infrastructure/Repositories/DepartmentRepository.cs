using Bank.Application.Interfaces;
using Bank.Application.Models;
using Bank.Domain.Entities;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly IBankContext _db;
    private readonly IUpdateExchangeInformation _updateExchange;

    public DepartmentRepository(IBankContext db, IUpdateExchangeInformation updateExchange)
    {
        _db = db;
        _updateExchange = updateExchange;
    }

    public async Task<DepartmentsCounter> GetOrderedDepartments(
        string cityName,
        string currName,
        Location location,
        Func<IEnumerable<DepartmentByDistance>, IOrderedEnumerable<DepartmentByDistance>> order,
        int page,
        int take = 10,
        bool onUpdate = true)
    {
        var collections = await GetDepartmentsWithDistance(cityName, currName, location, onUpdate);

        var list = order.Invoke(collections).ToList();

        DepartmentsCounter departments = new()
        {
            Count = list.Count,
            Departments = list.Skip(page * take).Take(take)
        };

        return departments;
    }

    public async Task<Department?> GetDepartmentWithLocationAndCurrencyAsync(int id)
    {
        var department = await _db.Departments
            .Include(d => d.Location)
            .Include(d => d.Currencies)
            .FirstOrDefaultAsync(d => d.Id == id);

        return department;
    }

    public async Task<Department?> GetDepartmentWithLocationAndCurrencyAndBankAsync(int id)
    {
        var department = await _db.Departments
            .Include(d => d.Bank)
            .Include(d => d.Location)
            .Include(d => d.Currencies)
            .FirstOrDefaultAsync(d => d.Id == id);

        return department;
    }

    private async Task<IEnumerable<DepartmentByDistance>> GetDepartmentsWithDistance(
        string cityName,
        string currName,
        Location location,
        bool onUpdate = true)
    {
        if (onUpdate)
        {
            await _updateExchange.UpdateAsync(cityName);
        }

        var collections = _db.Banks
            .Include(b => b.Departments.Where(d => d.Currencies.Exists(c => c.Currency.Name == currName)))
            .ThenInclude(d => d.Location)
            .Where(b => b.Departments.Count != 0)
            .SelectMany(b => b.Departments.Where(d => d.Location != null))
            .Include(d => d.City)
            .Include(d => d.Bank)
            .Include(d => d.Currencies.Where(c => c.Currency.Name == currName))
            .AsEnumerable()
            .Select(d => new DepartmentByDistance
            {
                Distance = Location.Distance(d.Location!, location),
                Department = d,
                CurrencyExchange = d.Currencies.First()
            })
            .Where(d => d.Distance <= d.Department.City.Radius);

        return collections;
    }
}