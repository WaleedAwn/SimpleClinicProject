using PersonsAPIDataAccessLayer.Doctors;
using PersonsAPIDataAccessLayer.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIBusinessLayer.Doctors
{
    public class Doctors
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        public DoctorsDTO SDTO
        {
            get { return new DoctorsDTO(Id, PersonId, Specialization); }
        }
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Specialization { get; set; }

        public Doctors(DoctorsDTO SDTO, enMode cMode = enMode.AddNew)
        {
            this.Id = SDTO.Id;
            this.PersonId = SDTO.PersonId;
            this.Specialization = SDTO.Specialization;
            this.Mode = cMode;
        }



        public static List<AllDoctorsInfoDTO> GetAllDoctors()
        {

            return DoctorsData.GetAllDoctors();
        }


        public static Doctors Find(int id)
        {
            DoctorsDTO sDTO = DoctorsData.GetDoctorsByID(id);
            if (sDTO != null)
            {
                return new Doctors(sDTO, enMode.Update);
            }
            return null;
        }

        public static AllDoctorsInfoDTO FindAllDoctorsByID(int id)
        {
            AllDoctorsInfoDTO sDTO = DoctorsData.GetAllDoctorsDataByID(id);
            if (sDTO != null)
            {
                return sDTO;
            }
            return null;
        }

        private bool _addNewDoctor()
        {
            Id = DoctorsData.AddNewDoctor(SDTO);
            return (Id != -1);

        }
        private bool _UpdateDoctor()
        {
            return DoctorsData.UpdateDoctor(SDTO); // Fixed: UpdatePerson instead of UpdateStudent
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_addNewDoctor())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                    break;
                case enMode.Update:
                    return _UpdateDoctor();
                    break;
                default:
                    break;
            }
            return false;
        }

        public static bool DeleteDoctor(int id)
        {
            return DoctorsData.DeleteDoctor(id);
        }
        public static bool CheckDoctorRelations(int id)
        {
            return DoctorsData.IsDoctorHasRelation(id);
        }
        public static bool IsDoctorExists(int id)
        {
            return DoctorsData.IsDoctorExist(id);
        }


    }
}
