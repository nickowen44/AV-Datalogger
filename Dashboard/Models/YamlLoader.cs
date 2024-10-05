using System;
using System.Collections.Generic;
using System.IO;
using Dashboard.ViewModels;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dashboard.Models;

public class YamlLoader : IYamlLoader
{
    public YamlData LoadYamlData(string filePath, ILogger<ScrutineeringViewModel> logger)
    {
        try
        {
            var yamlContent = File.ReadAllText(filePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<YamlData>(yamlContent);
        }
        catch (Exception ex)
        {
            logger.LogError("Error loading the yaml file: {message}", ex.Message);

            return new YamlData
            {
                Steps = new List<StepData>
                {
                    new()
                    {
                        Step = "Error loading the yaml file please check logs.",
                        Measurements = new List<string>(),
                        Id = "0",
                        Title = "Error",
                        Caution = ""
                    }
                },
                Top = "",
                Bottom = ""
            };
        }
    }
}