using Dashboard.Models;

namespace Dashboard.Utils;

public interface IYamlLoader
{
    YamlData LoadYamlData(string filePath);
}