using System.Linq;
using System.Threading.Tasks;
using AppointmentManager.Models;

namespace AppointmentManager.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            // Semilla para AppointmentStatus
            if (!db.AppointmentStatuses.Any())
            {
                db.AppointmentStatuses.AddRange(
                    new AppointmentStatus { Name = "Pending" },
                    new AppointmentStatus { Name = "Confirmed" },
                    new AppointmentStatus { Name = "Cancelled" }
                );
            }

            // Semilla para Specialties
            if (!db.Specialties.Any())
            {
                db.Specialties.AddRange(
                    new Specialty { Name = "General" , Description = "General practice"},
                    new Specialty { Name = "Physiotherapy", Description = "Physical therapy" },
                    new Specialty { Name = "Dermatology", Description = "Skin care" }
                );
            }

            await db.SaveChangesAsync();
        }
    }
}
