namespace User.Domain.ValueObject;

public class FullName : Common.Domain.ValueObject
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public string Patronymic { get; init; }

    public FullName(string name, string surname, string? patronymic)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Surname;
        yield return Patronymic;
    }
}