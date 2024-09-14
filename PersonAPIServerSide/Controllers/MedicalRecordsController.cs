using Microsoft.AspNetCore.Mvc;
using PersonsAPIBusinessLayer.MedicalRecords;
using PersonsAPIDataAccessLayer.MedicalRecords;

namespace MedicalRecordsAPIServerSide.Controllers
{
   [Route("api/MedicalRecords")]

        [ApiController]
        public class MedicalRecordsController : ControllerBase
        {


            [HttpGet("All", Name = "GetAllMedicalRecords")]

            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public ActionResult<IEnumerable<MedicalRecordsDTO>> GetAllMedicalRecords()
            {
                var MedicalRecordsList = MedicalRecords.GetAllMedicalRecords();
                if (MedicalRecordsList.Count == 0)
                {
                    return NotFound("No MedicalRecords Found");
                }
                return Ok(MedicalRecordsList);

            }

            [HttpGet("Find/{id}", Name = "GetMedicalRecordsByID")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public ActionResult<MedicalRecordsDTO> GetMedicalRecordsByID(int id)
            {
                if (id < 1)
                {
                    return BadRequest("bad Request");
                }

                MedicalRecords MedicalRecords = MedicalRecords.Find(id);

                if (MedicalRecords == null)
                {
                    return NotFound("No Patient found");
                }

                MedicalRecordsDTO sDTO = MedicalRecords.SDTO;


                return Ok(sDTO);
            }



            [HttpPost("Add", Name = "AddMedicalRecords")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]

            public ActionResult<MedicalRecordsDTO> AddNewMedicalRecords(MedicalRecordsDTO newMedicalRecordsDTO)
            {
                if (newMedicalRecordsDTO == null || string.IsNullOrEmpty(newMedicalRecordsDTO.VisitDescription)
                   || string.IsNullOrEmpty(newMedicalRecordsDTO.Diagnosis)
                   || string.IsNullOrEmpty(newMedicalRecordsDTO.AdditionalNotes))
                {
                    return BadRequest("Invalid MedicalRecords data");
                }

                MedicalRecords MedicalRecords = new PersonsAPIBusinessLayer.MedicalRecords.MedicalRecords(new MedicalRecordsDTO(newMedicalRecordsDTO.MedicalRecordID,
                    newMedicalRecordsDTO.VisitDescription, newMedicalRecordsDTO.Diagnosis, newMedicalRecordsDTO.AdditionalNotes));

                MedicalRecords.Save();
                newMedicalRecordsDTO.MedicalRecordID = MedicalRecords.MedicalRecordID;
                return CreatedAtRoute("GetMedicalRecordsByID", new { id = newMedicalRecordsDTO.MedicalRecordID }, newMedicalRecordsDTO);


            }



            [HttpPut("Update/{id}", Name = "UpdateMedicalRecords")]

            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public ActionResult<MedicalRecordsDTO> UpdateMedicalRecords(int id, MedicalRecordsDTO UpdateMedicalRecordsDTO)
            {
                if (id < 1 || UpdateMedicalRecordsDTO == null || string.IsNullOrEmpty(UpdateMedicalRecordsDTO.VisitDescription) || string.IsNullOrEmpty(UpdateMedicalRecordsDTO.Diagnosis)
                     || string.IsNullOrEmpty(UpdateMedicalRecordsDTO.AdditionalNotes))
                {
                    return BadRequest("Invalid MedicalRecords data");
                }
                MedicalRecords MedicalRecords = MedicalRecords.Find(id);


                if (MedicalRecords == null)
                {
                    return NotFound("No MedicalRecords found");
                }

                MedicalRecords.MedicalRecordID = id;
                MedicalRecords.VisitDescription = UpdateMedicalRecordsDTO.VisitDescription;
                MedicalRecords.Diagnosis = UpdateMedicalRecordsDTO.Diagnosis;
                MedicalRecords.AdditionalNotes = UpdateMedicalRecordsDTO.AdditionalNotes;

                if (MedicalRecords.Save())
                {
                    //return the DTO not the Full Object
                    return Ok(MedicalRecords.SDTO);
                }
                else
                {
                    return StatusCode(500, new { message = " Error Updating MedicalRecords" });
                }




            }



            [HttpDelete("Delete/{id}", Name = "DeleteMedicalRecords")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status409Conflict)]

            public ActionResult DeleteMedicalRecords(int id)
            {
                if (id < 1)
                {
                    return BadRequest("bad Request");
                }


                if (!MedicalRecords.IsMedicalRecordExists(id))
                {
                    return NotFound($"MedicalRecords with ID {id} not exist");
                }



                if (MedicalRecords.DeleteMedicalRecord(id))
                {
                    return Ok($"MedicalRecords with ID {id} has been deleted");
                }
                else
                {
                    return StatusCode(500, new { message = $"Error deleting MedicalRecords with ID {id}" });
                }

            }
        }
}
