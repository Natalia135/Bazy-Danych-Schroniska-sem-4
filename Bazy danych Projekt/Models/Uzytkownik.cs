using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bazy_danych_Projekt.Models
{
    [Table("uzytkownicy")]
    public class Uzytkownik
    {
        [Key]
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Email { get; set; }
        public string Haslo { get; set; }
        public string Rola { get; set; }

       
        public int? SchroniskoId { get; set; }
        public Schronisko Schronisko { get; set; }

        public ICollection<Adopcja> Adopcje { get; set; }
    }
}