using Dashboard.ViewModels;
using Microsoft.Extensions.Logging;

namespace Dashboard.Models;

public interface IYamlLoader
{
    YamlData LoadYamlData(string filePath, ILogger<ScrutineeringViewModel> logger);
}