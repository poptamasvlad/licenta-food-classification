using Microsoft.ML;
using FoodScanner.Models;

namespace FoodScanner.Services;

public class MLPredictionService
{
    private readonly MLContext _mlContext;
    private PredictionEngine<FoodMLData, FoodMLPrediction> _predictionEngine;
    private bool _isLoaded = false;
    private bool _loadFailed = false;

    public bool IsModelLoaded => _isLoaded;

    public MLPredictionService()
    {
        _mlContext = new MLContext(seed: 42);
    }

    public async Task InitializeAsync()
    {
        if (_isLoaded || _loadFailed) return;

        try
        {
            // Copiezi modelul din Assets in AppDataDirectory
            // MAUI nu poate citi direct din Raw assets cu ML.NET
            var modelFileName = "food_model.zip";
            var targetPath = Path.Combine(
                FileSystem.AppDataDirectory,
                modelFileName
            );

            // Copiezi o singura data
            if (!File.Exists(targetPath))
            {
                using var stream = await FileSystem
                    .OpenAppPackageFileAsync(modelFileName);
                using var fileStream = File.Create(targetPath);
                await stream.CopyToAsync(fileStream);
                Console.WriteLine("Model copiat din assets.");
            }

            // Incarci modelul
            var loadedModel = _mlContext.Model.Load(
                targetPath,
                out var _
            );

            _predictionEngine = _mlContext.Model
                .CreatePredictionEngine<FoodMLData, FoodMLPrediction>(
                    loadedModel
                );

            _isLoaded = true;
            Console.WriteLine("Model ML incarcat cu succes.");
        }
        catch (Exception ex)
        {
            _loadFailed = true;
            Console.WriteLine($"Eroare incarcare model ML: {ex.Message}");
        }
    }

    public float Predict(NutritionalInfo nutrition)
    {
        if (!_isLoaded || nutrition == null)
            return -1f;

        try
        {
            var input = new FoodMLData
            {
                Calories = (float)nutrition.Calories,
                Fat = (float)nutrition.Fat,
                SaturatedFat = (float)nutrition.SaturatedFat,
                Sugar = (float)nutrition.Sugar,
                Salt = (float)nutrition.Salt,
                Protein = (float)nutrition.Protein,
                Fiber = (float)nutrition.Fiber
            };

            var prediction = _predictionEngine.Predict(input);
            return Math.Clamp(prediction.PredictedScore, 0f, 100f);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare predictie ML: {ex.Message}");
            return -1f;
        }
    }
}