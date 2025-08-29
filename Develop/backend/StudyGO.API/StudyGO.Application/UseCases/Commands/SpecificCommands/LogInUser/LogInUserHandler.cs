using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Application.Services.Account;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Extensions;

namespace StudyGO.Application.UseCases.Commands.SpecificCommands.LogInUser;

public class LogInUserHandler : IRequestHandler<LogInUserCommand, Result<UserLoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    
    private readonly IPasswordHasher _passwordHasher;
    
    private readonly IJwtTokenProvider _jwtTokenProvider;
    
    private readonly ILogger<UserAccountService> _logger;

    public LogInUserHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher, 
        IJwtTokenProvider jwtTokenProvider, 
        ILogger<UserAccountService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
        _logger = logger;
    }
    
    public async Task<Result<UserLoginResponseDto>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        var credentials = request.UserLogin;
        
        Result<UserLoginResponse> userResult = await GetUserByEmail(credentials.Email, cancellationToken);
            
        if (!userResult.IsSuccess)
            return Result<UserLoginResponseDto>.Failure(userResult.ErrorMessage!, userResult.ErrorType);

        var user = userResult.Value!;

        if (!IsValidCredentials(credentials, user))
        {
            _logger.LogInformation("Аутентификация провалена для {Email}", LoggingExtensions.MaskEmail(credentials.Email));
            return Result<UserLoginResponseDto>.Failure("Invalid credentials", ErrorTypeEnum.AuthenticationError);
        }

        var response = GenerateLoginResponse(user);
        return Result<UserLoginResponseDto>.Success(response);
    }
    
    private async Task<Result<UserLoginResponse>> GetUserByEmail(string email, CancellationToken ct)
    {
        _logger.LogInformation("Получение аккаунта по email: {Email}", LoggingExtensions.MaskEmail(email));

        var result = await _userRepository.GetCredentialByEmail(email.ToLower(), ct);

        if (result.IsSuccess && result.Value != null)
            _logger.LogInformation("Аккаунт был получен {UserId}", result.Value.Id);
        else
            _logger.LogInformation("Аккаунт не существует");

        return result;
    }
    
    private UserLoginResponseDto GenerateLoginResponse(UserLoginResponse user)
    {
        _logger.LogInformation("Аутентификация пройдена, генерация токена...");

        var token = _jwtTokenProvider.GenerateToken(user);

        _logger.LogInformation("Токен успешно сгенерирован");

        return new UserLoginResponseDto
        {
            Id = user.Id,
            Token = token
        };
    }
    
    private bool IsValidCredentials(UserLoginRequest expected, UserLoginResponse actual) =>
        expected.Email.ToLower() == actual.Email &&
        expected.Password.VerifyPassword(actual.PasswordHash, _passwordHasher);
}