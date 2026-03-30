using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;
using Users.Domain.Rules;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Handlers;

public class GetUserByUserNameHandler : IRequestHandler<GetUserByUserNameCommand, UserDTO>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IUserDomainRulesConfigProvider provider;
    public GetUserByUserNameHandler
    (
        IUserRepository userRepository,
        IMapper mapper,
        IUserDomainRulesConfigProvider provider
    )
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.provider = provider;
    }
    public async Task<UserDTO> Handle(GetUserByUserNameCommand request, CancellationToken cancellationToken)
    {
        var username = new UserName(request.UserName,provider.UserNameDomainRules);
        var user = await userRepository.GetByName(username);
        return mapper.Map<UserDTO>(user);
    }
}