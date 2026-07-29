using Avalonia.Controls;
using ProxyOverlay.ViewModels;

namespace ProxyOverlay.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    public MainWindow(MainViewModel mv) : this()
    {
        DataContext = mv;
    }
}