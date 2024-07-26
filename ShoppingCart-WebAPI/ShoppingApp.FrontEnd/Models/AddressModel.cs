using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.FrontEnd.Models
{
    public class AddressModel
    {
        
        public int addressID { get; set; }

        [Required]
        public string street { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string country { get; set; }

        [Required]
        public string postalCode { get; set; }

        public bool IsPrimary { get; set; }
    }
}
