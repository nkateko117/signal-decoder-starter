using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SignalDecoder.Domain.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SignalDecoder.Api.Swagger;

public class SwaggerExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(SimulateRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["devices"] = CreateDevicesExample()
            };
        }
        else if (context.Type == typeof(SimulateResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["receivedSignal"] = CreateReceivedSignalExample(),
                ["activeDeviceCount"] = new OpenApiInteger(3),
                ["signalLength"] = new OpenApiInteger(4),
                ["totalDevices"] = new OpenApiInteger(5)
            };
        }
        else if (context.Type == typeof(DecodeRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["devices"] = CreateDevicesExample(),
                ["receivedSignal"] = CreateReceivedSignalExample(),
                ["tolerance"] = new OpenApiInteger(0)
            };
        }
        else if (context.Type == typeof(DecodeResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["solutions"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["transmittingDevices"] = new OpenApiArray
                        {
                            new OpenApiString("d01"), new OpenApiString("d03"), new OpenApiString("d05")
                        },
                        ["decodedSignals"] = new OpenApiObject
                        {
                            ["d01"] = new OpenApiArray { new OpenApiInteger(2), new OpenApiInteger(4), new OpenApiInteger(1), new OpenApiInteger(3) },
                            ["d03"] = new OpenApiArray { new OpenApiInteger(3), new OpenApiInteger(6), new OpenApiInteger(2), new OpenApiInteger(8) },
                            ["d05"] = new OpenApiArray { new OpenApiInteger(5), new OpenApiInteger(2), new OpenApiInteger(3), new OpenApiInteger(1) }
                        },
                        ["computedSum"] = CreateReceivedSignalExample(),
                        ["matchesReceived"] = new OpenApiBoolean(true)
                    }
                },
                ["solutionCount"] = new OpenApiInteger(1),
                ["solveTimeMs"] = new OpenApiLong(42)
            };
        }
    }

    private static OpenApiObject CreateDevicesExample() => new()
    {
        ["d01"] = new OpenApiArray { new OpenApiInteger(2), new OpenApiInteger(4), new OpenApiInteger(1), new OpenApiInteger(3) },
        ["d02"] = new OpenApiArray { new OpenApiInteger(7), new OpenApiInteger(1), new OpenApiInteger(5), new OpenApiInteger(2) },
        ["d03"] = new OpenApiArray { new OpenApiInteger(3), new OpenApiInteger(6), new OpenApiInteger(2), new OpenApiInteger(8) },
        ["d04"] = new OpenApiArray { new OpenApiInteger(1), new OpenApiInteger(0), new OpenApiInteger(9), new OpenApiInteger(4) },
        ["d05"] = new OpenApiArray { new OpenApiInteger(5), new OpenApiInteger(2), new OpenApiInteger(3), new OpenApiInteger(1) }
    };

    private static OpenApiArray CreateReceivedSignalExample() => new()
    {
        new OpenApiInteger(10), new OpenApiInteger(12), new OpenApiInteger(6), new OpenApiInteger(12)
    };
}