using Bazy_danych_Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bazy_danych_Projekt.Views
{
    /// <summary>
    /// Logika interakcji dla klasy WnioskiAdopcyjneWindow.xaml
    /// </summary>
    public partial class WnioskiAdopcyjneWindow : Window
    {
        public WnioskiAdopcyjneWindow(WnioskiAdopcyjneViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }
    }
}
