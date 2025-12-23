namespace Gateway.Api.Schema.Types.Authentication;

/// <summary>
/// Запрос на вход
/// </summary>
public class LoginUser
{
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public required string PersonalNubmer { get; init; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public required string Password { get; init; }
}
