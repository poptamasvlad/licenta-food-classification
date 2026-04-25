using ZXing.Net.Maui;
using FoodScanner.ViewModels; 

namespace FoodScanner.Views;

public partial class ScannerPage : ContentPage
{
    private readonly ScannerViewModel _viewModel;
    private bool _isProcessing = false;

    public ScannerPage(ScannerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _isProcessing = false;
        BarcodeReader.IsDetecting = true;

        await RequestCameraPermissionAsync();
        StartScanLineAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        BarcodeReader.IsDetecting = false;
    }

    private async Task RequestCameraPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            await DisplayAlert("Permisiune necesara",
                "Aplicatia are nevoie de acces la camera.", "OK");
    }

    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;
        _isProcessing = true;

        var barcode = e.Results.FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(barcode))
        {
            _isProcessing = false;
            return;
        }

        BarcodeReader.IsDetecting = false;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var product = await _viewModel.ProcessBarcodeAsync(barcode);

            if (product != null)
            {
                await Navigation.PushAsync(
                    new ResultPage(
                        Handler.MauiContext.Services
                            .GetService<ResultViewModel>(),
                        product
                    )
                );
            }
            else
            {
                await DisplayAlert("Produs negasit",
                    "Codul de bare nu a fost gasit in baza de date.",
                    "OK");
                BarcodeReader.IsDetecting = true;
                _isProcessing = false;
            }
        });
    }

    private void OnTorchClicked(object sender, EventArgs e)
    {
        _viewModel.ToggleTorch();
        BarcodeReader.IsTorchOn = _viewModel.IsTorchOn;
        TorchButton.Text = _viewModel.IsTorchOn ? "Torta ON" : "Torta OFF";
        TorchButton.BackgroundColor = _viewModel.IsTorchOn
            ? Color.FromArgb("#FFD700")
            : Color.FromArgb("#333333");
        TorchButton.TextColor = _viewModel.IsTorchOn
            ? Color.FromArgb("#333333")
            : Colors.White;
    }

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new HistoryPage(
                Handler.MauiContext.Services
                    .GetService<HistoryViewModel>()
            )
        );
    }

    private void StartScanLineAnimation()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            while (true)
            {
                await ScanLine.TranslateTo(0, -60, 1000);
                await ScanLine.TranslateTo(0, 60, 1000);
            }
        });
    }
}