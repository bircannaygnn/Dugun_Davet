using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dugun_Davet2.Models
{
    public class Cevap
    {
        [Required(ErrorMessage="Lütfen Ad alanını boş bırakmayınız")]
        public string Ad { get; set; }
        [Required(ErrorMessage = "Lütfen Soyad alanını boş bırakmayınız")]
        public string Soyad { get; set; }
        [Required(ErrorMessage = "Lütfen Email alanını boş bırakmayınız")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Lütfen Telefon alanını boş bırakmayınız")]
        public string Telefon { get; set; }
        [Required(ErrorMessage = "Lütfen bir seçim yapınız")]
        public bool? DuguneGelecekmi { get; set; }


    }
}
