namespace PotatoDistroHR.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, int id)
        : base($"{entityName} with ID {id} was not found")
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
