using System;
using System.Collections.Generic;
using System.IO;
using Dashboard.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dashboard.Utils;

public class YamlLoader : IYamlLoader
{
    public YamlData LoadYamlData(string filePath)
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
            Console.WriteLine("Error: " + ex.Message);

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