using FoodScanner.ViewModels;

namespace FoodScanner.Views;

public partial class ManualEntryPage : ContentPage
{
    private readonly ManualEntryViewModel _viewModel;

    public ManualEntryPage(
        ManualEntryViewModel viewModel,
        string barcode)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Barcode = barcode;
        BindingContext = _viewModel;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var (isValid, error) = _viewModel.Validate();

        if (!isValid)
        {
            await DisplayAlert("Date incomplete", error, "OK");
            return;
        }

        var product = await _viewModel.SaveAndClassifyAsync();

        // Navigam la ResultPage cu produsul salvat
        await Navigation.PushAsync(
            new ResultPage(
                Handler.MauiContext.Services
                    .GetService<ResultViewModel>(),
                product
            )
        );
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}