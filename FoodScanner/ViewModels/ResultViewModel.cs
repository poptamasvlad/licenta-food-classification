using FoodScanner.Models;
using FoodScanner.Services;

namespace FoodScanner.ViewModels;

public class ResultViewModel : BaseViewModel
{
    private Product _product;
    public Product Product
    {
        get => _product;
        set => SetProperty(ref _product, value);
    }

    private ClassificationResult _classification;
    public ClassificationResult Classification
    {
        get => _classification;
        set => SetProperty(ref _classification, value);
    }

    private Color _scoreColor;
    public Color ScoreColor
    {
        get => _scoreColor;
        set => SetProperty(ref _scoreColor, value);
    }

    private Color _nutriScoreColor;
    public Color NutriScoreColor
    {
        get => _nutriScoreColor;
        set => SetProperty(ref _nutriScoreColor, value);
    }

    private bool _hasImage;
    public bool HasImage
    {
        get => _hasImage;
        set => SetProperty(ref _hasImage, value);
    }

    private bool _hasWarnings;
    public bool HasWarnings
    {
        get => _hasWarnings;
        set => SetProperty(ref _hasWarnings, value);
    }

    private bool _hasPositives;
    public bool HasPositives
    {
        get => _hasPositives;
        set => SetProperty(ref _hasPositives, value);
    }

    public void LoadProduct(Product product)
    {
        Product = product;
        Title = product.Name;

        var classifier = new NutritionClassifier();
        Classification = classifier.Classify(product);

        product.HealthScore = Classification.Score;
        product.HealthLabel = Classification.Label;

        ScoreColor = Color.FromArgb(Classification.Color);
        NutriScoreColor = GetNutriScoreColor(product.NutriScore);
        HasImage = !string.IsNullOrEmpty(product.ImageUrl);
        HasWarnings = Classification.Warnings.Any();
        HasPositives = Classification.Positives.Any();
    }

    private Color GetNutriScoreColor(string nutriScore)
    {
        return (nutriScore?.Trim().ToUpper()) switch
        {
            "A" => Color.FromArgb("#1a9641"),
            "B" => Color.FromArgb("#a6d96a"),
            "C" => Color.FromArgb("#ffffbf"),
            "D" => Color.FromArgb("#fdae61"),
            "E" => Color.FromArgb("#d7191c"),
            _ => Color.FromArgb("#888888")
        };
    }
}