using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text;

namespace Rossy
{
    public class Sherlock
    {
        public class Configuration
        {
            public string Endpoint { get; set; }
            public string SubscriptionKey { get; set; }
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

        public string FullScan(string imageUrl)
        {
            var builder = new StringBuilder();
            builder.Append("----------------------------------------------------------");
            builder.Append("ANALYZE IMAGE - URL");
            

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            builder.Append($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            
            // Analyze the URL image 
            ImageAnalysis results = Client.AnalyzeImageAsync(imageUrl, features).Result;

            // Summarizes the image content.
            builder.Append("Summary:");
            foreach (var caption in results.Description.Captions)
            {
                builder.Append($"{caption.Text} with confidence {caption.Confidence}");
            }
            

            // Display categories the image is divided into.
            builder.Append("Categories:");
            foreach (var category in results.Categories)
            {
                builder.Append($"{category.Name} with confidence {category.Score}");
            }
            

            // Image tags and their confidence score
            builder.Append("Tags:");
            foreach (var tag in results.Tags)
            {
                builder.Append($"{tag.Name} {tag.Confidence}");
            }
            

            // Objects
            builder.Append("Objects:");
            foreach (var obj in results.Objects)
            {
                builder.Append($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            

            // Well-known (or custom, if set) brands.
            builder.Append("Brands:");
            foreach (var brand in results.Brands)
            {
                builder.Append($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
                $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
            }
            

            // Faces
            builder.Append("Faces:");
            foreach (var face in results.Faces)
            {
                builder.Append($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
            }
            

            // Adult or racy content, if any.
            builder.Append("Adult:");
            builder.Append($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
            builder.Append($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");


            return builder.ToString();

        }
        public string People(string imageUrl)
        {
            var builder = new StringBuilder();

            builder.Append("----------------------------------------------------------");
            builder.Append("ANALYZE IMAGE - URL");
            

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
            };

            builder.Append($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            
            // Analyze the URL image 
            ImageAnalysis results = Client.AnalyzeImageAsync(imageUrl, features).Result;

            // Image tags and their confidence score
            builder.Append("Tags:");
            foreach (var tag in results.Tags)
            {
                builder.Append($"{tag.Name} {tag.Confidence}");
            }
            

            // Faces
            builder.Append("Faces:");
            foreach (var face in results.Faces)
            {
                builder.Append($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
            }
            

            // Adult or racy content, if any.
            builder.Append("Adult:");
            builder.Append($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
            builder.Append($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");
            

            return builder.ToString();
        }


    }
}
