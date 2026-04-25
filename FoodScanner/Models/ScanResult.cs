using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodScanner.Models
{
    public class ScanResult
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string NutriScore { get; set; }
        public string HealthLabel { get; set; }
        public int HealthScore { get; set; }
        public double Calories { get; set; }
        public double Fat { get; set; }
        public double Sugar { get; set; }
        public double Salt { get; set; }
        public double Protein { get; set; }
        public string ImageUrl { get; set; }
        public DateTime ScannedAt { get; set; }

        public string ScannedAtFormatted =>
            ScannedAt.ToString("dd MMM yyyy, HH:mm");

        public Color NutriScoreColor => NutriScore switch
        {
            "A" => Color.FromArgb("#1a9641"),
            "B" => Color.FromArgb("#a6d96a"),
            "C" => Color.FromArgb("#ffffbf"),
            "D" => Color.FromArgb("#fdae61"),
            "E" => Color.FromArgb("#d7191c"),
            _ => Color.FromArgb("#888888")
        };
    }
}
