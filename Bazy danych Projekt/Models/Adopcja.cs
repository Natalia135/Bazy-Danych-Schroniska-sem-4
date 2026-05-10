using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bazy_danych_Projekt.Models
{
    [Table("adopcje")]
    public class Adopcja
    {
        [Key]
        public int Id { get; set; }

        public int ZwierzeId { get; set; }

        [ForeignKey("ZwierzeId")]
        public Zwierze Zwierze { get; set; }

        public int UzytkownikId { get; set; }

        [ForeignKey("UzytkownikId")]
        public Uzytkownik Uzytkownik { get; set; }

        public DateTime DataAdopcji { get; set; } = DateTime.Now;

        public string Status { get; set; } = "OCZEKUJE";
        // np. OCZEKUJE / ZAAKCEPTOWANA / ODRZUCONA
    }
}