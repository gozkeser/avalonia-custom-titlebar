---
title: "Markdown Dokümantasyon Kuralları ve Standartları"
description: "Teknik dokümantasyon, ADR, vizyon ve gereksinim belgeleri için evrensel yazım ve DOCX/PDF uyumluluk kuralları."
status: "approved"
date: "21.09.2026"
author:
  - "Gökhan Özkeser"
tags:
  - "kurallar"
  - "standartlar"
  - "markdown"
  - "dokumantasyon"
---

# Markdown Dokümantasyon Kuralları ve Standartları

Bu kural dosyası, proje genelinde teknik ve yazılım dokümantasyonu oluştururken ve güncellerken uyulması gereken biçimlendirme, dil, mimari, organizasyon ve **DOCX / PDF dönüştürme uyumluluğu** standartlarını belirler.

---

## 1. Dil ve Anlatım Standartları

- **Ana Dil:** Dokümantasyon dili **Türkçe**dir.
- **Teknik Terminoloji:** Yazılım ve mimari dünyasında evrensel kabul görmüş terimler ilk geçtiği yerde Türkçe karşılığıyla birlikte veya parantez içinde İngilizce olarak verilmelidir (Örn: *bağımlılık enjeksiyonu (dependency injection)*, *yük dengeleyici (load balancer)*, *uç nokta (endpoint)*).
- **Üslup:** Açık, net, doğrudan ve teknik bir üslup kullanılmalıdır.

---

## 2. Dosya Adlandırma ve Hiyerarşi

- **Dosya İsimleri:** İstisnasız `kebab-case.md` formatında ve küçük harflerle olmalıdır.
  - Örnek: `top-level-requirements.md`, `vision.md`, `system-architecture.md`
- **ADR Numaralandırma:** Karar kayıtları kronolojik sıra için 4 basamaklı ön ek almalıdır:
  - Örnek: `0001-postgresql-veritabani-secimi.md`, `0002-jwt-yetkilendirme-mimarisi.md`
- **Başlık Seviyeleri (Heading Hierarchy):**
  - Dosyada yalnızca **tek bir `#` (H1)** bulunmalıdır (Belge Başlığı).
  - Bölümler `##` (H2), alt bölümler `###` (H3) olarak devam etmelidir. Başlık seviyeleri atlanmamalıdır (H2'den direkt H4'e geçilmez).

---

## 3. Zorunlu YAML Frontmatter

Her Markdown dosyasının en başında aşağıdaki YAML meta veri bloğu eksiksiz yer almalıdır:

```yaml
---
title: "Belgenin Anlaşılır Başlığı"
description: "Belgenin kapsamını ve amacını özetleyen 1-2 cümlelik açıklama."
status: "draft | in-review | approved | deprecated"
date: "GG.AA.YYYY"
author:
  - "Gökhan Özkeser"
tags:
  - "vizyon"
  - "gereksinimler"
  - "mimari"
  - "api"
  - "adr"
---
```

---

## 4. Dizin Organizasyonu (`docs/`) ve Yaşam Döngüsü

Dokümantasyon süreci şu mühendislik sırasını izler:  
**Vizyon (`vision/vision.md`) → Gereksinimler (`requirements/`) → Sistem Tasarımı (`architecture/`) → Mimari Kararlar (`architecture/adr/`) → Arayüzler & Modeller (`api/`, `models/`)**

```text
docs/
├── vision/
│   ├── vision.md          # Projenin temel vizyonu, problemi ve değer önerisi
│   └── ...
├── requirements/
│   ├── top-level-requirements.md # Üst düzey fonksiyonel ve fonksiyonel olmayan gereksinimler
│   └── ...                # Detaylı alt gereksinimler
├── architecture/          # Sistem mimarisi, altyapı ve bileşen tasarımları
│   ├── high-level-system-architecture.md # Üst düzey sistem mimarisi
│   ├── adr/               # Mimari Karar Kayıtları (Architecture Decision Records)
│   │   ├── 0001-...md
│   │   └── ...
│   └── ...
├── api/                   # API uç nokta spesifikasyonları ve sözleşmeleri
├── models/                # Veri modelleri, şemalar, ER diyagramları
├── assets/                # Tüm görsel ve diyagram dosyaları
│   ├── diagrams/          # Çıktısı alınmış diyagram görselleri (.png / .svg)
│   ├── images/            # Ekran görüntüleri, logolar ve şemalar
│   └── templates/         # Word (DOCX) referans şablonları ve Pandoc Lua filtreleri
└── README.md              # Dokümantasyon dizin haritası ve genel rehber
```

---

## 5. DOCX ve PDF Dönüşüm Uyumluluk Kuralları

Markdown belgelerinin Word (`.docx`) veya PDF formatlarına dönüştürülmesinde mizanpaj ve içerik kaybı yaşanmaması için aşağıdaki kurallar **zorunludur**:

### 5.1. Sıfır Ham HTML Kuralı (Pure Markdown)
- `<br>`, `<div>`, `<span>`, `<p>`, `style="..."`, `align="..."` gibi HTML etiketleri kesinlikle kullanılmamalıdır.
- Boşluk ve hizalama için yalnızca standart Markdown kuralları kullanılmalıdır.

### 5.2. Görsel Bağlantıları ve Boyutlandırma Standartları
- **Göreceli Yollar (Relative Paths Only):** Görsel ve dosya bağlantılarında asla mutlak yol (`C:\...`, `file:///...`) kullanılmamalıdır. Yollar daima göreceli olmalıdır (Örn: `![Mimari Akış](../assets/diagrams/0001-mimari-akis.png)`).
- **Standart Boyutlandırma Öznitelikleri (`{width=100% height=18cm}`):** Görsellerin Word ve PDF çıktılarında küçük kalmasını, okunaksız olmasını veya dikeyde bir sayfayı aşarak alt sayfaya taşmasını engellemek için tüm mimari diyagram ve ön yüz mockup resim bağlantılarının sonuna `{width=100% height=18cm}` öznitelikleri eklenmelidir:
  ```markdown
  ![Açıklama](../assets/diagrams/ornek-diyagram.png){width=100% height=18cm}
  ```
  *Açıklama:* Pandoc bu özniteliklerle görseli tam sayfa metin genişliğine (16.51 cm) oturturken en-boy oranını (*aspect ratio*) korur ve yüksekliğin tek bir sayfayı (18 cm) aşmamasını garanti eder.

### 5.3. Diyagramlar İçin Çift Yaklaşım (Dual Diagramming) ve Saf Mermaid Ayrımı
DOCX ve temel PDF dönüştürücüler ham ```` ```mermaid ```` kod bloklarını doğrudan görselleştiremez. Bu nedenle:

> [!IMPORTANT]
> **Kapsam Ayrımı (Word Belgeleri vs. Saf Teknik Dokümanlar):**
> 1. **Word / PDF Çıktısı Alınacak Kurumsal Belgeler (`vision.md`, `high-level-system-architecture.md` vb.):** Çift yaklaşım uygulanır. Düzenlenebilir ```` ```mermaid ```` kodu korunur, render edilmiş `.png` görseli `docs/assets/diagrams/` altına kaydedilir ve kod bloğunun hemen altına standart görsel sözdizimi (`![Açıklama](../assets/diagrams/...png){width=100% height=18cm}`) eklenir.
> 2. **Geliştirici ve Mimari Şartnameler (Katman Detay Tasarımları `layers/*.md`, İş Akışları, ADR'ler):** Yalnızca **saf ```` ```mermaid ```` kod blokları** kullanılır. Altlarına **kesinlikle** `![...](...png)` görsel bağlantısı eklenmez ve PNG render araçları tetiklenmez. Bu belgeler geliştirici ortamlarında (GitHub, IDE önizlemesi vb.) dinamik olarak render edilir.

### 5.4. Tablo Standartları
- Standart GFM pipe tabloları (`| Başlık 1 | Başlık 2 |`) kullanılmalıdır.
- Tablo hücreleri içinde satır sonu, liste veya birleştirilmiş hücre (colspan/rowspan) yapıları kullanılmamalıdır.

### 5.5. Sayfa Kesmeleri (Pagination)
- PDF ve DOCX çıktılarında yeni bir sayfa başlatılması gereken yerlerde evrensel sayfa kesme etiketi kullanılmalıdır:
  ```markdown
  <!-- pagebreak -->
  ```

### 5.6. Kod Blokları, Satır Taşması ve ASCII Wireframe Standartları
- **Genel Kod Blokları:** Kod bloklarındaki satırlar Word/PDF taşmalarını önlemek için standart olarak **80-100 karakteri geçmemelidir**. Her kod bloğunda mutlaka dil belirteci yazılmalıdır (Örn: ```` ```json ````, ```` ```csharp ````).
- **ASCII Wireframe ve Kutu Çizimleri (Box-Drawing):** Ekran taslakları veya arayüz yerleşimleri monospace kutu çizim karakterleri (`┌ ─ ┐ │ └ ┘ ├ ┼ ┤ ┬ ┴`) ile çiziliyorsa:
  1. Monospace hizalamasını bozabilecek çift genişlikli (Wide/Ambiguous) emojiler veya özel geometrik semboller kullanılmamalı; standart 1 hücrelik karakterler tercih edilmelidir.
  2. Word (DOCX) dönüşümünde 100-110 karakter genişliğindeki kutuların alt satıra kırılarak bozulmaması için `filter-mermaid.lua` filtresi kutu çizim karakterlerini içeren veya `.wireframe` sınıfına sahip blokları otomatik olarak **8 pt Consolas** ve **sıkı satır aralığı** (`w:line="200"`) ile derler.
  3. Wireframe kod bloklarından önce mutlaka `<!-- pagebreak -->` eklenerek temiz bir sayfa başında başlaması sağlanmalıdır.

### 5.7. Tipografi, Listeler ve Bağlantılar
- **İç İçe Listeler:** En fazla **2-3 seviye** ile sınırlandırılmalıdır.
- **Dipnotlar:** Basılı veya PDF belgelerde bağlantı adreslerinin kaybolmaması için kritik harici referanslarda dipnot (`[^1]`) tercih edilmelidir.

---

## 6. Hazır Şablonlar (Boilerplate Templates)

Dokümantasyon türlerine göre hazırlanmış ve proje standartlarına tam uyumlu referans şablonlar `.agents/rules/` dizini altında bağımsız dosyalar olarak yer almaktadır. Yeni bir doküman oluştururken ilgili şablon dosyasının yapısı birebir esas alınmalıdır:

- [**Üst Düzey Sistem Mimarisi Şablonu (high-level-system-architecture-template.md)**](./high-level-system-architecture-template.md):  
  Sistem sınırları, kapsam ayrımı (In-Scope / Out-of-Scope), katman tanımları, bağımlılık matrisi ve tasarım kısıtlarını içeren üst düzey mimari şablonu.
- [**Detaylı Sistem Mimarisi Şablonu (system-architecture-template.md)**](./system-architecture-template.md):  
  C4 modeli, konteyner/katman yapısı, alt sistem mimarileri ve NFR izlenebilirliğini tanımlayan detaylı mimari tasarım şablonu.
- [**Mimari Karar Kaydı (ADR) Şablonu (adr-template.md)**](./adr-template.md):  
  Kritik teknik ve mimari seçimlerin gerekçelendirilmesi, alternatiflerin karşılaştırılması ve sonuçların kaydedilmesi için kullanılan şablon.
- [**API ve Servis Tasarımı Şablonu (api-design-template.md)**](./api-design-template.md):  
  Uç nokta sözleşmeleri, istek/yanıt şemaları, sekans akışları ve veri modellerini belgelemek için kullanılan şablon.
- [**Vizyon Dokümanı Şablonu (vision-template.md)**](./vision-template.md):  
  Ürün veya sistemin varoluş amacını, çözdüğü problemleri, hedef kitleyi ve temel yetenekleri tanımlayan vizyon şablonu.
- [**Üst Düzey Gereksinimler Şablonu (top-level-requirements-template.md)**](./top-level-requirements-template.md):  
  Sistem ve ürün gereksinimlerinin (fonksiyonel ve NFR) hiyerarşik ve izlenebilir biçimde listelendiği gereksinim şablonu.


