using Bazy_danych_Projekt.Data;
using Bazy_danych_Projekt.Models;
using Bazy_danych_Projekt.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bazy_danych_Projekt.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SchroniskoDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<Zwierze> ZwierzetaLista { get; set; }

        // --- SESJA UŻYTKOWNIKA ---
        [ObservableProperty]
        private Uzytkownik _zalogowanyUzytkownik;

        public bool CzyKtosZalogowany => ZalogowanyUzytkownik != null;
        public bool CzySchronisko => ZalogowanyUzytkownik?.Rola == "Schronisko";
        public bool CzyNormalnyUzytkownik => ZalogowanyUzytkownik?.Rola == "Uzytkownik";
        public bool CzyWolontariusz => ZalogowanyUzytkownik?.Rola == "Wolontariusz";

        // --- WIDOCZNOŚĆ NAKŁADEK ---
        [ObservableProperty]
        private bool _czyPokazacDodawanie = false;

        public MainViewModel(SchroniskoDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            ZwierzetaLista = new ObservableCollection<Zwierze>();

            // _dbContext.Database.EnsureDeleted();  <------- Odkonmnetujcie to sobie jak chce swieza baze dancyh 
            // po czym zakomentujcie po jednym odpaleniu bo wam za kazdym razem bedzie tworzylo nowa baze 
            _dbContext.Database.EnsureCreated();

            if (!_dbContext.Zwierzeta.Any())
            {
                DodajDaneTestowe();
            }

            _ = WczytajZwierzetaAsync();
        }

        // --- KOMENDY LOGOWANIA ---
        [RelayCommand]
        private void OtworzLogowanie()
        {
            var loginWindow = (System.Windows.Window)_serviceProvider.GetService(typeof(Views.LoginWindow));
            loginWindow.ShowDialog();

            var loginVM = loginWindow.DataContext as LoginViewModel;
            if (loginVM != null && loginVM.ZalogowanyUzytkownik != null)
            {
                ZalogowanyUzytkownik = loginVM.ZalogowanyUzytkownik;
                NotifyAuthChanges();
            }
        }

        [RelayCommand]
        private void Wyloguj()
        {
            ZalogowanyUzytkownik = null;
            NotifyAuthChanges();
        }

        // --- KOMENDY SZCZEGÓŁÓW ---
        [RelayCommand]
        private void PokazSzczegoly(Zwierze zwierze)
        {
            if (zwierze != null)
            {
                var szczegolyWindow = (System.Windows.Window)_serviceProvider.GetService(typeof(Views.ZwierzeSzczegolyWindow));
                var vm = szczegolyWindow.DataContext as ZwierzeSzczegolyViewModel;

                if (vm != null)
                {
                    vm.WybraneZwierze = zwierze;

                    vm.ZalogowanyUzytkownik = ZalogowanyUzytkownik;

                    // Blokada adopcji dla kont o roli schroniska
                    vm.CzyPokazacPrzyciskAdopcji = !(CzyKtosZalogowany && ZalogowanyUzytkownik.Rola == "Schronisko");

                    // Weryfikacja uprawnień do edycji rekordu zależnie od przypisania schroniska
                    vm.CzyMozeEdytowac = CzyKtosZalogowany &&
                                         ZalogowanyUzytkownik.Rola == "Schronisko" &&
                                         ZalogowanyUzytkownik.SchroniskoId == zwierze.SchroniskoId;
                }

                szczegolyWindow.ShowDialog();

                // Odświeżenie widoku w przypadku usunięcia rekordu w oknie szczegółów
                _ = WczytajZwierzetaAsync();
            }
        }

        // --- KOMENDY DODAWANIA ---
        [RelayCommand]
        private void OtworzDodawanie()
        {
            var dodajWindow = (System.Windows.Window)_serviceProvider.GetService(typeof(Views.DodajZwierzeWindow));
            var vm = dodajWindow.DataContext as DodajZwierzeViewModel;

            // Wstrzyknięcie identyfikatora schroniska do dodawania
            if (vm != null && ZalogowanyUzytkownik.SchroniskoId.HasValue)
            {
                vm.ZalogowaneSchroniskoId = ZalogowanyUzytkownik.SchroniskoId.Value;
            }

            dodajWindow.ShowDialog();

            // Aktualizacja kolekcji docelowej po poprawnej rejestracji obiektu
            if (vm != null && vm.DodaneZwierze != null)
            {
                ZwierzetaLista.Add(vm.DodaneZwierze);
            }
        }

        // --- KOMENDY PRZEGLADANIA WNIOSKOW ---

        [RelayCommand]
        private void OtworzWnioski()
        {
            var okno = (System.Windows.Window)_serviceProvider
        .GetService(typeof(Views.WnioskiAdopcyjneWindow));

            okno.ShowDialog();
        }

        // --- KOMENDY WYSZUKIWANIA ---
        [ObservableProperty]
        private string _tekstWyszukiwania = string.Empty;

        partial void OnTekstWyszukiwaniaChanged(string value)
        {
            OnPropertyChanged(nameof(WidoczneZwierzeta));
        }

        public System.Collections.Generic.IEnumerable<Zwierze> WidoczneZwierzeta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TekstWyszukiwania))
                    return ZwierzetaLista;

                var fraza = TekstWyszukiwania.ToLower();

                return ZwierzetaLista.Where(z =>
                    (z.Nazwa != null && z.Nazwa.ToLower().Contains(fraza)) ||
                    (z.Rasa != null && z.Rasa.ToLower().Contains(fraza)) ||
                    (z.Gatunek != null && z.Gatunek.ToLower().Contains(fraza)) ||
                    (z.Schronisko != null && z.Schronisko.Nazwa.ToLower().Contains(fraza)) ||
                    (z.Schronisko != null && z.Schronisko.Adres.ToLower().Contains(fraza))
                );
            }
        }

        // --- METODY POMOCNICZE ---
        private async Task WczytajZwierzetaAsync()
        {
            var dane = await _dbContext.Zwierzeta.Include(z => z.Schronisko).ToListAsync();
            ZwierzetaLista.Clear();
            foreach (var z in dane) ZwierzetaLista.Add(z);
            OnPropertyChanged(nameof(WidoczneZwierzeta));
        }

        private void NotifyAuthChanges()
        {
            // Propagacja zmian stanu autoryzacji do warstwy UI
            OnPropertyChanged(nameof(CzyKtosZalogowany));
            OnPropertyChanged(nameof(CzySchronisko));
            OnPropertyChanged(nameof(CzyNormalnyUzytkownik));
            OnPropertyChanged(nameof(CzyWolontariusz));
        }

        private void DodajDaneTestowe()
        {
            var schronisko = new Schronisko
            {
                Nazwa = "Nadzieja",
                Adres = "ul. Psia 1, Katowice",
                KodPocztowy = "40-001",
                Telefon = "123456789",
                Opis = "Testowe",
                NumerWNI = "PL12345678"
            };

            _dbContext.Schroniska.Add(schronisko);
            _dbContext.SaveChanges(); // Wymuszony zapis w celu wygenerowania ID dla kluczy obcych

            var kontoZwyklyUzytkownik = new Uzytkownik
            {
                Imie = "Jan Kowalski",
                Email = "user@test.pl",
                Haslo = "1234",
                Rola = "Uzytkownik"
            };

            var kontoAdminSchroniska = new Uzytkownik
            {
                Imie = "Admin Schroniska Nadzieja",
                Email = "admin@nadzieja.pl",
                Haslo = "1234",
                Rola = "Schronisko",
                SchroniskoId = schronisko.Id
            };

            _dbContext.Uzytkownicy.AddRange(kontoZwyklyUzytkownik, kontoAdminSchroniska);
            _dbContext.SaveChanges();

            _dbContext.Zwierzeta.AddRange(
                new Zwierze
                {
                    Nazwa = "Burek",
                    Gatunek = "Pies",
                    Rasa = "Mieszaniec",
                    Status = "DO ADOPCJI",
                    SchroniskoId = schronisko.Id,
                    Wiek = "3 lata",
                    Plec = "Samiec",
                    Waga = 15.5,
                    CzyKastrowany = true,
                    CzySzczepiony = true,
                    Opis = "Bardzo przyjazny i aktywny psiak."
                },
                new Zwierze
                {
                    Nazwa = "Luna",
                    Gatunek = "Pies",
                    Rasa = "Owczarek",
                    Status = "DO ADOPCJI",
                    SchroniskoId = schronisko.Id,
                    Wiek = "5 miesięcy",
                    Plec = "Samica",
                    Waga = 8.0,
                    CzyKastrowany = false,
                    CzySzczepiony = true,
                    Opis = "Szczeniak, który jeszcze dużo musi się nauczyć."
                }
            );

            _dbContext.SaveChanges();
        }
    }
}