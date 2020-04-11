using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Dtos
{
    public class CustomerUpdateDto
    {
        [Required(ErrorMessage = "Please give the first name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please give the first name")]
        public string LastName { get; set; }
        [Required]
        [Range(0, 100)]
        public int Age { get; set; }
    }
}
