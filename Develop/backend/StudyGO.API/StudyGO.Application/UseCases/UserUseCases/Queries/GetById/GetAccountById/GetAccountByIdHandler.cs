using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Services.Account;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.UserUseCases.Queries.GetById.GetAccountById;

public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, Result<UserDto?>>
{
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAccountByIdHandler> _logger;

    public GetAccountByIdHandler(
        IUserRepository userRepository, 
        IMapper mapper, 
        ILogger<GetAccountByIdHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<UserDto?>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Попытка получить аккаунт по id {UserId}", request.Id);
        Result<User?> result = await _userRepository.GetById(request.Id, cancellationToken);
            
        _logger.LogResult(result, 
            "Успешно найден пользователь",
            "Пользователь не найден",
            new {UserId = request.Id});
            
        return result.MapDataTo(x => _mapper.Map<UserDto?>(x));
    }
}