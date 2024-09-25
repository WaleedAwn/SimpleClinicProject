using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
            public async Task<ActionResult<IEnumerable<MedicalRecordsDTO>>> GetAllMedicalRecords()
            {
                var MedicalRecordsList = await MedicalRecords.GetAllMedicalRecords();
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
            public async Task<ActionResult<MedicalRecordsDTO>> GetMedicalRecordsByID(int id)
            {
                if (id < 1)
                {
                    return BadRequest("bad Request");
                }

                MedicalRecords MedicalRecords = await MedicalRecords.Find(id);

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

        public async Task<ActionResult<int>> AddNewMedicalRecords(MedicalRecordsDTO newMedicalRecordsDTO)
        {
            if (newMedicalRecordsDTO == null)
            {
                return BadRequest("Invalid MedicalRecords data");
            }

            // إنشاء نموذج MedicalRecords باستخدام بيانات المدخلات
            var medicalRecord = new PersonsAPIBusinessLayer.MedicalRecords.MedicalRecords(new MedicalRecordsDTO(
                0, // يمكن أن تكون قيمة جديدة، حيث يتم تعيين ID لاحقًا بعد الإضافة
                newMedicalRecordsDTO.VisitDescription ?? null,
                newMedicalRecordsDTO.Diagnosis ?? null,
                newMedicalRecordsDTO.AdditionalNotes ?? null
            ));

            // حفظ السجل
            await medicalRecord.SaveAsync();
            var newMedicalRecordID = medicalRecord.SDTO.MedicalRecordID;

                // تأكد من أن SaveAsync ترجع ID السجل الجديد
            // إرجاع ID السجل الجديد كاستجابة
            return CreatedAtRoute("GetMedicalRecordsByID", new { id = newMedicalRecordID }, newMedicalRecordID);
        }



        [HttpPut("Update/{id}", Name = "UpdateMedicalRecords")]

            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<MedicalRecordsDTO>> UpdateMedicalRecords(int id, MedicalRecordsDTO UpdateMedicalRecordsDTO)
            {
                if (id < 1 || UpdateMedicalRecordsDTO == null )
                {
                    return BadRequest("Invalid MedicalRecords data");
                }
                MedicalRecords MedicalRecords = await MedicalRecords.Find(id);


                if (MedicalRecords == null)
                {
                    return NotFound("No MedicalRecords found");
                }

                MedicalRecords.MedicalRecordID = id;
                MedicalRecords.VisitDescription = UpdateMedicalRecordsDTO.VisitDescription;
                MedicalRecords.Diagnosis = UpdateMedicalRecordsDTO.Diagnosis;
                MedicalRecords.AdditionalNotes = UpdateMedicalRecordsDTO.AdditionalNotes;

                if (await MedicalRecords.SaveAsync())
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

            public async Task<ActionResult> DeleteMedicalRecords(int id)
            {
                if (id < 1)
                {
                    return BadRequest("bad Request");
                }


                if (!(await MedicalRecords.IsMedicalRecordExists(id)))
                {
                    return NotFound($"MedicalRecords with ID {id} not exist");
                }



                if (await MedicalRecords.DeleteMedicalRecord(id))
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
