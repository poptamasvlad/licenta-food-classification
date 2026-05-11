using Microsoft.ML.Data;

namespace FoodScanner.Models;

public class FoodMLData
{
    [ColumnName("Calories")]
    public float Calories { get; set; }

    [ColumnName("Fat")]
    public float Fat { get; set; }

    [ColumnName("SaturatedFat")]
    public float SaturatedFat { get; set; }

    [ColumnName("Sugar")]
    public float Sugar { get; set; }

    [ColumnName("Salt")]
    public float Salt { get; set; }

    [ColumnName("Protein")]
    public float Protein { get; set; }

    [ColumnName("Fiber")]
    public float Fiber { get; set; }
}

public class FoodMLPrediction
{
    [ColumnName("Score")]
    public float PredictedScore { get; set; }
}