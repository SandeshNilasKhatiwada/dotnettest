namespace SydneyFitnessStudio.Models;
public class Reservation
{
    public int ReservationId { get; set; }

    public int PersonId { get; set; }
    public Person? Person { get; set; }

    public int FitnessClassId { get; set; }
    public FitnessClass? FitnessClass { get; set; }

    public DateTime ReservationDate { get; set; }
}
