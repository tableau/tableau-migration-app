using Avalonia.Controls;
using MigrationApp.GUI.ViewModels;

namespace MigrationApp.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}