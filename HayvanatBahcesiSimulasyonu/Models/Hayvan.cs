using HayvanatBahcesiSimulasyonu.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HayvanatBahcesiSimulasyonu.Models
{
    /// <summary>
    /// temel hayvan sınıfı tüm hayvanların ortak özelliklerini içerir
    /// 
    /// </summary>

    public class Hayvan
    {
        public int Id { get; set; }
        public HayvanTuru Turu { get; set; }
        public Cinsiyet Cinsiyeti {  get; set; }
        public Konum Konumu { get; set; }
        public double HareketHizi {  get; set; }
        public bool YasiyorMu { get; set; } =true;

        //static random tüm hayvanalr aynı random generator kullanır
        private static Random rastgele = new Random();

        //hayvan constructor'ı
        //yeni hayvan oluştururken tüm temel bilgileri set eder

        public Hayvan(int id,HayvanTuru turu,Cinsiyet cinsiyeti,Konum konum, double hareketHizi)
        {
            Id = id;
            Turu = turu;
            Cinsiyeti = cinsiyeti;
            Konumu = konum;
            HareketHizi = hareketHizi;
        }

        // hayvanı rastgele yönünde hareket ettirir
        //çünkü her turda hayvanların pozisyonu değişmeli

        public void HareketEt(double alanBoyutu)
        {
            if (!YasiyorMu) return; //ölü hayvan hareket etmez

            // -1 ile 1 arasında rastgele yön belirleme
            double yonX = (rastgele.NextDouble() - 0.5) * 2;
            double yonY = (rastgele.NextDouble() - 0.5) * 2;

            //yeni pozisyon hesaplama 
            double yeniX = Konumu.X + yonX * HareketHizi;
            double yeniY = Konumu.Y + yonY * HareketHizi;

            //alan sınırları kontrolü (0-500)
            yeniX=Math.Max(0,Math.Min(alanBoyutu,yeniX));
            yeniY=Math.Max(0, Math.Min(alanBoyutu, yeniY));

            Konumu = new Konum(yeniX,yeniY);

        }

        public override string ToString()
        {
            return $"{Turu} (ID:{Id},Cinsiyet:{Cinsiyeti}, Konum:{Konumu})";
        }
    }
    
}
