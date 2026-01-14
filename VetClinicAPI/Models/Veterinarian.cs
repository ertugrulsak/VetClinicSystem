using System.Text.Json.Serialization; 

namespace VetClinicAPI.Models
{
    public class Veterinarian
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        [JsonIgnore]
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}