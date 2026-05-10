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
using Bazy_danych_Projekt.ViewModels;
using System.Windows;

namespace Bazy_danych_Projekt.Views
{
    public partial class DodajZwierzeWindow : Window
    {
        public DodajZwierzeWindow(DodajZwierzeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.ZamknijOkno = this.Close;
        }
    }
}
