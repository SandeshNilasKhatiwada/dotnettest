namespace SydneyFitnessStudio.Models;
public class Person
{
    public int PersonId { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; } // "Client" or "Staff"
}
