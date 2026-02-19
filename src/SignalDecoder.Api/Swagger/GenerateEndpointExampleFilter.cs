using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SignalDecoder.Api.Swagger;

public class GenerateEndpointExampleFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.Name != nameof(Controllers.DevicesController.Generate))
            return;

        var example = new OpenApiObject
        {
            ["d01"] = new OpenApiArray { new OpenApiInteger(2), new OpenApiInteger(4), new OpenApiInteger(1), new OpenApiInteger(3) },
            ["d02"] = new OpenApiArray { new OpenApiInteger(7), new OpenApiInteger(1), new OpenApiInteger(5), new OpenApiInteger(2) },
            ["d03"] = new OpenApiArray { new OpenApiInteger(3), new OpenApiInteger(6), new OpenApiInteger(2), new OpenApiInteger(8) },
            ["d04"] = new OpenApiArray { new OpenApiInteger(1), new OpenApiInteger(0), new OpenApiInteger(9), new OpenApiInteger(4) },
            ["d05"] = new OpenApiArray { new OpenApiInteger(5), new OpenApiInteger(2), new OpenApiInteger(3), new OpenApiInteger(1) }
        };

        if (operation.Responses.TryGetValue("200", out var response))
        {
            foreach (var content in response.Content.Values)
            {
                content.Example = example;
            }
        }
    }
}