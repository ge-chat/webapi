using System;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

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