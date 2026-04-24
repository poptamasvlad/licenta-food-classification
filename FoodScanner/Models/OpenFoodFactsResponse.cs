using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

public class OpenFoodFactsResponse
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("product")]
    public OpenFoodFactsProduct Product { get; set; }
}

public class OpenFoodFactsProduct
{
    [JsonProperty("product_name")]
    public string ProductName { get; set; }

    [JsonProperty("brands")]
    public string Brands { get; set; }

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }

    [JsonProperty("nutriscore_grade")]
    public string NutriscoreGrade { get; set; }

    [JsonProperty("categories")]
    public string Categories { get; set; }

    [JsonProperty("nutriments")]
    public OpenFoodFactsNutriments Nutriments { get; set; }
}

public class OpenFoodFactsNutriments
{
    [JsonProperty("energy-kcal_100g")]
    public double Calories { get; set; }

    [JsonProperty("fat_100g")]
    public double Fat { get; set; }

    [JsonProperty("saturated-fat_100g")]
    public double SaturatedFat { get; set; }

    [JsonProperty("sugars_100g")]
    public double Sugar { get; set; }

    [JsonProperty("salt_100g")]
    public double Salt { get; set; }

    [JsonProperty("proteins_100g")]
    public double Protein { get; set; }

    [JsonProperty("fiber_100g")]
    public double Fiber { get; set; }
}
