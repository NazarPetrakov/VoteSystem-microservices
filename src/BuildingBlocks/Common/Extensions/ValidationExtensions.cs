using Common.Result;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common.Extensions;

public static class ValidationExtensions
{
    public static List<Error> GetErrors(this ModelStateDictionary modelState)
    {
        var errors = modelState.Values.SelectMany(x => x.Errors)
            .Select(e => new Error("Validation.Error", e.ErrorMessage));

        return errors.ToList();
    }
}
