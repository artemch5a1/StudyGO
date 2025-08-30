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

namespace StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUserPassword;

public class UpdateUserPasswordHandler : 
    IRequestHandler<UpdateUserPasswordCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<UpdateUserPasswordHandler> _logger;
    
    private readonly IPasswordHasher _passwordHasher;
    
    public UpdateUserPasswordHandler(
        IUserRepository userRepository, 
        IMapper mapper, 
        ILogger<UpdateUserPasswordHandler> logger, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.User;
        _logger.LogInformation("Обновление учетных данных аккаунта: {UserId}", dto.UserId);
        
        var userResult = await GetUserById(dto.UserId, cancellationToken);
        if (!userResult.IsSuccess)
            return Result<Guid>.Failure("Неверный ID", ErrorTypeEnum.NotFound);

        var existingUser = userResult.Value!;
        
        if (!IsValidOldPassword(dto.OldPassword, existingUser.PasswordHash))
            return Result<Guid>.Failure("Неверный пароль", ErrorTypeEnum.AuthenticationError);

        string passwordHash = dto.Password.HashedPassword(_passwordHasher);
        
        return await UpdateCredentials(dto.UserId, passwordHash, cancellationToken);
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

    private async Task<Result<Guid>> UpdateCredentials(Guid userId, string password, CancellationToken ct)
    {
        _logger.LogDebug("Сохранение обновленных данных в репозитории");
        return await _userRepository.UpdatePassword(userId, password, ct);
    }
}