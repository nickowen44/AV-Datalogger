using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dashboard.ViewModels;

public class ViewModelBase : ObservableObject, IDisposable
{
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}