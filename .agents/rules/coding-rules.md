---
title: "NIO Platform Kodlama Standartları ve İcra Kuralları"
description: "Yapay zeka asistanı ve geliştiriciler için sıfır hardcoded metin/renk, zorunlu tooltip, temiz mimari, C# ve Avalonia XAML bağlayıcı icra kuralları."
status: "approved"
date: "25.09.2026"
author:
  - "Gökhan Özkeser"
tags:
  - "kurallar"
  - "standartlar"
  - "coding-rules"
  - "csharp"
  - "xaml"
  - "localization"
  - "theming"
  - "tooltips"
---

# NIO Platform Kodlama Standartları ve İcra Kuralları

Bu kural dosyası, NIO platformu için yazılan veya düzenlenen her C#, Avalonia XAML, JSON ve mimari konfigürasyon dosyasında **istisnasız uyulması gereken bağlayıcı mühendislik standartlarını** belirler.

Yapay zeka asistanı (Antigravity), kod üretirken veya mevcut kodu güncellerken bu kuralları **asla atlayamaz veya gevşetemez**.

---

## 1. Demir Kural 1: Sıfır Hardcoded Metin (Zero Magic Strings - ADR-0010)

Kullanıcı arayüzünde görünen veya gösterilebilecek tek bir literal string dahi C# veya XAML kaynak koduna **sabit metin olarak yazılamaz**.

### XAML Standartları
- `Header`, `Content`, `Text`, `Title`, `ToolTip.Tip` gibi metin alan tüm özelliklerde `{loc:Loc Key=...}` veya `{loc:Loc ...}` Avalonia Markup Extension kullanımı **zorunludur**.
- ❌ **YASAK (DON'T):**
  ```xml
  <MenuItem Header="_File" />
  <Button Content="Theme" />
  <TextBlock Text="Ready" />
  <Window Title="NIO Platform" />
  ```
- ✅ **ZORUNLU (DO):**
  ```xml
  <MenuItem Header="{loc:Loc shell.menu.file}" />
  <Button Content="{loc:Loc shell.header.theme}" />
  <TextBlock Text="{loc:Loc shell.status.ready}" />
  <Window Title="{loc:Loc shell.header.title}" />
  ```

### C# / ViewModel Standartları
- ViewModel, Presenter veya servis katmanlarında kullanıcıya dönük hiçbir metin kod içinde tırnak içinde sabitlenemez (`string` literals yasaktır).
- Metinler `ILocalizationManager.GetString(key)` veya `LocalizationSource.Instance[key]` ile çözümlenmelidir.
- ❌ **YASAK (DON'T):**
  ```csharp
  Title = "Next-Gen Engineering Platform";
  StatusMessage = "Process failed!";
  ```
- ✅ **ZORUNLU (DO):**
  ```csharp
  Title = _localization.GetString("shell.header.title");
  StatusMessage = _localization.GetString("shell.status.process_failed");
  ```

### Çift Taraflı Sözlük Simetrisi (%100 Symmetrical Locales)
- Yeni bir yerelleştirme anahtarı eklendiğinde **istisnasız her iki sözlük dosyasına birden** yazılmalıdır:
  1. `src/NIO.Shell/Localization/locales/en-US.json`
  2. `src/NIO.Shell/Localization/locales/tr-TR.json`
- Anahtarlar hiyerarşik noktalı notasyon ile adlandırılmalıdır (Örn: `shell.header.theme`, `shell.toolbar.run_tooltip`).
- Mimari testler (`LocalizationDictionaryTests`) iki sözlük arasındaki %100 simetriyi otomatik olarak denetler; eksik anahtar içeren PR'lar veya derlemeler başarısız olur.

---

## 2. Demir Kural 2: Sıfır Hardcoded Renk (Zero Hardcoded Hex Colors - ADR-0012)

XAML dosyalarında `Background`, `Foreground`, `BorderBrush`, `Fill`, `Stroke` gibi görsel niteliklere doğrudan `#RRGGBB` veya `#AARRGGBB` hex rengi yazmak **KESİNLİKLE YASAKTIR**.

### Token Kullanım Zorunluluğu
- Tüm renkler ve fırçalar Avalonia'nın dinamik kaynak mekanizması üzerinden çağrılmalıdır: `{DynamicResource TokenKey}`.
- ❌ **YASAK (DON'T):**
  ```xml
  <Border Background="#141416" BorderBrush="#2D2D30">
    <TextBlock Foreground="#9CA3AF" Text="{loc:Loc shell.header.subtitle}" />
  </Border>
  ```
- ✅ **ZORUNLU (DO):**
  ```xml
  <Border Background="{DynamicResource Header.Background}" BorderBrush="{DynamicResource Border.Default}">
    <TextBlock Foreground="{DynamicResource Header.Subtitle}" Text="{loc:Loc shell.header.subtitle}" />
  </Border>
  ```

### Sözlük Tanımı ve İstisnalar
- **Yegane İstisna:** Tamamen şeffaf tıklama/sürükleme panelleri için `Background="Transparent"` veya `#00000000` kullanılabilir.
- **Yeni Renk İhtiyacı:** Yeni bir renge ihtiyaç duyulduğunda, renk asla XAML'a gömülemez. Önce aşağıdaki iki tema sözlüğüne karşılıklı token olarak eklenir, ardından XAML'da kullanılır:
  1. `src/NIO.Shell/Themes/Embedded/default-dark.theme.json`
  2. `src/NIO.Shell/Themes/Embedded/default-light.theme.json`
- Mimari testler (`XamlHardcodingRuleTests`) XAML dosyalarında hex renk taraması yapar ve ihlalleri anında kırar.

---

## 3. Demir Kural 3: Zorunlu Yerelleştirilmiş Tooltip & Kendini Açıklayan (Self-Explanatory) Arayüz

NIO, karmaşık mühendislik ve simülasyon hesaplamalarını yöneten profesyonel bir platformdur. Kullanıcının hiçbir kontrolde işlevi tahmin etmek zorunda kalmaması (**self-explanatory platform**) zorunlu bir tasarım ilkesidir (FR-040).

### Her İnteraktif Kontrolde Zorunlu Tooltip
- XAML'daki **her buton, araç çubuğu elemanı, menü aksiyonu, durum çubuğu göstergesi ve ikonik kontrol** üzerinde `ToolTip.Tip` tanımlanması zorunludur.
- ❌ **YASAK (DON'T):**
  ```xml
  <!-- Tooltip yok veya hardcoded metin -->
  <Button Classes="header-btn" Content="{loc:Loc shell.header.theme}" Command="{Binding ToggleThemeCommand}" />
  <Button Content="Run" ToolTip.Tip="Run Scenario" />
  ```
- ✅ **ZORUNLU (DO):**
  ```xml
  <!-- Yerelleştirilmiş, açıklayıcı tooltip -->
  <Button Classes="header-btn" 
          Content="{loc:Loc shell.header.theme}" 
          Command="{Binding ToggleThemeCommand}" 
          ToolTip.Tip="{loc:Loc shell.header.theme_tooltip}" />

  <Button Content="{loc:Loc shell.toolbar.run}" 
          ToolTip.Tip="{loc:Loc shell.toolbar.run_tooltip}" />
  ```

### Tooltip İçerik İlkeleri
1. **Sadece İsmi Tekrarlamayın:** Buton başlığı "Çalıştır" olan bir kontrole sadece "Çalıştır" tooltip'i yazmak yetersizdir. Ne yaptığı veya sonucu belirtilmelidir (Örn: *"Aktif senaryoyu simüle eder ve hesaplama akışını başlatır (F5)"*).
2. **Klavye Kısayolu:** Eğer eylemin bir kısayolu varsa, tooltip metninin sonunda parantez içinde belirtilmelidir.
3. **Simetrik Yerelleştirme:** Tooltip metinleri `_tooltip` son ekiyle hem `en-US.json` hem de `tr-TR.json` içine tanımlanmalıdır.

---

## 4. Demir Kural 4: C# Dil, Derleme ve Sıfır Uyarı Standartları (Zero Compiler Warnings)

Tüm projeler `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` konfigürasyonu ile derlenir. Çözüm genelinde **0 uyarı (warning) ve 0 hata** mutlak kuraldır.

### Yasaklı API'lar (Banned APIs)
- ❌ `Console.WriteLine` / `Console.Write` $\to$ **YASAKTIR.** Microsoft.Extensions.Logging `ILogger<T>` üzerinden yapılandırılmış (structured) loglama kullanılmalıdır.
- ❌ `Task.Wait()`, `Task.Result`, `.GetAwaiter().GetResult()` $\to$ **YASAKTIR.** UI kilitlenmelerini (thread starvation/deadlock) önlemek için saf `await` deseni kullanılmalıdır.
- ❌ `DateTime.Now` $\to$ **YASAKTIR.** Saat dilimi hatalarını önlemek için `DateTimeOffset.UtcNow` veya .NET 8/9 `TimeProvider` soyutlaması kullanılmalıdır.

### Analizörler ve Stub Metot Yönetimi
- **CA1822 (Mark members as static):** MVVM `[RelayCommand]` metotlarında veya henüz gövdesi doldurulmamış stub metotlarda analizör uyarısını susturmak için metodu statik yapmak yerine açık gerekçe ile suppress edilmelidir:
  ```csharp
  [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "RelayCommand for Avalonia XAML binding")]
  private void OpenSettings()
  {
      // Pending settings dialog implementation
  }
  ```
- **Null Safety:** C# 13 nullable reference types aktiftir (`<Nullable>enable</Nullable>`). Olası null referanslar `ArgumentNullException.ThrowIfNull(param)` veya nullable desenlerle kontrol edilmelidir.

---

## 5. Demir Kural 5: Katman İzolasyonu ve Temiz Mimari (ADR-0003 & ADR-0004)

NIO, katı bir Temiz Mimari (Clean Architecture) ve Headless çekirdek modeline dayanır.

### Bağımlılık Yönü (Dependency Flow)
```
NIO.Contracts  <-- En iç halka (Sıfır iç bağımlılık, saf DTO & interface)
      ▲
NIO.Abstractions (Yalnızca Contracts referansı)
      ▲
NIO.Core & NIO.Infrastructure (İş mantığı ve dış entegrasyonlar)
      ▲
NIO.Shell & NIO.App (Avalonia UI ve uygulama ana giriş noktası)
```

### Headless Çekirdek Kuralı
- `NIO.Contracts`, `NIO.Abstractions`, `NIO.Core` ve `NIO.Infrastructure` projeleri **asla Avalonia, UI kütüphaneleri veya `NIO.Shell` referansı içeremez**.
- Çekirdek iş mantığı, Linux sunucularda veya Docker container ortamlarında grafik arayüz olmadan (headless) çalışabilmelidir.
- Servis kayıtları her projenin kendi `ServiceCollectionExtensions` sınıfında toplanmalı ve arayüzler üzerinden DI konteynerine kaydedilmelidir.

---

## 6. Demir Kural 6: Kod Tabanında İstisnasız İngilizce Standardı

- C# sınıf, arabirim, metot, özellik ve değişken adları,
- XML dökümantasyon yorumları (`/// <summary>`),
- Kod içi satır yorumları (`//`),
- Hata ve durum kodları (`CORE_LOC_001`),
- JSON şema ve konfigürasyon anahtarları,
istisnasız **İngilizce** yazılmalıdır. Kod dosyalarında Türkçe karakter (`ç`, `ğ`, `ı`, `ö`, `ş`, `ü`, `İ`) tanımlayıcı veya yorum olarak kullanılamaz.

> [!NOTE]
> **İstisna:** Kullanıcı ile iletişim dili ve dokümantasyon dili Türkçedir. Son kullanıcı arayüzü Türkçe dil çevirileri ise yalnızca `tr-TR.json` kaynağında yer alır.

---

## 7. Özet Kontrol Listesi (Checklist)

Yeni bir özellik veya UI bileşeni eklerken kod tamamlanmadan önce şu 6 soruyu doğrulayın:

- [ ] 1. XAML veya C# içinde kullanıcıya dönük tek bir hardcoded metin var mı? (`{loc:Loc ...}` kullanıldı mı?)
- [ ] 2. Yeni eklenen yerelleştirme anahtarları hem `en-US.json` hem de `tr-TR.json` içinde tanımlandı mı?
- [ ] 3. XAML içinde hardcoded `#hex` renk kodu var mı? (`{DynamicResource ...}` kullanıldı mı?)
- [ ] 4. Eklenen tüm buton, menü ve interaktif kontrollerde yerelleştirilmiş `ToolTip.Tip` var mı?
- [ ] 5. Proje 0 hata ve 0 uyarı ile derleniyor mu? (`dotnet build` ve `dotnet test` yeşil mi?)
- [ ] 6. Tüm C# sembolleri, yorum satırları ve JSON anahtarları İngilizce mi?
