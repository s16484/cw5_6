using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.Models;
using cw5.DTOs.Request;
using cw5.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16484;Integrated Security=True";


        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            
            var st = new Student(); 
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.BirthDate = request.BirthDate;
            st.StudyName = request.StudyName;

            DateTime StartDate = DateTime.Now;

            int idEnrollment;
            int idstudies;
            EnrollStudentResponse response;


            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;
                try
                {
                    com.CommandText = "select IdStudy from Studies where name=@name";

                    com.Parameters.AddWithValue("name", request.StudyName);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Podane studia nie istnieja");
                    }
                    idstudies = (int)dr["IdStudy"];

                    dr.Close();

                    int nextIdEnrollment=1;

                    com.CommandText = "select max(ISNULL(IdEnrollment,0)) from Enrollment";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        nextIdEnrollment = (int)dr[0] + 1;
                    }
                    dr.Close();

                    com.CommandText = "select IdEnrollment from Enrollment where IdStudy=@idstudies and semester=1";
                    com.Parameters.AddWithValue("idstudies", idstudies);

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@IdEnrollment, @semester, @idstudy, @startdate)";
                        com.Parameters.AddWithValue("IdEnrollment", nextIdEnrollment);
                        com.Parameters.AddWithValue("semester", 1);
                        com.Parameters.AddWithValue("idstudy", idstudies);
                        com.Parameters.AddWithValue("startdate", StartDate);

                        com.Transaction = tran;
                        com.ExecuteNonQuery();
                  
                         idEnrollment = nextIdEnrollment;
                    }

                    else
                    {
                       idEnrollment = (int)dr["IdEnrollment"];
                     }

                    dr.Close();
                    
                    com.CommandText = "select IndexNumber from Student where IndexNumber = @index";
                    com.Parameters.AddWithValue("index", request.IndexNumber);
                    com.Transaction = tran;
                    
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return BadRequest("Student już istnieje w bazie!");

                    }
                    else
                    {
                        dr.Close();
                        com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@idStud, @fname, @lname, @bdate, @idEnroll)";
                        com.Parameters.AddWithValue("idStud", request.IndexNumber);
                        com.Parameters.AddWithValue("fname", request.FirstName);
                        com.Parameters.AddWithValue("lname", request.LastName);
                        com.Parameters.AddWithValue("bdate", request.BirthDate);
                        com.Parameters.AddWithValue("idEnroll", idEnrollment);
                        com.Transaction = tran;
                        com.ExecuteNonQuery();
                        tran.Commit();

                        response = new EnrollStudentResponse()
                        {
                            LastName = request.LastName,
                            IdEnrollment = idEnrollment,
                            IdStudy = idstudies,
                            Semester = 1,
                            StartDate = DateTime.Now
                        };

                        return Created("Dodano studenta: ", response);

                    }

                }

                catch (SqlException exc)
                {
                    tran.Rollback();
                    return BadRequest(exc);
                     
                }

            }

        }
    }
}