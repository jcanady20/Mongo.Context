namespace Mongo.Context.Example.Entities;

public class Contact
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public string Phone { get; set; }
    public bool IsDeleted { get; set; }

    public Address Address { get; set; }
}

