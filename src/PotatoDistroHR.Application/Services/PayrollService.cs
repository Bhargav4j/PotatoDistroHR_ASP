using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Application.Services;

/// <summary>
/// Service implementation for Payroll operations
/// </summary>
public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<PayrollService> _logger;

    public PayrollService(IPayrollRepository payrollRepository, IEmployeeRepository employeeRepository, ILogger<PayrollService> logger)
    {
        _payrollRepository = payrollRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Payroll>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all payrolls");
            return await _payrollRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all payrolls");
            throw;
        }
    }

    public async Task<Payroll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving payroll with ID: {PayrollId}", id);
            return await _payrollRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payroll with ID: {PayrollId}", id);
            throw;
        }
    }

    public async Task<Payroll> CreateAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new payroll for employee ID: {EmployeeId}", payroll.EmployeeId);
            payroll.CreatedDate = DateTime.UtcNow;
            payroll.IsActive = true;
            payroll.NetSalary = payroll.BaseSalary + payroll.Bonus - payroll.Deductions;
            var result = await _payrollRepository.AddAsync(payroll, cancellationToken);
            _logger.LogInformation("Payroll created successfully with ID: {PayrollId}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payroll for employee ID: {EmployeeId}", payroll.EmployeeId);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Payroll payroll, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating payroll with ID: {PayrollId}", id);
            var existingPayroll = await _payrollRepository.GetByIdAsync(id, cancellationToken);
            if (existingPayroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), id);
            }

            payroll.Id = id;
            payroll.ModifiedDate = DateTime.UtcNow;
            payroll.NetSalary = payroll.BaseSalary + payroll.Bonus - payroll.Deductions;
            await _payrollRepository.UpdateAsync(payroll, cancellationToken);
            _logger.LogInformation("Payroll with ID {PayrollId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payroll with ID: {PayrollId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting payroll with ID: {PayrollId}", id);
            var payroll = await _payrollRepository.GetByIdAsync(id, cancellationToken);
            if (payroll == null)
            {
                throw new EntityNotFoundException(nameof(Payroll), id);
            }

            await _payrollRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Payroll with ID {PayrollId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting payroll with ID: {PayrollId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Payroll>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving payrolls for employee ID: {EmployeeId}", employeeId);
            return await _payrollRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payrolls for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<IEnumerable<Payroll>> GetByPayPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving payrolls for pay period: {StartDate} to {EndDate}", startDate, endDate);
            return await _payrollRepository.GetByPayPeriodAsync(startDate, endDate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payrolls for pay period: {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<Payroll> GeneratePayrollAsync(int employeeId, DateTime payPeriodStart, DateTime payPeriodEnd, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating payroll for employee ID: {EmployeeId}", employeeId);
            var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
            if (employee == null)
            {
                throw new EntityNotFoundException(nameof(Employee), employeeId);
            }

            var payroll = new Payroll
            {
                EmployeeId = employeeId,
                BaseSalary = employee.Salary,
                Bonus = 0,
                Deductions = 0,
                PayPeriodStart = payPeriodStart,
                PayPeriodEnd = payPeriodEnd,
                PayDate = payPeriodEnd.AddDays(7),
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System"
            };

            payroll.NetSalary = payroll.BaseSalary + payroll.Bonus - payroll.Deductions;
            var result = await _payrollRepository.AddAsync(payroll, cancellationToken);
            _logger.LogInformation("Payroll generated successfully for employee ID: {EmployeeId}", employeeId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating payroll for employee ID: {EmployeeId}", employeeId);
            throw;
        }
    }
}
