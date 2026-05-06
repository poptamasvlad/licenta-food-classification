using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodScanner.Models;
using FoodScanner.Services;

namespace FoodScanner.ViewModels
{
    public class ScannerViewModel : BaseViewModel
    {
        private readonly FoodApiService _apiService;
        private readonly DatabaseService _databaseService;
        private readonly NutritionClassifier _classifier;

        private bool _isTorchOn;
        public bool IsTorchOn
        {
            get => _isTorchOn;
            set => SetProperty(ref _isTorchOn, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ScannerViewModel(FoodApiService apiService,
            DatabaseService databaseService,
            NutritionClassifier classifier)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            _classifier = classifier;
            Title = "Scanează produs";
            StatusMessage = "Îndreaptă camera spre codul de bare";
        }

        public async Task<(Product product, bool notFound)>
    ProcessBarcodeAsync(string barcode)
        {
            if (IsBusy) return (null, false);

            try
            {
                IsBusy = true;
                StatusMessage = "Se cauta produsul...";

                var (product, isManual) = await _apiService
                    .GetProductByBarcodeAsync(barcode);

                if (product == null)
                {
                    StatusMessage = "Produs negasit in baza de date";
                    return (null, true);
                }

                var classification = _classifier.Classify(product);
                product.HealthLabel = classification.Label;
                product.HealthScore = classification.Score;
                product.Classification = classification;

                await _databaseService.SaveScanAsync(product);

                StatusMessage = isManual
                    ? "Produs gasit din baza ta de date!"
                    : "Produs gasit!";

                return (product, false);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Eroare: {ex.Message}";
                return (null, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ToggleTorch()
        {
            IsTorchOn = !IsTorchOn;
        }
    }
}
