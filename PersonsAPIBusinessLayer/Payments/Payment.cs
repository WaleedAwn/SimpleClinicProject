using PersonsAPIBusinessLayer.Payments;
using PersonsAPIDataAccessLayer.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIBusinessLayer.Payments
{
    public class Payment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public PaymentDTO PDTO
        {
            get
            {
                return new PaymentDTO(this.Id, this.PaymentDate, this.PaymentMethodId, this.AmountPaid, this.AdditionalNotes);
            }
        }

        public PaymentDTOWithName PDTOWithName
        {
            get
            {
                return new PaymentDTOWithName(this.Id,this.PaidPatient, this.PaymentDate, this.PaymentMethodId, this.PaymentMethod, this.AmountPaid, this.AdditionalNotes);
            }
        }

        public int Id { get; set; }
        public string PaidPatient { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }
        public string? AdditionalNotes { get; set; }

        public Payment(PaymentDTO pDTO)
        {
            this.Id = pDTO.Id;
            this.PaymentDate  = pDTO.PaymentDate;
            this.PaymentMethodId = pDTO.PaymentMethodId;
            this.AmountPaid = pDTO.AmountPaid;
            this.AdditionalNotes = pDTO.AdditionalNotes;
            Mode = enMode.AddNew;
        }

        private Payment(PaymentDTOWithName pDTO)
        {
            this.Id = pDTO.Id;
            this.PaymentDate = pDTO.PaymentDate;
            this.PaidPatient = pDTO.PaidPatient;
            this.PaymentMethod = pDTO.PaymentMethod;
            this.PaymentMethodId = PDTO.PaymentMethodId;
            this.AmountPaid = pDTO.AmountPaid;
            this.AdditionalNotes = pDTO.AdditionalNotes;
            Mode = enMode.Update;
        }

        private bool _AddNewPayment()
        {
            this.Id = PaymentData.AddNewPayment(PDTO);
            return this.Id != -1;
        }
        private bool _UpdatePayment()
        {
            return PaymentData.UpdatePayment(PDTO);
        }

        public static List<PaymentDTOWithName> GetAllPayments()
        {
            return PaymentData.GetAllpayments();
        }
        public static Payment Find(int paymentId)
        {
            PaymentDTOWithName uDTO = PaymentData.GetPaymentById(paymentId);

            if (uDTO != null)
            {
                return new Payment(uDTO);
            }
            else
                return null;
        }

        

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPayment())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdatePayment();

            }


            return false;
        }

        public static bool Delete(int paymentId)
        {
            return PaymentData.DeletePayment(paymentId);
        }

        public static bool IsPaymentExists(int paymentId)
        {
            return PaymentData.IsPaymentExists(paymentId);
        }
        public static bool IsPaymentHasRelations(int paymentId)
        {
            return PaymentData.IsPaymentHasRelations(paymentId);
        }


    }
}
