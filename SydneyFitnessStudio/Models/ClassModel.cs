namespace SydneyFitnessStudio.Models;
public class FitnessClass
{
    public int FitnessClassId { get; set; }
    public string? ClassName { get; set; }
    public string? Instructor { get; set; }
    public DateTime Schedule { get; set; }
}
