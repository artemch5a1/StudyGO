using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Application.UseCases.Subscribers.RegisteredEvent;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Commands.RegistryUser;

public class RegistryUserHandler : IRequestHandler<RegistryUserCommand, Result<UserRegistryResponse>>
{
    private readonly IUserProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<RegistryUserHandler> _logger;

    private readonly IPasswordHasher _passwordHasher;
    
    private readonly UserProfileServiceOptions _options;

    private readonly IMediator _mediator;

    public RegistryUserHandler(
        IUserProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<RegistryUserHandler> logger, 
        IPasswordHasher passwordHasher,
        IOptionsSnapshot<UserProfileServiceOptions> options, 
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _mediator = mediator;
        _options = options.Value;
    }


    public async Task<Result<UserRegistryResponse>> Handle(RegistryUserCommand request, CancellationToken cancellationToken)
    {
        var profile = request.Profile;
        
        var resultCreate = await RegistryLogic(profile, cancellationToken);
        
        if(!resultCreate.IsSuccess)
            return resultCreate.MapDataTo(UserRegistryResponse.WithoutVerified);

        var id = resultCreate.Value;
        
        var information = new RegisteredInformation(
            id, 
            request.ConfirmEmailEndpoint, 
            profile.User.Email, 
            _options.RequireEmailVerification,
            _options.SchemeRegistry
        );
        
        await _mediator.Publish(new RegisteredEvent(information), cancellationToken);
        
        if (!_options.RequireEmailVerification)
            return resultCreate.MapDataTo(UserRegistryResponse.WithoutVerified);
        
        return resultCreate
            .MapDataTo(x => 
                UserRegistryResponse
                    .VerifiedByAnotherScheme(x, _options.SchemeRegistry));
    }
    
    private async Task<Result<Guid>> RegistryLogic(UserProfileRegistrDto profile,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Попытка регистрации пользователя с email: {Email}", 
            LoggingExtensions.MaskEmail(profile.User.Email));
        
        _logger.LogDebug("Валидация прошла успешно. Хеширование пароля...");
        profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);
            
        _logger.LogDebug("Маппинг...");
            
        UserProfile profileModel = _mapper.Map<UserProfile>(profile);
            
        _logger.LogDebug("Отправлен запрос в репозиторий");
            
        return await _userRepository.Create(profileModel, cancellationToken);
    }
}