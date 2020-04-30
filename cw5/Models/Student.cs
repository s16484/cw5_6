using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Models
{
    public class Student
    {
        //public int IdStudent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public int IdEnrollment { get; set; }
        //public string BirthDate { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        public string StudyName { get; set; }
        public int Semester { get; set; }
    }
}
