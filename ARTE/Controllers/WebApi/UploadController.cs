using CustomVisionAngular4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
namespace CustomVisionAngular4.Controllers
{
    [Route("api/upload")]
    public class UploadController : Controller
    {
        private string _TrainingKey;
        private string _PredictionKey;
        private string _PredictionURL;
        public UploadController(IOptions<ApplicationSettings> ApplicationSettings)
        {
            // Get the values from the appsettings.json file
            _TrainingKey = ApplicationSettings.Value.TrainingKey;
            _PredictionKey = ApplicationSettings.Value.PredictionKey;
            _PredictionURL = ApplicationSettings.Value.PredictionURL;
        }
        // api/Upload
        [HttpPost]
        public IActionResult Index(ICollection<IFormFile> files)
        {
            CustomVisionResponse FinalCustomVisionResponse =
                new CustomVisionResponse();
            if (!Request.HasFormContentType)
            {
                return BadRequest();
            }
            // Get the Form
            var form = Request.Form;
            // Process the first file passed 
            // (only one file should be passed)
            var file = form.Files[0];
            // Process file
            using (var readStream = file.OpenReadStream())
            {
                // Create a HttpClient to make the request
                using (HttpClient client = new HttpClient())
                {
                    // Set Prediction Key in the request headers 
                    client.DefaultRequestHeaders.Add("Prediction-Key", _PredictionKey);
                    // Serialize Request
                    MultipartFormDataContent _multiPartContent =
                        new MultipartFormDataContent();
                    StreamContent _imageData =
                        new StreamContent(readStream);
                    _imageData.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");
                    ContentDispositionHeaderValue _contentDispositionHeaderValue =
                        new ContentDispositionHeaderValue("form-data");
                    _contentDispositionHeaderValue.Name = "imageData";
                    _contentDispositionHeaderValue.FileName = file.Name;
                    _imageData.Headers.ContentDisposition = _contentDispositionHeaderValue;
                    _multiPartContent.Add(_imageData, "imageData");
                    // Make the request to the Custom Vision API
                    HttpResponseMessage response =
                        client.PostAsync(_PredictionURL, _multiPartContent).Result;

                    // Get the response
                    string ResponseContent = response.Content.ReadAsStringAsync().Result;
                    
                    // Convert the response to the CustomVisionResponse object
                    CustomVisionResponse TempCustomVisionResponse =
                        JsonConvert.DeserializeObject<CustomVisionResponse>(ResponseContent);
                    // Create the FinalCustomVisionResponse and set the main values to 
                    // the values in TempCustomVisionResponse
                    FinalCustomVisionResponse.Id = TempCustomVisionResponse.Id;
                    FinalCustomVisionResponse.Created = TempCustomVisionResponse.Created;
                    FinalCustomVisionResponse.Iteration = TempCustomVisionResponse.Iteration;
                    FinalCustomVisionResponse.Project = TempCustomVisionResponse.Project;
                    FinalCustomVisionResponse.Predictions = new List<Prediction>();
                    // The Predictions collection contains probabilities that are 
                    // in scientific notation that need to be converted to a percentage
                    foreach (var Prediction in TempCustomVisionResponse.Predictions)
                    {
                        // Make a Prediction object and set it to 
                        // the values in TempCustomVisionResponse.Predictions
                        Prediction objPrediction = new Prediction();
                        objPrediction.TagId = Prediction.TagId;
                        objPrediction.Tag = Prediction.Tag;
						objPrediction.Link = "https://onsitesolutionspt.weebly.com/" + Prediction.Tag;

						// Convert the Probability to a decimal 
						Decimal dProbability = 0;
                        Decimal.TryParse(Prediction.Probability, out dProbability);
                        // Convert the decimal to a percentage
                        objPrediction.Probability = dProbability.ToString("#0.##%");


                        // Add the Prediction object to the Predictions
                         Int32 precentage = Convert.ToInt32(Math.Round(dProbability, 2) * 100);
                         if (precentage >= 75)
                            FinalCustomVisionResponse.Predictions.Add(objPrediction);

					}
				}
            }
            // Return the CustomVisionResponse to the Angular application
            return Ok(FinalCustomVisionResponse);
        }
    }
}