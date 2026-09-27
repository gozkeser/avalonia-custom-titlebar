using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace TitleBarDemo;

/// <summary>
/// Demo host window verifying Custom App Title Bar generic slot injection and interactions.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnToggleCompactClicked(object? sender, RoutedEventArgs e)
    {
        CompactSwitch.IsChecked = !CompactSwitch.IsChecked;
    }

    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = app.ActualThemeVariant == ThemeVariant.Dark
                ? ThemeVariant.Light
                : ThemeVariant.Dark;
        }
    }
}