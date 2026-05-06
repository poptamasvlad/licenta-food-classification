using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodScanner.Helpers;
using FoodScanner.Models;
using FoodScanner.Services;

namespace FoodScanner.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly DatabaseHelper _databaseHelper;
    private List<ScanResult> _allScans = new();

    public ObservableCollection<ScanResult> FilteredScans { get; } = new();

    private string _selectedFilter = "Toate";
    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            SetProperty(ref _selectedFilter, value);
            ApplyFilter();
        }
    }

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        set => SetProperty(ref _isEmpty, value);
    }

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

    private double _averageCalories;
    public double AverageCalories
    {
        get => _averageCalories;
        set => SetProperty(ref _averageCalories, value);
    }

    public List<string> FilterOptions { get; } = new()
    {
        "Toate", "Sanatoase", "Moderate", "Nesanatoase"
    };

    public HistoryViewModel(
        DatabaseService databaseService,
        DatabaseHelper databaseHelper)
    {
        _databaseService = databaseService;
        _databaseHelper = databaseHelper;
        Title = "Istoric scanari";
    }

    public async Task LoadHistoryAsync()
    {
        IsBusy = true;

        _allScans = await _databaseService.GetAllScansAsync();
        TotalScans = _allScans.Count;
        HealthyPercentage = await _databaseHelper.GetHealthyPercentageAsync();
        AverageCalories = await _databaseHelper.GetAverageCaloriesAsync();

        ApplyFilter();
        IsBusy = false;
    }

    private void ApplyFilter()
    {
        FilteredScans.Clear();

        var filtered = SelectedFilter switch
        {
            "Sanatoase" => _allScans.Where(s => s.HealthScore >= 60),
            "Moderate" => _allScans.Where(s => s.HealthScore >= 40 && s.HealthScore < 60),
            "Nesanatoase" => _allScans.Where(s => s.HealthScore < 40),
            _ => _allScans.AsEnumerable()
        };

        foreach (var scan in filtered)
            FilteredScans.Add(scan);

        IsEmpty = FilteredScans.Count == 0;
    }

    public async Task DeleteScanAsync(ScanResult scan)
    {
        await _databaseService.DeleteScanAsync(scan);
        _allScans.Remove(scan);
        FilteredScans.Remove(scan);
        TotalScans = _allScans.Count;
        IsEmpty = FilteredScans.Count == 0;
    }

    public async Task ClearAllAsync()
    {
        await _databaseService.ClearAllAsync();
        _allScans.Clear();
        FilteredScans.Clear();
        TotalScans = 0;
        IsEmpty = true;
    }
}
