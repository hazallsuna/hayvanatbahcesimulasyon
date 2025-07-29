# Hayvanat Bahçesi Simülasyon Projesi

500x500'lük bir alanda çeşitli hayvanların yaşadığı ekolojik simülasyon projesi. Hayvanlar rastgele hareket eder, birbirlerini avlar ve ürer. 1000 hareket sonunda popülasyon değişimi analiz edilir.

## 📊 Hayvan Türleri ve Özellikleri

| Hayvan | Başlangıç Sayısı | Hareket Hızı | Avlama Mesafesi | Hedef Türler |
|--------|------------------|--------------|-----------------|--------------|
| Koyun | 30 (15♂, 15♀) | 2 birim | - | - |
| İnek | 10 (5♂, 5♀) | 2 birim | - | - |
| Tavuk | 10 (5♂, 5♀) | 1 birim | - | - |
| Horoz | 10 (5♂, 5♀) | 1 birim | - | - |
| Kurt | 10 (5♂, 5♀) | 3 birim | 4 birim | Koyun, Tavuk, Horoz |
| Aslan | 8 (4♂, 4♀) | 4 birim | 5 birim | İnek, Koyun |
| Avcı | 1 (♂) | 1 birim | 8 birim | Tüm hayvanlar |

## 🎯 Simülasyon Kuralları

### Hareket Sistemi
- Hayvanlar her turda rastgele yönde hareket eder
- Hareket mesafesi = `-1 ile +1 arası rastgele yön × HareketHızı`
- Alan sınırları: 0-500 koordinat sistemi
- Sınır dışına çıkış engellenir

### Avlanma Kuralları
- **Kurt**: 4 birim mesafede koyun, tavuk, horoz avlar
- **Aslan**: 5 birim mesafede inek, koyun avlar  
- **Avcı**: 8 birim mesafede herhangi bir hayvan avlar
- Her turda bir hayvan sadece bir av yapar
- Avlanan hayvan `YasiyorMu = false` olur

### Üreme Kuralları
- Aynı türden farklı cinsiyetteki hayvanlar
- 3 birim mesafe içinde yakınlaştığında üreme gerçekleşir
- Yeni doğan cinsiyeti rastgele belirlenir (%50-%50)
- Yeni doğan ebeveynlerin ortasına yakın konumlanır
- **Avcı üremez!**

## 🏛️ Sınıf Yapısı

### 1. Enum Sınıfları
```csharp
<<enumeration>> Cinsiyet
+ Erkek
+ Disi

<<enumeration>> HayvanTuru  
+ Koyun, Kurt, Inek, Tavuk, Horoz, Aslan, Avci
```

### 2. Model Sınıfları
```csharp
<<struct>> Konum
+ X: double
+ Y: double
+ Konum(x: double, y: double)
+ MesafeHesapla(digerKonum: Konum): double
+ ToString(): string

Hayvan
- rastgele: Random {static}
+ Id: int
+ Turu: HayvanTuru
+ Cinsiyeti: Cinsiyet  
+ Konumu: Konum
+ HareketHizi: double
+ YasiyorMu: bool = true
+ Hayvan(id, turu, cinsiyeti, konum, hareketHizi)
+ HareketEt(alanBoyutu: double): void
+ ToString(): string
```

### 3. Service Sınıfları
```csharp
SimulasyonMotoru
─────────────────────────────────────────────────────
- hayvanlar: List<Hayvan>
- alanBoyutu: double = 500
- rastgele: Random
- sonrakiId: int = 1
─────────────────────────────────────────────────────
<<Initialization Methods>>
+ SimulasyonMotoru()
- HayvanlarıBaslat(): void
- HayvanEkle(tur, sayi, cinsiyet, hiz): void

<<Simulation Core>>
+ SimulasyonuCalistir(toplamHareket: int): void
- TumHayvanlariHareketEttir(): void

<<Hunting System>>
- AvlanmaIslemi(): void
- KurtAvlanmasi(yasayanHayvanlar: List<Hayvan>): void
- AslanAvlanmasi(yasayanHayvanlar: List<Hayvan>): void  
- AvciAvlanmasi(yasayanHayvanlar: List<Hayvan>): void

<<Reproduction System>>
- UremeIslemi(): void
- TurIcinHizGetir(tur: HayvanTuru): double

<<Reporting System>>
- HayvanSayilariniGetir(): string
- DetayliSonuclariYazdir(): void
```

## 🔗 Sınıf İlişkileri

### Composition İlişkileri
```
SimulasyonMotoru ──♦── List<Hayvan>
"1"              "0..*"
```

### Aggregation İlişkileri  
```
Hayvan ──◊── Konum
"1"       "1"
```

### Association İlişkileri
```
Hayvan ────> Cinsiyet
"1"          "1"

Hayvan ────> HayvanTuru  
"1"          "1"
```

### Dependency İlişkileri
```
Program ┈┈┈> SimulasyonMotoru
```

## 🔄 Sequence Diagram - Simülasyon Akışı

```
Program → SimulasyonMotoru: new SimulasyonMotoru()
SimulasyonMotoru → SimulasyonMotoru: HayvanlarıBaslat()
Program → SimulasyonMotoru: SimulasyonuCalistir(1000)

loop 1000 hareket
    SimulasyonMotoru → Hayvan[*]: HareketEt(alanBoyutu)
    
    SimulasyonMotoru → SimulasyonMotoru: AvlanmaIslemi()
        SimulasyonMotoru → SimulasyonMotoru: KurtAvlanmasi()
        SimulasyonMotoru → SimulasyonMotoru: AslanAvlanmasi()
        SimulasyonMotoru → SimulasyonMotoru: AvciAvlanmasi()
    
    SimulasyonMotoru → SimulasyonMotoru: UremeIslemi()
    
    alt hareket % 100 == 0
        SimulasyonMotoru → Console: HayvanSayilariniGetir()
        Console → User: "Enter tuşuna basın..."
        User → Console: Enter
    end
end

SimulasyonMotoru → SimulasyonMotoru: DetayliSonuclariYazdir()
```

## 🔄 Activity Diagram - Simülasyon Algoritması

```
[Başla]
    ↓
[HayvanlarıBaslat()]
    ↓
[hareket = 1]
    ↓
[hareket <= 1000?] ──No──> [DetayliSonuclariYazdir()] ──> [Bitir]
    ↓ Yes
[TumHayvanlariHareketEttir()]
    ↓
[KurtAvlanmasi()]
    ↓
[AslanAvlanmasi()]  
    ↓
[AvciAvlanmasi()]
    ↓
[UremeIslemi()]
    ↓
[hareket % 100 == 0?] ──No──> [hareket++] ──┐
    ↓ Yes                                    │
[Durum Raporu Yazdır]                        │
    ↓                                        │
[Kullanıcı Enter bekle]                      │
    ↓                                        │
[hareket++] ─────────────────────────────────┘
    ↓
[Döngü başına dön]
```

## 🔄 State Machine Diagram - Hayvan Yaşam Döngüsü

```
[Doğum] ──> [Yaşıyor] ──> [Hareket Ediyor]
   ↑           ↓              ↓
   │      [Eşleşme Arayışı] [Avlanma/Avlanılma]
   │           ↓              ↓
   │      [Üreme Gerçekleşti] [Öldü]
   │           ↓              ↓
   └─── [Yeni Hayvan] ── [YasiyorMu = false]
```

## 📊 Çıktı Örneği

```
Simülasyon Başlatılıyor
Başlangıç durumu: Koyun: 30, Kurt: 10, Inek: 10, Tavuk: 10, Horoz: 10, Aslan: 8, Avci: 1

Hareket 100: Koyun: 28, Kurt: 8, Inek: 9, Tavuk: 7, Horoz: 8, Aslan: 6, Avci: 1
Devam etmek için Enter tuşuna basın...

Hareket 200: Koyun: 31, Kurt: 9, Inek: 8, Tavuk: 6, Horoz: 9, Aslan: 5, Avci: 1
Devam etmek için Enter tuşuna basın...

...

Hareket 1000: Koyun: 25, Kurt: 12, Inek: 4, Tavuk: 3, Horoz: 5, Aslan: 8, Avci: 1

=== Simülasyon tamamlandı ===
Son durum: Koyun: 25, Kurt: 12, Inek: 4, Tavuk: 3, Horoz: 5, Aslan: 8, Avci: 1

Toplam hayvan sayısı: 127
Yaşayan hayvan sayısı: 58  
Ölen hayvan sayısı: 69

Koyun: 25 (Erkek: 13, Dişi: 12)
Kurt: 12 (Erkek: 6, Dişi: 6)
Inek: 4 (Erkek: 2, Dişi: 2)
Tavuk: 3 (Erkek: 1, Dişi: 2)
Horoz: 5 (Erkek: 3, Dişi: 2)
Aslan: 8 (Erkek: 4, Dişi: 4)
Avci: 1 (Erkek: 1, Dişi: 0)
```

## 🚀 Teknik Özellikler

### Kullanılan Teknolojiler
- **.NET Framework/Core** 
- **C# 9.0+** 
- **LINQ** 

### OOP Prensipleri
- **Encapsulation**: Private metodlar ve field'lar
- **Single Responsibility**: Her sınıfın tek sorumluluğu var
- **Code Reusability**: `HayvanEkle()` metodu ile tekrar kullanım

### Design Patterns
- **Strategy Pattern**: Farklı avlanma stratejileri (Kurt, Aslan, Avcı)
- **Factory Pattern**: `HayvanEkle()` metodu ile hayvan üretimi

### Data Structures
- **List<T>**: Hayvan koleksiyonu
- **HashSet<int>**: Üreme işleminde kullanılan hayvanları takip
- **Dictionary**: Sayım işlemleri için

### Matematiksel Hesaplamalar
- **Öklid Mesafesi**: `√((x₁-x₂)² + (y₁-y₂)²)`
- **Rastgele Hareket**: Polar koordinat sistemi benzer yaklaşım
- **Sınır Kontrolü**: `Math.Max(0, Math.Min(sınır, değer))`

## 📖 Kaynak Kod Açıklamaları


#### Rastgele Hareket Algoritması
```csharp
// -1 ile 1 arasında rastgele yön belirleme (normalize edilmiş)
double yonX = (rastgele.NextDouble() - 0.5) * 2;
double yonY = (rastgele.NextDouble() - 0.5) * 2;

// Hareket mesafesi = yön × hız
double yeniX = Konumu.X + yonX * HareketHizi;
double yeniY = Konumu.Y + yonY * HareketHizi;
```

#### Mesafe Hesaplama (Öklid)
```csharp
public double MesafeHesapla(Konum digerKonum)
{
    return Math.Sqrt(Math.Pow(X-digerKonum.X,2) + Math.Pow(Y-digerKonum.Y,2));
}
```

#### Avlanma Optimizasyonu
```csharp
// Aynı turda tekrar avlanmayı önle
avlar.Remove(av); 
```

#### Üreme Mesafe Kontrolü
```csharp
var eslesecekDisi = disiler
    .Where(d => !kullanilanDisiler.Contains(d.Id) &&
                erkek.Konumu.MesafeHesapla(d.Konumu) <= 3)
    .FirstOrDefault();
```
