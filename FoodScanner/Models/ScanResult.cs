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
        public string NutriScore { get; set; }
        public string HealthLabel { get; set; }
        public DateTime ScannedAt { get; set; }
    }
}
