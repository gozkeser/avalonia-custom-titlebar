---
name: custom-titlebar
description: "Avalonia UI projelerinde 4 sütunlu, kompakt mod destekli, izole Custom App Title Bar bileşeni ve MainWindow entegrasyonu oluşturma rehberi ve şablonları."
---

# Custom App Title Bar İskeleti ve Entegrasyon Rehberi (Skill)

Bu skill; Avalonia UI tabanlı masaüstü projelerinde kurumsal, esnek, 4 sütunlu ve kompakt mod destekli bir **Özel Başlık Çubuğu (Custom App Title Bar)** bileşeni üretmek ve pencereye entegre etmek için kullanılır.

İlgili Kural Seti: `.agents/rules/custom-titlebar-rules.md`

---

## 1. Ne Zaman Kullanılır?
- Yeni bir Avalonia Shell veya masaüstü penceresi oluşturulduğunda.
- İşletim sisteminin standart başlık çubuğu kaldırılıp modern özel başlık çubuğu ekleneceğinde.
- Başlık çubuğuna VS Code benzeri layout toggle ikonları veya arama slotu (`HeaderContent`) ekleneceğinde.
- Standart (52px) ve Kompakt (36px) mod desteği gerektiğinde.

---

## 2. Üretilen Bileşenler ve Dosya Yapısı

```text
src/
└── [ProjeAdı].Shell/
    ├── Controls/
    │   └── TitleBar/
    │       ├── AppTitleBar.axaml        # 4-Column isolated XAML view
    │       └── AppTitleBar.axaml.cs     # Drag, maximize, and dependency properties
    └── Presentation/
        └── Views/
            ├── MainWindow.axaml         # WindowDecorations="BorderOnly" and titlebar injection
            └── MainWindow.axaml.cs
```

---

## 3. Vektörel Varlıkların (İkonların) Saklanma Standartları

1. **Pencere Kontrol İkonları (`Icon.Window.*`):**
   - `Controls/TitleBar/AppTitleBar.axaml` içindeki `<UserControl.Resources>` alanında yer almalıdır.
   - Bu sayede bileşen kendi kendine yeterli (self-contained) ve taşınabilir kalır; `App.axaml` kirlenmez.
2. **Yerleşim ve Mod İkonları (`Icon.Layout.*`, `Icon.MenuBar` vb.):**
   - `Themes/Icons/ShellIcons.axaml` veya `Assets/Icons/ShellIcons.axaml` içinde merkezi bir ResourceDictionary olarak tutulmalıdır.
   - Bu sözlük `App.axaml` içine `<ResourceInclude Source="avares://.../ShellIcons.axaml" />` ile dahil edilmelidir.

---

## 4. Hazır Şablonlar

### Şablon 1: `Controls/TitleBar/AppTitleBar.axaml`

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:loc="using:[AppNamespace].MarkupExtensions"
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
      <Setter Property="Foreground" Value="{DynamicResource Foreground.Secondary}" />
      <Setter Property="FontSize" Value="11" />
      <Setter Property="Padding" Value="7,3" />
      <Setter Property="CornerRadius" Value="4" />
      <Setter Property="Cursor" Value="Hand" />
    </Style>
    <Style Selector="Button.header-btn:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource Header.Button.Hover}" />
      <Setter Property="Foreground" Value="{DynamicResource Header.Foreground}" />
    </Style>

    <!-- Layout Toggle Icon Button Styles (VS Code Style: 24x22 Button, 16x16 Glyph) -->
    <Style Selector="Button.header-layout-icon-btn">
      <Setter Property="Background" Value="Transparent" />
      <Setter Property="BorderThickness" Value="0" />
      <Setter Property="Foreground" Value="{DynamicResource Foreground.Secondary}" />
      <Setter Property="Width" Value="24" />
      <Setter Property="Height" Value="22" />
      <Setter Property="Padding" Value="0" />
      <Setter Property="HorizontalContentAlignment" Value="Center" />
      <Setter Property="VerticalContentAlignment" Value="Center" />
      <Setter Property="CornerRadius" Value="4" />
      <Setter Property="Cursor" Value="Hand" />
    </Style>
    <Style Selector="Button.header-layout-icon-btn:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource Header.Button.Hover}" />
      <Setter Property="Foreground" Value="{DynamicResource Header.Foreground}" />
    </Style>

    <!-- Window Management Control Button Styles (38x30 Button, 10x10 Vector Glyph) -->
    <Style Selector="Button.win-control">
      <Setter Property="Background" Value="Transparent" />
      <Setter Property="BorderThickness" Value="0" />
      <Setter Property="Foreground" Value="{DynamicResource Foreground.Secondary}" />
      <Setter Property="Width" Value="38" />
      <Setter Property="Height" Value="30" />
      <Setter Property="Padding" Value="0" />
      <Setter Property="HorizontalContentAlignment" Value="Center" />
      <Setter Property="VerticalContentAlignment" Value="Center" />
      <Setter Property="CornerRadius" Value="4" />
      <Setter Property="Cursor" Value="Hand" />
    </Style>
    <Style Selector="Button.win-control:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource Header.Button.Hover}" />
      <Setter Property="Foreground" Value="{DynamicResource Header.Foreground}" />
    </Style>
    <Style Selector="Button.win-control-close:pointerover /template/ ContentPresenter">
      <Setter Property="Background" Value="{DynamicResource WindowControl.Close.Hover}" />
      <Setter Property="Foreground" Value="{DynamicResource Header.Foreground}" />
    </Style>
  </UserControl.Styles>

  <Border Classes="titlebar-container"
          Classes.compact="{Binding IsCompact, ElementName=RootTitleBar}"
          Background="{DynamicResource Header.Background}"
          BorderBrush="{DynamicResource Header.Border}"
          BorderThickness="0,0,0,1"
          PointerPressed="OnTitleBarPointerPressed">

    <Grid ColumnDefinitions="Auto,*,Auto,Auto">

      <!-- ================= COLUMN 0: Brand & Identity (Injected Generic Slots) ================= -->
      <Grid Grid.Column="0" ColumnDefinitions="Auto,Auto" VerticalAlignment="Center" Margin="12,0,16,0">
        <!-- Generic Injected Icon Slot -->
        <ContentPresenter Grid.Column="0"
                          Content="{Binding IconContent, ElementName=RootTitleBar}"
                          VerticalAlignment="Center"
                          Margin="0,0,10,0"
                          IsVisible="{Binding IconContent, ElementName=RootTitleBar, Converter={x:Static ObjectConverters.IsNotNull}}" />
        
        <!-- Title Block: Vertically centered with the icon when subtitle is hidden -->
        <StackPanel Grid.Column="1" VerticalAlignment="Center" Spacing="0">
          <!-- Title and Version Badge Row -->
          <StackPanel Orientation="Horizontal" Spacing="8" VerticalAlignment="Center">
            <TextBlock Text="{Binding Title, ElementName=RootTitleBar}"
                       FontWeight="Bold" FontSize="13"
                       Foreground="{DynamicResource Header.Foreground}"
                       VerticalAlignment="Center" />
            <Border Background="Transparent"
                    BorderBrush="{DynamicResource Badge.Border}"
                    BorderThickness="1" CornerRadius="3" Padding="4,1"
                    VerticalAlignment="Center"
                    IsVisible="{Binding VersionText, ElementName=RootTitleBar, Converter={x:Static StringConverters.IsNotNullOrEmpty}}">
              <TextBlock Text="{Binding VersionText, ElementName=RootTitleBar}"
                         FontSize="9" FontWeight="SemiBold"
                         Foreground="{DynamicResource Badge.Foreground}"
                         VerticalAlignment="Center" />
            </Border>
          </StackPanel>

          <!-- Subtitle Row: Hidden in compact mode -->
          <TextBlock Text="{Binding Subtitle, ElementName=RootTitleBar}"
                     FontSize="12" FontWeight="SemiBold"
                     Foreground="{DynamicResource Header.Subtitle}"
                     Margin="0,2,0,0"
                     VerticalAlignment="Center"
                     IsVisible="{Binding !IsCompact, ElementName=RootTitleBar}" />
        </StackPanel>
      </Grid>

      <!-- ================= COLUMN 1: Dynamic Content Slot & Window Drag/Maximize ================= -->
      <Panel Grid.Column="1" Background="Transparent" DoubleTapped="OnMiddleAreaDoubleTapped">
        <ContentPresenter Content="{Binding HeaderContent, ElementName=RootTitleBar}"
                          HorizontalAlignment="Center"
                          VerticalAlignment="Center" />
      </Panel>

      <!-- ================= COLUMN 2: Shell Layout & General Actions (2 Injected Rows) ================= -->
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

      <!-- ================= COLUMN 3: Window Management Controls ================= -->
      <StackPanel Grid.Column="3" Orientation="Horizontal" Spacing="0" VerticalAlignment="Center" Margin="0,0,6,0">
        <Rectangle Width="1" Height="18" Fill="{DynamicResource Border.Default}" Margin="4,0,8,0" />
        <Button x:Name="MinimizeButton" Classes="win-control" Click="OnMinimizeClicked" ToolTip.Tip="{loc:Loc shell.header.minimize_tooltip}">
          <PathIcon Data="{StaticResource Icon.Window.Minimize}" Width="10" Height="10" />
        </Button>
        <Button x:Name="MaximizeButton" Classes="win-control" Click="OnMaximizeRestoreClicked" ToolTip.Tip="{loc:Loc shell.header.maximize_tooltip}">
          <PathIcon x:Name="MaximizeIcon" Data="{StaticResource Icon.Window.Maximize}" Width="10" Height="10" />
        </Button>
        <Button x:Name="CloseButton" Classes="win-control win-control-close" Click="OnCloseClicked" ToolTip.Tip="{loc:Loc shell.header.close_tooltip}">
          <PathIcon Data="{StaticResource Icon.Window.Close}" Width="10" Height="10" />
        </Button>
      </StackPanel>

    </Grid>
  </Border>
</UserControl>
```

---

### Şablon 2: `Controls/TitleBar/AppTitleBar.axaml.cs`

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using [AppNamespace].Localization;

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
            ToolTip.SetTip(maxBtn, LocalizationSource.Instance.GetString(isMax ? "shell.header.restore_tooltip" : "shell.header.maximize_tooltip"));
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

### Şablon 3: `MainWindow.axaml` Entegrasyonu

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:titlebar="using:[AppNamespace].Controls.TitleBar"
        xmlns:loc="using:[AppNamespace].MarkupExtensions"
        xmlns:ext="using:[AppNamespace].MarkupExtensions"
        x:Class="[AppNamespace].Views.MainWindow"
        Title="{loc:Loc App.Title}"
        Width="1280" Height="800"
        WindowStartupLocation="CenterScreen"
        WindowDecorations="BorderOnly"
        ExtendClientAreaToDecorationsHint="True"
        ExtendClientAreaTitleBarHeightHint="52">

  <Grid RowDefinitions="Auto,*">

    <!-- 1. Custom App Title Bar (Standard 52px, Compact 36px) -->
    <titlebar:AppTitleBar Grid.Row="0"
                          Title="{loc:Loc App.Title}"
                          VersionText="{loc:Loc App.Version}"
                          Subtitle="{loc:Loc Keys='Company.Division,Company.Name'}"
                          IsCompact="{Binding IsCompactTitleBar}">

      <!-- Brand Logo Slot -->
      <titlebar:AppTitleBar.IconContent>
        <Image Source="{ext:Svg /Assets/logo.svg, Size=128}" Width="24" Height="24" />
      </titlebar:AppTitleBar.IconContent>

      <!-- Optional Center Slot: Search Box, Breadcrumb or Tabs -->
      <titlebar:AppTitleBar.HeaderContent>
        <Border Background="{DynamicResource Search.Background}"
                CornerRadius="5"
                BorderBrush="{DynamicResource Search.Border}"
                BorderThickness="1"
                Padding="8,3"
                Width="320">
          <TextBlock Text="{loc:Loc shell.header.search_placeholder}"
                     FontSize="11"
                     Foreground="{DynamicResource Foreground.Secondary}"
                     VerticalAlignment="Center" />
        </Border>
      </titlebar:AppTitleBar.HeaderContent>

      <!-- Column 2, Row 1: Primary Actions (Always Visible) -->
      <titlebar:AppTitleBar.PrimaryActions>
        <StackPanel Orientation="Horizontal" Spacing="2">
          <!-- Recommended 1st Action: Compact / Expand Mode Toggle Button -->
          <Button Classes="header-layout-icon-btn"
                  Command="{Binding ToggleCompactCommand}"
                  ToolTip.Tip="{loc:Loc shell.layout.toggle_compact_tooltip}">
            <Panel>
              <PathIcon Data="{StaticResource Icon.Layout.Compact}" Width="16" Height="16" IsVisible="{Binding !IsCompactTitleBar}" />
              <PathIcon Data="{StaticResource Icon.Layout.Expand}" Width="16" Height="16" IsVisible="{Binding IsCompactTitleBar}" />
            </Panel>
          </Button>
          <Button Classes="header-layout-icon-btn"
                  Command="{Binding ToggleMenuBarCommand}"
                  ToolTip.Tip="{loc:Loc shell.layout.toggle_menubar_tooltip}">
            <PathIcon Data="{StaticResource Icon.MenuBar}" Width="16" Height="16" />
          </Button>
        </StackPanel>
      </titlebar:AppTitleBar.PrimaryActions>

      <!-- Column 2, Row 2: Secondary Actions (Hidden in Compact Mode) -->
      <titlebar:AppTitleBar.SecondaryActions>
        <StackPanel Orientation="Horizontal" Spacing="4">
          <Button Classes="header-btn" Content="{loc:Loc shell.header.theme}" ToolTip.Tip="{loc:Loc shell.header.theme_tooltip}" />
          <Button Classes="header-btn" Content="{loc:Loc shell.header.settings}" ToolTip.Tip="{loc:Loc shell.header.settings_tooltip}" />
        </StackPanel>
      </titlebar:AppTitleBar.SecondaryActions>
    </titlebar:AppTitleBar>

    <!-- 2. Workspace / Document Area -->
    <Grid Grid.Row="1">
      <!-- Workspace content here -->
    </Grid>
  </Grid>
</Window>
```

---

## 5. İcra ve Doğrulama Kontrol Listesi (Checklist)

Başlık çubuğu eklendiğinde veya güncellendiğinde şu adımları kontrol edin:
- [ ] `MainWindow.axaml` üzerinde `WindowDecorations="BorderOnly"` tanımlı mı?
- [ ] Grid 4 sütunlu mu (`ColumnDefinitions="Auto,*,Auto,Auto"`)?
- [ ] Sütun 2'nin 1. satırında (PrimaryActions) ilk buton olarak Kompakt/Genişlet (`IsCompact`) toggle butonu var mı?
- [ ] Primary action butonları `24x22 px`, glif boyutları `16x16 px` mi?
- [ ] Window control butonları 10x10 px vektörel `PathIcon` mu?
- [ ] `IsCompact` özelliği `true` yapıldığında Subtitle ve Sütun 2'nin 2. satırı gizleniyor ve yükseklik 36px'e iniyor mu?
- [ ] Orta alana çift tıklandığında pencere büyüyüp/küçülüyor mu?
- [ ] Boş alandan tutulduğunda pencere taşınabiliyor mu?
- [ ] Tüm butonlarda `{loc:Loc ...}` ve `ToolTip.Tip` tanımlı mı?
- [ ] Renkler `{DynamicResource ...}` fırçalarına bağlı mı?
- [ ] `.csproj` içinde `<AvaloniaResource Include="Assets\**" />` ve `<EmbeddedResource Include="Localization\locales\*.json" />` tanımlı mı?
- [ ] Markdown ve kod blokları içindeki tüm yorum satırları İngilizce mi?
