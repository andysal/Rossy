using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.AI.Vision.Face;
using Rossy.Analyzers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Rossy
{
    public class Geordi
    {
        public Rossy.Configuration RossyConfig { get; private set; }

        public Geordi(Rossy.Configuration rossyConfiguration)
        {
            RossyConfig = rossyConfiguration ?? throw new ArgumentNullException(nameof(rossyConfiguration));
        }

        public async Task<AnalysisResult> AnalyzeAsync(Stream image, string utterance)
        {
            var rosetta = new Rosetta(RossyConfig.RosettaConfig);
            var intent = rosetta.GuessIntent(utterance);
            var analyzer = GetAnalyzer(intent);
            var imageBinaryDaya = BinaryData.FromStream(image);

            var imageAnalysisFeatures = analyzer.SetupImageAnalysisFeatures();
            var client = new ImageAnalysisClient(new Uri(RossyConfig.ImageAnalysysServiceConfig.Endpoint), new AzureKeyCredential(RossyConfig.ImageAnalysysServiceConfig.SubscriptionKey));
            Response<ImageAnalysisResult> imageAnalysisResult = await client.AnalyzeAsync(imageBinaryDaya, imageAnalysisFeatures);
            var imageAnalysis = imageAnalysisResult.Value;

            var requiredFaceAttributes = analyzer.SetupFaceAttributes();
            FaceClient faceClient = new FaceClient(new Uri(RossyConfig.FaceDetectionServiceConfig.Endpoint), new AzureKeyCredential(RossyConfig.FaceDetectionServiceConfig.SubscriptionKey));
            var response = await faceClient.DetectAsync(imageBinaryDaya, FaceDetectionModel.Detection03, FaceRecognitionModel.Recognition04, true, returnFaceAttributes: requiredFaceAttributes);
            IReadOnlyList<FaceDetectionResult> detectedFaces = response.Value;

            string log = analyzer.ProduceLog(imageAnalysis, detectedFaces);
            var language = rosetta.GuessLanguage(utterance);
            string speechText = language switch
            {
                "it" => analyzer.ProduceSpeechTextItalian(imageAnalysis, detectedFaces),
                "en" => analyzer.ProduceSpeechTextEnglish(imageAnalysis, detectedFaces),
                _ => analyzer.ProduceSpeechTextEnglish(imageAnalysis, detectedFaces)
            };
            return new AnalysisResult(speechText, log);
        }

        private IAnalyzer GetAnalyzer(string intent)
        {
            return intent switch
            {
                "People" => new PeopleAnalysis(),
                _ => new BasicAnalysis(),
            };
        }

        public class Configuration
        {
            public string Endpoint { get; set; }
            public string SubscriptionKey { get; set; }
        }

        public class AnalysisResult
        {
            public AnalysisResult(string result, string log)
            {
                Result = result ?? throw new ArgumentNullException(nameof(result));
                Log = log ?? throw new ArgumentNullException(nameof(log));
            }

            public string Result { get; private set; }
            public string Log { get; private set; }
        }
    }
}
