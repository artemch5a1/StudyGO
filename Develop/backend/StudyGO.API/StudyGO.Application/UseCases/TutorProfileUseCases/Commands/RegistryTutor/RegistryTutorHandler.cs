using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Application.UseCases.Subscribers.RegisteredEvent;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Commands.RegistryTutor;

public class RegistryTutorHandler : IRequestHandler<RegistryTutorCommand, Result<UserRegistryResponse>>
{
    private readonly ITutorProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<RegistryTutorHandler> _logger;

    private readonly IPasswordHasher _passwordHasher;
    
    private readonly TutorProfileServiceOptions _options;
    
    private readonly IMediator _mediator;
    
    public RegistryTutorHandler(
        ITutorProfileRepository userRepository,
        IMapper mapper, 
        ILogger<RegistryTutorHandler> logger, 
        IPasswordHasher passwordHasher, 
        IOptionsSnapshot<TutorProfileServiceOptions> options, 
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _options = options.Value;
        _mediator = mediator;
    }

    public async Task<Result<UserRegistryResponse>> Handle(RegistryTutorCommand request, CancellationToken cancellationToken)
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
            _options.SchemeReqistry
            );
        
        await _mediator.Publish(new RegisteredEvent(information), cancellationToken);
        
        if (!_options.RequireEmailVerification)
            return resultCreate.MapDataTo(UserRegistryResponse.WithoutVerified);
        
        return resultCreate
            .MapDataTo(x => 
                UserRegistryResponse
                    .VerifiedByAnotherScheme(x, _options.SchemeReqistry));
    }
    
    private async Task<Result<Guid>> RegistryLogic(TutorProfileRegistrDto profile,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Попытка регистрации учителя с email: {Email}", 
            LoggingExtensions.MaskEmail(profile.User.Email));
            
        _logger.LogDebug("Валидация прошла успешно. Хеширование пароля...");
        profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);
            
        _logger.LogDebug("Маппинг...");
            
        TutorProfile profileModel = _mapper.Map<TutorProfile>(profile);
            
        _logger.LogDebug("Отправлен запрос в репозиторий");
            
        return await _userRepository.Create(profileModel, cancellationToken);
    }
}