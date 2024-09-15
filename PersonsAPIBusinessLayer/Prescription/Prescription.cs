using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonsAPIDataAccessLayer.Prescriptions;

namespace PersonsAPIBusinessLayer.Prescriptions
{
    
        public class Prescription
        {
            public enum enMode { AddNew = 0, Update = 1 };
            public enMode Mode = enMode.AddNew;

            public PrescriptionsDTO SDTO
            {
                get {
                return new PrescriptionsDTO(PrescriptionID, MedicalRecordID, MedicationName,
                    Dosage, Frequency, StartDate, EndDate, SpecialInstructions); }
            }
        public int PrescriptionID { get; set; }
        public int MedicalRecordID { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? SpecialInstructions { get; set; }


        public Prescription(PrescriptionsDTO sDTO, enMode cMode = enMode.AddNew)
            {
                PrescriptionID = sDTO.PrescriptionID;
                MedicalRecordID = sDTO.MedicalRecordID;
                MedicationName = sDTO.MedicationName;
                Dosage = sDTO.Dosage;
                Frequency = sDTO.Frequency;
                StartDate = sDTO.StartDate;
                EndDate = sDTO.EndDate;
                SpecialInstructions = sDTO.SpecialInstructions;
                Mode = cMode;
            }

            public static List<PrescriptionsDTO> GetAllPrescription()
            {
                return PrescriptionsData.GetAllPrescriptions();
            }

            public static Prescription Find(int id)
            {
                PrescriptionsDTO sDTO = PrescriptionsData.GetByPrescriptionID(id);
                if (sDTO != null)
                {
                    return new Prescription(sDTO, enMode.Update);
                }
                return null;
            }

            private bool _AddNewPrescription()
            {
                PrescriptionID = PrescriptionsData.AddNewPrescription(SDTO);
                return PrescriptionID != -1;
            }

            private bool _UpdatePrescription()
            {
                return PrescriptionsData.UpdatePrescription(SDTO); // Fixed: UpdatePrescription instead of UpdateStudent
            }

            public bool Save()
            {
                switch (Mode)
                {
                    case enMode.AddNew:
                        if (_AddNewPrescription())
                        {
                            Mode = enMode.Update;
                            return true;
                        }
                        return false;
                    case enMode.Update:
                        return _UpdatePrescription();
                }
                return false;
            }

            public static bool DeletePrescription(int id)
            {
                return PrescriptionsData.DeletePrescription(id);
            }
            
            public static bool IsPrescriptionExists(int id)
            {
                return PrescriptionsData.IsExistPrescription(id);
            }
        }

    
}
