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

        string ProduceLog(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces);
        string ProduceSpeechTextEnglish(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces);
        string ProduceSpeechTextItalian(ImageAnalysisResult imageAnalysis, IEnumerable<FaceDetectionResult> detectedFaces);
    }
}
