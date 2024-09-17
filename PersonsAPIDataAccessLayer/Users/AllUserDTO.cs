namespace PersonsAPIBusinessLayer.Users
{
    public class AllUserDTO
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public AllUserDTO(int id,int personId, string name, string userName, string password)
        {
            this.Id = id;
            this.UserName = userName;
            this.Password = password;
            this.Name = name;
            this.PersonId = personId;
        }


    }

}
