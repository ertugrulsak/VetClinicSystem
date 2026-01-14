using System.Net;
using System.Net.Http.Json;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.SystemTests
{
   
    public class SystemTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public SystemTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        //  YENİ MÜŞTERİ KAYDI VE HASTA KABUL SÜRECİ
        [Fact]
        public async Task Scenario1_PatientOnboarding_Flow()
        {
            var owner = new PetOwner { FirstName = "Ahmet", LastName = "Sistem", Email = "ahmet@sys.com" };
            var ownerResponse = await _client.PostAsJsonAsync("/api/PetOwners", owner);
            ownerResponse.EnsureSuccessStatusCode();
            var createdOwner = await ownerResponse.Content.ReadFromJsonAsync<PetOwner>();

            var pet = new Pet
            {
                Name = "Pamuk",
                Species = "Kedi",
                Age = 2,
                PetOwnerId = createdOwner!.Id 
            };
            var petResponse = await _client.PostAsJsonAsync("/api/Pets", pet);
            petResponse.EnsureSuccessStatusCode();
            var createdPet = await petResponse.Content.ReadFromJsonAsync<Pet>();

            var getPetResponse = await _client.GetAsync($"/api/Pets/{createdPet!.Id}");
            var fetchedPet = await getPetResponse.Content.ReadFromJsonAsync<Pet>();

            Assert.NotNull(fetchedPet);
            Assert.Equal(createdOwner.Id, fetchedPet.PetOwnerId);
        }

        // TAM KLİNİK MUAYENE DÖNGÜSÜ
        [Fact]
        public async Task Scenario2_ClinicalVisit_CompleteFlow()
        {
            var ownerRes = await _client.PostAsJsonAsync("/api/PetOwners", new PetOwner { FirstName = "Senaryo2" });
            var owner = await ownerRes.Content.ReadFromJsonAsync<PetOwner>();

            var petRes = await _client.PostAsJsonAsync("/api/Pets", new Pet { Name = "Karabaş", PetOwnerId = owner!.Id });
            var pet = await petRes.Content.ReadFromJsonAsync<Pet>();

            var vetRes = await _client.PostAsJsonAsync("/api/Veterinarians", new Veterinarian { FullName = "Dr. House" });
            var vet = await vetRes.Content.ReadFromJsonAsync<Veterinarian>();

            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddHours(2),
                Notes = "Genel Muayene",
                PetId = pet!.Id,
                VeterinarianId = vet!.Id
            };
            var apptRes = await _client.PostAsJsonAsync("/api/Appointments", appointment);
            apptRes.EnsureSuccessStatusCode();
            var createdAppt = await apptRes.Content.ReadFromJsonAsync<Appointment>();

            var treatment = new Treatment
            {
                Description = "Kuduz Aşısı",
                Price = 500.0m,
                AppointmentId = createdAppt!.Id
            };
            var treatRes = await _client.PostAsJsonAsync("/api/Treatments", treatment);
            treatRes.EnsureSuccessStatusCode();

            var finalCheck = await _client.GetAsync($"/api/Treatments/{createdAppt.Id}");
            Assert.Equal(HttpStatusCode.Created, treatRes.StatusCode);
        }

        //HATALI VERİ GİRİŞİ VE DÜZELTME SÜRECİ
        [Fact]
        public async Task Scenario3_DataCorrection_Flow()
        {
            var pet = new Pet { Name = "Maviş", Species = "Kuş" };

            var ownerRes = await _client.PostAsJsonAsync("/api/PetOwners", new PetOwner { FirstName = "Dummy" });
            var owner = await ownerRes.Content.ReadFromJsonAsync<PetOwner>();
            pet.PetOwnerId = owner!.Id;

            var postRes = await _client.PostAsJsonAsync("/api/Pets", pet);
            var createdPet = await postRes.Content.ReadFromJsonAsync<Pet>();

            createdPet!.Species = "Muhabbet Kuşu";
            var putRes = await _client.PutAsJsonAsync($"/api/Pets/{createdPet.Id}", createdPet);
            putRes.EnsureSuccessStatusCode();

            var getRes = await _client.GetAsync($"/api/Pets/{createdPet.Id}");
            var updatedPet = await getRes.Content.ReadFromJsonAsync<Pet>();

            Assert.Equal("Muhabbet Kuşu", updatedPet!.Species);
        }

        // RANDEVU İPTAL SÜRECİ 
        [Fact]
        public async Task Scenario4_AppointmentCancellation_Flow()
        {
            var owner = await (await _client.PostAsJsonAsync("/api/PetOwners", new PetOwner { FirstName = "İptalci" })).Content.ReadFromJsonAsync<PetOwner>();
            var pet = await (await _client.PostAsJsonAsync("/api/Pets", new Pet { Name = "Test", PetOwnerId = owner!.Id })).Content.ReadFromJsonAsync<Pet>();
            var vet = await (await _client.PostAsJsonAsync("/api/Veterinarians", new Veterinarian { FullName = "Dr. Strange" })).Content.ReadFromJsonAsync<Veterinarian>();

            var appt = new Appointment { Notes = "İptal edilecek", PetId = pet!.Id, VeterinarianId = vet!.Id };
            var createRes = await _client.PostAsJsonAsync("/api/Appointments", appt);
            var createdAppt = await createRes.Content.ReadFromJsonAsync<Appointment>();

            var delRes = await _client.DeleteAsync($"/api/Appointments/{createdAppt!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, delRes.StatusCode);

            var checkRes = await _client.GetAsync($"/api/Appointments/{createdAppt.Id}");
            Assert.Equal(HttpStatusCode.NotFound, checkRes.StatusCode);
        }

        // PERSONEL DEĞİŞİMİ VE İŞ AKIŞI
        [Fact]
        public async Task Scenario5_StaffManagement_Flow()
        {
            var newVet = new Veterinarian { FullName = "Dr. Yeni", YearsOfExperience = 0 };
            var postRes = await _client.PostAsJsonAsync("/api/Veterinarians", newVet);
            var createdVet = await postRes.Content.ReadFromJsonAsync<Veterinarian>();

            createdVet!.YearsOfExperience = 5;
            createdVet.Specialization = "Cerrahi";
            await _client.PutAsJsonAsync($"/api/Veterinarians/{createdVet.Id}", createdVet);

            var listRes = await _client.GetAsync("/api/Veterinarians");
            var vetList = await listRes.Content.ReadFromJsonAsync<List<Veterinarian>>();

            Assert.NotNull(vetList);
            Assert.Contains(vetList, v => v.FullName == "Dr. Yeni" && v.Specialization == "Cerrahi");
        }

        // ÇOKLU EVCİL HAYVAN SAHİPLİĞİ 
        [Fact]
        public async Task Scenario6_MultiPetOwnership_Flow()
        {
            var owner = new PetOwner { FirstName = "Büyük", LastName = "Aile", Email = "aile@test.com" };
            var ownerRes = await _client.PostAsJsonAsync("/api/PetOwners", owner);
            ownerRes.EnsureSuccessStatusCode();
            var createdOwner = await ownerRes.Content.ReadFromJsonAsync<PetOwner>();

            var cat = new Pet
            {
                Name = "Tekir",
                Species = "Kedi",
                Age = 3,
                PetOwnerId = createdOwner!.Id
            };
            await _client.PostAsJsonAsync("/api/Pets", cat);

            var dog = new Pet
            {
                Name = "Karabaş",
                Species = "Köpek",
                Age = 5,
                PetOwnerId = createdOwner.Id 
            };
            await _client.PostAsJsonAsync("/api/Pets", dog);

            var listRes = await _client.GetAsync("/api/Pets");
            var allPets = await listRes.Content.ReadFromJsonAsync<List<Pet>>();

            Assert.NotNull(allPets);
            var ownersPets = allPets.Where(p => p.PetOwnerId == createdOwner.Id).ToList();

            Assert.Equal(2, ownersPets.Count);
            Assert.Contains(ownersPets, p => p.Name == "Tekir");   
            Assert.Contains(ownersPets, p => p.Name == "Karabaş");
        }
    }
}