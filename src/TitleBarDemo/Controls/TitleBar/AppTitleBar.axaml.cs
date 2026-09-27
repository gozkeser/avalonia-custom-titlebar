using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using TitleBarDemo.Localization;

namespace TitleBarDemo.Controls.TitleBar;

/// <summary>
/// Fully generic, reusable 4-column Custom Application Title Bar supporting content slots and compact mode.
/// Complies with modern desktop shell standards and best practices.
/// </summary>
public partial class AppTitleBar : UserControl
{
    public static readonly StyledProperty<object?> IconContentProperty =
        AvaloniaProperty.Register<AppTitleBar, object?>(nameof(IconContent));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<AppTitleBar, string>(nameof(Title), defaultValue: string.Empty);

    public static readonly StyledProperty<string> VersionTextProperty =
        AvaloniaProperty.Register<AppTitleBar, string>(nameof(VersionText), defaultValue: string.Empty);

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<AppTitleBar, string>(nameof(Subtitle), defaultValue: string.Empty);

    public static readonly StyledProperty<object?> HeaderContentProperty =
        AvaloniaProperty.Register<AppTitleBar, object?>(nameof(HeaderContent));

    public static readonly StyledProperty<object?> PrimaryActionsProperty =
        AvaloniaProperty.Register<AppTitleBar, object?>(nameof(PrimaryActions));

    public static readonly StyledProperty<object?> SecondaryActionsProperty =
        AvaloniaProperty.Register<AppTitleBar, object?>(nameof(SecondaryActions));

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<AppTitleBar, bool>(nameof(IsCompact), defaultValue: false);

    public object? IconContent
    {
        get => GetValue(IconContentProperty);
        set => SetValue(IconContentProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string VersionText
    {
        get => GetValue(VersionTextProperty);
        set => SetValue(VersionTextProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public object? HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public object? PrimaryActions
    {
        get => GetValue(PrimaryActionsProperty);
        set => SetValue(PrimaryActionsProperty, value);
    }

    public object? SecondaryActions
    {
        get => GetValue(SecondaryActionsProperty);
        set => SetValue(SecondaryActionsProperty, value);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    private Window? _parentWindow;

    static AppTitleBar()
    {
        IsCompactProperty.Changed.AddClassHandler<AppTitleBar>((x, e) => x.OnIsCompactChanged(e));
    }

    public AppTitleBar()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _parentWindow = TopLevel.GetTopLevel(this) as Window ?? VisualRoot as Window;
        if (_parentWindow != null)
        {
            _parentWindow.PropertyChanged += OnWindowPropertyChanged;
            UpdateMaximizeButtonContent(_parentWindow.WindowState);
        }
        UpdateWindowHeightHint(IsCompact);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_parentWindow != null)
        {
            _parentWindow.PropertyChanged -= OnWindowPropertyChanged;
            _parentWindow = null;
        }
    }

    private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty && e.NewValue is WindowState state)
        {
            UpdateMaximizeButtonContent(state);
        }
    }

    private void UpdateMaximizeButtonContent(WindowState state)
    {
        var isMax = state == WindowState.Maximized;

        if (MaximizeIcon != null)
        {
            var resourceKey = isMax ? "Icon.Window.Restore" : "Icon.Window.Maximize";
            if (this.TryFindResource(resourceKey, out var res) && res is Geometry geom)
            {
                MaximizeIcon.Data = geom;
            }
        }

        if (MaximizeButton != null)
        {
            ToolTip.SetTip(MaximizeButton, LocalizationSource.Instance.GetString(isMax ? "shell.header.restore_tooltip" : "shell.header.maximize_tooltip"));
        }
    }

    private void OnIsCompactChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isCompact)
        {
            UpdateWindowHeightHint(isCompact);
        }
    }

    private void UpdateWindowHeightHint(bool isCompact)
    {
        if (GetParentWindow() is { } window)
        {
            window.ExtendClientAreaTitleBarHeightHint = isCompact ? 36.0 : 52.0;
        }
    }

    private Window? GetParentWindow()
    {
        return _parentWindow ?? TopLevel.GetTopLevel(this) as Window ?? VisualRoot as Window;
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            GetParentWindow()?.BeginMoveDrag(e);
        }
    }

    private void OnMiddleAreaDoubleTapped(object? sender, TappedEventArgs e)
    {
        ToggleWindowState();
    }

    private void OnMinimizeClicked(object? sender, RoutedEventArgs e)
    {
        if (GetParentWindow() is { } window)
        {
            window.WindowState = WindowState.Minimized;
        }
    }

    private void OnMaximizeRestoreClicked(object? sender, RoutedEventArgs e)
    {
        ToggleWindowState();
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        GetParentWindow()?.Close();
    }

    private void ToggleWindowState()
    {
        if (GetParentWindow() is { } window)
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
            UpdateMaximizeButtonContent(window.WindowState);
        }
    }
}
