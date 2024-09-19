using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class FooterViewModel : ViewModelBase 
{
    private readonly IDataStore _dataStore;

    public string CarID => _dataStore.AvStatusData?.CarId ?? "0";
    public string UTCTime => _dataStore.AvStatusData?.UTCTime ?? "0";
    
    public FooterViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.AvDataUpdated += OnAvDataChanged;
    }
    public FooterViewModel()
    {
        _dataStore = new DataStore(new DummyConnector());
    }
    
    /// <summary>
    ///     Notifies the view that the AV data has changed.
    /// </summary>
    private void OnAvDataChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("AV Data Updated in FooterViewModel");
        OnPropertyChanged(nameof(CarID));
        OnPropertyChanged(nameof(UTCTime));
    }
    
    public void Dispose()
    {
        _dataStore.AvDataUpdated -= OnAvDataChanged;

        _dataStore.Dispose();

        GC.SuppressFinalize(this);
    }
    
}

