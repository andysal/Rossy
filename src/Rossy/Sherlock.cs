using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace Rossy
{
    public class Sherlock
    {
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

        public ComputerVisionClient Client { get; private set; }

        public Configuration Config { get; private set; }

        public Sherlock(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Client = Authenticate(Config.Endpoint, Config.SubscriptionKey);
        }

        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                                { Endpoint = endpoint };
            return client;
        }

        public AnalysisResult Analyze(string blobUrl, string intent)
        {
            switch (intent)
            {
                case "People":
                    return this.People(blobUrl);
                case "FullScan":
                default:
                    return this.FullScan(blobUrl);
            }
        }

        private AnalysisResult FullScan(string imageUrl)
        {
            var resultBuilder = new StringBuilder();
            var logBuilder = new StringBuilder();
            logBuilder.Append("----------------------------------------------------------\n");
            logBuilder.Append("ANALYZE IMAGE - FULL SCAN\n");
            

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            logBuilder.Append($"Analyzing the image {Path.GetFileName(imageUrl)}...\n");
            
            // Analyze the URL image 
            ImageAnalysis results = Client.AnalyzeImageAsync(imageUrl, features).Result;

            // Summarizes the image content.
            logBuilder.Append("Summary:\n");
            foreach (var caption in results.Description.Captions)
            {
                logBuilder.Append($"{caption.Text} with confidence {caption.Confidence}\n");
            }
            resultBuilder.Append($"{results.Description.Captions.First().Text}");

            // Display categories the image is divided into.
            logBuilder.Append("Categories:\n");
            foreach (var category in results.Categories)
            {
                logBuilder.Append($"{category.Name} with confidence {category.Score}\n");
            }
            

            // Image tags and their confidence score
            logBuilder.Append("Tags:\n");
            foreach (var tag in results.Tags)
            {
                logBuilder.Append($"{tag.Name} {tag.Confidence}\n");
            }
            

            // Objects
            logBuilder.Append("Objects:\n");
            foreach (var obj in results.Objects)
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
            foreach (var face in results.Faces)
            {
                logBuilder.Append($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                $"{face.FaceRectangle.Top + face.FaceRectangle.Height}\n");
            }
            
            return new AnalysisResult(resultBuilder.ToString(), logBuilder.ToString());

        }
        private AnalysisResult People(string imageUrl)
        {
            var resultBuilder = new StringBuilder();
            var logBuilder = new StringBuilder();

            logBuilder.Append("----------------------------------------------------------\n");
            logBuilder.Append("ANALYZE IMAGE - PEOPLE\n");
            

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
            };

            logBuilder.Append($"Analyzing the image {Path.GetFileName(imageUrl)}...\n");
            
            // Analyze the URL image 
            ImageAnalysis results = Client.AnalyzeImageAsync(imageUrl, features).Result;

            // Summarizes the image content.
            logBuilder.Append("Summary:\n");
            foreach (var caption in results.Description.Captions)
            {
                logBuilder.Append($"{caption.Text} with confidence {caption.Confidence}\n");
            }        

            // Faces
            logBuilder.Append("Faces:\n");
            foreach (var face in results.Faces)
            {
                logBuilder.Append($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                $"{face.FaceRectangle.Top + face.FaceRectangle.Height}\n");
            }

            if(results.Faces.Count==0)
            {
                resultBuilder.Append("There are no people around");
            }
            else if(results.Faces.Count==1)
            {
                var face = results.Faces.First();
                resultBuilder.Append($"There is one {face.Gender} person of age {face.Age} around");
            }
            else
            {
                resultBuilder.Append($"There are {results.Faces.Count} people around. More in detail: ");
                foreach (var face in results.Faces)
                {
                    resultBuilder.Append($"a {face.Gender} of age {face.Age}, ");
                }
                resultBuilder.Append("."); //a little hack
            }

            return new AnalysisResult(resultBuilder.ToString(), logBuilder.ToString());
        }


    }
}
