using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUserСredentials;

public class UpdateUserСredentialsHandler : 
    IRequestHandler<UpdateUserСredentialsCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<UpdateUserСredentialsHandler> _logger;
    
    private readonly IPasswordHasher _passwordHasher;
    
    public UpdateUserСredentialsHandler(
        IUserRepository userRepository, 
        IMapper mapper, 
        ILogger<UpdateUserСredentialsHandler> logger, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(UpdateUserСredentialsCommand request, CancellationToken cancellationToken)
    {
        var dto = request.User;
        _logger.LogInformation("Обновление учетных данных аккаунта: {UserId}", dto.UserId);
        
        var userResult = await GetUserById(dto.UserId, cancellationToken);
        if (!userResult.IsSuccess)
            return Result<Guid>.Failure("Неверный ID", ErrorTypeEnum.NotFound);

        var existingUser = userResult.Value!;
        
        if (!IsValidOldPassword(dto.OldPassword, existingUser.PasswordHash))
            return Result<Guid>.Failure("Неверный пароль", ErrorTypeEnum.AuthenticationError);
        
        var updatedUser = PrepareUpdatedUser(dto);
        
        return await UpdateCredentials(updatedUser, cancellationToken);
    }
    
    private async Task<Result<User?>> GetUserById(Guid userId, CancellationToken ct)
    {
        _logger.LogDebug("Получение пользователя по Id: {UserId}", userId);
        return await _userRepository.GetById(userId, ct);
    }

    private bool IsValidOldPassword(string providedPassword, string storedHash)
    {
        _logger.LogDebug("Проверка старого пароля");
        var isValid = providedPassword.VerifyPassword(storedHash, _passwordHasher);

        if (!isValid)
            _logger.LogInformation("Неверный пароль для подтверждения обновления");

        return isValid;
    }

    private User PrepareUpdatedUser(UserUpdateСredentialsDto dto)
    {
        _logger.LogDebug("Успешное подтверждение, хеширование нового пароля...");
        dto.Password = dto.Password.HashedPassword(_passwordHasher);

        _logger.LogDebug("Маппинг DTO в модель User");
        return _mapper.Map<User>(dto);
    }

    private async Task<Result<Guid>> UpdateCredentials(User updatedUser, CancellationToken ct)
    {
        _logger.LogDebug("Сохранение обновленных данных в репозитории");
        return await _userRepository.UpdateСredentials(updatedUser, ct);
    }
}