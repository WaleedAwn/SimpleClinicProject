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
            get { return new MedicalRecordsDTO(MedicalRecordID, VisitDescription, Diagnosis, AdditionalNotes); }
        }
        
        public int MedicalRecordID { get; set; }
        public string? VisitDescription { get; set; }
        public string? Diagnosis { get; set; }
        public string? AdditionalNotes { get; set; }
    




        public MedicalRecords(MedicalRecordsDTO sDTO, enMode cMode = enMode.AddNew)
        {
            MedicalRecordID = sDTO.MedicalRecordID;
            VisitDescription = sDTO.VisitDescription;
            Diagnosis = sDTO.Diagnosis;
            AdditionalNotes = sDTO.AdditionalNotes;
            
            Mode = cMode;
        }

        public static async Task<List<MedicalRecordsDTO>> GetAllMedicalRecords()
        {
            return await MedicalRecordsData.GetAllMedicalRecords();
        }

        public static async Task<MedicalRecords> Find(int MedicalRecordID)
        {
            MedicalRecordsDTO sDTO = await MedicalRecordsData.GetMedicalRecordByID(MedicalRecordID);
            if (sDTO != null)
            {
                return new MedicalRecords(sDTO, enMode.Update);
            }
            return null;
        }

        private async Task<int> _AddNewMedicalRecord()
        {
            MedicalRecordID = await MedicalRecordsData.AddNewMedicalRecord(SDTO);
            return MedicalRecordID ;
        }

        private Task<bool> _UpdateMedicalRecord()
        {
            return MedicalRecordsData.UpdateMedicalRecord(SDTO); // Fixed: UpdatePerson instead of UpdateStudent
        }

        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (await _AddNewMedicalRecord()!=-1)
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return await _UpdateMedicalRecord();
            }
            return false;
        }

        public static async Task<bool> DeleteMedicalRecord(int MedicalRecordID)
        {
            return await MedicalRecordsData.DeleteMedicalRecord(MedicalRecordID);
        }
     
        public static async Task<bool> IsMedicalRecordExists(int MedicalRecordID)
        {
            return await MedicalRecordsData.IsMedicalRecordExist(MedicalRecordID);
        }
    }

}
