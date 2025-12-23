using User.Domain;

namespace User.Application;

public interface IJwtService
{
    /// <summary>
    /// Генерирует новый Access Token.
    /// </summary>
    /// <param name="teacher">Модель для аутентификации.</param>
    /// <returns>Строка с JWT Access Token.</returns>
    public string GenerateAccessToken(Teacher teacher);
    
    /// <summary>
    /// Генерирует случайный Refresh Token.
    /// </summary>
    /// <returns>Строка с Refresh Token.</returns>
    public string GenerateRefreshToken();
    
    /// <summary>
    /// Сохраняет Refresh Token в базе данных.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <param name="refreshToken">Refresh Token.</param>
    public Task SaveRefreshToken(Guid userId, string refreshToken);
    
    /// <summary>
    /// Обновляет Access Token, используя действительный Refresh Token.
    /// </summary>
    /// <param name="refreshToken">Строка Refresh Token.</param>
    /// <returns>Новый Access Token или null, если Refresh Token недействителен.</returns>
    public Task<string?> RefreshAccessToken(string refreshToken);

    /// <summary>
    /// Получение UserId из accessToken.
    /// </summary>
    /// <returns>UserID.</returns>
    public Guid? GetUserIdFromAccessToken(string accessToken);

    /// <summary>
    /// Проверка валидности AccessToken.
    /// </summary>
    /// <returns>Ok.</returns>
    public Task<string> ValidateAccessToken(string accessToken);

}
