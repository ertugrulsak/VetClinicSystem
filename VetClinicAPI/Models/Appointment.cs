namespace VetClinicAPI.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; } 
        public string Notes { get; set; } = string.Empty;
        public int PetId { get; set; }
        public Pet? Pet { get; set; }
        public int VeterinarianId { get; set; }
        public Veterinarian? Veterinarian { get; set; }
        public List<Treatment> Treatments { get; set; } = new List<Treatment>();
    }
}