using FoodScanner.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace FoodScanner.Views;

public partial class ScannerPage : ContentPage
{
    private readonly ScannerViewModel _viewModel;
    private bool _isProcessing = false;
    private CancellationTokenSource _animationCts;
    private CameraBarcodeReaderView _barcodeReader;

    public ScannerPage(ScannerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = new CancellationTokenSource();

        _isProcessing = false;

        await Task.Delay(300);
        RebuildCamera();
        StartScanLineAnimation(_animationCts.Token);
        await RequestCameraPermissionAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _animationCts?.Cancel();

        if (_barcodeReader != null)
            _barcodeReader.IsDetecting = false;
    }

    private void RebuildCamera()
    {
        // Găsești containerul camerei din XAML — un Grid numit CameraContainer
        if (_barcodeReader != null)
        {
            _barcodeReader.BarcodesDetected -= OnBarcodesDetected;
            CameraContainer.Children.Remove(_barcodeReader);
        }

        _barcodeReader = new CameraBarcodeReaderView
        {
            IsDetecting = true,
            Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormat.Ean8 |
                          BarcodeFormat.Ean13 |
                          BarcodeFormat.UpcA |
                          BarcodeFormat.UpcE,
                AutoRotate = true,
                Multiple = false
            }
        };

        _barcodeReader.BarcodesDetected += OnBarcodesDetected;
        CameraContainer.Children.Insert(0, _barcodeReader);
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

        if (_barcodeReader != null)
            _barcodeReader.IsDetecting = false;

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

                if (_barcodeReader != null)
                    _barcodeReader.IsDetecting = true;
                _isProcessing = false;
            }
        });
    }

    private void OnTorchClicked(object sender, EventArgs e)
    {
        _viewModel.ToggleTorch();

        if (_barcodeReader != null)
            _barcodeReader.IsTorchOn = _viewModel.IsTorchOn;

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

    private void StartScanLineAnimation(CancellationToken token)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await ScanLine.TranslateTo(0, -60, 1000);
                if (token.IsCancellationRequested) break;
                await ScanLine.TranslateTo(0, 60, 1000);
            }
            ScanLine.TranslationY = 0;
        });
    }
}