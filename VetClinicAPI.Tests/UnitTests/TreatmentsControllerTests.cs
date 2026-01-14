using Microsoft.AspNetCore.Mvc;
using VetClinicAPI.Controllers;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.UnitTests
{
    public class TreatmentsControllerTests
    {
        // Kayıtlı tedavilerin listelenmesi
        [Fact]
        public async Task GetTreatments_HepsiGelmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Treatments.Add(new Treatment { Id = 1, Description = "A", Price = 100 });
            context.Treatments.Add(new Treatment { Id = 2, Description = "B", Price = 200 });
            await context.SaveChangesAsync();

            var controller = new TreatmentsController(context);
            var result = await controller.GetTreatments();

            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        // Veritabanından fiyat bilgisinin çekilmesi
        [Fact]
        public async Task GetTreatment_FiyatKontrolu()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Treatments.Add(new Treatment { Id = 1, Price = 500 });
            await context.SaveChangesAsync();

            var controller = new TreatmentsController(context);
            var result = await controller.GetTreatment(1);

            Assert.Equal(500, result.Value?.Price);
        }

        // Yeni İşlem Ekleme
        [Fact]
        public async Task PostTreatment_YeniIslemEkleme()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new TreatmentsController(context);
            var islem = new Treatment { Id = 1, Description = "Tırnak Kesimi", Price = 150 };

            var result = await controller.PostTreatment(islem);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var eklenenIslem = created.Value as Treatment;

            Assert.NotNull(eklenenIslem);
            Assert.Equal("Tırnak Kesimi", eklenenIslem.Description);
        }

        // Tutuşmayan ID güncelleme kontrolü
        [Fact]
        public async Task PutTreatment_IdUyuşmazlığı_BadRequestDonmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new TreatmentsController(context);
            var islem = new Treatment { Id = 1 };

            var result = await controller.PutTreatment(2, islem);

            Assert.IsType<BadRequestResult>(result);
        }

        // Kayıt silme işlemi
        [Fact]
        public async Task DeleteTreatment_SilmeIslemi()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Treatments.Add(new Treatment { Id = 1, Description = "Sil" });
            await context.SaveChangesAsync();

            var controller = new TreatmentsController(context);
            await controller.DeleteTreatment(1);

            Assert.Equal(0, context.Treatments.Count());
        }
    }
}