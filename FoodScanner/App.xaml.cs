using FoodScanner.Views;

namespace FoodScanner;

public partial class App : Application
{
    public App(ScannerPage scannerPage)
    {
        InitializeComponent();
        MainPage = new NavigationPage(scannerPage)
        {
            BarBackgroundColor = Colors.White,
            BarTextColor = Color.FromArgb("#333333")
        };
    }
}
