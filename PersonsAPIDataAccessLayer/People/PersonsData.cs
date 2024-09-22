
using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace PersonsAPIDataAccessLayer.People
{
    public class PersonsDTO
    {
        public PersonsDTO(int id, string personName, DateTime dateOfBirth, string gender, string PhoneNumber, string email, string address)
        {
            Id = id;
            PersonName = personName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            this.PhoneNumber = PhoneNumber;
            Email = email;
            Address = address;
        }

        public int Id { get; set; }
        public string PersonName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

    }

    public class PersonsData
    { 

        public static List<PersonsDTO> GetAllPersons()
        {
            var PersonsList = new List<PersonsDTO>();


            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPersons", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PersonsList.Add(new PersonsDTO
                            (
                                //(PersonID, Name, DateOfBirth, Gender, PhoneNumber, Email, Address
                                reader.GetInt32(reader.GetOrdinal("PersonID")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                reader.GetString(reader.GetOrdinal("Gender")),
                                reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Address"))

                            ));
                        }
                    }
                }
                conn.Close();

                return PersonsList;
            }



        }

        public static PersonsDTO GetPersonByID(int PersonID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPersonByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PersonID", PersonID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PersonsDTO
                             (

                                 reader.GetInt32(reader.GetOrdinal("PersonID")),
                                 reader.GetString(reader.GetOrdinal("Name")),
                                 reader.GetDateTime(reader.GetOrdinal("DateOfBirth")), // Extract only the Date part
                                 reader.GetString(reader.GetOrdinal("Gender")),
                                 reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                 reader.GetString(reader.GetOrdinal("Email")),
                                 reader.GetString(reader.GetOrdinal("Address"))


                             );
                        }

                        else { return null; }
                    }
                }



            }



        }

        public static int AddNewPerson(PersonsDTO NewPersonDTOInfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_AddNewPerson", connection))
                {
                    //(PersonID, Name, DateOfBirth, Gender, PhoneNumber, Email, Address
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@PersonID", NewPersonDTOInfo.PersonID);
                    command.Parameters.AddWithValue("@Name", NewPersonDTOInfo.PersonName);
                    command.Parameters.AddWithValue("@DateOfBirth", NewPersonDTOInfo.DateOfBirth.Date);
                    command.Parameters.AddWithValue("@Gender", NewPersonDTOInfo.Gender);
                    command.Parameters.AddWithValue("@PhoneNumber", NewPersonDTOInfo.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", NewPersonDTOInfo.Email);
                    command.Parameters.AddWithValue("@Address", NewPersonDTOInfo.Address);

                    var outPutIdParm = new SqlParameter("@NewPersonId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutIdParm);
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)outPutIdParm.Value;

                }

            }

        }

        public static bool UpdatePerson(PersonsDTO UpdatePersonDTOinfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_UpdatePerson", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PersonID", UpdatePersonDTOinfo.Id);
                    command.Parameters.AddWithValue("@Name", UpdatePersonDTOinfo.PersonName);
                    command.Parameters.AddWithValue("@DateOfBirth", UpdatePersonDTOinfo.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", UpdatePersonDTOinfo.Gender);
                    command.Parameters.AddWithValue("@PhoneNumber", UpdatePersonDTOinfo.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", UpdatePersonDTOinfo.Email);
                    command.Parameters.AddWithValue("@Address", UpdatePersonDTOinfo.Address);

                    var outPutIdParm = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutIdParm);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)outPutIdParm.Value > 0;

                }

            }
        }

        public static bool DeletePerson(int PersonId)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_DeletePerson", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PersonID", PersonId);


                    connection.Open();

                    rowsAffected = (int)command.ExecuteScalar();
                    return rowsAffected == 1;
                }

            }




        }


        public static bool IsPersonExist(int PersonId)
        {
            int isFound = 0;
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckPersonExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PersonId", PersonId);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    connection.Open();
                    command.ExecuteNonQuery();

                    isFound = (int)returnParameter.Value;

                    isExist = isFound != 0;
                }
            }
            return isExist;

        }

        public static bool IsPersonHasRelation(int PersonId)
        {
            bool isFound = false;
            int returnValue = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckPersonRelations", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PersonId", PersonId);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    returnValue = (int)returnParameter.Value;
                    isFound = returnValue != 0;
                }
            }
            return isFound;

        }



    }
}



