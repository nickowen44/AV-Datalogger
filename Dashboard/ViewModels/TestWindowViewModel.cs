using System;

namespace Dashboard.ViewModels;

public class TestWindowViewModel : ViewModelBase
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}