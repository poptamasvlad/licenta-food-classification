using FoodScanner.Services;
using FoodScanner.Views;

namespace FoodScanner;

public partial class App : Application
{
    public App(
        ScannerPage scannerPage,
        MLPredictionService mlService)
    {
        InitializeComponent();

        MainPage = new NavigationPage(scannerPage)
        {
            BarBackgroundColor = Colors.White,
            BarTextColor = Color.FromArgb("#333333")
        };

        // Incarci modelul ML in background
        // fara sa blochezi pornirea aplicatiei
        Task.Run(async () => await mlService.InitializeAsync());
    }
}
