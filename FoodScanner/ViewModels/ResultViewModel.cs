using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodScanner.Models;

namespace FoodScanner.ViewModels
{
    public class ResultViewModel : BaseViewModel
    {
        private Product _product;
        public Product Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        private Color _scoreColor;
        public Color ScoreColor
        {
            get => _scoreColor;
            set => SetProperty(ref _scoreColor, value);
        }

        public void LoadProduct(Product product)
        {
            Product = product;
            Title = product.Name;
            ScoreColor = GetColorForScore(product.NutriScore);
        }

        private Color GetColorForScore(string nutriScore)
        {
            return nutriScore switch
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
}
