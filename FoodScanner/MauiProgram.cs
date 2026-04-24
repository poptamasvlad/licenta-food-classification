using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using FoodScanner.Services;

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

            builder.Services.AddSingleton<FoodApiService>();
            //builder.Services.AddSingleton<DatabaseService>();
            //builder.Services.AddSingleton<NutritionClassifier>();

            //builder.Services.AddTransient<ScannerViewModel>();
            //builder.Services.AddTransient<ResultViewModel>();
            //builder.Services.AddTransient<HistoryViewModel>();

    #if DEBUG
            builder.Logging.AddDebug();
    #endif

            return builder.Build();
        }
    }
}