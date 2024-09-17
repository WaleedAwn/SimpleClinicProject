namespace PersonsAPIDataAccessLayer.Payments
{
    public class PaymentDTOWithName
    {
        public PaymentDTOWithName(int id, string paidPatient, DateTime paymentDate, string paymentMethod, decimal amountPaid, string? additionalNotes)
        {
            Id = id;
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            AmountPaid = amountPaid;
            AdditionalNotes = additionalNotes;
            PaidPatient = paidPatient;
        }

        public int Id { get; set; }
        public string PaidPatient { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public string? AdditionalNotes { get; set; }

    }
}
