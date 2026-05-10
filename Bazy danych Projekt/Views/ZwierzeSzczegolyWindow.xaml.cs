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

namespace Bazy_danych_Projekt.Views
{
    public partial class ZwierzeSzczegolyWindow : Window
    {
        public ZwierzeSzczegolyWindow(ZwierzeSzczegolyViewModel viewModel)
        {
            InitializeComponent();
                        
            this.DataContext = viewModel; 
            
            viewModel.ZamknijOkno = () => this.Close(); 
        }
    }
}
