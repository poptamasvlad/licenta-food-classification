using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodScanner.Models
{
    public class Product
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string ImageUrl { get; set; }
        public string NutriScore { get; set; }
        public string Category { get; set; }
        public string HealthLabel { get; set; }
        public int HealthScore { get; set; }
        public NutritionalInfo Nutrition { get; set; }

        public bool IsHealthy => HealthScore >= 60;
    }
}
