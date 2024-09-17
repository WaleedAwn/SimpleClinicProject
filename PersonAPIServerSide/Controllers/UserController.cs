using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonsAPIDataAccessLayer.Users;
using PersonsAPIBusinessLayer.Users;

namespace PersonAPIServerSide.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/UserApi")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllUsers")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            List<AllUserDTO> userList = PersonsAPIBusinessLayer.Users.User.GetAllUsers();

            if (userList.Count == 0)
            {
                return NotFound("No Users Found!");
            }

            return Ok(userList);
        }

        [HttpGet("Find/Id={id}", Name = "GetUserById")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<AllUserDTO> GetUserById(int id)
        {
            if (id < 1)
                return BadRequest($"Not accepted ID {id}");

            User user = PersonsAPIBusinessLayer.Users.User.Find(id);

            if (user == null)
                return NotFound($"No user found with Id {id}");

            AllUserDTO uDTO = user.AllUDTO;
            return Ok(uDTO);

        }

        [HttpGet("Find/UserName={userName}", Name = "GetUserByName")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<UserDTO> GetUserByName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest($"Username is required!");

            User user = PersonsAPIBusinessLayer.Users.User.Find(userName);

            if (user == null)
                return NotFound($"No user found with UserName = {userName}");

            AllUserDTO uDTO = user.AllUDTO;
            return Ok(uDTO);

        }


        [HttpPost("Add", Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<UserDTO> AddUser(UserDTO newUserDTO)
        {
            //we validate the data here
            if (newUserDTO == null || string.IsNullOrEmpty(newUserDTO.UserName)|| string.IsNullOrEmpty(newUserDTO.Password))
            {
                return BadRequest("Invalid User data.");
            }

            if(!PersonsAPIBusinessLayer.People.Persons.IsPersonExists(newUserDTO.PersonId))
            {
                return NotFound($"No person with Id={newUserDTO.PersonId}");
            }


            if (PersonsAPIBusinessLayer.Users.User.IsPersonUser(newUserDTO.PersonId))
            {
                return Conflict($"person with Id=[{newUserDTO.PersonId}] is already user, choose another person");
            }

            if (PersonsAPIBusinessLayer.Users.User.IsUserExists(newUserDTO.UserName))
            {
                return Conflict("UserName already exist, choose another one");
            }

            if(newUserDTO.Password.Length < 6 )
            {
                return BadRequest("Password must be greater >= 6 chars");
            }

            User user = new User(new UserDTO(newUserDTO.Id,newUserDTO.PersonId , newUserDTO.UserName, newUserDTO.Password));

            newUserDTO.Id = user.Id;

            if (user.Save())
                return Ok(user.UDTO);
            else
                return StatusCode(500, new { message = "Erorr adding user" });


            //we return the DTO only not the full user object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            //return Ok();
        }

        [HttpPut("Update/Id={id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<UserDTO> UpdateUser(int id, UserDTO updateUser)
        {
            if (id < 1 || updateUser == null ||
                updateUser.PersonId < 1||
                string.IsNullOrEmpty(updateUser.UserName) ||
                string.IsNullOrEmpty(updateUser.Password))
            {
                return BadRequest("Invalid User data.");
            }

            updateUser.Id = id;

            if(!PersonsAPIBusinessLayer.Users.User.IsUserExists(id))
            {
                return NotFound($"User with Id={id} is not exists");
            }

            User user = PersonsAPIBusinessLayer.Users.User.Find(id);


            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            if (!PersonsAPIBusinessLayer.People.Persons.IsPersonExists(updateUser.PersonId) )
            {
                return NotFound($"No person with Id={updateUser.PersonId}");
            }

            if (updateUser.PersonId != user.PersonId && PersonsAPIBusinessLayer.Users.User.IsPersonUser(updateUser.PersonId))
            {
                return Conflict($"Person with Id=[{updateUser.PersonId}] is already user, choose another person");
            }

            if (updateUser.UserName != user.UserName && PersonsAPIBusinessLayer.Users.User.IsUserExists(updateUser.UserName))
            {
                return Conflict("UserName already exist, choose another one");
            }



            user.PersonId = updateUser.PersonId;
            user.UserName = updateUser.UserName;
            user.Password = updateUser.Password;

            if (user.Save())
                //we return the DTO not the full student object.
                return Ok(user.UDTO);
            else
                return StatusCode(500, new { message = "Erorr updating user" });

        }


        //here we use HttpDelete method
        [HttpDelete("Delete/Id={id}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUser(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }


            if (PersonsAPIBusinessLayer.Users.User.Delete(id))
                return Ok($"User with ID {id} has been deleted.");
            else
                return NotFound($"User with ID {id} not found. no rows deleted!");
        }

        [HttpPost("CheckCredentials/UserName={userName}/Password{password}", Name = "CheckUserCredentials")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        
        public ActionResult CheckUserCredentials(string userName,string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return BadRequest($"Username/Password is required!");

            if (!PersonsAPIBusinessLayer.Users.User.CheckUserCredentials(userName, password))
                return Unauthorized("Invalid Username/password!");



            return Ok(true);

        }


    }
}
