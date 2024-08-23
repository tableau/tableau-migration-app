using Avalonia.Controls;
using EsmbMigration.GUI.ViewModels;

namespace EsmbMigration.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}