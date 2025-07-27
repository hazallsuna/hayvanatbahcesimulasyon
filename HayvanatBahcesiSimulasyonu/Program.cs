using System;
using HayvanatBahcesiSimulasyonu.Services;

namespace HayvanatBahcesiSimulasyonu
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hayvanat Bahçesi Simülasyonu Başlatılıyor...");
            Console.WriteLine();

            // simülasyonu çalıştır
            SimulasyonMotoru simulasyon = new SimulasyonMotoru();
            simulasyon.SimulasyonuCalistir(1000);

           
            Console.WriteLine("Çıkmak için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}