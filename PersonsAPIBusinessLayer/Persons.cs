/*
using System;
using Microsoft.Data.SqlClient;
using PersonsAPIDataAccessLayer;


namespace PersonsAPIBusinessLayer
{
    public class Persons
    {

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public PersonsDTO SDTO
        {
            get { return new PersonsDTO(this.PersonID, this.PersonName, this.DateOfBirth, this.Gender, this.PhoneNumber, this.Email, this.Address); }
        }

        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }


        public Persons(PersonsDTO SDTO ,enMode cMode=enMode.AddNew)
        {
            this.PersonID= SDTO.PersonID;
            this.PersonName= SDTO.PersonName;
            this.DateOfBirth= SDTO.DateOfBirth;
            this.Gender= SDTO.Gender;
            this.PhoneNumber= SDTO.PhoneNumber;
            this.Email= SDTO.Email;
            this.Address= SDTO.Address;
            
            Mode = cMode;

        }

        public static List<PersonsDTO> GetAllPersons()
        {
            return PersonsData.GetAllPersons();
        }

        public static Persons Find(int ID)
        {
            PersonsDTO SDTO =PersonsData.GetPersonByID(ID);
            if (SDTO != null)
            { return new Persons(SDTO, enMode.Update);
            }
            else { return null; }
        }

        private bool _AddNewPerson()
        {
            this.PersonID = PersonsData.AddNewPerson(SDTO);
            return (this.PersonID != -1);
        }
        private bool _UpdatePerson()
        {
            return PersonsData.UpdatePerson(SDTO);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return true;

                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdatePerson();

            }
            return false;

        }

        public static bool DeletePerson(int id)
        {
            return PersonsData.DeletePerson(id);

        }



    }
}

*/


using System;
using System.Collections.Generic;
using PersonsAPIDataAccessLayer;

namespace PersonsAPIBusinessLayer
{
    public class Persons
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public PersonsDTO SDTO
        {
            get { return new PersonsDTO(this.PersonID, this.PersonName, this.DateOfBirth, this.Gender, this.PhoneNumber, this.Email, this.Address); }
        }

        public int PersonID { get; set; }
        public string PersonName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public Persons(PersonsDTO sDTO, enMode cMode = enMode.AddNew)
        {
            this.PersonID = sDTO.Id;
            this.PersonName = sDTO.PersonName;
            this.DateOfBirth = sDTO.DateOfBirth;
            this.Gender = sDTO.Gender;
            this.PhoneNumber = sDTO.PhoneNumber;
            this.Email = sDTO.Email;
            this.Address = sDTO.Address;
            Mode = cMode;
        }

        public static List<PersonsDTO> GetAllPersons()
        {
            return PersonsData.GetAllPersons();
        }

        public static Persons Find(int id)
        {
            PersonsDTO sDTO = PersonsData.GetPersonByID(id);
            if (sDTO != null)
            {
                return new Persons(sDTO, enMode.Update);
            }
            return null;
        }

        private bool _AddNewPerson()
        {
            this.PersonID = PersonsData.AddNewPerson(SDTO);
            return (this.PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            return PersonsData.UpdatePerson(SDTO); // Fixed: UpdatePerson instead of UpdateStudent
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdatePerson();
            }
            return false;
        }

        public static bool DeletePerson(int id)
        {
            return PersonsData.DeletePerson(id);
        }
        public static bool CheckPersonRelations(int id)
        {
            return PersonsData.IsPersonHasRelation(id);
        }
        public static bool IsPersonExists(int id)
        {
            return PersonsData.IsPersonExist(id);
        }
    }
}
