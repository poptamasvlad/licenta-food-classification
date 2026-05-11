using FoodScanner.Models;
using FoodScanner.Services;

namespace FoodScanner.ViewModels;

public class ManualEntryViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly NutritionClassifier _classifier;

    public string Barcode { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _brand;
    public string Brand
    {
        get => _brand;
        set => SetProperty(ref _brand, value);
    }

    private string _calories;
    public string Calories
    {
        get => _calories;
        set => SetProperty(ref _calories, value);
    }

    private string _fat;
    public string Fat
    {
        get => _fat;
        set => SetProperty(ref _fat, value);
    }

    private string _saturatedFat;
    public string SaturatedFat
    {
        get => _saturatedFat;
        set => SetProperty(ref _saturatedFat, value);
    }

    private string _sugar;
    public string Sugar
    {
        get => _sugar;
        set => SetProperty(ref _sugar, value);
    }

    private string _salt;
    public string Salt
    {
        get => _salt;
        set => SetProperty(ref _salt, value);
    }

    private string _protein;
    public string Protein
    {
        get => _protein;
        set => SetProperty(ref _protein, value);
    }

    private string _fiber;
    public string Fiber
    {
        get => _fiber;
        set => SetProperty(ref _fiber, value);
    }

    public ManualEntryViewModel(
       DatabaseService databaseService,
        NutritionClassifier classifier)
    {
        _databaseService = databaseService;
        _classifier = classifier;
        Title = "Adauga produs manual";
    }

    public (bool isValid, string error) Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return (false, "Numele produsului este obligatoriu.");

        if (!double.TryParse(Calories, out _))
            return (false, "Caloriile trebuie sa fie un numar valid.");

        if (!double.TryParse(Fat, out _))
            return (false, "Grasimile trebuie sa fie un numar valid.");

        if (!double.TryParse(Sugar, out _))
            return (false, "Zaharurile trebuie sa fie un numar valid.");

        if (!double.TryParse(Salt, out _))
            return (false, "Sarea trebuie sa fie un numar valid.");

        return (true, null);
    }

    public async Task<Product> SaveAndClassifyAsync()
    {
        var manual = new ManualProduct
        {
            Barcode = Barcode,
            Name = Name.Trim(),
            Brand = Brand?.Trim() ?? "",
            Calories = ParseDouble(Calories),
            Fat = ParseDouble(Fat),
            SaturatedFat = ParseDouble(SaturatedFat),
            Sugar = ParseDouble(Sugar),
            Salt = ParseDouble(Salt),
            Protein = ParseDouble(Protein),
            Fiber = ParseDouble(Fiber)
        };

        await _databaseService.SaveManualProductAsync(manual);

        var product = manual.ToProduct();
        var classification = _classifier.Classify(product);
        product.HealthLabel = classification.Label;
        product.HealthScore = classification.Score;
        product.Classification = classification;

        await _databaseService.SaveScanAsync(product);

        return product;
    }

    private double ParseDouble(string value)
        => double.TryParse(value, out var result) ? result : 0;
}