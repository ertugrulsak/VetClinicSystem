using Microsoft.AspNetCore.Mvc;
using VetClinicAPI.Controllers;
using VetClinicAPI.Models;
using VetClinicAPI.Tests.Helpers;
using Xunit;

namespace VetClinicAPI.Tests.UnitTests
{
    public class AppointmentsControllerTests
    {
        // Randevuları sayma
        [Fact]
        public async Task GetAppointments_KayitSayisi_DogruOlmali()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Appointments.Add(new Appointment { Id = 1, Notes = "Aşı" });
            context.Appointments.Add(new Appointment { Id = 2, Notes = "Kontrol" });
            await context.SaveChangesAsync();

            var controller = new AppointmentsController(context);
            var result = await controller.GetAppointments();

            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        // Tekil Randevu Notu Kontrolü
        [Fact]
        public async Task GetAppointment_Notlar_DogruGelmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Appointments.Add(new Appointment { Id = 1, Notes = "Acil Durum" });
            await context.SaveChangesAsync();

            var controller = new AppointmentsController(context);
            var result = await controller.GetAppointment(1);

            Assert.Equal("Acil Durum", result.Value?.Notes);
        }

        // Tarih ile Randevu Ekleme
        [Fact]
        public async Task PostAppointment_Tarih_Kaydedilmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AppointmentsController(context);
            var bugun = DateTime.Today;
            var randevu = new Appointment { Id = 1, AppointmentDate = bugun, Notes = "Tarih Testi" };

            await controller.PostAppointment(randevu);

            var kayit = await context.Appointments.FindAsync(1);
            Assert.NotNull(kayit);

            Assert.Equal(bugun, kayit.AppointmentDate);
        }

        // Olmayan  Kaydın İstendiği Durum
        [Fact]
        public async Task GetAppointment_BulunamayanId_404Donmeli()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            var controller = new AppointmentsController(context);

            var result = await controller.GetAppointment(50);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Silme
        [Fact]
        public async Task DeleteAppointment_BasariliSilme()
        {
            using var context = TestHelper.GetInMemoryDbContext();
            context.Appointments.Add(new Appointment { Id = 10, Notes = "Sil" });
            await context.SaveChangesAsync();

            var controller = new AppointmentsController(context);
            var result = await controller.DeleteAppointment(10);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Appointments.FindAsync(10));
        }
    }
}