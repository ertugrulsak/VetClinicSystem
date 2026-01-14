namespace VetClinicAPI.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public int Age { get; set; }
        public int PetOwnerId { get; set; }
        public PetOwner? Owner { get; set; } 
    }
}