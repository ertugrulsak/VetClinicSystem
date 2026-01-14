namespace VetClinicAPI.Models
{
    public class PetOwner
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<Pet> Pets { get; set; } = new List<Pet>();
    }
}