using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonsAPIBusinessLayer.Patients;
using PersonsAPIBusinessLayer.People;
using PersonsAPIDataAccessLayer.Patients;

namespace PersonAPIServerSide.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Patient")]

    [ApiController]
    public class PatientController : ControllerBase
    {


        [HttpGet("All", Name = "GetAllPatients")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PatientAllInfoDTO>> GetAllPatients()
        {
            var PatientsList = Patients.GetAllPatients();
            if (PatientsList.Count == 0)
            {
                return NotFound("No Persons Found");
            }
            return Ok(PatientsList);

        }


        [HttpGet("Find/{id}", Name = "GetPatientByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //Return all Patient Datat by ID
        public ActionResult<PatientAllInfoDTO> GetPatientByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }

            PatientAllInfoDTO patient = Patients.FindAllPatientsDataByID(id);

            if (patient == null)
            {
                return NotFound("No Patients found");
            }

            //PatientsDTO sDTO = patient.SDTO;


            return Ok(patient);
        }

        [HttpPost("Add", Name = "AddPatient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]


        public ActionResult<PatientsDTO> AddNewPatient(PatientsDTO newPatientDTO)
        {
            if (newPatientDTO == null)
            {
                return BadRequest("Invalid Patient data");
            }

            if (!Persons.IsPersonExists(newPatientDTO.PersonId))
            {
                return Conflict("Can't Add this Patient because it does not  found on the Person Table");
            }

            Patients patient = new PersonsAPIBusinessLayer.Patients.Patients(new PatientsDTO(newPatientDTO.Id, newPatientDTO.PersonId));

            patient.Save();
            newPatientDTO.Id = patient.Id;
            return CreatedAtRoute("GetPatientByID", new { id = newPatientDTO.Id }, newPatientDTO);


        }

        [HttpDelete("Delete/{id}", Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public ActionResult DeletePatient(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }


            if (!Patients.IsPatientExists(id))
            {
                return NotFound($"Patient with ID {id} not exist");
            }

            if (Patients.CheckPatientRelations(id))
                return Conflict("Can't delete this Patient because it has relations in other tables");


            if (Patients.DeletePatient(id))
            {
                return Ok($"Patient with ID {id} has been deleted");
            }
            else
            {
                return StatusCode(500, new { message = $"Error deleting Patient with ID {id}" });
            }


        }

        [HttpPut("Update/{id}", Name = "UpdatePatient")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PatientsDTO> UpdatePatient(int id, PatientsDTO UpdatePatrientDTO)
        {
            if (id < 1 || UpdatePatrientDTO == null)
            {
                return BadRequest("Invalid patient data");
            }
            Patients patient = Patients.Find(id);


            if (patient == null)
            {
                return NotFound("No Patient found");
            }

            //patient.Id = UpdatePatrientDTO.Id; // this to allow for user to enter the ID Again  
            patient.Id = id; // this take the Entered ID 
            patient.PersonId = UpdatePatrientDTO.PersonId;

            if (patient.Save())
            {
                //return the DTO not the Full Object
                return Ok(patient.SDTO);
            }
            else
            {
                return StatusCode(500, new { message = " Error Updating Patient" });
            }




        }



    }
}
