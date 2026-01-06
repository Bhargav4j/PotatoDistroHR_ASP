using Microsoft.Extensions.Logging;
using PotatoDistroHR.Domain.Entities;
using PotatoDistroHR.Domain.Exceptions;
using PotatoDistroHR.Domain.Interfaces.Repositories;
using PotatoDistroHR.Domain.Interfaces.Services;

namespace PotatoDistroHR.Application.Services;

/// <summary>
/// Service implementation for Department operations
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all departments");
            return await _departmentRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            throw;
        }
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving department with ID: {DepartmentId}", id);
            var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (department == null)
            {
                _logger.LogWarning("Department with ID {DepartmentId} not found", id);
            }
            return department;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID: {DepartmentId}", id);
            throw;
        }
    }

    public async Task<Department> CreateAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new department: {DepartmentName}", department.Name);
            department.CreatedDate = DateTime.UtcNow;
            department.IsActive = true;
            var result = await _departmentRepository.AddAsync(department, cancellationToken);
            _logger.LogInformation("Department created successfully with ID: {DepartmentId}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department: {DepartmentName}", department.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating department with ID: {DepartmentId}", id);
            var existingDepartment = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (existingDepartment == null)
            {
                throw new EntityNotFoundException(nameof(Department), id);
            }

            department.Id = id;
            department.ModifiedDate = DateTime.UtcNow;
            await _departmentRepository.UpdateAsync(department, cancellationToken);
            _logger.LogInformation("Department with ID {DepartmentId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department with ID: {DepartmentId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting department with ID: {DepartmentId}", id);
            var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (department == null)
            {
                throw new EntityNotFoundException(nameof(Department), id);
            }

            await _departmentRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Department with ID {DepartmentId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department with ID: {DepartmentId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Department>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching departments with term: {SearchTerm}", searchTerm);
            return await _departmentRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching departments with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
