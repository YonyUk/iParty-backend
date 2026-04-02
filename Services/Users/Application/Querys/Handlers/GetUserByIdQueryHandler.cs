using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Querys.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDTO>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetById(request.Id, cancellationToken);
        return mapper.Map<UserDTO>(user);
    }
}