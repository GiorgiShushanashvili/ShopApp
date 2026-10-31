namespace Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public class NotFoundException(string entity, Guid id)
    : Exception($"{entity} with id '{id}' was not found.");
