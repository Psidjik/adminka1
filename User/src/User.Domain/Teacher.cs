using User.Domain.Common.Domain;
using User.Domain.ValueObject;

namespace User.Domain;

public class Teacher : Entity<Guid>
{
    public string PersonalNumber { get; private set; }
    public string Password { get; private set; }
    public FullName FullName { get; private set; }
    
    /// <summary>
    /// Список RefreshTokens для этого пользователя.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    public Teacher(string personalNumber, string password, FullName fullName)
    {
        Id = Guid.NewGuid();
        PersonalNumber = personalNumber;
        Password = password;
        FullName = fullName;
    }

    protected Teacher() { }


    public void SetPassword(string password)
    {
        Password = BCrypt.Net.BCrypt.HashPassword(password);
    }


    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, Password);
    }


    public void ChangeFullName(FullName newFullName)
    {
        FullName = newFullName ?? throw new ArgumentNullException(nameof(newFullName));
    }
}