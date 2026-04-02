using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Querys.Handlers;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, IEnumerable<UserDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUsersByRoleQueryHandler(IUserRepository userRepository,IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<IEnumerable<UserDTO>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersByRole(request.Role,cancellationToken);
        return mapper.Map<IEnumerable<UserDTO>>(users);
    }
}