using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rossy.Analyzers
{
    public interface IAnalyzer
    {
        List<VisualFeatureTypes?> SetupAnalysisFeatures();
        string ProduceLog(ImageAnalysis imageAnalysis);
        string ProduceSpeechTextEnglish(ImageAnalysis imageAnalysis);
        string ProduceSpeechTextItalian(ImageAnalysis imageAnalysis);
    }
}
