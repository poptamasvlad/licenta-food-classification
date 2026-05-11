using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace FoodScanner.Research;

using CsvHelper.Configuration.Attributes;

public class ResearchProduct
{
    [Name("barcode")]
    public string Barcode { get; set; }

    [Name("name")]
    public string Name { get; set; }

    [Name("brand")]
    public string Brand { get; set; }

    [Name("calories")]
    public float Calories { get; set; }

    [Name("fat")]
    public float Fat { get; set; }

    [Name("saturated_fat")]
    public float SaturatedFat { get; set; }

    [Name("sugar")]
    public float Sugar { get; set; }

    [Name("salt")]
    public float Salt { get; set; }

    [Name("protein")]
    public float Protein { get; set; }

    [Name("fiber")]
    public float Fiber { get; set; }

    [Name("official_nutriscore")]
    public string OfficialNutriscore { get; set; }
}

public class ResearchResult
{
    public string Name { get; set; }
    public string Brand { get; set; }
    public string OfficialNutriscore { get; set; }
    public int OfficialScore { get; set; }
    public int MLScore { get; set; }
    public string MLLabel { get; set; }
    public int Difference { get; set; }
    public bool IsAgreement { get; set; }
    public string Category { get; set; }
}

public class ResearchAnalyzer
{
    private readonly string _modelPath;

    public ResearchAnalyzer(string modelPath)
    {
        _modelPath = modelPath;
    }

    public List<ResearchProduct> LoadProducts(string csvPath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<ResearchProduct>().ToList();
    }

    public int NutriscoreToScore(string grade) => grade?.ToUpper() switch
    {
        "A" => 85,
        "B" => 70,
        "C" => 50,
        "D" => 30,
        "E" => 15,
        _ => 50
    };

    public string ScoreToLabel(int score) => score switch
    {
        >= 80 => "Foarte sanatos",
        >= 60 => "Sanatos",
        >= 40 => "Moderat",
        _ => "Nesanatos"
    };

    public string NutriscoreToCategory(string grade) => grade?.ToUpper() switch
    {
        "A" or "B" => "Sanatos",
        "C" => "Moderat",
        "D" or "E" => "Nesanatos",
        _ => "Necunoscut"
    };

    public bool ScoresAgree(int mlScore, string officialGrade)
    {
        var mlCategory = mlScore switch
        {
            >= 60 => "Sanatos",
            >= 40 => "Moderat",
            _ => "Nesanatos"
        };

        var officialCategory = NutriscoreToCategory(officialGrade);
        return mlCategory == officialCategory;
    }
}