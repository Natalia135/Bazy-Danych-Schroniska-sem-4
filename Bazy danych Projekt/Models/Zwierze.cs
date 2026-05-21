using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bazy_danych_Projekt.Models
{
    [Table("zwierzeta")]
    public class Zwierze
    {
        [Key]
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Gatunek { get; set; }
        public string Rasa { get; set; }
        public decimal CenaAdopcji { get; set; }
        //public string Status { get; set; }
        public StatusZwierzecia Status { get; set; }


        public int? SchroniskoId { get; set; }

        [ForeignKey("SchroniskoId")]
        public Schronisko Schronisko { get; set; }

        public string Wiek { get; set; }
        public string Plec { get; set; }
        public double Waga { get; set; }
        public bool CzyKastrowany { get; set; }
        public bool CzySzczepiony { get; set; }
        public string Opis { get; set; }

        //czy to ma byc?
        public ICollection<Adopcja> Adopcje { get; set; }
    }
}