using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace DashboardTests;

public  static class FindControlHelper
{
    public static T FindControlInVisualTree<T>(Visual root, string controlName) where T : Control
    {
        if (root == null)
            return null;

        if (root is T control && control.Name == controlName)
            return control;

        foreach (var child in root.GetVisualChildren())
        {
            var result = FindControlInVisualTree<T>(child, controlName);
            if (result != null)
                return result;
        }

        return null;
    }
}