using Microsoft.ML;
using FoodScanner.ML.Trainer;

Console.WriteLine("=== FoodScanner ML Trainer ===");

var rawCsvPath = @"C:\ML\en.openfoodfacts.org.products.csv";
var cleanCsvPath = @"C:\ML\food_clean.csv";
var modelOutputPath = @"C:\ML\food_model.zip";

if (!File.Exists(rawCsvPath))
{
    Console.WriteLine($"Fisierul brut nu exista: {rawCsvPath}");
    return;
}

Console.WriteLine("\nVrei sa regenerezi dataset-ul curat? Recomandat daca ai modificat DataPreparation.cs.");
Console.Write("Scrie da/nu: ");
var regenerate = Console.ReadLine()?.Trim().ToLower() == "da";

if (regenerate && File.Exists(cleanCsvPath))
{
    File.Delete(cleanCsvPath);
    Console.WriteLine("Dataset vechi sters.");
}

if (!File.Exists(cleanCsvPath))
{
    DataPreparation.LoadHeaders(rawCsvPath);
    DataPreparation.PrepareDataset(rawCsvPath, cleanCsvPath, 500000);
}
else
{
    Console.WriteLine("Dataset curat gasit, se sare pregatirea.");
}

var mlContext = new MLContext(seed: 42);

Console.WriteLine("\nSe incarca datele...");
var dataView = mlContext.Data.LoadFromTextFile<FoodData>(
    cleanCsvPath,
    hasHeader: true,
    separatorChar: ','
);

var rowCount = mlContext.Data.CreateEnumerable<FoodData>(
    dataView,
    reuseRowObject: false
).Count();

Console.WriteLine($"Randuri incarcate: {rowCount}");

if (rowCount < 10000)
{
    Console.WriteLine("Atentie: dataset-ul are putine randuri. Rezultatele pot fi slabe.");
}

var split = mlContext.Data.TrainTestSplit(
    dataView,
    testFraction: 0.2,
    seed: 42
);

Console.WriteLine("\nSe construieste pipeline-ul...");

var pipeline = mlContext.Transforms
    .Concatenate("Features",
        nameof(FoodData.Calories),
        nameof(FoodData.Fat),
        nameof(FoodData.SaturatedFat),
        nameof(FoodData.Sugar),
        nameof(FoodData.Salt),
        nameof(FoodData.Protein),
        nameof(FoodData.Fiber))
    .Append(mlContext.Regression.Trainers.FastTree(
        numberOfLeaves: 100,
        numberOfTrees: 500,
        minimumExampleCountPerLeaf: 5,
        learningRate: 0.05
    ));

Console.WriteLine("\nSe antreneaza modelul...");
var stopwatch = System.Diagnostics.Stopwatch.StartNew();

var model = pipeline.Fit(split.TrainSet);

stopwatch.Stop();
Console.WriteLine($"Antrenare finalizata in {stopwatch.Elapsed.TotalSeconds:F1}s");

Console.WriteLine("\nSe evalueaza modelul...");
var predictions = model.Transform(split.TestSet);
var metrics = mlContext.Regression.Evaluate(predictions);

Console.WriteLine("\n=== Rezultate evaluare ===");
Console.WriteLine($"R²: {metrics.RSquared:F4}");
Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:F2}");
Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F2}");

if (metrics.RSquared < 0.5)
{
    Console.WriteLine("Model slab. Verifica datele, numarul de randuri valide si coloanele folosite.");
}
else if (metrics.RSquared < 0.7)
{
    Console.WriteLine("Model acceptabil, dar poate fi imbunatatit.");
}
else
{
    Console.WriteLine("Model bun pentru folosire initiala.");
}

Console.WriteLine($"\nSe salveaza modelul la: {modelOutputPath}");
mlContext.Model.Save(model, dataView.Schema, modelOutputPath);
Console.WriteLine("Model salvat cu succes!");

Console.WriteLine("\n=== Test rapid ===");

var predictor = mlContext.Model.CreatePredictionEngine<FoodData, FoodPrediction>(model);

var testSanatos = new FoodData
{
    Calories = 80f,
    Fat = 1f,
    SaturatedFat = 0.2f,
    Sugar = 3f,
    Salt = 0.1f,
    Protein = 8f,
    Fiber = 4f
};

var testMediu = new FoodData
{
    Calories = 250f,
    Fat = 9f,
    SaturatedFat = 3f,
    Sugar = 12f,
    Salt = 0.8f,
    Protein = 6f,
    Fiber = 2f
};

var testNesanatos = new FoodData
{
    Calories = 520f,
    Fat = 28f,
    SaturatedFat = 12f,
    Sugar = 35f,
    Salt = 1.8f,
    Protein = 2f,
    Fiber = 0.5f
};

PrintPrediction("Produs sanatos", predictor.Predict(testSanatos).PredictedScore);
PrintPrediction("Produs mediu", predictor.Predict(testMediu).PredictedScore);
PrintPrediction("Produs nesanatos", predictor.Predict(testNesanatos).PredictedScore);

Console.WriteLine("\nGata! Copiaza food_model.zip in proiectul MAUI.");
Console.WriteLine("\nApasa orice tasta pentru a inchide...");
Console.ReadKey();

static void PrintPrediction(string name, float score)
{
    var clampedScore = Math.Clamp(score, 0, 100);
    Console.WriteLine($"{name}: {clampedScore:F0}/100");
}