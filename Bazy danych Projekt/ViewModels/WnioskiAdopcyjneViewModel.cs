using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Bazy_danych_Projekt.Data;
using Bazy_danych_Projekt.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Bazy_danych_Projekt.ViewModels
{
    public partial class WnioskiAdopcyjneViewModel : ObservableObject
    {
        private readonly SchroniskoDbContext _db;

        [ObservableProperty]
        private ObservableCollection<Adopcja> _wnioski;

        [ObservableProperty]
        private Adopcja _wybranyWniosek;

        public WnioskiAdopcyjneViewModel(SchroniskoDbContext dbContext)
        {
            _db = dbContext;
            Zaladuj();
        }

        private void Zaladuj()
        {
            var lista = _db.Adopcje
                .Include(a => a.Zwierze)
                .Include(a => a.Uzytkownik)
                .Where(a => a.Status == "OCZEKUJE")
                .ToList();

            _wnioski = new ObservableCollection<Adopcja>(lista);
        }

        [RelayCommand]
        private void Akceptuj()
        {
            if (_wybranyWniosek == null) return;

            _wybranyWniosek.Status = "ZAAKCEPTOWANO";

            var zwierze = _db.Zwierzeta
                .FirstOrDefault(z => z.Id == _wybranyWniosek.ZwierzeId);

            if (zwierze != null)
            {
                zwierze.Status = "ZAADOPTOWANY";
            }

            _db.SaveChanges();

            MessageBox.Show("Wniosek zaakceptowany");

            Zaladuj();
        }

        [RelayCommand]
        private void Odrzuc()
        {
            if (_wybranyWniosek == null) return;

            _wybranyWniosek.Status = "ODRZUCONO";

            var zwierze = _db.Zwierzeta
                .FirstOrDefault(z => z.Id == _wybranyWniosek.ZwierzeId);

            if (zwierze != null)
            {
                zwierze.Status = "DOSTĘPNY";
            }

            _db.SaveChanges();

            MessageBox.Show("Wniosek odrzucony");

            Zaladuj();
        }
    }
}