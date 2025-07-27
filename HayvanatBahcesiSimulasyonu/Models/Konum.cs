using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HayvanatBahcesiSimulasyonu.Models
{
    /// <summary>
    /// 2D koordinat sistemi için struct
    /// küçük, değer tipi veri yapısı olduğu için hafızada verimli
    /// </summary>
    public struct Konum
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Konum(double x, double y)
        { X = x; Y = y; }

        // iki nokta arasındaki öklid mesafesini hesaplar
        //avlama ve üreme mesafesi kontrolü için

        public double MesafeHesapla(Konum digerKonum)
        {
            return Math.Sqrt(Math.Pow(X-digerKonum.X,2) + Math.Pow(Y-digerKonum.Y,2));
        }

        public override string ToString()
        {
            return $"({X:F1},{Y:F1})";
        }


    }
}
