using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonsAPIBusinessLayer.Payments;
using PersonsAPIDataAccessLayer.Payments;
using System.Diagnostics;


namespace PersonAPIServerSide.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/PaymentApi")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        [HttpGet("All", Name = "GetAllPayments")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<PaymentDTOWithName>> GetAllPayments()
        {
            List<PaymentDTOWithName> paymentList = Payment.GetAllPayments();

            if (paymentList.Count == 0)
            {
                return NotFound("No payments Found!");
            }

            return Ok(paymentList);
        }


        [HttpGet("Find/Id={id}", Name = "GetPaymentById")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PaymentDTOWithName> GetPaymentById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            Payment payment = Payment.Find(id);

            if (payment == null)
                return NotFound($"No payment found with Id {id}");

            PaymentDTOWithName pDTO = payment.PDTOWithName;
            return Ok(pDTO);

        }

        [HttpPost("Add", Name = "AddPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PaymentDTO> AddPayment(PaymentDTO newPaymentDTO)
        {
            //we validate the data here
            if (newPaymentDTO == null || string.IsNullOrEmpty(newPaymentDTO.PaymentMethod) || newPaymentDTO.AmountPaid < 0)
            {
                return BadRequest("Invalid Payment data.");
            }

            
            Payment payment = new Payment(new PaymentDTO(newPaymentDTO.Id,newPaymentDTO.PaymentDate,newPaymentDTO.PaymentMethod,newPaymentDTO.AmountPaid,newPaymentDTO.AdditionalNotes));

            newPaymentDTO.Id = payment.Id;

            if (payment.Save())
                return Ok(payment.PDTO);
            else
                return StatusCode(500, new { message = "Erorr adding payment" });


            //we return the DTO only not the full payment object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            //return Ok();
        }


        [HttpPut("Update/Id={id}", Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<PaymentDTO> UpdatePayment(int id, PaymentDTO updatePayment)
        {
            if (id<1 || updatePayment == null || string.IsNullOrEmpty(updatePayment.PaymentMethod) || updatePayment.AmountPaid < 0)
            {
                return BadRequest("Invalid Payment data.");
            }

            updatePayment.Id = id;

            if (!Payment.IsPaymentExists(id))
            {
                return NotFound($"payment with Id={id} is not exists");
            }

            Payment payment = Payment.Find(id);


            if (payment == null)
            {
                return NotFound($"payment with ID {id} not found.");
            }


            payment.PaymentDate = updatePayment.PaymentDate;
            payment.PaymentMethod = updatePayment.PaymentMethod;
            payment.AmountPaid = updatePayment.AmountPaid;
            payment.AdditionalNotes = updatePayment.AdditionalNotes;
            
            

            if (payment.Save())
                //we return the DTO not the full student object.
                return Ok(payment.PDTO);
            else
                return StatusCode(500, new { message = "Erorr updating payment" });

        }

        //here we use HttpDelete method
        [HttpDelete("Delete/Id={id}", Name = "DeletePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletePayment(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            if (!Payment.IsPaymentExists(id))
            {
                return NotFound($"Payment with id {id} not exists");
            }

            if(Payment.IsPaymentHasRelations(id))
            {
                return Conflict($"Payment cannot be deleted\nBecause it has relations with other tables!");
            }

            if (Payment.Delete(id))
                return Ok($"Payment with ID {id} has been deleted.");
            else
                return NotFound($"Payment with ID {id} not found. no rows deleted!");
        }



    }
}
