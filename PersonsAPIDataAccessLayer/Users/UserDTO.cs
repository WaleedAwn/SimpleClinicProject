using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Users
{
    public class UserDTO
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserDTO(int id,int personId, string userName, string password)
        {
            this.Id = id;
            this.PersonId = personId;
            this.UserName = userName;
            this.Password = password;
        }

    }
}
