using cw3.Models;
using cw5.DTOs.Request;
using cw5.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace cw5.Services
{
    public interface IStudentDbService
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);

        public Student GetStudent(string id);
        public Claim[] Login(LoginRequestDTO request);

    }
}
