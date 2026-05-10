using Bazy_danych_Projekt.Data;
using Bazy_danych_Projekt.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Bazy_danych_Projekt.ViewModels
{
    public partial class DodajZwierzeViewModel : ObservableObject
    {
        private readonly SchroniskoDbContext _dbContext;

        [ObservableProperty] private string _nowaNazwa;
        [ObservableProperty] private string _nowaRasa;
        [ObservableProperty] private decimal _nowaCena;
        [ObservableProperty] private string _nowyGatunek = "Pies"; // Wartość domyślna

        [ObservableProperty] private string _nowyWiek;
        [ObservableProperty] private string _nowaPlec = "Samiec"; // Wartość domyślna
        [ObservableProperty] private double _nowaWaga;
        [ObservableProperty] private bool _nowyCzyKastrowany;
        [ObservableProperty] private bool _nowyCzySzczepiony;
        [ObservableProperty] private string _nowyOpis;
        [ObservableProperty] private Zwierze _edytowaneZwierze;

        public Action ZamknijOkno { get; set; }
        public Zwierze DodaneZwierze { get; private set; }

        // Przechowujemy ID zalogowanego schroniska
        public int ZalogowaneSchroniskoId { get; set; }

        public void ZaładujDane(Zwierze zwierze)
        {
            EdytowaneZwierze = zwierze;
            NowaNazwa = zwierze.Nazwa;
            NowyGatunek = zwierze.Gatunek;
            NowaRasa = zwierze.Rasa;
            NowyWiek = zwierze.Wiek;
            NowaPlec = zwierze.Plec;
            NowaWaga = zwierze.Waga;
            NowyCzyKastrowany = zwierze.CzyKastrowany;
            NowyCzySzczepiony = zwierze.CzySzczepiony;
            NowyOpis = zwierze.Opis;
            NowaCena = zwierze.CenaAdopcji;
        }

        public DodajZwierzeViewModel(SchroniskoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [RelayCommand]
        private void Zapisz()
        {
            // 1. Walidacja podstawowa 
            if (string.IsNullOrWhiteSpace(NowaNazwa))
            {
                System.Windows.MessageBox.Show("Imię zwierzaka jest wymagane!");
                return;
            }

            try
            {
                if (EdytowaneZwierze != null)
                {
                    // --- TRYB EDYCJI ---
                    // Aktualizujemy dane na istniejącym obiekcie, który EF już śledzi
                    EdytowaneZwierze.Nazwa = NowaNazwa;
                    EdytowaneZwierze.Gatunek = NowyGatunek;
                    EdytowaneZwierze.Rasa = NowaRasa;
                    EdytowaneZwierze.Wiek = NowyWiek;
                    EdytowaneZwierze.Plec = NowaPlec;
                    EdytowaneZwierze.Waga = NowaWaga;
                    EdytowaneZwierze.CzyKastrowany = NowyCzyKastrowany;
                    EdytowaneZwierze.CzySzczepiony = NowyCzySzczepiony;
                    EdytowaneZwierze.Opis = NowyOpis;
                    EdytowaneZwierze.CenaAdopcji = NowaCena;

                    _dbContext.Zwierzeta.Update(EdytowaneZwierze);
                }
                else
                {
                    // --- TRYB DODAWANIA ---
                    var noweZwierze = new Zwierze
                    {
                        Nazwa = NowaNazwa,
                        Gatunek = NowyGatunek,
                        Rasa = NowaRasa ?? "Mieszaniec",
                        CenaAdopcji = NowaCena,
                        Status = "DO ADOPCJI",
                        SchroniskoId = ZalogowaneSchroniskoId,
                        Wiek = NowyWiek ?? "Nieznany",
                        Plec = NowaPlec,
                        Waga = NowaWaga,
                        CzyKastrowany = NowyCzyKastrowany,
                        CzySzczepiony = NowyCzySzczepiony,
                        Opis = NowyOpis ?? "Brak opisu"
                    };

                    _dbContext.Zwierzeta.Add(noweZwierze);
                    DodaneZwierze = noweZwierze; 
                }
                                
                _dbContext.SaveChanges();
                                
                ZamknijOkno?.Invoke();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd podczas zapisywania: {ex.Message}");
            }
        }
    }
}