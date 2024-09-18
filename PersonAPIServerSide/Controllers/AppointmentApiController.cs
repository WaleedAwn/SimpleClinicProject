using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonsAPIBusinessLayer;
using PersonsAPIBusinessLayer.People;
using PersonsAPIDataAccessLayer.DTOs;
using PersonsAPIDataAccessLayer.People;

namespace PersonAPIServerSide.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/AppointmentApi")]
    [ApiController]
    public class AppointmentApiController : ControllerBase
    {

        [HttpGet("All", Name = "GetAllAppointments")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<AllAppointmentDTO>> GetAllAppointments()
        {
            var appointmentList = Appointment.GetAllAppointments();
            if (appointmentList.Count == 0)
            {
                return NotFound("No Appointments Found");
            }
            return Ok(appointmentList);

        }


        [HttpGet("Find/Id/{id}", Name = "GetAppointmentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AllAppointmentDTO> GetAppointmentByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }

            Appointment appointment = Appointment.Find(id);

            if (appointment == null)
            {
                return NotFound($"No Appointment found with Id {id}");
            }

            AllAppointmentDTO aDTO = appointment.AllADTO;


            return Ok(aDTO);
        }


        [HttpPost("Add", Name = "AddAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<AppointmentDTO> AddNewAppointment(AppointmentDTO newAppointmentDTO)
        {
            if (newAppointmentDTO == null || newAppointmentDTO.PatientId < 1
               || newAppointmentDTO.DoctorId < 1
               || newAppointmentDTO.AppointmentStatus < 1 || newAppointmentDTO.AppointmentStatus > 3
               )
            {
                return BadRequest("Invalid Appointment data");
            }

            Appointment appointment = new Appointment(new AppointmentDTO(newAppointmentDTO.Id,newAppointmentDTO.PatientId,newAppointmentDTO.DoctorId,newAppointmentDTO.AppointmentDate,newAppointmentDTO.AppointmentStatus,newAppointmentDTO.MedicalRecordId,newAppointmentDTO.PaymentId));

            if(appointment.Save())
                newAppointmentDTO.Id = appointment.Id;
            else
                return StatusCode(500, new { message = " Error adding appointment" });


            return Ok(newAppointmentDTO);

        }


        [HttpPut("Update/{id}", Name = "UpdateAppointment")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AppointmentDTO> UpdateAppointment(int id, AppointmentDTO aDTO)
        {
            if (id<1 || aDTO == null || aDTO.PatientId < 1
               || aDTO.DoctorId < 1
               || aDTO.AppointmentStatus < 1 || aDTO.AppointmentStatus > 3
               )
            {
                return BadRequest("Invalid Appointment data");
            }

            Appointment appointment = Appointment.Find(id);


            if (appointment == null)
            {
                return NotFound($"No Appointment found with Id {id}");
            }

            appointment.Id = id;
            
            appointment.PatientId = aDTO.PatientId;
            appointment.DoctorId = aDTO.DoctorId;
            appointment.AppointmentDate = aDTO.AppointmentDate;
            appointment.AppointmentStatus = aDTO.AppointmentStatus;
            appointment.MedicalRecordId = aDTO.MedicalRecordId;
            appointment.PaymentId = aDTO.PaymentId;

            if (appointment.Save())
            {

                //return the DTO not the Full Object
                return Ok(appointment.aDTO);
            }
            else
            {
                return StatusCode(500, new { message = " Error Updating Appointment" });
            }




        }


        [HttpDelete("Delete/{id}", Name = "DeleteAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public ActionResult DeleteAppointment(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted Id {id}");
            }


            if (!Appointment.IsAppointmentExist(id))
            {
                return NotFound($"Appointment with ID {id} not exist");
            }

            if(Appointment.IsAppointmentHasRelations(id))
            {
                return BadRequest($"Appointment with Id[{id}] cannot be deleted\nbecause it is related with tables");
            }


            if (Appointment.Delete(id))
            {
                return Ok($"Appointment with ID {id} has been deleted");
            }
            else
            {
                return StatusCode(500, new { message = $"Error deleting appointment with ID {id}" });
            }


        }




    }
}
