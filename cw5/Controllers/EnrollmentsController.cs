using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.Models;
using cw5.DTOs.Request;
using cw5.DTOs.Responses;
using cw5.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _dbservice;

        public EnrollmentsController(IStudentDbService dbservice)
        {
            _dbservice = dbservice;
        }


        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response;
            try
            {
                response = _dbservice.EnrollStudent(request);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("Dodano studenta", response);

        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {
            PromoteStudentsResponse response;
            try
            {
                response = _dbservice.PromoteStudents(request);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("Promocja na kolejny semestr nadana", response);

        }

    }






}