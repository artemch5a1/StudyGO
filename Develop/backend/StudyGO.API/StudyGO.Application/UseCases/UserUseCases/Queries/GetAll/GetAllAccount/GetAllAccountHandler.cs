using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.UserUseCases.Queries.GetAll.GetAllAccount;

public class GetAllAccountHandler : IRequestHandler<GetAllAccountQuery, Result<List<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAllAccountHandler> _logger;

    public GetAllAccountHandler(
        IUserRepository userRepository, 
        IMapper mapper, 
        ILogger<GetAllAccountHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<List<UserDto>>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Попытка получить всех пользователей");
            
        Result<List<User>> result = await _userRepository.GetAll(cancellationToken);
            
        _logger.LogInformation("Получено {count} аккаунтов", result.Value?.Count);
            
        return result.MapDataTo(_mapper.Map<List<UserDto>>);
    }
}