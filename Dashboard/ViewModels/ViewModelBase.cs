using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dashboard.ViewModels;

public class ViewModelBase : ObservableObject
{
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}