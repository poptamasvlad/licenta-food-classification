using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodScanner.Models
{
    public class ClassificationResult
    {
        public int Score { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public string Emoji { get; set; }
        public List<string> Warnings { get; set; } = new();
        public List<string> Positives { get; set; } = new();
        public string Summary { get; set; }
    }
}
