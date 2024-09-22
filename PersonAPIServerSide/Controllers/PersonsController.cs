using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using PersonsAPIBusinessLayer.People;
using PersonsAPIDataAccessLayer.People;
using PersonsAPIBusinessLayer.Patients;
using PersonsAPIDataAccessLayer.Patients;
using PersonsAPIDataAccessLayer.Doctors;
using PersonsAPIBusinessLayer.Doctors;


namespace PersonAPIServerSide.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Persons")]
    [ApiController]
    public class PersonsController : ControllerBase
    {

        
        [HttpGet("All", Name = "GetAllPersons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PersonsDTO>> GetAllPersons()
        {
            var PersonList = Persons.GetAllPersons();
            if (PersonList.Count == 0)
            {
                return NotFound("No Persons Found");
            }
            return Ok(PersonList);

        }

        [HttpGet("Find/{id}", Name = "GetPersonByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PersonsDTO> GetPersonByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }

            Persons person = Persons.Find(id);

            if (person == null)
            {
                return NotFound("No Patient found");
            }

            PersonsDTO sDTO = person.SDTO;


            return Ok(sDTO);
        }



        [HttpPost("Add",Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PersonsDTO> AddNewPerson(PersonsDTO newPersonDTO)
        {
            if (newPersonDTO == null || string.IsNullOrEmpty(newPersonDTO.PersonName)
               || string.IsNullOrEmpty(newPersonDTO.Gender)
               || string.IsNullOrEmpty(newPersonDTO.PhoneNumber) || string.IsNullOrEmpty(newPersonDTO.Email)
               || string.IsNullOrEmpty(newPersonDTO.Address))
            {
                return BadRequest("Invalid Person data");
            }

            Persons person = new PersonsAPIBusinessLayer.People.Persons(new PersonsDTO(newPersonDTO.Id,
                newPersonDTO.PersonName, newPersonDTO.DateOfBirth, newPersonDTO.Gender, newPersonDTO.PhoneNumber
                , newPersonDTO.Email, newPersonDTO.Address));

            person.Save();
            newPersonDTO.Id = person.PersonID;
            return CreatedAtRoute("GetPersonByID", new { id = newPersonDTO.Id }, newPersonDTO);


        }



        [HttpPut("Update/{id}", Name = "UpdatePerson")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PersonsDTO> UpdatePerson(int id, PersonsDTO UpdatePersonDTO)
        {
            if (id < 1 || UpdatePersonDTO == null || string.IsNullOrEmpty(UpdatePersonDTO.PersonName) || string.IsNullOrEmpty(UpdatePersonDTO.Gender)
                 || string.IsNullOrEmpty(UpdatePersonDTO.PhoneNumber) || string.IsNullOrEmpty(UpdatePersonDTO.Email) || string.IsNullOrEmpty(UpdatePersonDTO.Address))
            {
                return BadRequest("Invalid Person data");
            }
            Persons persons = Persons.Find(id);


            if (persons == null)
            {
                return NotFound("No Persons found");
            }

            persons.PersonID = id;
            persons.PersonName = UpdatePersonDTO.PersonName;
            persons.DateOfBirth = UpdatePersonDTO.DateOfBirth;
            persons.Gender = UpdatePersonDTO.Gender;
            persons.PhoneNumber = UpdatePersonDTO.PhoneNumber;
            persons.Email = UpdatePersonDTO.Email;
            persons.Address = UpdatePersonDTO.Address;

            if (persons.Save())
            {
                //return the DTO not the Full Object
                return Ok(persons.SDTO);
            }
            else
            {
                return StatusCode(500, new { message = " Error Updating Person" });
            }




        }


        [HttpDelete("Delete/{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public ActionResult DeletePerson(int id)
        {
            if (id < 1)
            {
                return BadRequest("bad Request");
            }


            if (!Persons.IsPersonExists(id))
            {
                return NotFound($"Person with ID {id} not exist");
            }

            if (Persons.CheckPersonRelations(id))
                return Conflict("Can't delete this Person because it has relations in other tables");


            if (Persons.DeletePerson(id))
            {
                return Ok($"Person with ID {id} has been deleted");
            }
            else
            {
                return StatusCode(500, new { message = $"Error deleting person with ID {id}" });
            }


        }

    



    }




}




