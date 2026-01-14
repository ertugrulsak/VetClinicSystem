using Microsoft.AspNetCore.Mvc;
using VetClinicAPI.Controllers;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.UnitTests
{
    public class VeterinariansControllerTests
    {
        // Veterinerlerin listelenmesi
        [Fact]
        public async Task GetVeterinarians_ListeDoluysa_Gelmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Veterinarians.Add(new Veterinarian { Id = 1, FullName = "Dr. Ali" });
            await context.SaveChangesAsync();

            var controller = new VeterinariansController(context);
            var result = await controller.GetVeterinarians();

            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        // ID'si verilen veterinerin çalışma alanı isteği testi
        [Fact]
        public async Task GetVeterinarian_IdDogruysa_AlanGelmeli() 
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Veterinarians.Add(new Veterinarian { Id = 1, FullName = "Dr. Veli", Specialization = "Cerrahi" });
            await context.SaveChangesAsync();

            var controller = new VeterinariansController(context);
            var result = await controller.GetVeterinarian(1);

            Assert.Equal("Cerrahi", result.Value?.Specialization);
        }

        //Veritabanında olmayan veterinerin silme işlemi isteği testi
        [Fact]
        public async Task DeleteVeterinarian_OlmayanId_NotFoundVermeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new VeterinariansController(context);

            var result = await controller.DeleteVeterinarian(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // Yeni veteriner ekleme testi
        [Fact]
        public async Task PostVeterinarian_YeniDoktor_Eklenmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new VeterinariansController(context);
            var vet = new Veterinarian { Id = 5, FullName = "Dr. Ayşe" };

            var result = await controller.PostVeterinarian(vet);

            var donenDeger = Assert.IsType<CreatedAtActionResult>(result.Result);
            var eklenenVet = donenDeger.Value as Veterinarian;

            Assert.NotNull(eklenenVet);
            Assert.Equal(5, eklenenVet.Id);
        }

        // Veterinerin uzmanlık alanı güncellemesi testi
        [Fact]
        public async Task PutVeterinarian_Uzmanlik_Guncellenmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Veterinarians.Add(new Veterinarian { Id = 1, FullName = "Dr. Test", Specialization = "Yok" });
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var controller = new VeterinariansController(context);
            var guncelVet = new Veterinarian { Id = 1, FullName = "Dr. Test", Specialization = "Dahiliye" };

            var result = await controller.PutVeterinarian(1, guncelVet);

            Assert.IsType<NoContentResult>(result);
            var dbKayit = await context.Veterinarians.FindAsync(1);
            Assert.NotNull(dbKayit);
            Assert.Equal("Dahiliye", dbKayit.Specialization);
        }
    }
}