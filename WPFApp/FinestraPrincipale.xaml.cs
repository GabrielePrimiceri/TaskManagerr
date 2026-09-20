using System.Windows;
using WPFApp.ModelliVista;

namespace WPFApp;

public partial class FinestraPrincipale : Window
{
    public FinestraPrincipale(ViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
