using System.Collections.ObjectModel;
using System.Windows;
using Dacdx.Protocol;

namespace Dacdx.Windows.App;

public partial class MainWindow : Window
{
    private readonly DiscoveryClient _discovery = new();
    private readonly AudioSender _audioSender = new();
    private LoopbackStreamer? _loopbackStreamer;

    public ObservableCollection<DiscoveredDevice> Devices { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        ChannelColumn.ItemsSource = Enum.GetValues<ChannelRole>();
    }

    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Scanning...";
        try
        {
            Devices.Clear();
            var found = await _discovery.ScanAsync(TimeSpan.FromSeconds(3), CancellationToken.None);
            foreach (var device in found)
            {
                Devices.Add(device);
            }

            StatusText.Text = $"Scan complete. Found {Devices.Count} sound endpoints.";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Scan failed: " + ex.Message;
        }
    }

    private async void SendTone_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Sending test tone...";
        try
        {
            await _audioSender.SendTestToneAsync(Devices.ToArray(), TimeSpan.FromSeconds(2), CancellationToken.None);
            StatusText.Text = "Test tone sent.";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Test tone failed: " + ex.Message;
        }
    }

    private void Loopback_Click(object sender, RoutedEventArgs e)
    {
        if (_loopbackStreamer is not null)
        {
            _loopbackStreamer.Dispose();
            _loopbackStreamer = null;
            LoopbackButton.Content = "Start loopback";
            StatusText.Text = "Loopback stopped.";
            return;
        }

        var enabled = Devices.Where(d => d.Enabled).ToArray();
        if (enabled.Length == 0)
        {
            StatusText.Text = "Select at least one endpoint before loopback.";
            return;
        }

        _loopbackStreamer = new LoopbackStreamer(enabled);
        _loopbackStreamer.Start();
        LoopbackButton.Content = "Stop loopback";
        StatusText.Text = "Loopback capture started.";
    }
}
