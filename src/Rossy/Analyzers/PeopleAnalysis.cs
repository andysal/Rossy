using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rossy.Analyzers
{
    public class PeopleAnalysis : IAnalyzer
    {
        public List<VisualFeatureTypes> SetupAnalysisFeatures()
        {
            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
            };

            return features;
        }

        public string ProduceLog(ImageAnalysis imageAnalysis)
        {
            var logBuilder = new StringBuilder();

            logBuilder.Append("----------------------------------------------------------\n");
            logBuilder.Append("ANALYZE IMAGE - PEOPLE\n");

            // Summarizes the image content.
            logBuilder.Append("Summary:\n");
            foreach (var caption in imageAnalysis.Description.Captions)
            {
                logBuilder.Append($"{caption.Text} with confidence {caption.Confidence}\n");
            }

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
            if (imageAnalysis.Faces.Count == 0)
            {
                resultBuilder.Append("There are no people around");
            }
            else if (imageAnalysis.Faces.Count == 1)
            {
                var face = imageAnalysis.Faces.First();
                resultBuilder.Append($"There is one {face.Gender} person of age {face.Age}.");
            }
            else
            {
                resultBuilder.Append($"There are {imageAnalysis.Faces.Count} people around. More in detail: ");
                foreach (var face in imageAnalysis.Faces)
                {
                    resultBuilder.Append($"a {face.Gender} of age {face.Age}, ");
                }
                resultBuilder.Append("."); //a little hack
            }

            return resultBuilder.ToString();
        }

        public string ProduceSpeechTextItalian(ImageAnalysis imageAnalysis)
        {
            var resultBuilder = new StringBuilder();
            if (imageAnalysis.Faces.Count == 0)
            {
                resultBuilder.Append("Non vedo persone");
            }
            else if (imageAnalysis.Faces.Count == 1)
            {
                var face = imageAnalysis.Faces.First();
                resultBuilder.Append(face.Gender == Gender.Female ? "C'è una donna" : "C'è un uomo");
                resultBuilder.Append($" che sembra avere { face.Age} anni");
            }
            else
            {
                resultBuilder.Append($"Ci sono {imageAnalysis.Faces.Count}. Più precisamente: ");
                foreach (var face in imageAnalysis.Faces)
                {
                    resultBuilder.Append(face.Gender == Gender.Female ? "una donna" : "un uomo");
                    resultBuilder.Append($" che sembra avere {face.Age} anni, ");
                }
                resultBuilder.Append("."); //a little hack
            }

            return resultBuilder.ToString();
        }
    }
}
