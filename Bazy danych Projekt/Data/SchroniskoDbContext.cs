using Bazy_danych_Projekt.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bazy_danych_Projekt.Data
{
    public class SchroniskoDbContext : DbContext
    {
        public DbSet<Schronisko> Schroniska { get; set; }
        public DbSet<Zwierze> Zwierzeta { get; set; }
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }

        public DbSet<Adopcja> Adopcje { get; set; }
        public SchroniskoDbContext(DbContextOptions<SchroniskoDbContext> options)
            : base(options)
        { }
        
    }
}