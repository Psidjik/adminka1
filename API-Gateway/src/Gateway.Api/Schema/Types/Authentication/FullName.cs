namespace Gateway.Api.Schema.Types.Authentication;

/// <summary>
/// Полное имя пользователя
/// </summary>
public class FullName
{
    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// Фамилия
    /// </summary>
    public string Surname { get; private set; }
    
    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; private set; }
}
