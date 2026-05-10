using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bazy_danych_Projekt.Models
{
    [Table("schroniska")]
    public class Schronisko
    {
        [Key]
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public string KodPocztowy { get; set; }
        public string Telefon { get; set; }
        public string Opis { get; set; }
        public string NumerWNI { get; set; }
    }
}