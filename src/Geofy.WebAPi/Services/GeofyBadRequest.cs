using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Geofy.WebAPI.Services
{
    public class GeofyBadRequest : ObjectResult
    {
        public GeofyBadRequest(ModelStateDictionary modelState) 
            : base(modelState.Values.SelectMany(x => x.Errors.Select(err => err.ErrorMessage) ))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}