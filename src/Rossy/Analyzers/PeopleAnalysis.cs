using Azure;
using Azure.AI.Vision;
using Azure.AI.Vision.Face;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rossy.Analyzers
{
    public class PeopleAnalysis : IAnalyzer
    {
        public VisualFeatures SetupImageAnalysisFeatures()
        {
            var features = 
                VisualFeatures.Caption | VisualFeatures.DenseCaptions |
                VisualFeatures.People | VisualFeatures.Tags;

            return features;
        }

        public IEnumerable<FaceAttributeType> SetupFaceAttributes()
        {
            var requiredFaceAttributes = new FaceAttributeType[] {
                FaceAttributeType.Age,
                FaceAttributeType.Smile,
                FaceAttributeType.FacialHair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Glasses,
            };

            return requiredFaceAttributes;
        }

        public string ProduceLog(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces)
        {
            var logBuilder = new StringBuilder();

            logBuilder.Append("----------------------------------------------------------\n");
            logBuilder.Append("ANALYZE IMAGE - PEOPLE\n");

            // Summarizes the image content.
            logBuilder.Append("Summary:\n");
            foreach (var caption in imageAnalysis.DenseCaptions.Values)
            {
                logBuilder.Append($"{caption.Text} with confidence {caption.Confidence}\n");
            }

            // People
            logBuilder.Append("Faces:\n");
            foreach (var face in imageAnalysis.People.Values)
            {
                logBuilder.Append($"A person at location " +
                $"{face.BoundingBox.X}, {face.BoundingBox.X + face.BoundingBox.Width}, " +
                $"{face.BoundingBox.Y}, {face.BoundingBox.Y + face.BoundingBox.Height}\n");
            }
            logBuilder.Append("----------------------------------------------------------\n");

            return logBuilder.ToString();
        }

        public string ProduceSpeechTextEnglish(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces)
        {
            var resultBuilder = new StringBuilder();
            if (detectedFaces.Count() == 0)
                resultBuilder.Append("There are no people around");
            else if (detectedFaces.Count() == 1)
            {
                var face = detectedFaces.First();
                resultBuilder.Append($"There is one person of age {face.FaceAttributes.Age}.");
            }
            else
            {
                resultBuilder.Append($"There are {detectedFaces.Count()} people around. More in detail: ");
                foreach (var face in detectedFaces)
                {
                    resultBuilder.Append($"a person of age {face.FaceAttributes.Age}, ");
                }
                resultBuilder.Append("."); //a little hack
            }
            var ssml = Modem.BuildSsmlAsync(resultBuilder.ToString(), "en").Result;
            return ssml;
        }

        public string ProduceSpeechTextItalian(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces)
        {
            var resultBuilder = new StringBuilder();
            if (detectedFaces.Count() == 0)
                resultBuilder.Append("Non vedo persone");
            else if (detectedFaces.Count() == 1)
            {
                var face = detectedFaces.First();
                resultBuilder.Append($"C'è una persona che sembra avere un'età di {face.FaceAttributes.Age} anni.");
            }
            else
            {
                resultBuilder.Append($"Ci sono {detectedFaces.Count()} persone. Più precisamente: ");
                foreach (var face in detectedFaces)
                {
                    resultBuilder.Append($"Una persona che sembra avere un'età di {face.FaceAttributes.Age} anni.");
                }
                resultBuilder.Append("."); //a little hack
            }
            var ssml = Modem.BuildSsmlAsync(resultBuilder.ToString(), "it").Result;
            return ssml;
        }
    }
}
