using PersonsAPIBusinessLayer.People;

using PersonsAPIDataAccessLayer.Patients;
using PersonsAPIDataAccessLayer.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIBusinessLayer.Patients
{
    public class Patients
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        public PatientsDTO SDTO
        {
            get { return new PatientsDTO(Id, PersonId); }
        }

        public int Id { get; set; }
        public int PersonId { get; set; }


        public Patients(PatientsDTO SDTO,enMode cMode=enMode.AddNew)
        {
            this.Id = SDTO.Id;
            this.PersonId = SDTO.PersonId;
            this.Mode = cMode;
        }

        // to get All patient info from Person table
        public static List<PatientAllInfoDTO> GetAllPatients() { 
        
        return PatientsData.GetAllPatients();
        }

        public static Patients Find(int id)
        {
            PatientsDTO sDTO = PatientsData.GetPatientsByID(id);
            if (sDTO != null)
            {
                return new Patients(sDTO, enMode.Update);
            }
            return null;
        }

        public static PatientAllInfoDTO FindAllPatientsDataByID(int id)
        {
            PatientAllInfoDTO sDTO = PatientsData.GetAllPatientsDataByID(id);
            if (sDTO != null)
            {
                return sDTO;
            }
            return null;
        }

        private bool _addNewPatient()
        {
            Id = PatientsData.AddNewPatient(SDTO);
            return (Id!=-1);

        }
        private bool _UpdatePatient()
        {
            return PatientsData.UpdatePatient(SDTO); // Fixed: UpdatePerson instead of UpdateStudent
        }
        public  bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_addNewPatient())
                    {
                      Mode=enMode.Update;
                        return true;
                    }
                    return false;
                    break;
                case enMode.Update:
                    return _UpdatePatient();
                    break;
                default:
                    break;
            }
            return false ;
        }

        public static bool DeletePatient(int id)
        {
            return PatientsData.DeletePatient(id);
        }
        public static bool CheckPatientRelations(int id)
        {
            return PatientsData.IsPatientHasRelation(id);
        }
        public static bool IsPatientExists(int id)
        {
            return PatientsData.IsPatientExist(id);
        }


    }
}
