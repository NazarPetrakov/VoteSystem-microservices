using Common.Result;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthService.API.Extensions;

public static class ValidationExtensions
{
    //Add to common module
    public static List<Error> GetErrors(this ModelStateDictionary modelState)
    {
        var errors = modelState.Values.SelectMany(x => x.Errors)
            .Select(e => new Error("Validation.Error", e.ErrorMessage));

        return errors.ToList();
    }
}
