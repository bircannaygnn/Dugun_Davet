using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dugun_Davet2.Models
{
    public class Repository
    {
        private static List<Cevap> cevaplar = new List<Cevap>();
        public static IEnumerable<Cevap> Cevaplar => cevaplar;
        public static void CevapEkle(Cevap cevap)
        {
            cevaplar.Add(cevap);
        }

    }

}
