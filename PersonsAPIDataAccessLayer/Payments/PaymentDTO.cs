using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Payments
{
    public class PaymentDTO
    {
        public PaymentDTO(int id, DateTime paymentDate, int paymentMethodId, decimal amountPaid, string? additionalNotes)
        {
            Id = id;
            PaymentDate = paymentDate;
            PaymentMethodId = paymentMethodId;
            AmountPaid = amountPaid;
            AdditionalNotes = additionalNotes;
        }

        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal AmountPaid   { get; set; }
        public string? AdditionalNotes { get; set; }
        
    }
}
