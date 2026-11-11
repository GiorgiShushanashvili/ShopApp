namespace Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public class NotFoundException(string entity, Guid id)
    : Exception($"{entity} with id '{id}' was not found.");

/// <summary>The write conflicted with a concurrent change or a database constraint.</summary>
public class ConflictException(string message, Exception? inner = null) : Exception(message, inner);
