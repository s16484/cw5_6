using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw5.DTOs.Request
{
    public class EnrollStudentRequest
    {

        [Required]
        [RegularExpression("^s[0-9]+$")]
        [MaxLength(15)]
        public string IndexNumber { get; set; }
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        [MaxLength(150)]
        public string StudyName { get; set; }


    }
}
