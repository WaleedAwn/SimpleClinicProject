using Azure.Core;
using PersonsAPIDataAccessLayer;
using PersonsAPIDataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonsAPIBusinessLayer
{
    public class Appointment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public AllAppointmentDTO AllADTO
        {
            get
            {
                return new AllAppointmentDTO(this.Id, this.PatientId, this.PersonName, this.DoctorId, this.DoctorName, this.AppointmentDate, this.AppointmentStatusCaption, this.MedicalRecordId, this.PaymentId)
;           }
        }

        public AppointmentDTO aDTO
        {
            get
            {
                return new AppointmentDTO(this.Id, this.PatientId, this.DoctorId, this.AppointmentDate, this.AppointmentStatus, this.MedicalRecordId, this.PaymentId);
            }
        }

        public Appointment(AppointmentDTO aDTO,enMode mode = enMode.AddNew)
        {
            this.Id = aDTO.Id;
            this.PatientId = aDTO.PatientId;
            this.DoctorId = aDTO.DoctorId;
            this.AppointmentDate = aDTO.AppointmentDate;
            this.AppointmentStatus = aDTO.AppointmentStatus;
            this.MedicalRecordId = aDTO.MedicalRecordId;
            this.PaymentId = aDTO.PaymentId;
            Mode = mode;
        }
        public Appointment(AllAppointmentDTO ADTO, enMode mode =enMode.AddNew )
        {
            this.Id = ADTO.Id;
            PatientId = ADTO.PatientId;
            PersonName = ADTO.PersonName;
            DoctorId = ADTO.DoctorId;
            DoctorName = ADTO.DoctorName;
            AppointmentDate = ADTO.AppointmentDate;
            AppointmentStatusCaption = ADTO.AppointmentStatus;
            MedicalRecordId = ADTO.MedicalRecordId;
            PaymentId = ADTO.PaymentId;
            Mode = mode;
        }
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PersonName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string AppointmentStatusCaption { get; set; }
        public byte AppointmentStatus { get; set; }

        public int? MedicalRecordId { get; set; }
        public int? PaymentId { get; set; }

        private bool _AddNewAppointment()
        {
            this.Id = AppointmentData.AddNewAppointment(aDTO);
            return this.Id != -1;
        }

        private bool _UpdateAppointment()
        {
            
            return AppointmentData.UpdateAppointment(aDTO);
        }

        public static List<AllAppointmentDTO> GetAllAppointments()
        {
            return AppointmentData.GetAllAppointments();
        }

        public static Appointment Find(int id)
        {
            AllAppointmentDTO aDTO = AppointmentData.GetAppointmentByID(id);
            if (aDTO != null)
                return new Appointment(aDTO, enMode.Update);
            else
                return null;
        }
        
        public static bool Delete(int id)
        {
            return AppointmentData.DeleteAppointment(id);
        }

        public static bool IsAppointmentExist(int id)
        {
            return AppointmentData.IsAppointmentExist(id);
        }

        public static bool IsAppointmentHasRelations(int id)
        {
            return AppointmentData.IsAppointmentHasRelations(id);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAppointment())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateAppointment();
            }
            return false;
        }

        public static bool IsPersonHasActiveAppointmentWithDoctor(int paitentId, int doctorId)
        {
            return AppointmentData.IsPatientHasActiveAppointmentWithDoctor(paitentId, doctorId);
        }

        public static List<AllAppointmentDTO> GetAllPatientAppointments(int patientId)
        {
            return AppointmentData.GetAllPatientAppointments(patientId);
        }

    }
}
