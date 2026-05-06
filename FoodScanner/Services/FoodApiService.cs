using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FoodScanner.Models;

namespace FoodScanner.Services
{
    public class FoodApiService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseService _databaseService;
        private const string BaseUrl =
            "https://world.openfoodfacts.org/api/v2/product/";

        public FoodApiService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "FoodScannerLicenta/1.0 (contact@example.com)"
            );
        }

        public async Task<(Product product, bool isManual)>
            GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                // Verifici mai intai baza de date locala
                var manualProduct = await _databaseService
                    .GetManualProductAsync(barcode);

                if (manualProduct != null)
                    return (manualProduct.ToProduct(), true);

                // Apoi cauti in Open Food Facts
                var url = $"{BaseUrl}{barcode}.json";
                var response = await _httpClient.GetStringAsync(url);
                var data = JsonConvert
                    .DeserializeObject<OpenFoodFactsResponse>(response);

                if (data?.Status != 1 || data.Product == null)
                    return (null, false);

                return (MapToProduct(barcode, data.Product), false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return (null, false);
            }
        }

        private Product MapToProduct(string barcode, OpenFoodFactsProduct p)
        {
            return new Product
            {
                Barcode = barcode,
                Name = p.ProductName ?? "Produs necunoscut",
                Brand = p.Brands ?? "Brand necunoscut",
                ImageUrl = p.ImageUrl,
                NutriScore = p.NutriscoreGrade?.Trim().ToUpper() ?? "?",
                Category = p.Categories,
                Nutrition = new NutritionalInfo
                {
                    Calories = p.Nutriments?.Calories ?? 0,
                    Fat = p.Nutriments?.Fat ?? 0,
                    SaturatedFat = p.Nutriments?.SaturatedFat ?? 0,
                    Sugar = p.Nutriments?.Sugar ?? 0,
                    Salt = p.Nutriments?.Salt ?? 0,
                    Protein = p.Nutriments?.Protein ?? 0,
                    Fiber = p.Nutriments?.Fiber ?? 0
                }
            };
        }
    }
}