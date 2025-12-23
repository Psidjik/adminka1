using User.Domain.Common.Domain;

namespace User.Domain;

/// <summary>
/// RefreshToken
/// </summary>
public class RefreshToken : Entity<Guid>
{
    public RefreshToken(Guid teacherId, string token, DateTimeOffset expiresAt)
    {
        TeacherId = teacherId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    private RefreshToken()
    {
    }

    /// <summary>
    /// Id пользователя
    /// </summary>
    public Guid TeacherId { get; set; }
    
    /// <summary>
    /// Токен
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// Время, когда токен истечет
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }
    
    /// <summary>
    /// Время создания токена
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Истек ли токен
    /// </summary>
    public bool IsExpired() => DateTimeOffset.UtcNow >= ExpiresAt;
    
    /// <summary>
    /// Пользователь
    /// </summary>
    public Teacher Teacher { get; private set; }
}
