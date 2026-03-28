using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Commands.Handlers;

public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, UserDTO>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUserByIdCommandHandler(IUserRepository userRepository,IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<UserDTO> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetById(request.Id,cancellationToken);
        return mapper.Map<UserDTO>(user);
    }
}