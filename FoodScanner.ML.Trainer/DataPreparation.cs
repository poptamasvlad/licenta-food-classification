namespace FoodScanner.ML.Trainer;

public class DataPreparation
{
    private static Dictionary<string, int>? _headerMap;

    public static void LoadHeaders(string csvPath)
    {
        var firstLine = File.ReadLines(csvPath).First();
        var headers = firstLine.Split('\t');

        _headerMap = headers
            .Select((h, i) => new { Name = h.Trim(), Index = i })
            .ToDictionary(x => x.Name, x => x.Index);
    }

    public static void PrepareDataset(
        string inputPath,
        string outputPath,
        int maxRows = 500000)
    {
        Console.WriteLine("Se pregatesc datele...");

        var lines = new List<string>
        {
            "calories,fat,saturated_fat,sugar,salt,protein,fiber,health_score"
        };

        int processed = 0;
        int skipped = 0;

        foreach (var line in File.ReadLines(inputPath))
        {
            if (processed == 0)
            {
                processed++;
                continue;
            }

            if (lines.Count >= maxRows + 1)
                break;

            var columns = ParseCsvLine(line);

            try
            {
                var nutriscoreGrade = GetColumn(columns, "nutriscore_grade");

                var calories = ParseNullableFloat(GetColumn(columns, "energy-kcal_100g"));
                var fat = ParseNullableFloat(GetColumn(columns, "fat_100g"));
                var saturatedFat = ParseNullableFloat(GetColumn(columns, "saturated-fat_100g"));
                var sugar = ParseNullableFloat(GetColumn(columns, "sugars_100g"));
                var salt = ParseNullableFloat(GetColumn(columns, "salt_100g"));
                var protein = ParseNullableFloat(GetColumn(columns, "proteins_100g"));
                var fiber = ParseNullableFloat(GetColumn(columns, "fiber_100g"));

                if (string.IsNullOrWhiteSpace(nutriscoreGrade))
                {
                    skipped++;
                    continue;
                }

                if (calories == null || fat == null || saturatedFat == null ||
                    sugar == null || salt == null || protein == null || fiber == null)
                {
                    skipped++;
                    continue;
                }

                if (!IsValidNutritionValue(calories.Value, 0, 900) ||
                    !IsValidNutritionValue(fat.Value, 0, 100) ||
                    !IsValidNutritionValue(saturatedFat.Value, 0, 100) ||
                    !IsValidNutritionValue(sugar.Value, 0, 100) ||
                    !IsValidNutritionValue(salt.Value, 0, 100) ||
                    !IsValidNutritionValue(protein.Value, 0, 100) ||
                    !IsValidNutritionValue(fiber.Value, 0, 100))
                {
                    skipped++;
                    continue;
                }

                float healthScore = nutriscoreGrade.ToUpper().Trim() switch
                {
                    "A" => 90f,
                    "B" => 72f,
                    "C" => 54f,
                    "D" => 32f,
                    "E" => 15f,
                    _ => -1f
                };

                if (healthScore < 0)
                {
                    skipped++;
                    continue;
                }

                lines.Add(
                    $"{calories.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{fat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{saturatedFat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{sugar.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{salt.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{protein.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{fiber.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{healthScore.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
                );

                processed++;

                if (processed % 10000 == 0)
                    Console.WriteLine($"Procesate: {processed} | Valide: {lines.Count - 1} | Sarite: {skipped}");
            }
            catch
            {
                skipped++;
            }
        }

        File.WriteAllLines(outputPath, lines);

        Console.WriteLine($"Dataset pregatit: {lines.Count - 1} produse valide");
        Console.WriteLine($"Produse sarite: {skipped}");
        Console.WriteLine($"Salvat la: {outputPath}");
    }

    private static bool IsValidNutritionValue(float value, float min, float max)
    {
        return value >= min && value <= max;
    }

    private static float? ParseNullableFloat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return float.TryParse(
            value,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out var result
        ) ? result : null;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == '\t' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(c);
        }

        result.Add(current.ToString());
        return result.ToArray();
    }

    private static string GetColumn(string[] columns, string name)
    {
        if (_headerMap != null &&
            _headerMap.TryGetValue(name, out int idx) &&
            idx >= 0 &&
            idx < columns.Length)
        {
            return columns[idx];
        }

        return string.Empty;
    }
}