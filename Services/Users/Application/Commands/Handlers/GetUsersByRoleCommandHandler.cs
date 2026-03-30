using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Commands.Handlers;

public class GetUsersByRoleCommandHandler : IRequestHandler<GetUsersByRoleCommand, IEnumerable<UserDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUsersByRoleCommandHandler(IUserRepository userRepository,IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<IEnumerable<UserDTO>> Handle(GetUsersByRoleCommand request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersByRole(request.Role);
        return mapper.Map<IEnumerable<UserDTO>>(users);
    }
}