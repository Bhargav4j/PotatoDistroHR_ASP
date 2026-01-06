using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Application.Services;

/// <summary>
/// Service implementation for Employee operations
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all employees");
            return await _employeeRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all employees");
            throw;
        }
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving employee with ID: {EmployeeId}", id);
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
            }
            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee with ID: {EmployeeId}", id);
            throw;
        }
    }

    public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new employee: {FirstName} {LastName}", employee.FirstName, employee.LastName);
            employee.CreatedDate = DateTime.UtcNow;
            employee.IsActive = true;
            var result = await _employeeRepository.AddAsync(employee, cancellationToken);
            _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee: {FirstName} {LastName}", employee.FirstName, employee.LastName);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Employee employee, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);
            var existingEmployee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (existingEmployee == null)
            {
                throw new EntityNotFoundException(nameof(Employee), id);
            }

            employee.Id = id;
            employee.ModifiedDate = DateTime.UtcNow;
            await _employeeRepository.UpdateAsync(employee, cancellationToken);
            _logger.LogInformation("Employee with ID {EmployeeId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee with ID: {EmployeeId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
            {
                throw new EntityNotFoundException(nameof(Employee), id);
            }

            await _employeeRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Employee with ID {EmployeeId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee with ID: {EmployeeId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching employees with term: {SearchTerm}", searchTerm);
            return await _employeeRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching employees with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving employees for department ID: {DepartmentId}", departmentId);
            return await _employeeRepository.GetByDepartmentAsync(departmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees for department ID: {DepartmentId}", departmentId);
            throw;
        }
    }
}
