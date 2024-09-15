using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using PersonsAPIBusinessLayer.Prescriptions;
using PersonsAPIDataAccessLayer.Prescriptions;

namespace PrescriptionAPIServerSide.Controllers
{
   
        //[Route("api/[controller]")]
        [Route("api/Prescriptions")]

        [ApiController]
        public class PrescriptionController : ControllerBase
        {


            [HttpGet("All", Name = "GetAllPrescriptions")]

            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public ActionResult<IEnumerable<PrescriptionsDTO>> GetAllPrescriptions()
            {
                var PrescriptionList = Prescription.GetAllPrescription();
                if (PrescriptionList.Count == 0)
                {
                    return NotFound("No Prescriptions Found");
                }
                return Ok(PrescriptionList);

            }

            [HttpGet("Find/{id}", Name = "GetPrescriptionByID")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public ActionResult<PrescriptionsDTO> GetPrescriptionByID(int id)
            {
                if (id < 1)
                {
                    return BadRequest("bad Request");
                }

                Prescription Prescription = Prescription.Find(id);

                if (Prescription == null)
                {
                    return NotFound("No Patient found");
                }

                PrescriptionsDTO sDTO = Prescription.SDTO;


                return Ok(sDTO);
            }



            [HttpPost("Add", Name = "AddPrescription")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]

            public ActionResult<PrescriptionsDTO> AddNewPrescription(PrescriptionsDTO newPrescriptionDTO)
            {
                if (newPrescriptionDTO == null || newPrescriptionDTO.MedicalRecordID==null || string.IsNullOrEmpty(newPrescriptionDTO.MedicationName)
                   || string.IsNullOrEmpty(newPrescriptionDTO.Dosage) || string.IsNullOrEmpty(newPrescriptionDTO.Frequency) || newPrescriptionDTO.StartDate==null||newPrescriptionDTO.EndDate == null)
                {
                    return BadRequest("Invalid Prescription data");
                }

                Prescription Prescription = new PersonsAPIBusinessLayer.Prescriptions.Prescription(new PrescriptionsDTO(newPrescriptionDTO.PrescriptionID,
                    newPrescriptionDTO.MedicalRecordID, newPrescriptionDTO.MedicationName, newPrescriptionDTO.Dosage, newPrescriptionDTO.Frequency, newPrescriptionDTO.StartDate
                    , newPrescriptionDTO.EndDate, newPrescriptionDTO.SpecialInstructions));

                Prescription.Save();
                newPrescriptionDTO.PrescriptionID = Prescription.PrescriptionID;
                return CreatedAtRoute("GetPrescriptionByID", new { id = newPrescriptionDTO.PrescriptionID }, newPrescriptionDTO);


            }



            [HttpPut("Update/{id}", Name = "UpdatePrescription")]

            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public ActionResult<PrescriptionsDTO> UpdatePrescription(int id, PrescriptionsDTO UpdatePrescriptionDTO)
            {
                if (id < 1 || UpdatePrescriptionDTO == null ||   UpdatePrescriptionDTO.MedicalRecordID == null || string.IsNullOrEmpty(UpdatePrescriptionDTO.MedicationName)
                   || string.IsNullOrEmpty(UpdatePrescriptionDTO.Dosage) || string.IsNullOrEmpty(UpdatePrescriptionDTO.Frequency) || UpdatePrescriptionDTO.StartDate == null || UpdatePrescriptionDTO.EndDate == null)
                {
                    return BadRequest("Invalid Prescription data");
                }
                Prescription Prescriptions = Prescription.Find(id);


                if (Prescriptions == null)
                {
                    return NotFound("No Prescriptions found");
                }

                Prescriptions.PrescriptionID = id;
                Prescriptions.MedicalRecordID = UpdatePrescriptionDTO.MedicalRecordID;
                Prescriptions.MedicationName = UpdatePrescriptionDTO.MedicationName;
                Prescriptions.Dosage = UpdatePrescriptionDTO.Dosage;
                Prescriptions.Frequency = UpdatePrescriptionDTO.Frequency;
                Prescriptions.StartDate = UpdatePrescriptionDTO.StartDate;
                Prescriptions.EndDate = UpdatePrescriptionDTO.EndDate;
                Prescriptions.SpecialInstructions = UpdatePrescriptionDTO.SpecialInstructions;

                if (Prescriptions.Save())
                {
                    //return the DTO not the Full Object
                    return Ok(Prescriptions.SDTO);
                }
                else
                {
                    return StatusCode(500, new { message = " Error Updating Prescription" });
                }




            }



            [HttpDelete("Delete/{id}", Name = "DeletePrescription")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status409Conflict)]

            public ActionResult DeletePrescription(int id)
            {
                if (id < 1)
                {
                    return BadRequest("bad Request");
                }


                if (!Prescription.IsPrescriptionExists(id))
                {
                    return NotFound($"Prescription with ID {id} not exist");
                }

                

                if (Prescription.DeletePrescription(id))
                {
                    return Ok($"Prescription with ID {id} has been deleted");
                }
                else
                {
                    return StatusCode(500, new { message = $"Error deleting Prescription with ID {id}" });
                }


            }





        }
    }
