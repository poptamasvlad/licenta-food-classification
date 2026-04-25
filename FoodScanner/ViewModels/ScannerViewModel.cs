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
        //private readonly NutritionClassifier _classifier;

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
            DatabaseService databaseService)
            //NutritionClassifier classifier)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            //_classifier = classifier;
            Title = "Scanează produs";
            StatusMessage = "Îndreaptă camera spre codul de bare";
        }

        public async Task<Product> ProcessBarcodeAsync(string barcode)
        {
            if (IsBusy) return null;

            try
            {
                IsBusy = true;
                StatusMessage = "Se caută produsul...";

                var product = await _apiService.GetProductByBarcodeAsync(barcode);

                if (product == null)
                {
                    StatusMessage = "Produs negăsit în baza de date";
                    return null;
                }

                //var classification = _classifier.Classify(product);
                //product.HealthLabel = classification.Label;
                //product.HealthScore = classification.Score;

                await _databaseService.SaveScanAsync(product);

                StatusMessage = "Produs găsit!";
                return product;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Eroare: {ex.Message}";
                return null;
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
