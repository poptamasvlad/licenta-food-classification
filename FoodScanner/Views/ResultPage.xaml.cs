using FoodScanner.Models;
using FoodScanner.ViewModels;

namespace FoodScanner.Views;

public partial class ResultPage : ContentPage
{
    private readonly ResultViewModel _viewModel;

    public ResultPage(ResultViewModel viewModel, Product product)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.LoadProduct(product);
        BindingContext = _viewModel;
    }

    private async void OnScanAgainClicked(object sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Navigation.PopAsync(true);
        });
    }
}