using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rossy.Analyzers
{
    public abstract class Analyzer
    {
        public abstract List<VisualFeatureTypes> SetupAnalysisFeatures();
        public abstract string ProduceLog(ImageAnalysis imageAnalysis);
        public abstract string ProduceSpeechText(ImageAnalysis imageAnalysis);

        public static Analyzer GetAnalyzer(string intent)
        {
            switch(intent)
            {
                case "People":
                    return new PeopleAnalysis();
                case "FullScan":
                default:
                    return new FullScanAnalysis();
            }
        }
    }
}
