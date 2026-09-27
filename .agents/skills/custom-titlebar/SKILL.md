---
name: custom-titlebar
description: "Comprehensive guide, architectural constraints, pitfalls, and production-ready templates for creating and integrating an isolated 4-column custom application title bar with compact mode support in Avalonia UI."
---

# Custom App Title Bar Architecture & Integration Skill

This skill provides an all-in-one guide, architectural constraints, known pitfalls, and production-ready templates for implementing a modular, enterprise-grade **Custom Application Title Bar (`AppTitleBar`)** with dynamic compact mode support in Avalonia UI (.NET 8 / 9 / 10).

---

## 1. When to Use This Skill

Activate this skill when:
- Creating a modern desktop shell or window in Avalonia UI.
- Removing native OS title bars and replacing them with a custom branded title bar.
- Implementing a 4-column layout (Brand/Identity, Center Search/Breadcrumbs, Primary Actions, and Window State Controls).
- Supporting dynamic **Compact Mode** (switching between 52px standard height and 36px dense height).
- Designing theme-aware (Light / Dark) desktop applications with zero hardcoded magic strings.

---

## 2. Core Architecture & Constraints

1. **Window-Level Configuration (`WindowDecorations`):**
   - The root window must set `WindowDecorations="BorderOnly"`.
   - `ExtendClientAreaToDecorationsHint="True"` and `ExtendClientAreaTitleBarHeightHint="52"` are mandatory to enable native DWM snap layouts, shadows, and smooth edge rendering.
   - *(Note: `ExtendClientAreaChromeHints` was removed in Avalonia 12 and must not be used).*

2. **4-Column Grid Layout (`Auto, *, Auto, Auto`):**
   - **Column 0 (`Auto` - Identity):** Brand logo (`IconContent` slot), Application Title, Version Badge, and Subtitle.
   - **Column 1 (`*` - Header Slot & Drag Area):** Flexible center area for `HeaderContent` (search box, omnibar, tabs) with `Background="Transparent"` for window drag and double-click maximize/restore.
   - **Column 2 (`Auto` - Actions):** Two-row stacked actions:
     - **Row 1 (`PrimaryActions`):** Quick command buttons (e.g., Compact toggle, layout switchers). Always visible.
     - **Row 2 (`SecondaryActions`):** Ancillary actions (Theme, Settings, Help). Collapses automatically in Compact Mode.
   - **Column 3 (`Auto` - Window Controls):** Native window controls separated by a subtle vertical divider: Minimize, Maximize/Restore, and Close buttons.

3. **Compact Mode Mechanics (52px $\leftrightarrow$ 36px):**
   - Controlled via the boolean styled property `IsCompact`.
   - **Standard Mode (`IsCompact="False"`):** Height is `52px`. Subtitle and Row 2 (`SecondaryActions`) are visible.
   - **Compact Mode (`IsCompact="True"`):** Height is `36px`. Subtitle and Row 2 collapse; title and logo vertically center mathematically (`VerticalAlignment="Center"`).
   - Code-behind must dynamically update the parent window's `ExtendClientAreaTitleBarHeightHint` to `36.0` or `52.0`.

4. **Vector Asset Encapsulation:**
   - Window control icons (`Icon.Window.Minimize`, `Icon.Window.Maximize`, `Icon.Window.Restore`, `Icon.Window.Close`) must be declared as `StreamGeometry` resources directly inside `AppTitleBar.axaml` `<UserControl.Resources>`. This keeps the title bar 100% self-contained and portable.
   - Action icons (e.g., `Icon.Layout.*`) are passed through slots or referenced from application-level dictionaries.

5. **Theme Adaptation & Zero Hardcoding:**
   - Colors must bind to `{DynamicResource ...}` tokens (`AppTitleBarBackgroundBrush`, `AppTitleBarForegroundBrush`, etc.).
   - Text, headers, and tooltips should use localization markup extensions (`{loc:Loc ...}`) or MVVM bindings.

---

## 3. Critical Technical Pitfalls (Must-Know Gotchas)

1. **`.csproj` Embedded Resource Declarations:**
   When loading icons via `avares://` or JSON localization files from embedded streams, `.csproj` must explicitly include them. Missing entries will cause runtime `FileNotFoundException`:
   ```xml
   <ItemGroup>
     <AvaloniaResource Include="Assets\**" />
     <EmbeddedResource Include="Localization\locales\*.json" />
   </ItemGroup>
   ```

2. **Avalonia 12 NuGet Binary Incompatibility:**
   Do not reference packages compiled strictly for Avalonia 11 (e.g., `Avalonia.Svg.Skia 11.x`) on Avalonia 12 (.NET 10). They can throw silent runtime `TypeLoadException` (e.g., regarding `IBinding`) and crash the application before the window opens. Use pure XAML `StreamGeometry` / `PathIcon` for glyphs.

3. **Dynamic DWM Height Synchronization:**
   Changing only the visual height of `Border` in XAML does not resize the OS caption hit-test area. The code-behind must update `Window.ExtendClientAreaTitleBarHeightHint` whenever `IsCompact` changes.

4. **Agent Headless Execution on Windows:**
   Background sub-processes or headless AI test runners cannot render GUI windows to the active Windows desktop (`winsta0`). Agents should not treat a lack of desktop window popup as a crash during headless background commands; interactive verification must be tested via regular console commands (`dotnet run`).

---

## 4. Generated File Structure

When integrating into a project, maintain the following directory layout:

```text
src/
└── [YourApp]/
    ├── Controls/
    │   └── TitleBar/
    │       ├── AppTitleBar.axaml        # 4-Column isolated XAML view & vector resources
    │       └── AppTitleBar.axaml.cs     # Slots, window drag, maximize, and compact sync
    └── Views/
        ├── MainWindow.axaml             # WindowDecorations="BorderOnly" & title bar injection
        └── MainWindow.axaml.cs
```

---

## 5. Production-Ready Templates

### Template 1: `Controls/TitleBar/AppTitleBar.axaml`

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="[AppNamespace].Controls.TitleBar.AppTitleBar"
             x:Name="RootTitleBar">

  <UserControl.Resources>
    <!-- Window Control Vector Geometries (10x10 px Pixel-Perfect Standard) -->
    <StreamGeometry x:Key="Icon.Window.Minimize">M0 5h10v1H0z</StreamGeometry>
    <StreamGeometry x:Key="Icon.Window.Maximize">M0 0h10v10H0V0zm1 1v8h8V1H1z</StreamGeometry>
    <StreamGeometry x:Key="Icon.Window.Restore">M2 0h8v8h-2v2H0V2h2V0zm1 1v1h5v5h1V1H3zm-2 2v6h6V3H1z</StreamGeometry>
    <StreamGeometry x:Key="Icon.Window.Close">M1.05 0L0 1.05 3.95 5 0 8.95 1.05 10 5 6.05 8.95 10 10 8.95 6.05 5 10 1.05 8.95 0 5 3.95 1.05 0z</StreamGeometry>
  </UserControl.Resources>

  <UserControl.Styles>
    <!-- Standard Mode: 52px, Compact Mode: 36px -->
    <Style Selector="Border.titlebar-container">
      <Setter Property="Height" Value="52" />
    </Style>
    <Style Selector="Border.titlebar-container.compact">
      <Setter Property="Height" Value="36" />
    </Style>

    <!-- General Header Button Styles -->
    <Style Selector="Button.header-btn">
      <Setter Property="Background" Value="Transparent" />
      <Setter Property="BorderThickness" Value="0" />
      <Setter Property="Foreground" Value="{DynamicResource AppTitleBarForegroundBrush}" />
      <Setter Property="FontSize" Value="11" />
      <Setter Property="Padding" Value="7,3" />
      <Setter Property="CornerRadius" Value="4" />
      <Setter Property="Cursor" Value="Hand" />
    </Style>
    <Style Selector="Button.header-btn:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource AppTitleBarButtonHoverBrush}" />
      <Setter Property="Foreground" Value="{DynamicResource AppTitleBarForegroundBrush}" />
    </Style>

    <!-- Action / Tool Button Styles (24x22 Button, 16x16 Glyph) -->
    <Style Selector="Button.header-action-btn">
      <Setter Property="Background" Value="Transparent" />
      <Setter Property="BorderThickness" Value="0" />
      <Setter Property="Foreground" Value="{DynamicResource AppTitleBarForegroundBrush}" />
      <Setter Property="Width" Value="24" />
      <Setter Property="Height" Value="22" />
      <Setter Property="Padding" Value="0" />
      <Setter Property="HorizontalContentAlignment" Value="Center" />
      <Setter Property="VerticalContentAlignment" Value="Center" />
      <Setter Property="CornerRadius" Value="4" />
      <Setter Property="Cursor" Value="Hand" />
    </Style>
    <Style Selector="Button.header-action-btn:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource AppTitleBarButtonHoverBrush}" />
      <Setter Property="Foreground" Value="{DynamicResource AppTitleBarForegroundBrush}" />
    </Style>

    <!-- Window Management Control Button Styles (38x30 Button, 10x10 Vector Glyph) -->
    <Style Selector="Button.win-control">
      <Setter Property="Background" Value="Transparent" />
      <Setter Property="BorderThickness" Value="0" />
      <Setter Property="Foreground" Value="{DynamicResource AppTitleBarForegroundBrush}" />
      <Setter Property="Width" Value="38" />
      <Setter Property="Height" Value="30" />
      <Setter Property="Padding" Value="0" />
      <Setter Property="HorizontalContentAlignment" Value="Center" />
      <Setter Property="VerticalContentAlignment" Value="Center" />
      <Setter Property="CornerRadius" Value="4" />
      <Setter Property="Cursor" Value="Hand" />
    </Style>
    <Style Selector="Button.win-control:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource AppTitleBarButtonHoverBrush}" />
      <Setter Property="Foreground" Value="{DynamicResource AppTitleBarForegroundBrush}" />
    </Style>
    <Style Selector="Button.win-control-close:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource AppTitleBarCloseHoverBrush}" />
      <Setter Property="Foreground" Value="#FFFFFF" />
    </Style>
  </UserControl.Styles>

  <Border Classes="titlebar-container"
          Classes.compact="{Binding IsCompact, ElementName=RootTitleBar}"
          Background="{DynamicResource AppTitleBarBackgroundBrush}"
          BorderBrush="{DynamicResource AppTitleBarBorderBrush}"
          BorderThickness="0,0,0,1"
          PointerPressed="OnTitleBarPointerPressed">

    <Grid ColumnDefinitions="Auto,*,Auto,Auto">

      <!-- ================= COLUMN 0: Brand & Identity ================= -->
      <Grid Grid.Column="0" ColumnDefinitions="Auto,Auto" VerticalAlignment="Center" Margin="12,0,16,0">
        <!-- Brand Icon Slot -->
        <ContentPresenter Grid.Column="0"
                          Content="{Binding IconContent, ElementName=RootTitleBar}"
                          VerticalAlignment="Center"
                          Margin="0,0,10,0"
                          IsVisible="{Binding IconContent, ElementName=RootTitleBar, Converter={x:Static ObjectConverters.IsNotNull}}" />

        <!-- Title Block: Mathematically centered with logo when subtitle is hidden -->
        <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="0">
          <StackPanel Orientation="Horizontal" Spacing="8" VerticalAlignment="Center">
            <TextBlock Text="{Binding Title, ElementName=RootTitleBar}"
                       FontWeight="Bold" FontSize="13"
                       Foreground="{DynamicResource AppTitleBarForegroundBrush}"
                       VerticalAlignment="Center" />
            <Border Background="Transparent"
                    BorderBrush="{DynamicResource AppTitleBarBorderBrush}"
                    BorderThickness="1" CornerRadius="3" Padding="4,1"
                    VerticalAlignment="Center"
                    IsVisible="{Binding VersionText, ElementName=RootTitleBar, Converter={x:Static StringConverters.IsNotNullOrEmpty}}">
              <TextBlock Text="{Binding VersionText, ElementName=RootTitleBar}"
                         FontSize="9" FontWeight="SemiBold"
                         Foreground="{DynamicResource AppTitleBarForegroundBrush}"
                         VerticalAlignment="Center" />
            </Border>
          </StackPanel>

          <!-- Subtitle Row: Hidden in compact mode -->
          <TextBlock Text="{Binding Subtitle, ElementName=RootTitleBar}"
                     FontSize="12" FontWeight="SemiBold"
                     Foreground="{DynamicResource AppTitleBarForegroundBrush}"
                     Opacity="0.75"
                     Margin="0,2,0,0"
                     VerticalAlignment="Center"
                     IsVisible="{Binding !IsCompact, ElementName=RootTitleBar}" />
        </StackPanel>
      </Grid>

      <!-- ================= COLUMN 1: Header Slot & Drag / Double-Click ================= -->
      <Panel Grid.Column="1" Background="Transparent" DoubleTapped="OnMiddleAreaDoubleTapped">
        <ContentPresenter Content="{Binding HeaderContent, ElementName=RootTitleBar}"
                          HorizontalAlignment="Center"
                          VerticalAlignment="Center" />
      </Panel>

      <!-- ================= COLUMN 2: Actions (2 Rows) ================= -->
      <StackPanel Grid.Column="2" VerticalAlignment="Center" Spacing="2" Margin="8,0">
        <!-- Row 1: Primary Actions (Always Visible) -->
        <ContentPresenter Content="{Binding PrimaryActions, ElementName=RootTitleBar}"
                          HorizontalAlignment="Right"
                          VerticalAlignment="Center" />

        <!-- Row 2: Secondary Actions (Hidden in Compact Mode) -->
        <ContentPresenter Content="{Binding SecondaryActions, ElementName=RootTitleBar}"
                          HorizontalAlignment="Right"
                          VerticalAlignment="Center"
                          IsVisible="{Binding !IsCompact, ElementName=RootTitleBar}" />
      </StackPanel>

      <!-- ================= COLUMN 3: Native Window Controls ================= -->
      <StackPanel Grid.Column="3" Orientation="Horizontal" Spacing="0" VerticalAlignment="Center" Margin="0,0,6,0">
        <Rectangle Width="1" Height="18" Fill="{DynamicResource AppTitleBarBorderBrush}" Margin="4,0,8,0" />
        <Button x:Name="MinimizeButton" Classes="win-control" Click="OnMinimizeClicked" ToolTip.Tip="Minimize">
          <PathIcon Data="{StaticResource Icon.Window.Minimize}" Width="10" Height="10" />
        </Button>
        <Button x:Name="MaximizeButton" Classes="win-control" Click="OnMaximizeRestoreClicked" ToolTip.Tip="Maximize">
          <PathIcon x:Name="MaximizeIcon" Data="{StaticResource Icon.Window.Maximize}" Width="10" Height="10" />
        </Button>
        <Button x:Name="CloseButton" Classes="win-control win-control-close" Click="OnCloseClicked" ToolTip.Tip="Close">
          <PathIcon Data="{StaticResource Icon.Window.Close}" Width="10" Height="10" />
        </Button>
      </StackPanel>

    </Grid>
  </Border>
</UserControl>
```

---

### Template 2: `Controls/TitleBar/AppTitleBar.axaml.cs`

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace [AppNamespace].Controls.TitleBar;

/// <summary>
/// Fully generic, reusable 4-column Custom Application Title Bar supporting content slots and compact mode.
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

        var maxIcon = this.FindControl<PathIcon>("MaximizeIcon");
        if (maxIcon != null)
        {
            var resourceKey = isMax ? "Icon.Window.Restore" : "Icon.Window.Maximize";
            if (Application.Current?.TryFindResource(resourceKey, out var res) == true && res is Geometry geom)
            {
                maxIcon.Data = geom;
            }
        }

        var maxBtn = this.FindControl<Button>("MaximizeButton");
        if (maxBtn != null)
        {
            ToolTip.SetTip(maxBtn, isMax ? "Restore" : "Maximize");
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
```

---

### Template 3: `MainWindow.axaml` Integration

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:titlebar="using:[AppNamespace].Controls.TitleBar"
        x:Class="[AppNamespace].Views.MainWindow"
        Title="Modern Desktop Application"
        Width="1280" Height="800"
        WindowStartupLocation="CenterScreen"
        WindowDecorations="BorderOnly"
        ExtendClientAreaToDecorationsHint="True"
        ExtendClientAreaTitleBarHeightHint="52">

  <Grid RowDefinitions="Auto,*">

    <!-- 1. Custom App Title Bar (Row 0) -->
    <titlebar:AppTitleBar Grid.Row="0"
                          Title="Your App"
                          VersionText="v1.0.0"
                          Subtitle="Modern Desktop Shell"
                          IsCompact="{Binding IsCompactTitleBar}">

      <!-- Column 0: Brand Logo Slot -->
      <titlebar:AppTitleBar.IconContent>
        <Image Source="/Assets/logo.png" Width="20" Height="20" />
      </titlebar:AppTitleBar.IconContent>

      <!-- Column 1: Center Header Slot (Search / Omnibox) -->
      <titlebar:AppTitleBar.HeaderContent>
        <TextBox Width="320" Watermark="Quick Search (Ctrl+P)..." />
      </titlebar:AppTitleBar.HeaderContent>

      <!-- Column 2, Row 1: Primary Actions (Always Visible) -->
      <titlebar:AppTitleBar.PrimaryActions>
        <StackPanel Orientation="Horizontal" Spacing="4">
          <!-- Recommended 1st Action: Compact Mode Toggle -->
          <Button Classes="header-action-btn"
                  Command="{Binding ToggleCompactCommand}"
                  ToolTip.Tip="Toggle Compact Mode">
            <TextBlock Text="⇕" FontSize="14" />
          </Button>
          <Button Classes="header-action-btn"
                  Command="{Binding ToggleSidebarCommand}"
                  ToolTip.Tip="Toggle Sidebar">
            <TextBlock Text="☰" FontSize="14" />
          </Button>
        </StackPanel>
      </titlebar:AppTitleBar.PrimaryActions>

      <!-- Column 2, Row 2: Secondary Actions (Hidden in Compact Mode) -->
      <titlebar:AppTitleBar.SecondaryActions>
        <StackPanel Orientation="Horizontal" Spacing="4">
          <Button Classes="header-btn" Content="Theme" Command="{Binding ToggleThemeCommand}" />
          <Button Classes="header-btn" Content="Settings" Command="{Binding OpenSettingsCommand}" />
        </StackPanel>
      </titlebar:AppTitleBar.SecondaryActions>

    </titlebar:AppTitleBar>

    <!-- 2. Workspace / Document Area (Row 1) -->
    <Border Grid.Row="1" Background="{DynamicResource AppSurfaceBackgroundBrush}">
      <!-- Workspace content here -->
    </Border>

  </Grid>
</Window>
```

---

## 6. Verification Checklist

Always verify the following when implementing or reviewing the title bar:
- [ ] Root Window has `WindowDecorations="BorderOnly"` and `ExtendClientAreaToDecorationsHint="True"`.
- [ ] Root Window has `ExtendClientAreaTitleBarHeightHint="52"` initially.
- [ ] Window controls use `10x10 px` vector path icons (`PathIcon`).
- [ ] Switching `IsCompact="True"` collapses Subtitle and Row 2, adjusts height to `36px`, and updates `ExtendClientAreaTitleBarHeightHint="36.0"`.
- [ ] Double-clicking empty middle area toggles Maximize / Restore.
- [ ] Dragging empty space moves the window seamlessly.
- [ ] Brushes are resolved via `{DynamicResource ...}` tokens.
- [ ] `.csproj` includes `<AvaloniaResource Include="Assets\**" />`.
