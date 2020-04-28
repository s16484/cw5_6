using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private  IDbService _dbService;
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16484;Integrated Security=True";
       
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]

        public IActionResult GetStudent(string orderBy)
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
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                    st.StudyName = dr["Name"].ToString();
                    st.Semester = Int16.Parse(dr["Semester"].ToString());
                    list.Add(st);
                }

            }
            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentDetails(string id)
        {
            using (var con = new SqlConnection(ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = con;
                command.CommandText = "select IndexNumber, FirstName, LastName, BirthDate, Name, Semester, s.IdEnrollment "
                   + "from Student s, Enrollment e, Studies studies "
                   + "where s.IdEnrollment = e.IdEnrollment AND e.IdStudy = studies.IdStudy "
                   +"AND indexnumber=@id";
                command.Parameters.AddWithValue("id", id);

                // SQL injetion
                // localhost:65055/api/students/a';DROP TABLE students;--

                con.Open();
                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                    st.StudyName = dr["Name"].ToString();
                    st.Semester = Int16.Parse(dr["Semester"].ToString());
                    return Ok(st);
                }
            }
            return NotFound();

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