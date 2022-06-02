using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Auth
{
    public static class Utils
    {
        public static async Task<ObjectResult> WrapExceptionAsync<T>(Task<T> func)
        {
            try
            {
                return new OkObjectResult(await func);
            }
            catch (Exception e)
            {
                var error = e.GetBaseException();
                return new ObjectResult(error.GetType().Name + ": " + error.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
