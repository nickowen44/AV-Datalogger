using Dashboard.Models;
using Dashboard.ViewModels;
using Microsoft.Extensions.Logging;

namespace Dashboard.Utils;

public interface IYamlLoader
{
    YamlData LoadYamlData(string filePath, ILogger<ScrutineeringViewModel> logger);
}