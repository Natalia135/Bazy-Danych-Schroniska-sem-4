using Bazy_danych_Projekt.Data;
using Bazy_danych_Projekt.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows;

namespace Bazy_danych_Projekt.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly SchroniskoDbContext _dbContext;

        // Dane logowania
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _haslo;

        // Dane rejestracji (wspólne)
        [ObservableProperty] private string _rejImie;
        [ObservableProperty] private string _rejEmail;
        [ObservableProperty] private string _rejHaslo;

        // Dodatkowe dane dla konta typu Schronisko
        [ObservableProperty] private string _rejRola = "Uzytkownik";
        [ObservableProperty] private string _rejAdres;
        [ObservableProperty] private string _rejNumerWNI;
        [ObservableProperty] private string _rejKodPocztowy;
        [ObservableProperty] private string _rejTelefon;

        // Aktualizacja interfejsu po zmianie wybranej roli
        partial void OnRejRolaChanged(string value)
        {
            OnPropertyChanged(nameof(CzyRejestracjaSchroniska));
        }

        // Flaga określająca widoczność formularza schroniska
        public bool CzyRejestracjaSchroniska => RejRola == "Schronisko";

        public Uzytkownik ZalogowanyUzytkownik { get; private set; }
        public Action ZamknijOkno { get; set; }

        public LoginViewModel(SchroniskoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private void UstawRole(string wybranaRola)
        {
            RejRola = wybranaRola;
        }

        [RelayCommand]
        private void Zaloguj()
        {
            var user = _dbContext.Uzytkownicy.FirstOrDefault(u => u.Email == Email && u.Haslo == Haslo);

            if (user != null)
            {
                ZalogowanyUzytkownik = user;
                ZamknijOkno?.Invoke();
            }
            else
            {
                MessageBox.Show("Niepoprawny email lub hasło!", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Zarejestruj()
        {
            if (string.IsNullOrWhiteSpace(RejEmail) || string.IsNullOrWhiteSpace(RejHaslo))
            {
                MessageBox.Show("Wypełnij wszystkie obowiązkowe pola!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_dbContext.Uzytkownicy.Any(u => u.Email == RejEmail))
            {
                MessageBox.Show("Konto z podanym adresem email już istnieje!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RejRola == "Schronisko")
            {
                // Rejestracja nowego schrobniska w bazie danych
                var noweSchronisko = new Schronisko
                {
                    Nazwa = RejImie,
                    Adres = RejAdres ?? "",
                    KodPocztowy = RejKodPocztowy ?? "",
                    Telefon = RejTelefon ?? "",
                    NumerWNI = RejNumerWNI ?? "",
                    Opis = "Nowe schronisko na platformie."
                };

                _dbContext.Schroniska.Add(noweSchronisko);
                _dbContext.SaveChanges(); // Wymuszenie zapisu do wygenerowania klucza głównego

                // Utworzenie konta użytkownika i powiązanie z nowym schroniskiem
                var noweKonto = new Uzytkownik
                {
                    Imie = RejImie,
                    Email = RejEmail,
                    Haslo = RejHaslo,
                    Rola = RejRola,
                    SchroniskoId = noweSchronisko.Id
                };
                _dbContext.Uzytkownicy.Add(noweKonto);
            }
            else
            {
                // Rejestracja standardowego konta użytkownika
                var noweKonto = new Uzytkownik
                {
                    Imie = RejImie,
                    Email = RejEmail,
                    Haslo = RejHaslo,
                    Rola = "Uzytkownik"
                };
                _dbContext.Uzytkownicy.Add(noweKonto);
            }

            _dbContext.SaveChanges();
            MessageBox.Show("Konto utworzone! Możesz się teraz zalogować.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

            // Czyszczenie formularza rejestracji
            RejImie = "";
            RejEmail = "";
            RejHaslo = "";
            RejAdres = "";
            RejNumerWNI = "";
        }
    }
}