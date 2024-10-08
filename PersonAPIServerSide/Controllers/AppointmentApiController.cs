﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PersonsAPIBusinessLayer;
using PersonsAPIBusinessLayer.People;
using PersonsAPIDataAccessLayer;
using PersonsAPIDataAccessLayer.Doctors;
using PersonsAPIDataAccessLayer.DTOs;
using PersonsAPIDataAccessLayer.Patients;
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

        
        [HttpGet("All/PatienId={patientId}", Name = "GetAllPatientAppointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<AllAppointmentDTO>> GetAllPatientAppointments(int patientId)
        {
            var appointmentList = Appointment.GetAllPatientAppointments(patientId);
            if (appointmentList.Count == 0)
            {
                return NotFound($"No Appointments for patient Id ={patientId} Found");
            }

            return Ok(appointmentList);
        }


        [HttpGet("Find/Id={id}", Name = "GetAppointmentByID")]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<AppointmentDTO> AddNewAppointment(AppointmentDTO newAppointmentDTO)
        {
            if (newAppointmentDTO == null || newAppointmentDTO.PatientId < 1
               || newAppointmentDTO.DoctorId < 1
               )
            {
                return BadRequest("Invalid Appointment data");
            }

            if(newAppointmentDTO.AppointmentStatus < 1 || newAppointmentDTO.AppointmentStatus > 3)
            {
                return BadRequest($"appointment status are 1 -> New, 2 -> Cancelled, 3 -> Completed!");
            }

            if (!PatientsData.IsPatientExist(newAppointmentDTO.PatientId))
            {
                return NotFound($"No patient with Id ={newAppointmentDTO.PatientId}");
            }

            if (!DoctorsData.IsDoctorExist(newAppointmentDTO.DoctorId))
            {
                return NotFound($"No doctor with Id ={newAppointmentDTO.DoctorId}");
            }

            if(Appointment.IsPersonHasActiveAppointmentWithDoctor(newAppointmentDTO.PatientId,newAppointmentDTO.DoctorId))
            {
                return Conflict($"Patient with Id ={newAppointmentDTO.PatientId}, \n has active appointment with Doctor with Id ={newAppointmentDTO.DoctorId}");
            }

            Appointment appointment = new Appointment(new AppointmentDTO(newAppointmentDTO.Id,newAppointmentDTO.PatientId,newAppointmentDTO.DoctorId,newAppointmentDTO.AppointmentDate,newAppointmentDTO.AppointmentStatus,newAppointmentDTO.MedicalRecordId,newAppointmentDTO.PaymentId));

            if(appointment.Save())
            {
                newAppointmentDTO.Id = appointment.Id;
                return Ok(newAppointmentDTO);
            }
            else
                return StatusCode(500, new { message = " Error adding appointment" });

        }


        [HttpPut("Update/Id={id}", Name = "UpdateAppointment")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<AppointmentDTO> UpdateAppointment(int id, AppointmentDTO aDTO)
        {
            if (id<1 || aDTO == null || aDTO.PatientId < 1
               || aDTO.DoctorId < 1
               || aDTO.AppointmentStatus < 1 || aDTO.AppointmentStatus > 4
               )
            {
                return BadRequest("Invalid Appointment data");
            }


            if (aDTO.AppointmentStatus < 1 || aDTO.AppointmentStatus > 4)
            {
                return BadRequest($"appointment status are 1 -> New, 2 -> Cancelled, 3 -> Waiting, 4->Completed!");
            }

            if (!PatientsData.IsPatientExist(aDTO.PatientId))
            {
                return NotFound($"No patient with Id ={aDTO.PatientId}");
            }

            if (!DoctorsData.IsDoctorExist(aDTO.DoctorId))
            {
                return NotFound($"No doctor with Id ={aDTO.DoctorId}");
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


        [HttpDelete("Delete/Id={id}", Name = "DeleteAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


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
                return Conflict($"Appointment with Id[{id}] cannot be deleted\nbecause it is related with tables");
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


        [HttpPut("UpdateStatus/Id={id}/Status={status}", Name = "UpdateAppointmentStatus")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<AppointmentDTO> UpdateAppointmentStatus(int id, byte status)
        {
            if (id <1 )
            {
                return BadRequest("Invalid Appointment Id");
            }

            Appointment appointment = Appointment.Find(id);

            if (appointment == null)
            {
                return NotFound($"No Appointment found with Id {id}");
            }

            if (appointment.AppointmentStatus == 2)
            {
                return Conflict($"Appointment With Id [{id}] is already cancelled!");
            }

            if (appointment.AppointmentStatus == 3)
            {
                return Conflict($"Appointment With Id [{id}] is already waiting!");
            }

            if (appointment.AppointmentStatus == 4)
            {
                return Conflict($"Appointment With Id [{id}] is already completed!");
            }

            switch(status)
            {
                case 2:
                    {
                        if (appointment.MedicalRecordId != null)
                            return Conflict($"Cannot cancel Appointment with Id[{id}]\nbecause it has medical Record relations");

                    }
                    break;
                case 3:
                    {
                        if(appointment.PaymentId == null)
                        {
                            return Conflict("Pay the fees first to be in the waiting Stage!");
                        }
                    }
                    break;
                case 4:
                    {
                        if (!Appointment.IsAppointmentHasRelations(id))
                            return Conflict($"Appointment with Id[{id}] not complete yet");

                    }
                    break;
                default:
                    break;
            }
            

            if ( status < 1 || status > 4)
            {
                return BadRequest($"appointment status are 1 -> New, 2 -> Cancelled, 3 -> Waiting, 4->Completed!");
            }



            if (Appointment.UpdateStatus(id,status))
            {
                //return the DTO not the Full Object
                return Ok(true);
            }
            else
            {
                return StatusCode(500, new { message = " Error Updating Appointment status"});
            }

        }





    }
}
