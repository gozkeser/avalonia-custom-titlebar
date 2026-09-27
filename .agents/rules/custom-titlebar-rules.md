---
title: "Avalonia UI Custom App Title Bar Tasarım Standartları ve İcra Kuralları"
description: "Avalonia UI için özel pencere başlık çubuğu (Custom Title Bar) 4 sütunlu grid yerleşimi, kompakt mod, WindowDecorations kuralları ve etkileşim standartları."
status: "approved"
date: "27.09.2026"
author:
  - "Gökhan Özkeser"
tags:
  - "kurallar"
  - "standartlar"
  - "titlebar"
  - "custom-titlebar"
  - "avalonia"
  - "xaml"
  - "window-decorations"
---

# Avalonia UI Custom App Title Bar Tasarım Standartları ve İcra Kuralları

Bu kural dosyası, .NET ve Avalonia UI tabanlı masaüstü uygulamalarında kullanılacak **Özel Uygulama Başlık Çubuğu (Custom App Title Bar)** bileşeninin mimari standartlarını, Grid yerleşimini, generic slot yapısını ve etkileşim kurallarını belirler.

Bu doküman **sadece** başlık çubuğu (Title Bar) katmanını kapsar. MenuBar, ToolBar, StatusBar veya SideBar gibi diğer Shell bileşenleri ayrı kural setlerinde tanımlanır.

---

## 1. Demir Kural 1: Pencere Seviyesi Yapılandırması (WindowDecorations)

Pencere kontrol butonları (`Minimize`, `Maximize/Restore`, `Close`) XAML içerisinde özel olarak çizildiği için işletim sisteminin native pencere başlığı ve butonları **kesinlikle kaldırılmalıdır**.

- Pencere tanımında `WindowDecorations="BorderOnly"` kullanımı **zorunludur**.
- Pencere içeriğinin en tepeye pürüzsüz uzanması ve Windows 11 DWM snap/shadow desteği için `ExtendClientAreaToDecorationsHint="True"` ve `ExtendClientAreaTitleBarHeightHint="52"` kullanımı **zorunludur**.
- *(Not: Avalonia 12 ile birlikte `ExtendClientAreaChromeHints` özelliği kaldırılmıştır; kullanılmamalıdır).*

### Standart Pencere Başlığı
- ✅ **ZORUNLU (DO):**
  ```xml
  <Window xmlns="https://github.com/avaloniaui"
          WindowStartupLocation="CenterScreen"
          WindowDecorations="BorderOnly"
          ExtendClientAreaToDecorationsHint="True"
          ExtendClientAreaTitleBarHeightHint="52">
  ```
- ❌ **YASAK (DON'T):**
  ```xml
  <!-- OS controls not hidden or deprecated API used -->
  <Window WindowDecorations="Full" ... />
  <Window ExtendClientAreaChromeHints="NoChrome" ... />
  ```

---

## 2. Demir Kural 2: 4 Sütunlu Grid Mimarisi (`Auto,*,Auto,Auto`)

Özel başlık çubuğu, bir `Border` kapsayıcısı içinde **kesinlikle 4 sütunlu** bir `Grid` olarak kurgulanmalıdır:

```text
┌──────────────┬─────────────────────────┬───────────────────────────────────────────┬──────────────┐
│ Sütun 0      │ Sütun 1                 │ Sütun 2                                   │ Sütun 3      │
│ (Auto)       │ (*)                     │ (Auto)                                    │ (Auto)       │
├──────────────┼─────────────────────────┼───────────────────────────────────────────┼──────────────┤
│ Brand & Info │ HeaderContent & Drag    │ Shell Actions & Layout Toggles (2 Satır)  │ Window State │
│              │                         │ ├─ Satır 1: [⇕] [☰] [⎕] [▤] (Görünüm İkon)│              │
│ [Logo] Başlık│ [ Slot / Sürükleme ]    │ └─ Satır 2: [Yardım] [Ayarlar] [Profil]   │ [—] [🗖] [✕] │
│      v1.0 Sub│                         │                                           │              │
└──────────────┴─────────────────────────┴───────────────────────────────────────────┴──────────────┘
```

### Sütun Dağılımı ve Görevleri:
1. **Sütun 0 (`Auto`) — Marka & Kimlik (Sol Bölge):**
   - Logo (`IconContent` slotu), Ana Başlık (`FontWeight="Bold"`), Versiyon Rozeti (`Border` içinde şeffaf arka plan + mat border) ve Alt Başlık (`Subtitle`, `FontSize="12"`).
2. **Sütun 1 (`*`) — Dinamik İçerik Slotu & Sürükleme / Büyütme Alanı:**
   - Genişleyen esnek alan. Arka planda `Background="Transparent"` olan hit-test alanı, üzerinde opsiyonel `HeaderContent` slotu (arama kutusu, sekme çubuğu, proje yolu vb.).
3. **Sütun 2 (`Auto`) — Aksiyonlar ve Kontroller (Sağ-İç Bölge):**
   - Dikeyde 2 satırlı esnek bir hiyerarşiye sahiptir:
     - **1. Satır (Üst / Birincil - `PrimaryActions`):** Hızlı erişim / Layout toggle ikon butonları (`24x22 px` buton, `16x16 px` ikon).
       > [!IMPORTANT]
       > **Kompakt Mod Geçiş Butonu Standartı:** `PrimaryActions` grubunun **ilk butonu** olarak Kompakt / Genişlet modunu (`IsCompact`) toggle eden butonun konulması **şiddetle tavsiye edilir**. Kullanıcı, çalışma alanındaki ayarlara gitmeden doğrudan başlık çubuğundan kompakt moda (`52px` $\leftrightarrow$ `36px`) geçebilmelidir.
     - **2. Satır (Alt / İkincil - `SecondaryActions`):** Genel veya modüle özgü aksiyon butonları (Yardım, Ayarlar, Tema vb.). Kompakt modda otomatik olarak gizlenir.
4. **Sütun 3 (`Auto`) — Pencere Yönetim Butonları (En Sağ Bölge):**
   - Sütun 2'den ince bir dikey ayırıcı (`Rectangle Width="1" Height="18"`) ile ayrılır.
   - Sırasıyla: Minimize (`Icon.Window.Minimize`), Maximize/Restore (`Icon.Window.Maximize` / `Icon.Window.Restore`), Kapat (`Icon.Window.Close`).
   - Font karakterleri yerine piksel kusursuzluğu, DPI ve baseline kaymasını önlemek için **10x10 px vektörel `PathIcon`** kullanımı esastır.
   - Kapatma butonunun hover rengi temadaki kırmızı/alarm fırçasına (`WindowControl.Close.Hover`) bağlanmalıdır.

---

## 3. Demir Kural 3: Kompakt Mod (Compact Mode) Standardı

Başlık çubuğu ekran tasarrufu için **Standart Mod** (iki satırlı) ve **Kompakt Mod** (tek satırlı) destekleyecek esneklikte tasarlanmalıdır.

- `IsCompact` (Boolean) styled property tanımlanmalıdır.
- **Standart Mod (Varsayılan):**
  - Yükseklik: **52px**
  - Sütun 0: Logo, Başlık, Versiyon Rozeti ve **Subtitle** görünür.
  - Sütun 2: **1. Satır** (`PrimaryActions`) ve **2. Satır** (`SecondaryActions`) görünür.
- **Kompakt Mod (`IsCompact="True"`):**
  - Yükseklik: **36px**
  - Sütun 0: **Subtitle gizlenir**. Başlık ve Rozet bloğu logonun dikey merkezine (`VerticalAlignment="Center"`) tam oturmalıdır.
  - Sütun 2: **2. Satır (`SecondaryActions`) gizlenir**; sadece 1. Satırdaki kompakt görünüm ikonları kalır.
- **Pencere Senkronizasyonu:** `IsCompact` değeri değiştiğinde, pencerenin `ExtendClientAreaTitleBarHeightHint` değeri de code-behind üzerinden eşzamanlı olarak `36.0` veya `52.0` olarak güncellenmelidir.

```text
[KOMPAKT MOD (36px)]
┌──────────────┬─────────────────────────┬───────────────────────────────────────────┬──────────────┐
│ [Logo] Başlık│ [ Slot / Sürükleme ]    │ [⇕] [☰] [⎕] [▤] (Layout İkonlar)          │ [—] [🗖] [✕] │
└──────────────┴─────────────────────────┴───────────────────────────────────────────┴──────────────┘
```

---

## 4. Demir Kural 4: Vektörel Varlıkların (SVG / StreamGeometry) Konumlandırma Standardı

Bileşen mimarisinin temiz kalması ve taşınabilirlik (reusability) için vektörel ikonlar doğru katmanlarda tutulmalıdır:

1. **Özel Başlık Çubuğu İkonları (`Icon.Window.*`):**
   - **Konum:** `Controls/TitleBar/AppTitleBar.axaml` içindeki `<UserControl.Resources>` alanı.
   - **Gerekçe:** Pencere kontrol butonları (`Minimize`, `Maximize`, `Close`) başlık çubuğunun dahili ve ayrılmaz bir parçasıdır. Bileşen başka projelere kopyalandığında `App.axaml`'a bağımlı olmadan bağımsız olarak çalışabilmelidir (Self-contained).
2. **Çalışma Alanı ve Shell İkonları (`Icon.Layout.*`, `Icon.MenuBar` vb.):**
   - **Konum:** `src/Themes/Icons/ShellIcons.axaml` veya `src/Assets/Icons/ShellIcons.axaml` gibi merkezi bir `ResourceDictionary` dosyası.
   - **Gerekçe:** Bu ikonlar dışarıdan enjekte edilen butonlarda kullanılır ve çalışma alanının diğer panellerinde de paylaşılabilir. `App.axaml` içinde bir satır `<ResourceInclude Source="avares://.../ShellIcons.axaml" />` ile dahil edilmelidir.

---

## 5. Demir Kural 5: Etkileşim ve Hit-Test Yönetimi (Drag & Maximize)

Pencere başlık çubuğundaki boş alanlar işletim sistemi penceresini sürükleme ve büyütme işlemlerini eksiksiz yerine getirmelidir:

1. **Pencereyi Taşıma (Drag):**
   - Başlık çubuğunun ana `Border` kapsayıcısında veya Sütun 1'deki boş panelde `PointerPressed` yakalanmalı ve `e.GetCurrentPoint(this).Properties.IsLeftButtonPressed` kontrolüyle pencerenin `BeginMoveDrag(e)` fonksiyonu çağrılmalıdır.
2. **Çift Tıklama ile Ekranı Kaplama (Maximize / Restore):**
   - Sütun 1'in boş zemininde `DoubleTapped` olayı dinlenmeli ve `window.WindowState` değeri `Maximized` ile `Normal` arasında toggle edilmelidir.
3. **Buton ve Kontrollerin İzolasyonu:**
   - Butonlar, arama kutuları ve slot kontrolleri pointer olayını tükettiğinden (`e.Handled = true`), bu kontrollere tıklandığında pencere sürüklenmemelidir.
   - Sürükleme alanının hit-test alabilmesi için arka planı mutlaka `Background="Transparent"` olarak belirtilmelidir.

---

## 6. Demir Kural 6: Sıfır Hardcoded Metin ve Renk İlkesi

Kullanıcı arayüzünde doğrudan hardcoded (sabit) metin ve renk kullanımı kesinlikle yasaktır:

- **Metinler:** Tüm `Text`, `Content` ve `ToolTip.Tip` alanlarında `{loc:Loc ...}` extension kullanılmalıdır.
- **Renkler:** Asla hex renk kodu (`#FFFFFF`, `#1E1E1E`) yazılmamalıdır; `{DynamicResource Header.Background}`, `{DynamicResource Header.Foreground}`, `{DynamicResource WindowControl.Close.Hover}` gibi anlamsal tema fırçaları kullanılmalıdır.
- **Tooltips:** Tüm ikon ve pencere kontrol butonlarında `ToolTip.Tip` tanımlanması **zorunludur**.

---

## 7. Bileşen İzolasyonu ve Generic Slot Mimarisi: `AppTitleBar` Standardı

Başlık çubuğu bileşeni doğrudan `MainWindow.axaml` içerisine gömülmemelidir. `Controls/TitleBar/AppTitleBar.axaml` olarak izole bir kontrol haline getirilmeli; marka logosu, başlık, alt başlık ve butonlar dışarıdan enjekte edilerek bileşenin **%100 Generic Shell** kalması sağlanmalıdır:

### Desteklenen Generic Slotlar ve Özellikler:
- `IconContent`: Marka logosu / SVG ikonu slotu (Column 0).
- `Title`: Uygulama başlığı (`{App.Title}`).
- `VersionText`: Versiyon metni / rozet (`{App.Version}`).
- `Subtitle`: Bölüm ve şirket adı formatında alt başlık (`{Company.Division}, {Company.Name}`).
- `HeaderContent`: Sütun 1 dinamik içerik slotu (arama kutusu, sekmeler vb.).
- `PrimaryActions`: Sütun 2, 1. satır aksiyon butonları (kompakt modda daima görünür).
- `SecondaryActions`: Sütun 2, 2. satır aksiyon butonları (`IsCompact="True"` olduğunda otomatik gizlenir).
- `IsCompact`: Standart mod (52px) ile kompakt mod (36px) geçiş anahtarı.

```xml
<!-- MainWindow.axaml Usage -->
<Grid RowDefinitions="Auto, *">
  <titlebar:AppTitleBar Grid.Row="0"
                        Title="{loc:Loc App.Title}"
                        VersionText="{loc:Loc App.Version}"
                        Subtitle="{loc:Loc Keys='Company.Division,Company.Name'}"
                        IsCompact="{Binding IsCompactTitleBar}">
    <!-- Brand Logo Slot -->
    <titlebar:AppTitleBar.IconContent>
      <Image Source="{ext:Svg /Assets/logo.svg, Size=128}" Width="24" Height="24" />
    </titlebar:AppTitleBar.IconContent>

    <!-- Column 1: Optional Search / Tabs Slot -->
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

    <!-- Column 2, Row 1: Primary Actions (Compact Toggle + Layout Icons) -->
    <titlebar:AppTitleBar.PrimaryActions>
      <StackPanel Orientation="Horizontal" Spacing="2">
        <!-- Recommended First Action: Compact / Expand Mode Toggle -->
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
</Grid>
```

---

## 8. Demir Kural 8: Sıfırdan Proje Kurulumu ve Kritik Tuzaklar (Pitfalls & Gotchas)

Sıfırdan bir Avalonia 12 projesi oluştururken ve Custom TitleBar entegre ederken şu 4 kritik teknik tuzağa dikkat edilmelidir:

1. **`.csproj` Varlık Tanımları (`AvaloniaResource` Zorunluluğu):**
   `Assets/` dizinindeki vektör veya görsellerin `avares://` şemasıyla bulunabilmesi için `.csproj` dosyasına aşağıdaki tanımın eklenmesi şarttır (aksi takdirde `FileNotFoundException` oluşur):
   ```xml
   <!-- Project.csproj: Embedded resources configuration -->
   <ItemGroup>
     <AvaloniaResource Include="Assets\**" />
     <EmbeddedResource Include="Localization\locales\*.json" />
   </ItemGroup>
   ```

2. **Avalonia 12 NuGet Paket Uyuşmazlığı (`TypeLoadException`):**
   Avalonia 12 (.NET 10) üzerinde Avalonia 11 için derlenmiş paketler (örneğin `Avalonia.Svg.Skia 11.x`) XAML parse anında `TypeLoadException (IBinding)` fırlatarak uygulamanın UI açılmadan sessizce sonlanmasına neden olur. Vektörel glifler için saf XAML `StreamGeometry` / `PathIcon` kullanılmalıdır.

3. **Dinamik DWM Yükseklik Eşzamanlaması (`ExtendClientAreaTitleBarHeightHint`):**
   Kompakt modda yalnızca XAML `Border` yüksekliği (`36px`) değil, code-behind üzerinden pencerenin `window.ExtendClientAreaTitleBarHeightHint` değeri de `36.0` olarak güncellenmelidir; aksi takdirde işletim sistemi başlık boşluğu 52px kalır ve dikey merkezleme bozulur.

4. **Ajan / Arka Plan Oturum İzolasyonu (Headless Execution):**
   Windows işletim sisteminde arka plan alt süreçleri (AI agent terminal görevleri) masaüstü oturumuna (`winsta0`) GUI penceresi çizemez. Ajanlar uygulamanın ekranda belirmemesini kod hatası olarak yorumlamamalıdır; masaüstü GUI doğrulaması mutlaka kullanıcının interaktif terminalinden (`dotnet run`) yapılmalıdır.

