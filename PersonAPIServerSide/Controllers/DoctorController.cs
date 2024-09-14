using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonsAPIBusinessLayer.Doctors;
using PersonsAPIBusinessLayer.People;
using PersonsAPIDataAccessLayer.Doctors;

namespace PersonAPIServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {

        [HttpGet("All", Name = "GetAllDoctors")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<DoctorsDTO>> GetAllDoctorsData()
        {
            var DoctorsList = Doctors.GetAllDoctors();
            if (DoctorsList.Count == 0)
            {
                return NotFound("No Persons Found");
            }
            return Ok(DoctorsList);

        }

        //Return all Doctors Data by ID


        [HttpGet("Find/{id}", Name = "GetDoctorByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AllDoctorsInfoDTO> GetDoctorByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }

            AllDoctorsInfoDTO doctor = Doctors.FindAllDoctorsByID(id);

            if (doctor == null)
            {
                return NotFound("No Doctors found");
            }

            //DoctorsDTO sDTO = doctor.SDTO;


            return Ok(doctor);
        }

        [HttpPost("Add", Name = "AddDoctor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]


        public ActionResult<DoctorsDTO> AddNewDoctor(DoctorsDTO newDoctorDTO)
        {
            if (newDoctorDTO == null)
            {
                return BadRequest("Invalid Doctors data");
            }

            if (!Persons.IsPersonExists(newDoctorDTO.PersonId))
            {
                return Conflict("Can't Add this Doctor because it does not  found on the Person Table");
            }

            Doctors doctors = new PersonsAPIBusinessLayer.Doctors.Doctors(new DoctorsDTO(newDoctorDTO.Id, newDoctorDTO.PersonId, newDoctorDTO.Specialization));

            doctors.Save();
            newDoctorDTO.Id = doctors.Id;
            return CreatedAtRoute("GetDoctorByID", new { id = newDoctorDTO.Id }, newDoctorDTO);


        }


        [HttpDelete("Delete/{id}", Name = "DeleteDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public ActionResult DeleteDoctor(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }


            if (!Doctors.IsDoctorExists(id))
            {
                return NotFound($"Doctor with ID {id} not exist");
            }

            if (Doctors.CheckDoctorRelations(id))
                return Conflict("Can't delete this Doctor because it has relations in other tables");


            if (Doctors.DeleteDoctor(id))
            {
                return Ok($"Doctor with ID {id} has been deleted");
            }
            else
            {
                return StatusCode(500, new { message = $"Error deleting Doctor with ID {id}" });
            }


        }

        [HttpPut("Update/{id}", Name = "UpdateDoctor")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<DoctorsDTO> UpdateDoctor(int id, DoctorsDTO UpdateDoctorDTO)
        {
            if (id < 1 || UpdateDoctorDTO == null)
            {
                return BadRequest("Invalid Doctor data");
            }
            Doctors doctor = Doctors.Find(id);


            if (doctor == null)
            {
                return NotFound("No Doctors found");
            }

            doctor.Id = id;
            doctor.PersonId = UpdateDoctorDTO.PersonId;
            doctor.Specialization = UpdateDoctorDTO.Specialization;


            if (doctor.Save())
            {
                //return the DTO not the Full Object
                return Ok(doctor.SDTO);
            }
            else
            {
                return StatusCode(500, new { message = " Error Updating Doctor" });
            }




        }


    }
}
