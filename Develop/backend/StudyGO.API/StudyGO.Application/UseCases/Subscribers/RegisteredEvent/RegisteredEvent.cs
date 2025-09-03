using MediatR;
using StudyGO.Contracts.Contracts;

namespace StudyGO.Application.UseCases.Subscribers.RegisteredEvent;

public record RegisteredEvent(RegisteredInformation Information) : INotification;