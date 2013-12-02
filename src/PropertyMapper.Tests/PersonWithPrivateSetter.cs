namespace PropertyMapper.Tests
{
  public class PersonWithPrivateSetter
  {
    public PersonWithPrivateSetter(string lastName)
    {
      LastName = lastName;
    }

    public string FirstName { get; set; }
    public string LastName { get; private set; }
    public int Age { get; set; }
  }
}