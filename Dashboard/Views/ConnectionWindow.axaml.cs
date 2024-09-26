using System;
using System.IO.Ports;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Dashboard.Views;

public partial class ConnectionWindow : UserControl
{
    public ConnectionWindow()
    {
        InitializeComponent();
    }


    /*private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
    }*/

    private void RefreshPorts(object? sender, RoutedEventArgs? e)
    {
        var cmbSerialPort = this.Find<ComboBox>("cmbSerialPort");
        //cmbSerialPort.Items.Add(null);
        cmbSerialPort.SelectedIndex = 0;

        // Get available serial ports
        var ports = SerialPort.GetPortNames();

        // Add ports to ComboBox
        if (ports.Length != 0)
        {
            //cmbSerialPort.Items.Add(ports);

            foreach (var n in ports)
            {
                cmbSerialPort.Items.Add(
                    new ComboBoxItem()
                    {
                        Content = n
                    }
                );
            }
            cmbSerialPort.SelectedIndex = 0;
        }
        else
        {
            cmbSerialPort.Items.Add("No Ports Available");
        }
    }
}