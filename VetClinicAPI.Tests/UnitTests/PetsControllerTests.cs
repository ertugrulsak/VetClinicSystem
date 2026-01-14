using Microsoft.AspNetCore.Mvc;
using VetClinicAPI.Controllers;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.UnitTests
{
    public class PetsControllerTests
    {
        // Hayvanları listeleme 
        [Fact]
        public async Task GetPets_TumHayvanlari_Getirmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Pets.Add(new Pet { Id = 1, Name = "Boncuk", Species = "Kedi" });
            context.Pets.Add(new Pet { Id = 2, Name = "Karabaş", Species = "Köpek" });
            await context.SaveChangesAsync();

            var controller = new PetsController(context);

            var sonuc = await controller.GetPets();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Pet>>>(sonuc);
            Assert.NotNull(actionResult.Value);
            Assert.Equal(2, actionResult.Value.Count());
        }

        //Tek kayıt listeleme testi
        [Fact]
        public async Task GetPet_IdIle_TekHayvanGetirmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Pets.Add(new Pet { Id = 3, Name = "Maviş", Age = 2 });
            await context.SaveChangesAsync();
            var controller = new PetsController(context);

            var sonuc = await controller.GetPet(3);

            Assert.Equal("Maviş", sonuc.Value?.Name);
            Assert.Equal(2, sonuc.Value?.Age);
        }

        //Kayıtlı olmayan hayvanın bulunamadı olarak dönmesi
        [Fact]
        public async Task GetPet_OlmayanId_HataDonmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new PetsController(context);

            var sonuc = await controller.GetPet(100);

            Assert.IsType<NotFoundResult>(sonuc.Result);
        }

        // Yeni hayvan ekleme
        [Fact]
        public async Task PostPet_HayvanEkleme_Calismali()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new PetsController(context);
            var yeniPet = new Pet { Id = 1, Name = "Pamuk", Species = "Tavşan" };

            var sonuc = await controller.PostPet(yeniPet);

            var createdResult = Assert.IsType<CreatedAtActionResult>(sonuc.Result);
            var eklenenPet = createdResult.Value as Pet;

            Assert.NotNull(eklenenPet);
            Assert.Equal("Tavşan", eklenenPet.Species);
        }

        // Güncelleme Testi
        [Fact]
        public async Task PutPet_IsimGuncelleme_Calismali()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Pets.Add(new Pet { Id = 1, Name = "Eski İsim" });
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var controller = new PetsController(context);
            var guncelPet = new Pet { Id = 1, Name = "Yeni İsim" };

            var sonuc = await controller.PutPet(1, guncelPet);

            Assert.IsType<NoContentResult>(sonuc);
            var dbPet = await context.Pets.FindAsync(1);
            Assert.NotNull(dbPet);
            Assert.Equal("Yeni İsim", dbPet.Name);
        }
    }
}