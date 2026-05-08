using Microsoft.ML.Data;

namespace FoodScanner.ML.Trainer;

public class FoodData
{
    [LoadColumn(0)]
    public float Calories { get; set; }

    [LoadColumn(1)]
    public float Fat { get; set; }

    [LoadColumn(2)]
    public float SaturatedFat { get; set; }

    [LoadColumn(3)]
    public float Sugar { get; set; }

    [LoadColumn(4)]
    public float Salt { get; set; }

    [LoadColumn(5)]
    public float Protein { get; set; }

    [LoadColumn(6)]
    public float Fiber { get; set; }

    [LoadColumn(7), ColumnName("Label")]
    public float HealthScore { get; set; }
}

public class FoodPrediction
{
    [ColumnName("Score")]
    public float PredictedScore { get; set; }
}