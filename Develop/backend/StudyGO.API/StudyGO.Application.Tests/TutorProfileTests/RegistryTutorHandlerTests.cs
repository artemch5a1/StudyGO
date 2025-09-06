using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StudyGO.Application.Options;
using StudyGO.Application.UseCases.TutorProfileUseCases.Commands.RegistryTutor;
using StudyGO.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Models;

namespace StudyGO.Application.Tests.TutorProfileTests;

public class RegistryTutorHandlerTests
{
    private readonly Mock<ITutorProfileRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<RegistryTutorHandler>> _loggerMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    
    private Mock<IOptionsSnapshot<TutorProfileServiceOptions>> _mockOptions = new Mock<IOptionsSnapshot<TutorProfileServiceOptions>>();
    
    private RegistryTutorHandler CreateHandler()
    {
        _mockOptions.Setup(x => x.Value).Returns(new TutorProfileServiceOptions
        {
            SchemeRegistry = RegistryScheme.DefaultVerified,
            RequireEmailVerification = true
        });
        
        return new RegistryTutorHandler(
            _repoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _passwordHasherMock.Object,
           _mockOptions.Object,
            _mediatorMock.Object);
    }

    private RegistryTutorCommand CreateCommand()
    {
        return new RegistryTutorCommand(
            new TutorProfileRegistrDto
            {
                User = new UserCreateDto { Email = "test@test.com", Password = "12345" }
            },
            "http://confirm"
        );
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRepositoryFails()
    {
        var handler = CreateHandler();
        var command = CreateCommand();

        _repoMock
            .Setup(r => 
                r.Create(
                    It.IsAny<TutorProfile>(), 
                    It.IsAny<CancellationToken>()
                    )
                )
            .ReturnsAsync(Result<Guid>.Failure("error"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), default), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldHashPassword_BeforeSaving()
    {
        // Arrange
        var handler = CreateHandler();
        var command = CreateCommand();

        _passwordHasherMock.Setup(h => h.GeneratePasswordHash("12345")).Returns("hashed");

        _mapperMock.Setup(m => m.Map<TutorProfile>(It.IsAny<TutorProfileRegistrDto>()))
            .Returns<TutorProfileRegistrDto>(dto =>
            {
                Assert.Equal("hashed", dto.User.Password);
               return It.IsAny<TutorProfile>();
            });

        _repoMock.Setup(
                r => r.Create(
                    It.IsAny<TutorProfile>(), 
                    It.IsAny<CancellationToken>()
                )
                )
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(h => h.GeneratePasswordHash("12345"), Times.Once);
    }
}