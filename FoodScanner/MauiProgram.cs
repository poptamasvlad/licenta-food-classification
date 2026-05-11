using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using FoodScanner.Services;
using FoodScanner.ViewModels;
using FoodScanner.Helpers;
using FoodScanner.Views;

namespace FoodScanner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MLPredictionService>();
            builder.Services.AddSingleton<NutritionClassifier>();
            builder.Services.AddSingleton<FoodApiService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<DatabaseHelper>();

            builder.Services.AddTransient<ScannerViewModel>();
            builder.Services.AddTransient<ResultViewModel>();
            builder.Services.AddTransient<HistoryViewModel>();
            builder.Services.AddTransient<ManualEntryViewModel>();
            builder.Services.AddTransient<StatsViewModel>();

            builder.Services.AddTransient<ScannerPage>();
            builder.Services.AddTransient<ResultPage>();
            builder.Services.AddTransient<HistoryPage>();
            builder.Services.AddTransient<ManualEntryPage>();
            builder.Services.AddTransient<StatsPage>();

            return builder.Build();
        }
    }
}