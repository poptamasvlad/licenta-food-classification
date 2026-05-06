using SQLite;

namespace FoodScanner.Models;

public class ManualProduct
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Barcode { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double SaturatedFat { get; set; }
    public double Sugar { get; set; }
    public double Salt { get; set; }
    public double Protein { get; set; }
    public double Fiber { get; set; }
    public DateTime AddedAt { get; set; }

    public Product ToProduct()
    {
        return new Product
        {
            Barcode = Barcode,
            Name = Name,
            Brand = Brand,
            NutriScore = "?",
            Nutrition = new NutritionalInfo
            {
                Calories = Calories,
                Fat = Fat,
                SaturatedFat = SaturatedFat,
                Sugar = Sugar,
                Salt = Salt,
                Protein = Protein,
                Fiber = Fiber
            }
        };
    }
}