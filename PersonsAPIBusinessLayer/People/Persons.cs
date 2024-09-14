using System;
using System.Collections.Generic;
using PersonsAPIDataAccessLayer.People;

namespace PersonsAPIBusinessLayer.People
{
    public class Persons
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public PersonsDTO SDTO
        {
            get { return new PersonsDTO(PersonID, PersonName, DateOfBirth, Gender, PhoneNumber, Email, Address); }
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
            PersonID = sDTO.Id;
            PersonName = sDTO.PersonName;
            DateOfBirth = sDTO.DateOfBirth;
            Gender = sDTO.Gender;
            PhoneNumber = sDTO.PhoneNumber;
            Email = sDTO.Email;
            Address = sDTO.Address;
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
            PersonID = PersonsData.AddNewPerson(SDTO);
            return PersonID != -1;
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
