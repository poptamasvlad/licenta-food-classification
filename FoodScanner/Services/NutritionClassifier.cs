using FoodScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodScanner.Services
{
    public class NutritionClassifier
    {
        public ClassificationResult Classify(Product product)
        {
            var result = new ClassificationResult();
            int totalScore = 0;

            totalScore += ScoreFromNutriScore(product.NutriScore);
            totalScore += ScoreFromNegativeNutrients(product.Nutrition, result);
            totalScore += ScoreFromPositiveNutrients(product.Nutrition, result);

            result.Score = Math.Clamp(totalScore, 0, 100);
            result.Label = GetLabel(result.Score);
            result.Color = GetColor(result.Score);
            result.Emoji = GetEmoji(result.Score);
            result.Summary = BuildSummary(result);

            return result;
        }

        private int ScoreFromNutriScore(string nutriScore)
        {
            return nutriScore switch
            {
                "A" => 50,
                "B" => 40,
                "C" => 30,
                "D" => 15,
                "E" => 0,
                _ => 25
            };
        }

        private int ScoreFromNegativeNutrients(NutritionalInfo n, ClassificationResult result)
        {
            if (n == null) return 15;
            int score = 30;

            if (n.Sugar > 22.5)
            {
                score -= 15;
                result.Warnings.Add("Continut foarte ridicat de zahar");
            }
            else if (n.Sugar > 10)
            {
                score -= 8;
                result.Warnings.Add("Continut ridicat de zahar");
            }

            if (n.SaturatedFat > 5)
            {
                score -= 8;
                result.Warnings.Add("Continut ridicat de grasimi saturate");
            }
            else if (n.SaturatedFat > 2.5)
            {
                score -= 4;
                result.Warnings.Add("Continut moderat de grasimi saturate");
            }

            if (n.Salt > 1.5)
            {
                score -= 7;
                result.Warnings.Add("Continut ridicat de sare");
            }
            else if (n.Salt > 0.6)
            {
                score -= 3;
                result.Warnings.Add("Continut moderat de sare");
            }

            if (n.Calories > 400)
            {
                score -= 5;
                result.Warnings.Add("Produs caloric");
            }

            return Math.Max(score, 0);
        }

        private int ScoreFromPositiveNutrients(NutritionalInfo n, ClassificationResult result)
        {
            if (n == null) return 0;
            int score = 0;

            if (n.Protein > 10)
            {
                score += 10;
                result.Positives.Add("Bogat in proteine");
            }
            else if (n.Protein > 5)
            {
                score += 5;
                result.Positives.Add("Sursa buna de proteine");
            }

            if (n.Fiber > 6)
            {
                score += 10;
                result.Positives.Add("Bogat in fibre");
            }
            else if (n.Fiber > 3)
            {
                score += 5;
                result.Positives.Add("Sursa buna de fibre");
            }

            return Math.Min(score, 20);
        }

        private string GetLabel(int score) => score switch
        {
            >= 80 => "Foarte sanatos",
            >= 60 => "Sanatos",
            >= 40 => "Moderat",
            _ => "Nesanatos"
        };

        private string GetColor(int score) => score switch
        {
            >= 80 => "#1a9641",
            >= 60 => "#a6d96a",
            >= 40 => "#fdae61",
            _ => "#d7191c"
        };

        private string GetEmoji(int score) => score switch
        {
            >= 80 => "++",
            >= 60 => "+",
            >= 40 => "~",
            _ => "-"
        };

        private string BuildSummary(ClassificationResult result)
        {
            if (!result.Warnings.Any() && !result.Positives.Any())
                return "Produs cu valori nutritionale medii.";

            var parts = new List<string>();

            if (result.Positives.Any())
                parts.Add(string.Join(", ", result.Positives).ToLower());

            if (result.Warnings.Any())
                parts.Add($"atentie la {string.Join(", ", result.Warnings).ToLower()}");

            return string.Join(". ", parts) + ".";
        }
    }
}
