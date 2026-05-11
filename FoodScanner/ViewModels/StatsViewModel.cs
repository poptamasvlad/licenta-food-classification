using FoodScanner.Helpers;
using FoodScanner.Models;
using FoodScanner.Services;

namespace FoodScanner.ViewModels;

public class NutriScoreBar
{
    public string Grade { get; set; }
    public int Count { get; set; }
    public string Color { get; set; }
    public double BarWidth { get; set; }
}

public class StatsViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly DatabaseHelper _databaseHelper;
    private List<NutriScoreBar> _nutriScoreDistribution = new();
    private List<ScanResult> _recentScans = new();


    private int _totalScans;
    public int TotalScans
    {
        get => _totalScans;
        set => SetProperty(ref _totalScans, value);
    }

    private int _healthyPercentage;
    public int HealthyPercentage
    {
        get => _healthyPercentage;
        set => SetProperty(ref _healthyPercentage, value);
    }

    private double _averageScore;
    public double AverageScore
    {
        get => _averageScore;
        set => SetProperty(ref _averageScore, value);
    }

    private double _averageCalories;
    public double AverageCalories
    {
        get => _averageCalories;
        set => SetProperty(ref _averageCalories, value);
    }

    private string _motivationTitle;
    public string MotivationTitle
    {
        get => _motivationTitle;
        set => SetProperty(ref _motivationTitle, value);
    }

    private string _motivationMessage;
    public string MotivationMessage
    {
        get => _motivationMessage;
        set => SetProperty(ref _motivationMessage, value);
    }

    private Color _motivationColor;
    public Color MotivationColor
    {
        get => _motivationColor;
        set => SetProperty(ref _motivationColor, value);
    }

    public List<NutriScoreBar> NutriScoreDistribution
    {
        get => _nutriScoreDistribution;
        set => SetProperty(ref _nutriScoreDistribution, value);
    }
    public List<ScanResult> RecentScans
    {
        get => _recentScans;
        set => SetProperty(ref _recentScans, value);
    }

    public StatsViewModel(
        DatabaseService databaseService,
        DatabaseHelper databaseHelper)
    {
        _databaseService = databaseService;
        _databaseHelper = databaseHelper;
        Title = "Statistici";
    }

    public async Task LoadStatsAsync()
    {
        IsBusy = true;

        var allScans = await _databaseService.GetAllScansAsync();

        TotalScans = allScans.Count;
        HealthyPercentage = await _databaseHelper.GetHealthyPercentageAsync();
        AverageCalories = await _databaseHelper.GetAverageCaloriesAsync();

        AverageScore = allScans.Any()
            ? allScans.Average(s => s.HealthScore)
            : 0;

        // Distributie Nutri-Score
        var distribution = await _databaseHelper
            .GetNutriScoreDistributionAsync();
        int maxCount = distribution.Values.Any()
            ? distribution.Values.Max()
            : 1;

        NutriScoreDistribution = new List<NutriScoreBar>
        {
            new() { Grade = "A", Count = distribution["A"], Color = "#1a9641",
                    BarWidth = CalcBarWidth(distribution["A"], maxCount) },
            new() { Grade = "B", Count = distribution["B"], Color = "#a6d96a",
                    BarWidth = CalcBarWidth(distribution["B"], maxCount) },
            new() { Grade = "C", Count = distribution["C"], Color = "#ffffbf",
                    BarWidth = CalcBarWidth(distribution["C"], maxCount) },
            new() { Grade = "D", Count = distribution["D"], Color = "#fdae61",
                    BarWidth = CalcBarWidth(distribution["D"], maxCount) },
            new() { Grade = "E", Count = distribution["E"], Color = "#d7191c",
                    BarWidth = CalcBarWidth(distribution["E"], maxCount) }
        };

        RecentScans = allScans.Take(5).ToList();    

        // Mesaj motivational bazat pe scorul mediu
        SetMotivation(HealthyPercentage);

        SetProperty(ref _nutriScoreDistribution, NutriScoreDistribution);
        SetProperty(ref _recentScans, RecentScans);

        IsBusy = false;
    }

    private double CalcBarWidth(int count, int max)
    {
        if (max == 0) return 0;
        return Math.Max((double)count / max * 200, count > 0 ? 8 : 0);
    }

    private void SetMotivation(int healthyPct)
    {
        if (healthyPct >= 70)
        {
            MotivationTitle = "Alegeri excelente!";
            MotivationMessage = $"{healthyPct}% din produsele scanate sunt sanatoase. Continua tot asa!";
            MotivationColor = Color.FromArgb("#1a9641");
        }
        else if (healthyPct >= 50)
        {
            MotivationTitle = "Pe drumul cel bun";
            MotivationMessage = $"{healthyPct}% din produse sunt sanatoase. Mai ai loc de imbunatatire!";
            MotivationColor = Color.FromArgb("#fdae61");
        }
        else
        {
            MotivationTitle = "Atentie la alimentatie";
            MotivationMessage = $"Doar {healthyPct}% din produse sunt sanatoase. Incearca sa alegi mai bine!";
            MotivationColor = Color.FromArgb("#d7191c");
        }
    }
}