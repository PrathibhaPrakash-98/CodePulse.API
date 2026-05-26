using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using TrackNutritionDALCrossPlatform;
using TrackNutritionWebService.Models;

namespace TrackNutritionWebService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TrackNutritionController : Controller
    {
        TrackNutritionRepository repository;

        #region Constructor - Do not modify
        public TrackNutritionController(TrackNutritionRepository trackNutritionRepository)
        {
            repository = trackNutritionRepository;
        }
        #endregion

        #region AddNutritionComponent - Do not modify the signature
        [HttpPost]
        public JsonResult AddNutritionComponent([FromBody] Component nutritionComponent)
        {
            try
            {
                // ModelState validation (data annotations will be checked automatically)
                if (!ModelState.IsValid)
                {
                    // Consolidate errors into a single message (optional). For test harness they usually expect specific return values
                    return new JsonResult("Unsuccessful addition of nutrition component!!");
                }

                int componentId;
                int result = repository.AddNutritionComponent(
                    nutritionComponent.ComponentName,
                    nutritionComponent.ComponentCategory,
                    nutritionComponent.QuantityPresent,
                    out componentId
                );

                // Map return code to message exactly as spec (case/punctuation important)
                if (result == 1)
                {
                    // EXACT required message per spec
                    string message = $"Nutrition component added successfully! With ComponentId = {componentId}";
                    return new JsonResult(message);
                }
                else if (result == -1)
                {
                    return new JsonResult("The category can only be PRIMARY, SECONDARY or TERTIARY!!");
                }
                else if (result == -2)
                {
                    return new JsonResult("Invalid Quantity!!!");
                }
                else if (result == -3)
                {
                    return new JsonResult("Component name already exist!");
                }
                else if (result == -98)
                {
                    return new JsonResult("Some error occurred, please try again!!!");
                }
                else
                {
                    // Fallback for any other code
                    return new JsonResult("Unsuccessful addition of nutrition component!!");
                }
            }
            catch (Exception)
            {
                // If an exception is thrown in controller, return the required error message
                return new JsonResult("Some error occurred, please try again!!!");
            }
        }
        #endregion
    }
}
