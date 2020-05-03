using cw3.Models;
using cw5.DTOs.Request;
using cw5.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw5.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s16484;Integrated Security=True";

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
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
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE name=@name";

                    com.Parameters.AddWithValue("name", request.StudyName);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        throw new ArgumentException("Podane studia nie istnieja");
                    }
                    idstudies = (int)dr["IdStudy"];

                    dr.Close();

                    int nextIdEnrollment = 1;

                    com.CommandText = "SELECT MAX(ISNULL(IdEnrollment,0)) FROM Enrollment";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        nextIdEnrollment = (int)dr[0] + 1;
                    }
                    dr.Close();

                    com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@idstudies AND semester=1";
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
                        throw new ArgumentException("Student już istnieje w bazie!");

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
                        return response;
                    }
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    throw new ArgumentException(exc.Message);
                }

            }
        }

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            PromoteStudentsResponse response;

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                com.Transaction = tran;
                try
                {
                    com.CommandText = "SELECT IdStudy FROM Studies WHERE name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        throw new ArgumentException("Podane studia nie istnieja");

                    }
                    dr.Close();

                    com.CommandText = "SELECT IdEnrollment FROM Enrollment, Studies " +
                                        "WHERE Enrollment.IdStudy = Studies.IdStudy " +
                                        "AND Studies.Name = @name " +
                                        "AND Enrollment.Semester = @semester ";
                    com.Parameters.AddWithValue("semester", request.Semester);

                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        throw new ArgumentException("Wpis z podanym semestrem nie istnieje w bazie");
                    }
                    dr.Close();
                    com.Parameters.Clear();

                    com.CommandText = "EXEC [dbo].[PromoteStudents] @studies, @semester";
                    com.Parameters.AddWithValue("studies", request.Studies);
                    com.Parameters.AddWithValue("semester", request.Semester);
                    com.ExecuteNonQuery();

                    com.CommandText = "SELECT * FROM enrollment " +
                                        "WHERE idStudy = (SELECT idStudy FROM Studies WHERE name = @studiesName) " +
                                        "AND semester = @newSemester ";
                    com.Parameters.AddWithValue("studiesName", request.Studies);
                    com.Parameters.AddWithValue("newSemester", request.Semester + 1);

                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        response = new PromoteStudentsResponse()
                        {
                            IdEnrollment = (int)dr["IdEnrollment"],
                            IdStudy = (int)dr["IdStudy"],
                            Semester = (int)dr["Semester"],
                            StartDate = DateTime.Now
                        };

                        dr.Close();
                        tran.Commit();
                        return response;
                    }
                    else
                        throw new ArgumentException("Problem przy tworzeniu odpowiedzi");
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    throw new ArgumentException(exc.Message);
                }

            }
        }

        public Student GetStudent(string index)
        {
            using (var con = new SqlConnection(ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = con;
                command.CommandText = "select IndexNumber, FirstName, LastName, BirthDate, Name, Semester, s.IdEnrollment "
                   + "from Student s, Enrollment e, Studies studies "
                   + "where s.IdEnrollment = e.IdEnrollment AND e.IdStudy = studies.IdStudy "
                   + "AND indexnumber=@id";
                command.Parameters.AddWithValue("id", index);

                con.Open();
                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    Student st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString()).Date;
                    st.IdEnrollment = Int32.Parse(dr["IdEnrollment"].ToString());
                    st.StudyName = dr["Name"].ToString();
                    st.Semester = Int16.Parse(dr["Semester"].ToString());
                    return st;
                }
            }
            return null;


        }

    }
}
