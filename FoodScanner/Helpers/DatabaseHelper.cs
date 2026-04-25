using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodScanner.Services;

namespace FoodScanner.Helpers
{
    public class DatabaseHelper
    {
        private readonly DatabaseService _databaseService;

        public DatabaseHelper(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<Dictionary<string, int>> GetNutriScoreDistributionAsync()
        {
            var scans = await _databaseService.GetAllScansAsync();

            return new Dictionary<string, int>
        {
            { "A", scans.Count(s => s.NutriScore == "A") },
            { "B", scans.Count(s => s.NutriScore == "B") },
            { "C", scans.Count(s => s.NutriScore == "C") },
            { "D", scans.Count(s => s.NutriScore == "D") },
            { "E", scans.Count(s => s.NutriScore == "E") }
        };
        }

        public async Task<double> GetAverageCaloriesAsync()
        {
            var scans = await _databaseService.GetAllScansAsync();
            if (!scans.Any()) return 0;
            return Math.Round(scans.Average(s => s.Calories), 1);
        }

        public async Task<string> GetMostScannedNutriScoreAsync()
        {
            var distribution = await GetNutriScoreDistributionAsync();
            return distribution.OrderByDescending(x => x.Value)
                .FirstOrDefault().Key ?? "?";
        }

        public async Task<int> GetHealthyPercentageAsync()
        {
            var all = await _databaseService.GetAllScansAsync();
            if (!all.Any()) return 0;

            var healthy = all.Count(s => s.HealthScore >= 60);
            return (int)Math.Round((double)healthy / all.Count * 100);
        }
    }
}
