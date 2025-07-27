using HayvanatBahcesiSimulasyonu.Enums;
using HayvanatBahcesiSimulasyonu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HayvanatBahcesiSimulasyonu.Services
{
    /// <summary>
    /// tüm simülasyon mantığını yönetir
    /// single responsibility principle
    /// </summary>
    public class SimulasyonMotoru
    {
        private List<Hayvan> hayvanlar; //tüm hayvanları tutar
        private readonly double alanBoyutu = 500; //sabit alan boyutu
        private readonly Random rastgele = new Random(); //rastgele işlemler için
        private int sonrakiId = 1; //yeni hayvan Id için sayaç

        public SimulasyonMotoru()
        {
            hayvanlar = new List<Hayvan>();
            HayvanlarıBaslat();
        }

        //başlangıç hayvan popülasyonunu oluşturur
        //belirtilen sayıda hayvan oluşturmak için 

        private void HayvanlarıBaslat()
        {
            // 30 koyun 15e-15d 2 br hareket
            HayvanEkle(HayvanTuru.Koyun, 15, Cinsiyet.Erkek, 2);
            HayvanEkle(HayvanTuru.Koyun, 15, Cinsiyet.Disi, 2);

            // 10 inek 5e-5d 2 br hareket  
            HayvanEkle(HayvanTuru.Inek, 5, Cinsiyet.Erkek, 2);
            HayvanEkle(HayvanTuru.Inek, 5, Cinsiyet.Disi, 2);

            // 10 tavuk 5d-5e  1 br hareket
            HayvanEkle(HayvanTuru.Tavuk, 5, Cinsiyet.Erkek, 1);
            HayvanEkle(HayvanTuru.Tavuk, 5, Cinsiyet.Disi, 1);

            // 10 kurt 5d-5e - 3 br hareket
            HayvanEkle(HayvanTuru.Kurt, 5, Cinsiyet.Erkek, 3);
            HayvanEkle(HayvanTuru.Kurt, 5, Cinsiyet.Disi, 3);

            // 10 horoz 5d-5e  1 br hareket
            HayvanEkle(HayvanTuru.Horoz, 5, Cinsiyet.Erkek, 1);
            HayvanEkle(HayvanTuru.Horoz, 5, Cinsiyet.Disi, 1);

            // 8 aslan 4e-4d  br hareket
            HayvanEkle(HayvanTuru.Aslan, 4, Cinsiyet.Erkek, 4);
            HayvanEkle(HayvanTuru.Aslan, 4, Cinsiyet.Disi, 4);

            // 1 avcı 1 br hareket (cinsiyeti önemsiz)
            HayvanEkle(HayvanTuru.Avci, 1, Cinsiyet.Erkek, 1);

        }

        ///belirtilen türde ve sayıda hayvan ekler
        ///kod tekrarını önlemek için 
        private void HayvanEkle(HayvanTuru tur, int sayi, Cinsiyet cinsiyet, double hiz)
        {
            for (int i = 0; i < sayi; i++)
            {
                Konum konum = new Konum(
                    rastgele.NextDouble() * alanBoyutu,
                    rastgele.NextDouble() * alanBoyutu);
                hayvanlar.Add(new Hayvan(sonrakiId++, tur, cinsiyet, konum, hiz));
            }
        }

        /// ana simülasyon döngüsü
        /// 1000 hareket boyunca simülasyonu kontrollü şekilde yürütmek için

        public void SimulasyonuCalistir(int toplamHareket)
        {
            Console.WriteLine("Simülasyon Başlatılıyor");
            Console.WriteLine($"Başlangıç durumu:{HayvanSayilariniGetir()}");
            Console.WriteLine();

            for (int hareket = 1; hareket <= toplamHareket; hareket++)
            {
                //tüm hayvanları hareket ettir
                TumHayvanlariHareketEttir();

                //avlama işlemlerini gerçekleştir
                AvlanmaIslemi();

                //üreme işlemlerini kontrol et
                UremeIslemi();

                //her 100 hareket sonunda durum raporu 
                if (hareket % 100 == 0)
                {
                    Console.WriteLine($"Hareket{hareket}:{HayvanSayilariniGetir()}");
                    // son hareket değilse bekle
                    if (hareket < toplamHareket)
                    {
                        Console.WriteLine("Devam etmek için Enter tuşuna basın...");
                        Console.ReadLine();
                        Console.WriteLine();
                    }

                }
            }
            Console.WriteLine();
            Console.WriteLine("=== Simülasyon tamamlandı ===");
            Console.WriteLine($"Son durum: {HayvanSayilariniGetir()}");
            DetayliSonuclariYazdir();
        }

        ///tüm canlı hayvanları hareket ettirir
        private void TumHayvanlariHareketEttir()
        {
            foreach (var hayvan in hayvanlar.Where(h => h.YasiyorMu))
            {
                hayvan.HareketEt(alanBoyutu);
            }
        }

        ///tüm avlama kurallarını işler
        ///3 farklı avcı türü var
        private void AvlanmaIslemi()
        {
            var yasayanHayvanlar = hayvanlar.Where(h => h.YasiyorMu).ToList();

            //kurtların avlanması ->koyun,tavuk,horoz
            KurtAvlanmasi(yasayanHayvanlar);

            //aslanların avlanması -> inek,koyun
            AslanAvlanmasi(yasayanHayvanlar);

            //avcının avlanması -> tüm hayvanlar
            AvciAvlanmasi(yasayanHayvanlar);
        }

        //kurt avlanması 4 birim yakınlık koyun, tavuk, horozları avlar

        private void KurtAvlanmasi(List<Hayvan> yasayanHayvanlar)
        {
            var kurtlar = yasayanHayvanlar.Where(h => h.Turu == HayvanTuru.Kurt).ToList();
            var avlar = yasayanHayvanlar.Where(h => h.Turu == HayvanTuru.Koyun ||
                                              h.Turu == HayvanTuru.Tavuk ||
                                              h.Turu == HayvanTuru.Horoz).ToList();
            foreach (var kurt in kurtlar)
            {
                var yakinAvlar = avlar.Where(a => a.Konumu.MesafeHesapla(kurt.Konumu) <= 4).ToList();
                if (yakinAvlar.Count > 0)
                {
                    var av = yakinAvlar[rastgele.Next(yakinAvlar.Count)];
                    av.YasiyorMu = false;
                    avlar.Remove(av); // aynı turda tekrar avlanmayı önle
                }
            }
        }

        /// aslanlar 5 br yakınlıktaki inek ve koyunları avlar
        private void AslanAvlanmasi(List<Hayvan> yasayanHayvanlar)
        {
            var aslanlar = yasayanHayvanlar.Where(h => h.Turu == HayvanTuru.Aslan).ToList();
            var avlar = yasayanHayvanlar.Where(h => h.Turu == HayvanTuru.Inek ||
                                              h.Turu == HayvanTuru.Koyun).ToList();

            foreach (var aslan in aslanlar)
            {
                var yakinAvlar = avlar.Where(a => a.Konumu.MesafeHesapla(aslan.Konumu) <= 5).ToList();
                if (yakinAvlar.Count > 0)
                {
                    var av = yakinAvlar[rastgele.Next(yakinAvlar.Count)];
                    av.YasiyorMu = false;
                    avlar.Remove(av); // aynı turda tekrar avlanmayı önle
                }
            }
        }

        /// avcı 8 br yakınlıktaki herhangib bir hayvanı avlayabilir

        private void AvciAvlanmasi(List<Hayvan> yasayanHayvanlar)
        {
            var avcilar = yasayanHayvanlar.Where(h => h.Turu == HayvanTuru.Avci).ToList();
            var tumAvlar = yasayanHayvanlar.Where(h => h.Turu != HayvanTuru.Avci).ToList();

            foreach (var avci in avcilar)
            {
                var yakinAvlar = tumAvlar.Where(a => a.Konumu.MesafeHesapla(avci.Konumu) <= 8).ToList();
                if (yakinAvlar.Count > 0)
                {
                    var av = yakinAvlar[rastgele.Next(yakinAvlar.Count)];
                    av.YasiyorMu = false;
                    tumAvlar.Remove(av); // aynı turda tekrar avlanmayı önle
                }
            }
        }

        /// üreme işlemi
        /// aynı türden farklı cinsiyetteki hayvanlar 3 br yaklaştığında ürer

        private void UremeIslemi()
        {
            var yasayanHayvanlar = hayvanlar.Where(h => h.YasiyorMu).ToList();
            var yeniDoganlar = new List<Hayvan>();

            // her hayvan turu için üreme kontrolü
            foreach (HayvanTuru tur in Enum.GetValues(typeof(HayvanTuru)))
            {
                if (tur == HayvanTuru.Avci) continue; // Avcı üremez

                var erkekler = yasayanHayvanlar.Where(h => h.Turu == tur && h.Cinsiyeti == Cinsiyet.Erkek).ToList();
                var disiler = yasayanHayvanlar.Where(h => h.Turu == tur && h.Cinsiyeti == Cinsiyet.Disi).ToList();

                var kullanilanDisiler = new HashSet<int>();

                foreach (var erkek in erkekler)
                {
                    var eslesecekDisi = disiler
                        .Where(d => !kullanilanDisiler.Contains(d.Id) &&
                                    erkek.Konumu.MesafeHesapla(d.Konumu) <= 3)
                        .FirstOrDefault();

                    if (eslesecekDisi != null)
                    {
                        // yeni hayvan oluştur
                        var yeniCinsiyet = rastgele.Next(2) == 0 ? Cinsiyet.Erkek : Cinsiyet.Disi;
                        var hiz = TurIcinHizGetir(tur);

                        var yeniKonum = new Konum(
                            (erkek.Konumu.X + eslesecekDisi.Konumu.X) / 2 + (rastgele.NextDouble() - 0.5) * 10,
                            (erkek.Konumu.Y + eslesecekDisi.Konumu.Y) / 2 + (rastgele.NextDouble() - 0.5) * 10
                        );

                        // alan sınırı kontrolü
                        yeniKonum = new Konum(
                            Math.Max(0, Math.Min(alanBoyutu, yeniKonum.X)),
                            Math.Max(0, Math.Min(alanBoyutu, yeniKonum.Y))
                        );

                        yeniDoganlar.Add(new Hayvan(sonrakiId++, tur, yeniCinsiyet, yeniKonum, hiz));
                        kullanilanDisiler.Add(eslesecekDisi.Id);
                    }
                }
            }
            //yeni doganları listeye ekliyorz
            hayvanlar.AddRange(yeniDoganlar);
        }

        ///hayvan turune göre hareket hızını döndürür
        ///çünkü her hayvanın farklı hızı var

        private double TurIcinHizGetir(HayvanTuru tur)
        {
            return tur switch
            {
                HayvanTuru.Koyun => 2,
                HayvanTuru.Kurt => 3,
                HayvanTuru.Inek => 2,
                HayvanTuru.Tavuk => 1,
                HayvanTuru.Horoz => 1,
                HayvanTuru.Aslan => 4,
                HayvanTuru.Avci => 1,
                _ => 1
            };
        }

        /// güncel hayvan sayılarını string olarak döndürür
        private string HayvanSayilariniGetir()
        {
            var yasayanHayvanlar = hayvanlar.Where(h => h.YasiyorMu).ToList();
            var sayilar = yasayanHayvanlar.GroupBy(h => h.Turu)
                                        .ToDictionary(g => g.Key, g => g.Count());

            return string.Join(", ",
                Enum.GetValues<HayvanTuru>()
                    .Select(tur => $"{tur}: {sayilar.GetValueOrDefault(tur, 0)}")
            );
        }

        ///simülasyon sonunda detaylı rapor yazdır

        private void DetayliSonuclariYazdir()
        {
            var yasayanHayvanlar = hayvanlar.Where(h => h.YasiyorMu).ToList();
            var toplamOlu = hayvanlar.Count(h => !h.YasiyorMu);

            Console.WriteLine($"Toplam hayvan sayısı: {hayvanlar.Count}");
            Console.WriteLine($"Yaşayan hayvan sayısı: {yasayanHayvanlar.Count}");
            Console.WriteLine($"Ölen hayvan sayısı: {toplamOlu}");
            Console.WriteLine();


            foreach (HayvanTuru tur in Enum.GetValues<HayvanTuru>())
            {
                var turHayvanları = yasayanHayvanlar.Where(h => h.Turu == tur).ToList();
                var erkekler = turHayvanları.Count(h => h.Cinsiyeti == Cinsiyet.Erkek);
                var disiler = turHayvanları.Count(h => h.Cinsiyeti == Cinsiyet.Disi);

                Console.WriteLine($"{tur}: {turHayvanları.Count} (Erkek: {erkekler}, Dişi: {disiler})");
            }
        }
    }
}