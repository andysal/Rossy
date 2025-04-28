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
    public class BasicAnalysis : IAnalyzer
    {
        public VisualFeatures SetupImageAnalysisFeatures()
        {
            var features =
                VisualFeatures.Caption | VisualFeatures.DenseCaptions |
                VisualFeatures.Objects | VisualFeatures.People |
                VisualFeatures.Read | VisualFeatures.SmartCrops |
                VisualFeatures.Tags;

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
            logBuilder.Append("ANALYZE IMAGE - FULL SCAN\n");

            // Summarizes the image content.
            logBuilder.Append("Summary:\n");
            foreach (var caption in imageAnalysis.DenseCaptions.Values)
            {
                logBuilder.Append($"{caption.Text} with confidence {caption.Confidence}\n");
            }

            // Image tags and their confidence score
            logBuilder.Append("Tags:\n");
            foreach (var tag in imageAnalysis.Tags.Values)
            {
                logBuilder.Append($"{tag.Name} {tag.Confidence}\n");
            }

            // Objects
            logBuilder.Append("Objects:\n");
            foreach (var obj in imageAnalysis.Objects.Values)
            {
                foreach (var tag in obj.Tags)
                    logBuilder.Append($"{tag.Name} with confidence {tag.Confidence}");
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
            var ssml = Modem.BuildSsmlAsync(imageAnalysis.Caption.Text, "en").Result;
            return ssml;
        }

        public string ProduceSpeechTextItalian(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces)
        {
            var ssml = Modem.BuildSsmlAsync(imageAnalysis.Caption.Text, "it").Result;
            return ssml;
        }
    }
}
