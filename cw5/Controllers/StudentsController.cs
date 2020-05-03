using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.Models;
using cw5.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private IStudentDbService _dbService;
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16484;Integrated Security=True";
       
        public StudentsController(IStudentDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var list = new List<Student>();

            using (var con = new SqlConnection(ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = con;
                command.CommandText = "select FirstName, LastName, BirthDate, Name, Semester, IndexNumber, s.IdEnrollment "
                    + "from Student s, Enrollment e, Studies studies "
                    + "where s.IdEnrollment = e.IdEnrollment AND e.IdStudy = studies.IdStudy";

                con.Open();
                var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString()).Date;
                    st.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                    st.StudyName = dr["Name"].ToString();
                    st.Semester = Int16.Parse(dr["Semester"].ToString());
                    list.Add(st);
                }

            }
            return Ok(list);
        }



        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            
            var student = _dbService.GetStudent(id);

            if (student == null)
            {
                return NotFound("Nie znaleziono studenta"); ;
            }
            return Ok(student);
          

        }
/*
       [HttpGet]

       public string GetStudentSort(string orderBy)
       {
           return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
        }
*/
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            // add to database
            // genrating index number

            student.IndexNumber = $"s{new Random().Next(1, 20000)}";

            return Ok(student);
        }

        [HttpPut("{id}")]

        public IActionResult UpdateStudent(int id, Student student)
        {
            return Ok("Aktualizacja zakończona powodzeniem");
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie zakończone");
        }

    }
}