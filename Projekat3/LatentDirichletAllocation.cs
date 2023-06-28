using Microsoft.ML;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    public class TextData
    {
        public string Text { get; set; }
    }

    public class TransformedTextData : TextData
    {
        public float[] Features { get; set; }
    }
    class LatentDirichletAllocation
    {
        private static ConcurrentBag<string> _comments= new ConcurrentBag<string>();
        private static int count = 0;
        public static PredictionEngine<TextData, TransformedTextData> PredictionEngine { get; set; }

        public static void ProccessData(ConcurrentBag<string> coms)
        {
            _comments.Clear();
            _comments = coms;
            count= _comments.Count;
          
        }
        public static void RunAnalysis(int topicNum)
        {
            var mlContext = new MLContext();

            var samples = new List<TextData>();
            for(int i=0;i< count/2;i++)
            {
                samples.Add(new TextData() {Text= _comments.ElementAt(i) });
            }

            var dataview = mlContext.Data.LoadFromEnumerable(samples);

            var pipeline = mlContext.Transforms.Text.NormalizeText("NormalizedText",
                "Text")
                .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens",
                    "NormalizedText"))
                .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens"))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Tokens"))
                .Append(mlContext.Transforms.Text.ProduceNgrams("Tokens"))
                .Append(mlContext.Transforms.Text.LatentDirichletAllocation(
                    "Features", "Tokens", numberOfTopics: topicNum));

            var transformer = pipeline.Fit(dataview);
            
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TextData,
                TransformedTextData>(transformer);
            PredictionEngine = predictionEngine;
        }
        public static string GetPrediction(string text)
        {
            var prediction = PredictionEngine.Predict(new TextData() { Text = text });
            return PrintLdaFeatures(prediction);
        }

        private static string PrintLdaFeatures(TransformedTextData prediction)
        {
            string result = "";
            for (int i = 0; i < prediction.Features.Length; i++)
            {
                Console.Write($"{prediction.Features[i]:F4}  ");
                result += $"{prediction.Features[i]:F4}  ";
            }
            result+="\n";
            Console.WriteLine();
            return result;
        }
    }
}
