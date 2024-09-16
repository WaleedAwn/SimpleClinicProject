using System;
using PersonsAPIDataAccessLayer.MedicalRecords;
namespace PersonsAPIBusinessLayer.MedicalRecords
{
    public class MedicalRecords
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public MedicalRecordsDTO SDTO
        {
            get { return new MedicalRecordsDTO(MedicalRecordID, VisitDescription,Diagnosis,AdditionalNotes); }
        }


        public int MedicalRecordID { get; set; }
        public string VisitDescription { get; set; }
        public string Diagnosis { get; set; }
        public string AdditionalNotes { get; set; }


        public MedicalRecords(MedicalRecordsDTO sDTO, enMode cMode = enMode.AddNew)
        {
            MedicalRecordID = sDTO.MedicalRecordID;
            VisitDescription = sDTO.VisitDescription;
            Diagnosis = sDTO.Diagnosis;
            AdditionalNotes = sDTO.AdditionalNotes;
            
            Mode = cMode;
        }

        public static List<MedicalRecordsDTO> GetAllMedicalRecords()
        {
            return MedicalRecordsData.GetAllMedicalRecords();
        }

        public static MedicalRecords Find(int MedicalRecordID)
        {
            MedicalRecordsDTO sDTO = MedicalRecordsData.GetMedicalRecordByID(MedicalRecordID);
            if (sDTO != null)
            {
                return new MedicalRecords(sDTO, enMode.Update);
            }
            return null;
        }

        private bool _AddNewMedicalRecord()
        {
            MedicalRecordID = MedicalRecordsData.AddNewMedicalRecord(SDTO);
            return MedicalRecordID != -1;
        }

        private bool _UpdateMedicalRecord()
        {
            return MedicalRecordsData.UpdateMedicalRecord(SDTO); // Fixed: UpdatePerson instead of UpdateStudent
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewMedicalRecord())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateMedicalRecord();
            }
            return false;
        }

        public static bool DeleteMedicalRecord(int MedicalRecordID)
        {
            return MedicalRecordsData.DeleteMedicalRecord(MedicalRecordID);
        }
     
        public static bool IsMedicalRecordExists(int MedicalRecordID)
        {
            return MedicalRecordsData.IsMedicalRecordExist(MedicalRecordID);
        }
    }

}
