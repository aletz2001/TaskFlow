namespace TaskFlow.Api.Domain;
public sealed class DomainException(string message) : Exception(message);