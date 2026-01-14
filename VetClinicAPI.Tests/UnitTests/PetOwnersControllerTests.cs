using Microsoft.AspNetCore.Mvc;
using VetClinicAPI.Controllers;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.UnitTests
{
    public class PetOwnersControllerTests
    {
        //Veritabanında kayıtlar varsa hepsinin listelenmesi
        [Fact]
        public async Task GetPetOwners_KayitVarsa_ListeyiDonmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.PetOwners.Add(new PetOwner { Id = 1, FirstName = "Ahmet", LastName = "Yilmaz" });
            context.PetOwners.Add(new PetOwner { Id = 2, FirstName = "Ayse", LastName = "Kara" });
            await context.SaveChangesAsync();

            var controller = new PetOwnersController(context);

            var sonuc = await controller.GetPetOwners();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<PetOwner>>>(sonuc);
            var gelenListe = Assert.IsAssignableFrom<IEnumerable<PetOwner>>(actionResult.Value);

            Assert.Equal(2, gelenListe.Count());
        }

        // Var olan bir ID ile sorgu yapılması
        [Fact]
        public async Task GetPetOwner_IdVarsa_DogruKisiyiDonmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.PetOwners.Add(new PetOwner { Id = 5, FirstName = "Mehmet", Email = "m@m.com" });
            await context.SaveChangesAsync();

            var controller = new PetOwnersController(context);

            var sonuc = await controller.GetPetOwner(5);

            Assert.Equal("Mehmet", sonuc.Value?.FirstName);
        }

        // Olmayan bir ID sorulması senaryosu
        [Fact]
        public async Task GetPetOwner_IdYoksa_NotFoundDonmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new PetOwnersController(context);

            var sonuc = await controller.GetPetOwner(99);

            Assert.IsType<NotFoundResult>(sonuc.Result);
        }

        // Yeni bir kayıt ekleyince veritabanı sayısı artması
        [Fact]
        public async Task PostPetOwner_YeniKayit_BasariylaEklenmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new PetOwnersController(context);
            var yeniSahip = new PetOwner { Id = 10, FirstName = "Zeynep", LastName = "Demir" };

            var sonuc = await controller.PostPetOwner(yeniSahip);

            var olusturulan = Assert.IsType<CreatedAtActionResult>(sonuc.Result);
            var model = Assert.IsType<PetOwner>(olusturulan.Value);

            Assert.Equal("Zeynep", model.FirstName);
            Assert.Equal(1, context.PetOwners.Count());
        }

        // Silme işleminin başarılı olması
        [Fact]
        public async Task DeletePetOwner_IdVarsa_KaydiSilmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.PetOwners.Add(new PetOwner { Id = 1, FirstName = "Silinecek" });
            await context.SaveChangesAsync();

            var controller = new PetOwnersController(context);

            var sonuc = await controller.DeletePetOwner(1);

            Assert.IsType<NoContentResult>(sonuc); 
            Assert.Equal(0, context.PetOwners.Count());
        }
    }
}