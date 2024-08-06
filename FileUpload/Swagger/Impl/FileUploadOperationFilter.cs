using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IEnumerable<IFormFile>))
            .ToList();

        if (fileParams.Count == 0) return;

        var fileParamNames = fileParams.Select(p => p.Name).ToList();

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = fileParamNames.ToDictionary(name => name, _ => new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }),
                        Required = fileParamNames.ToHashSet()
                    }
                }
            }
        };
    }
}