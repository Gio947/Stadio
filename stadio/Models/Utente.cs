using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace stadio.Models
{
    public class Account
    {
        [Required]
        [Remote(action: "IsAlreadySigned", controller: "Home", HttpMethod = "POST", ErrorMessage = "User già in uso. ")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "{0} deve contenere almeno {2} caratteri e massimo {1}. ")]
        public string username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "{0} deve contenere almeno {2} caratteri e massimo {1}. ")]
        public string password { get; set; }

        public string accesso { get; set; }
    }

    public class Utente : Account
    {
        public int id { get; set; }

        [Required]
        [StringLength(30)]
        public string nome { get; set; }

        [Required]
        [StringLength(30)]
        public string cognome { get; set; }

        [Required]
        public string sesso { get; set; }

        [Required]
        [Display(Name = "Data di nascita")]
        [DataType(DataType.Date)]
        [Remote(action: "IsDataRegFuture", controller: "Home", HttpMethod = "POST", ErrorMessage = "data di nascita non valida. ")]
        public DateTime dataNascita { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal conto { get; set; }

        public string livello { get; set; }

        public List<Biglietto> listaBiglietti { get; set; }

        public int GetAge(DateTime reference, DateTime birthday)
        {   
            int age = reference.Year - birthday.Year;
            if (reference < birthday.AddYears(age))
                age--;
            return age; 
        }
    }
}