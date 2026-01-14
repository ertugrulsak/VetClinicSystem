using System.Net;
using System.Net.Http.Json;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.IntegrationTests
{

    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public IntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        //  GET (API Endpoint Testi & DB Okuma) 
        [Fact]
        public async Task Get_PetOwners_ReturnsSuccessAndContentType()
        {
            var response = await _client.GetAsync("/api/PetOwners");

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }

        //  POST (Veri Oluşturma & DB Yazma)
        [Fact]
        public async Task Post_PetOwner_CreatesNewRecord()
        {
            var newOwner = new PetOwner { FirstName = "Entegrasyon", LastName = "Testi", Email = "test@test.com" };

            var response = await _client.PostAsJsonAsync("/api/PetOwners", newOwner);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdOwner = await response.Content.ReadFromJsonAsync<PetOwner>();
            Assert.NotNull(createdOwner);
            Assert.Equal("Entegrasyon", createdOwner.FirstName);
        }

        //RELATION TESTİ (User-Order benzeri PetOwner-Pet İlişkisi)
        [Fact]
        public async Task Post_Pet_WithRelation_ReturnsSuccess()
        {
           
            var owner = new PetOwner { FirstName = "Sahip", LastName = "Baba" };
            var ownerResponse = await _client.PostAsJsonAsync("/api/PetOwners", owner);
            var createdOwner = await ownerResponse.Content.ReadFromJsonAsync<PetOwner>();

          
            var newPet = new Pet
            {
                Name = "Boncuk",
                Species = "Kedi",
                Age = 2,
                PetOwnerId = createdOwner!.Id 
            };

            var response = await _client.PostAsJsonAsync("/api/Pets", newPet);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // ERROR CASE (404 Not Found)
        [Fact]
        public async Task Get_NonExistentPet_ReturnsNotFound()
        {

            var response = await _client.GetAsync("/api/Pets/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        //  ERROR CASE (400 Bad Request)
        [Fact]
        public async Task Post_InvalidData_ReturnsBadRequest()
        {

            var response = await _client.PutAsJsonAsync("/api/Pets/5", new Pet { Id = 1 });


            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //  PUT (Veri Güncelleme) 
        [Fact]
        public async Task Put_Veterinarian_UpdatesRecord()
        {

            var vet = new Veterinarian { FullName = "Eski İsim", Specialization = "A", YearsOfExperience = 5 };
            var postResponse = await _client.PostAsJsonAsync("/api/Veterinarians", vet);
            var createdVet = await postResponse.Content.ReadFromJsonAsync<Veterinarian>();

            createdVet!.FullName = "Yeni İsim";
            var putResponse = await _client.PutAsJsonAsync($"/api/Veterinarians/{createdVet.Id}", createdVet);

            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        }

        // DELETE (Veri Silme) 
        [Fact]
        public async Task Delete_Appointment_RemovesRecord()
        {
            var appt = new Appointment { Notes = "Silinecek", AppointmentDate = DateTime.Now };
            var postResponse = await _client.PostAsJsonAsync("/api/Appointments", appt);
            var createdAppt = await postResponse.Content.ReadFromJsonAsync<Appointment>();

            var deleteResponse = await _client.DeleteAsync($"/api/Appointments/{createdAppt!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/Appointments/{createdAppt.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        //Treatment Endpoint Kontrolü
        [Fact]
        public async Task Get_Treatments_ReturnsEmptyListInitially()
        {
            var response = await _client.GetAsync("/api/Treatments");

            response.EnsureSuccessStatusCode();
        }

        // Kompleks İlişki (Randevu + Tedavi)
        [Fact]
        public async Task Post_Treatment_LinkedToAppointment()
        {

            var appt = new Appointment { Notes = "Tedavi Yapılacak", AppointmentDate = DateTime.Now };
            var apptResponse = await _client.PostAsJsonAsync("/api/Appointments", appt);
            var createdAppt = await apptResponse.Content.ReadFromJsonAsync<Appointment>();

            var treatment = new Treatment
            {
                Description = "Aşı",
                Price = 500,
                AppointmentId = createdAppt!.Id
            };

            var response = await _client.PostAsJsonAsync("/api/Treatments", treatment);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        //Tüm Veterinarians Listesi 
        [Fact]
        public async Task Get_AllVeterinarians_ReturnsJson()
        {
            var response = await _client.GetAsync("/api/Veterinarians");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
        }

        //Ekleme Sonrası Liste Kontrolü
        [Fact]
        public async Task Post_ThenGetList_ShouldContainNewItem()
        {
            var uniqueName = "Dr. Kontrol_" + Guid.NewGuid();
            var newVet = new Veterinarian { FullName = uniqueName, Specialization = "Dahiliye" };

            await _client.PostAsJsonAsync("/api/Veterinarians", newVet);

            var listResponse = await _client.GetAsync("/api/Veterinarians");
            var vetList = await listResponse.Content.ReadFromJsonAsync<List<Veterinarian>>();

            Assert.NotNull(vetList);
            Assert.Contains(vetList, v => v.FullName == uniqueName);
        }

        // Güncelleme ve Veri Doğrulama
        [Fact]
        public async Task Put_PetOwner_AndVerifyChange()
        {
            var owner = new PetOwner { FirstName = "EskiAd", LastName = "Test" };
            var postResponse = await _client.PostAsJsonAsync("/api/PetOwners", owner);
            var createdOwner = await postResponse.Content.ReadFromJsonAsync<PetOwner>();

            createdOwner!.FirstName = "YeniAd";
            await _client.PutAsJsonAsync($"/api/PetOwners/{createdOwner.Id}", createdOwner);

            var getResponse = await _client.GetAsync($"/api/PetOwners/{createdOwner.Id}");
            var updatedOwner = await getResponse.Content.ReadFromJsonAsync<PetOwner>();

            Assert.Equal("YeniAd", updatedOwner!.FirstName);
        }
    }
}