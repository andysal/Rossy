using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rossy.Analyzers
{
    public class FullScanAnalysis : IAnalyzer
    {
        public List<VisualFeatureTypes> SetupAnalysisFeatures()
        {
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            return features;
        }

        public string ProduceLog(ImageAnalysis imageAnalysis)
        {
            var logBuilder = new StringBuilder();
            logBuilder.Append("----------------------------------------------------------\n");
            logBuilder.Append("ANALYZE IMAGE - FULL SCAN\n");

            // Summarizes the image content.
            logBuilder.Append("Summary:\n");
            foreach (var caption in imageAnalysis.Description.Captions)
            {
                logBuilder.Append($"{caption.Text} with confidence {caption.Confidence}\n");
            }

            // Display categories the image is divided into.
            logBuilder.Append("Categories:\n");
            foreach (var category in imageAnalysis.Categories)
            {
                logBuilder.Append($"{category.Name} with confidence {category.Score}\n");
            }


            // Image tags and their confidence score
            logBuilder.Append("Tags:\n");
            foreach (var tag in imageAnalysis.Tags)
            {
                logBuilder.Append($"{tag.Name} {tag.Confidence}\n");
            }


            // Objects
            logBuilder.Append("Objects:\n");
            foreach (var obj in imageAnalysis.Objects)
            {
                logBuilder.Append($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}\n");
            }


            //// Well-known (or custom, if set) brands.
            //builder.Append("Brands:\n");
            //foreach (var brand in results.Brands)
            //{
            //    builder.Append($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
            //    $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}\n");
            //}


            // Faces
            logBuilder.Append("Faces:\n");
            foreach (var face in imageAnalysis.Faces)
            {
                logBuilder.Append($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                $"{face.FaceRectangle.Top + face.FaceRectangle.Height}\n");
            }

            logBuilder.Append("----------------------------------------------------------\n");

            return logBuilder.ToString();
        }

        public string ProduceSpeechTextEnglish(ImageAnalysis imageAnalysis)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.Append("<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">");
            resultBuilder.Append("<voice name=\"en-US-GuyNeural\">");
            resultBuilder.Append($"{imageAnalysis.Description.Captions.First().Text}");
            resultBuilder.Append("</voice>");
            resultBuilder.Append("</speak>");
            return resultBuilder.ToString();
        }

        public string ProduceSpeechTextItalian(ImageAnalysis imageAnalysis)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.Append("<speak version=\"1.0\" xmlns=\"https://www.w3.org/2001/10/synthesis\" xml:lang=\"it-IT\">");
            resultBuilder.Append("<voice name=\"it-IT-ElsaNeural\">");
            resultBuilder.Append($"{imageAnalysis.Description.Captions.First().Text}");
            resultBuilder.Append("</voice>");
            resultBuilder.Append("</speak>");
            return resultBuilder.ToString();
        }
    }
}
