using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodScanner.Models;

namespace FoodScanner.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        private async Task InitializeAsync()
        {
            if (_isInitialized) return;

            var dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "foodscanner.db"
            );

            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<ScanResult>();

            _isInitialized = true;
        }

        public async Task<int> SaveScanAsync(Product product)
        {
            await InitializeAsync();

            var existing = await _database.Table<ScanResult>()
                .Where(s => s.Barcode == product.Barcode)
                .FirstOrDefaultAsync();

            var scan = new ScanResult
            {
                Barcode = product.Barcode,
                ProductName = product.Name,
                Brand = product.Brand,
                NutriScore = product.NutriScore,
                HealthLabel = product.HealthLabel,
                HealthScore = product.HealthScore,
                Calories = product.Nutrition?.Calories ?? 0,
                Fat = product.Nutrition?.Fat ?? 0,
                Sugar = product.Nutrition?.Sugar ?? 0,
                Salt = product.Nutrition?.Salt ?? 0,
                Protein = product.Nutrition?.Protein ?? 0,
                ImageUrl = product.ImageUrl,
                ScannedAt = DateTime.Now
            };

            if (existing != null)
            {
                scan.Id = existing.Id;
                return await _database.UpdateAsync(scan);
            }

            return await _database.InsertAsync(scan);
        }

        public async Task<List<ScanResult>> GetAllScansAsync()
        {
            await InitializeAsync();
            return await _database.Table<ScanResult>()
                .OrderByDescending(s => s.ScannedAt)
                .ToListAsync();
        }

        public async Task<ScanResult> GetScanByBarcodeAsync(string barcode)
        {
            await InitializeAsync();
            return await _database.Table<ScanResult>()
                .Where(s => s.Barcode == barcode)
                .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteScanAsync(ScanResult scan)
        {
            await InitializeAsync();
            return await _database.DeleteAsync(scan);
        }

        public async Task<int> GetTotalScansAsync()
        {
            await InitializeAsync();
            return await _database.Table<ScanResult>().CountAsync();
        }

        public async Task<List<ScanResult>> GetHealthyScansAsync()
        {
            await InitializeAsync();
            return await _database.Table<ScanResult>()
                .Where(s => s.HealthScore >= 60)
                .ToListAsync();
        }

        public async Task ClearAllAsync()
        {
            await InitializeAsync();
            await _database.DeleteAllAsync<ScanResult>();
        }
    }
}
