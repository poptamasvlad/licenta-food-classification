using FoodScanner.Models;
using FoodScanner.ViewModels;

namespace FoodScanner.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;

    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadHistoryAsync();
    }

    private void OnFilterTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is string filter)
            _viewModel.SelectedFilter = filter;
    }

    private async void OnProductTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not ScanResult scan) return;

        bool delete = await DisplayAlert(
            scan.ProductName,
            $"Scor: {scan.HealthScore}/100\nScandatat: {scan.ScannedAtFormatted}",
            "Sterge",
            "Inchide"
        );

        if (delete)
            await _viewModel.DeleteScanAsync(scan);
    }

    private async void OnClearAllTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Sterge tot istoricul",
            "Esti sigur? Aceasta actiune nu poate fi anulata.",
            "Sterge",
            "Anuleaza"
        );

        if (confirm)
            await _viewModel.ClearAllAsync();
    }
}