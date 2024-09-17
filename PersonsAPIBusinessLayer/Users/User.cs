using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonsAPIDataAccessLayer;
using PersonsAPIDataAccessLayer.Users;


namespace PersonsAPIBusinessLayer.Users
{
    public class User
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public UserDTO UDTO
        {
            get { return new UserDTO(this.Id,this.PersonId, this.UserName, this.Password); }
        }
        public AllUserDTO AllUDTO
        {
            get { return new AllUserDTO(this.Id, this.PersonId, this.Name, this.UserName, this.Password); }
        }


        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public User(UserDTO UDTO, enMode cMode = enMode.AddNew)
        {
            Id = UDTO.Id;
            PersonId = UDTO.PersonId;
            UserName = UDTO.UserName;
            Password = UDTO.Password;
            Mode = cMode;
        }

        private User(AllUserDTO UDTO)
        {
            Id = UDTO.Id;
            PersonId = UDTO.PersonId;
            Name = UDTO.Name;
            UserName = UDTO.UserName;
            Password = UDTO.Password;
            Mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            this.Id = UserData.AddNewUser(UDTO);
            return this.Id != -1;
        }
        private bool _UpdateUser()
        {
            return UserData.UpdateUser(UDTO);
        }

        public static List<AllUserDTO> GetAllUsers()
        {
            return UserData.GetAllUsers();
        }
        public static User Find(int userId)
        {
            AllUserDTO uDTO = UserData.GetUserById(userId);

            if (uDTO != null)
            {
                return new User(uDTO);
            }
            else
                return null;
        }

        public static User Find(string userName)
        {
            AllUserDTO uDTO = UserData.GetUserByUserName(userName);

            if (uDTO != null)
            {
                return new User(uDTO);
            }
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateUser();

            }


            return false;
        }

        public static bool Delete(int userId)
        {
            return UserData.DeleteUser(userId);
        }
    
        public static bool CheckUserCredentials(string usreName, string password)
        {
            return UserData.CheckUserCredentials(usreName, password);
        }

        public static bool IsUserExists(string userName)
        {
            return UserData.IsUserExists(userName);
        }
        public static bool IsUserExists(int userId)
        {
            return UserData.IsUserExists(userId);
        }

        public static bool IsPersonUser(int personId)
        {
            return UserData.IsPersonUser(personId);
        }

    }
}
