using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.DTOs
{
    public class AppointmentDTO
    {
        public AppointmentDTO(int id, int patientId, int doctorId, DateTime appointmentDate, byte appointmentStatus, int? medicalRecordId, int? paymentId)
        {
            Id = id;
            PatientId = patientId;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            AppointmentStatus = appointmentStatus;
            MedicalRecordId = medicalRecordId;
            PaymentId = paymentId;
        }

        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        
        public byte AppointmentStatus { get; set; }

        public int? MedicalRecordId { get; set; }
        public int? PaymentId { get; set; }


    }


    public class AllAppointmentDTO
    {
        public AllAppointmentDTO(int id, int patientId, string personName, int doctorId, string doctorName, DateTime appointmentDate, string appointmentStatus, int? medicalRecordId, int? paymentId)
        {
            Id = id;
            PatientId = patientId;
            PersonName = personName;
            DoctorName = doctorName;
            DoctorId = doctorId;
            AppointmentDate = appointmentDate;
            AppointmentStatus = appointmentStatus;
            MedicalRecordId = medicalRecordId;
            PaymentId = paymentId;
        }

        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PersonName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string AppointmentStatus { get; set; }

        public int? MedicalRecordId { get; set; }
        public int? PaymentId { get; set; }


    }



}
