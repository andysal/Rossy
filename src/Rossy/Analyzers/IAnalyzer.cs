using Azure;
using Azure.AI.Vision;
using Azure.AI.Vision.Face;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rossy.Analyzers
{
    public interface IAnalyzer
    {
        VisualFeatures SetupImageAnalysisFeatures();
        IEnumerable<FaceAttributeType> SetupFaceAttributes();

        string ProduceLog(ImageAnalysisResult imageAnalysis, IReadOnlyList<FaceDetectionResult> detectedFaces);
        string ProduceSpeechTextEnglish(ImageAnalysisResult imageAnalysis, IReadOnlyList<FaceDetectionResult> detectedFaces);
        string ProduceSpeechTextItalian(ImageAnalysisResult imageAnalysis, IReadOnlyList<FaceDetectionResult> detectedFaces);
    }
}
