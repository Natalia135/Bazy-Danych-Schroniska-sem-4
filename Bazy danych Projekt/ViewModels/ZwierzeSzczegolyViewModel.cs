using Bazy_danych_Projekt.Data;
using Bazy_danych_Projekt.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Bazy_danych_Projekt.ViewModels
{
    public partial class ZwierzeSzczegolyViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(KastrowanyTekst))]
        [NotifyPropertyChangedFor(nameof(SzczepionyTekst))]
        private Zwierze _wybraneZwierze;

        [ObservableProperty]
        private bool _czyPokazacPrzyciskAdopcji = true;

        [ObservableProperty]
        private bool _czyMozeEdytowac;

        [ObservableProperty]
        private Uzytkownik _zalogowanyUzytkownik;

        private readonly SchroniskoDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;

        public string KastrowanyTekst => WybraneZwierze?.CzyKastrowany == true ? "Tak" : "Nie";
        public string SzczepionyTekst => WybraneZwierze?.CzySzczepiony == true ? "Tak" : "Nie";

        public ZwierzeSzczegolyViewModel(SchroniskoDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        // Metoda wywoływana automatycznie przez CommunityToolkit przy zmianie WybraneZwierze
        partial void OnWybraneZwierzeChanged(Zwierze value)
        {
            OnPropertyChanged(nameof(KastrowanyTekst));
            OnPropertyChanged(nameof(SzczepionyTekst));
        }

        public Action ZamknijOkno { get; set; }

        [RelayCommand]
        private void Zamknij() => ZamknijOkno?.Invoke();

        //[RelayCommand]
        //private void Adoptuj()
        //{
        //    MessageBox.Show($"Wysłano zapytanie o adopcję zwierzaka: {WybraneZwierze.Nazwa}!", "Adopcja", MessageBoxButton.OK, MessageBoxImage.Information);
        //}
        [RelayCommand]
        private void Adoptuj()
        {
            if (WybraneZwierze == null) return;

            var adopcja = new Adopcja
            {
                ZwierzeId = WybraneZwierze.Id,
                UzytkownikId = _zalogowanyUzytkownik.Id,
                Status = "OCZEKUJE",
                DataAdopcji = DateTime.Now
            };

            _dbContext.Adopcje.Add(adopcja);

            WybraneZwierze.Status = "W TRAKCIE ADOPCJI";
            _dbContext.Zwierzeta.Update(WybraneZwierze);

            _dbContext.SaveChanges();

            MessageBox.Show("Zgłoszenie adopcji wysłane!");
        }

        [RelayCommand]
        private async Task UsunZwierze()
        {
            if (WybraneZwierze == null) return;

            var result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć podopiecznego o imieniu {WybraneZwierze.Nazwa}?",
                "Potwierdzenie usunięcia",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _dbContext.Zwierzeta.Remove(WybraneZwierze);
                await _dbContext.SaveChangesAsync();
                ZamknijOkno?.Invoke();
            }
        }

        [RelayCommand]
        private void EdytujZwierze()
        {
            var edycjaWindow = (System.Windows.Window)_serviceProvider.GetService(typeof(Views.DodajZwierzeWindow));
            var vm = edycjaWindow.DataContext as DodajZwierzeViewModel;

            if (vm != null)
            {
                vm.ZaładujDane(WybraneZwierze);
                edycjaWindow.Owner = Application.Current.MainWindow;
                edycjaWindow.ShowDialog();

                // Odświeżenie danych po powrocie z edycji
                OnPropertyChanged(nameof(WybraneZwierze));
                OnPropertyChanged(nameof(KastrowanyTekst));
                OnPropertyChanged(nameof(SzczepionyTekst));
            }
        }
    }
}