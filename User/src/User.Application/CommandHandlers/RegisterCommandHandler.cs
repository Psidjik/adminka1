using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using User.Application.Command;
using User.Application.Models;
using User.Domain;
using User.Domain.Data;

namespace User.Application.CommandHandlers;

public class RegisterCommandHandler(UserDbContext authServiceDbContext) : IRequestHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await authServiceDbContext.Teachers.AnyAsync(u => u.PersonalNumber == request.PersonalNumber, cancellationToken))
            throw new AuthenticationFailureException("Этот email уже занят");
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        var teacher = new Teacher(request.PersonalNumber, hashedPassword, request.FullName );
        
        await authServiceDbContext.Teachers.AddAsync(teacher, cancellationToken);

        await authServiceDbContext.SaveChangesAsync(cancellationToken);
        
        return new RegisterResult();
    }
}
