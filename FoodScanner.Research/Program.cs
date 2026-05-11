using Microsoft.ML;
using FoodScanner.Research;

Console.WriteLine("=== FoodScanner Research Analysis ===");

var modelPath = @"C:\ML\food_model.zip";
var productsPath = @"C:\Vlad\Facultate\Licenta\FoodScanner\FoodScanner.Research\Data\romanian_products.csv";
var outputPath = @"C:\Vlad\Facultate\Licenta\FoodScanner\FoodScanner.Research\Data\research_results.csv";

Directory.CreateDirectory(@"C:\Research");

if (!File.Exists(modelPath))
{
    Console.WriteLine($"EROARE: Modelul ML nu a fost gasit la {modelPath}");
    Console.WriteLine("Ruleaza mai intai FoodScanner.ML.Trainer");
    Console.ReadKey();
    return;
}

if (!File.Exists(productsPath))
{
    Console.WriteLine($"EROARE: Fisierul de produse nu a fost gasit la {productsPath}");
    Console.ReadKey();
    return;
}

var analyzer = new ResearchAnalyzer(modelPath);
var products = analyzer.LoadProducts(productsPath);

Console.WriteLine($"Produse incarcate: {products.Count}");

// Incarci modelul ML
var mlContext = new MLContext(seed: 42);
var model = mlContext.Model.Load(modelPath, out var _);
var predictor = mlContext.Model
    .CreatePredictionEngine<MLFoodData, MLFoodPrediction>(model);

// Analizezi fiecare produs
var results = new List<ResearchResult>();

foreach (var product in products)
{
    var input = new MLFoodData
    {
        Calories = product.Calories,
        Fat = product.Fat,
        SaturatedFat = product.SaturatedFat,
        Sugar = product.Sugar,
        Salt = product.Salt,
        Protein = product.Protein,
        Fiber = product.Fiber
    };

    var prediction = predictor.Predict(input);
    int mlScore = (int)Math.Clamp(
        Math.Round(prediction.PredictedScore), 0, 100);
    int officialScore = analyzer.NutriscoreToScore(
        product.OfficialNutriscore);

    results.Add(new ResearchResult
    {
        Name = product.Name,
        Brand = product.Brand,
        OfficialNutriscore = product.OfficialNutriscore.ToUpper(),
        OfficialScore = officialScore,
        MLScore = mlScore,
        MLLabel = analyzer.ScoreToLabel(mlScore),
        Difference = Math.Abs(mlScore - officialScore),
        IsAgreement = analyzer.ScoresAgree(mlScore, product.OfficialNutriscore),
        Category = analyzer.NutriscoreToCategory(product.OfficialNutriscore)
    });
}

// Statistici globale
int totalProducts = results.Count;
int agreements = results.Count(r => r.IsAgreement);
double agreementRate = (double)agreements / totalProducts * 100;
double avgDifference = results.Average(r => r.Difference);
double avgMLScore = results.Average(r => r.MLScore);

Console.WriteLine("\n=== Rezultate Globale ===");
Console.WriteLine($"Total produse analizate: {totalProducts}");
Console.WriteLine($"Concordanta cu Nutri-Score oficial: {agreements}/{totalProducts} ({agreementRate:F1}%)");
Console.WriteLine($"Diferenta medie de scor: {avgDifference:F1} puncte");
Console.WriteLine($"Scor ML mediu: {avgMLScore:F1}/100");

// Statistici pe categorii
Console.WriteLine("\n=== Concordanta pe Categorii ===");
foreach (var category in new[] { "Sanatos", "Moderat", "Nesanatos" })
{
    var catResults = results.Where(r => r.Category == category).ToList();
    if (!catResults.Any()) continue;

    int catAgreements = catResults.Count(r => r.IsAgreement);
    double catRate = (double)catAgreements / catResults.Count * 100;
    Console.WriteLine($"{category}: {catAgreements}/{catResults.Count} ({catRate:F1}%)");
}

// Produse cu cea mai mare discrepanta
Console.WriteLine("\n=== Top 5 Discrepante ===");
foreach (var r in results.OrderByDescending(r => r.Difference).Take(5))
{
    Console.WriteLine($"{r.Name}: ML={r.MLScore} | Oficial={r.OfficialScore} | Diferenta={r.Difference}");
}

// Produse cu concordanta perfecta
Console.WriteLine("\n=== Exemple Concordanta Perfecta ===");
foreach (var r in results.Where(r => r.IsAgreement).Take(5))
{
    Console.WriteLine($"{r.Name}: ML={r.MLScore} ({r.MLLabel}) | Nutri-Score={r.OfficialNutriscore}");
}

// Salvezi rezultatele in CSV
var csvLines = new List<string>
{
    "Produs,Brand,NutriScore_Oficial,Scor_Oficial,Scor_ML,Eticheta_ML,Diferenta,Concordanta,Categorie"
};

foreach (var r in results)
{
    csvLines.Add(
        $"{r.Name},{r.Brand},{r.OfficialNutriscore}," +
        $"{r.OfficialScore},{r.MLScore},{r.MLLabel}," +
        $"{r.Difference},{(r.IsAgreement ? "DA" : "NU")},{r.Category}"
    );
}

File.WriteAllLines(outputPath, csvLines);
Console.WriteLine($"\nRezultate salvate la: {outputPath}");
Console.WriteLine("Deschide fisierul CSV in Excel pentru grafice.");

Console.WriteLine("\nApasa orice tasta pentru a inchide...");
Console.ReadKey();