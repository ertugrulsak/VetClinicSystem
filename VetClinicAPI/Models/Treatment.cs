using System.Text.Json.Serialization;

namespace VetClinicAPI.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty; 
        public decimal Price { get; set; } 
        public string MedicationGiven { get; set; } = string.Empty;
        public int AppointmentId { get; set; }
        [JsonIgnore]
        public Appointment? Appointment { get; set; }
    }
}