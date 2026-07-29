using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection;
using ProxyOverlay.Services;
using ProxyOverlay.ViewModels;
using ProxyOverlay.Views;
using Avalonia.Platform.Storage;

namespace ProxyOverlay;

public partial class App : Application
{    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        PrepareTempDirectory();
    }

    private void PrepareTempDirectory()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
        if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        MagickNET.SetTempDirectory(path);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var mainWindow = new MainWindow();
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        collection.AddSingleton(mainWindow.StorageProvider);

        Services = collection.BuildServiceProvider();
        var mainViewModel = Services.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainViewModel;
        mainViewModel.Initialize();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        } else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            single.MainView = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public new static App? Current => Application.Current as App;
    public IServiceProvider? Services { get; private set; }
}
