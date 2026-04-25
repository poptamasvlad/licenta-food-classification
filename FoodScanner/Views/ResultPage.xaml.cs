using FoodScanner.Models;
using FoodScanner.ViewModels;

namespace FoodScanner.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage(ResultViewModel viewModel, Product product)
    {
        InitializeComponent();
    }
}