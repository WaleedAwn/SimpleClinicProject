using Microsoft.Data.SqlClient;
using PersonsAPIBusinessLayer.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Users
{
    public class UserData
    {
        public static List<AllUserDTO> GetAllUsers()
        {
            List<AllUserDTO> usersList = new List<AllUserDTO>();

            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usersList.Add(new AllUserDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("UserId")),
                                reader.GetInt32(reader.GetOrdinal("PersonId")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetString(reader.GetOrdinal("UserName")),
                                reader.GetString(reader.GetOrdinal("Password"))
                            ));
                        }
                    }
                }

            }

            return usersList;
        }

        public static AllUserDTO GetUserById(int userId)
        {

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetUserById", connection))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AllUserDTO(
                                reader.GetInt32(reader.GetOrdinal("UserId")),
                                reader.GetInt32(reader.GetOrdinal("PersonId")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetString(reader.GetOrdinal("UserName")),
                                reader.GetString(reader.GetOrdinal("Password"))
                                );

                        }
                        else
                            return null;
                    }


                }
            }

        }

        public static AllUserDTO GetUserByUserName(string userName)
        {

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetUserByUserName", connection))
                {


                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AllUserDTO(
                                reader.GetInt32(reader.GetOrdinal("UserId")),
                                reader.GetInt32(reader.GetOrdinal("PersonId")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetString(reader.GetOrdinal("UserName")),
                                reader.GetString(reader.GetOrdinal("Password"))
                                );

                        }
                        else
                            return null;
                    }


                }

            }
        }

        public static int AddNewUser(UserDTO userDTO)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            using (var command = new SqlCommand("SP_AddNewUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PersonId", userDTO.PersonId);
                command.Parameters.AddWithValue("@UserName", userDTO.UserName);
                command.Parameters.AddWithValue("@Password", userDTO.Password);


                var outputIdParam = new SqlParameter("@NewUserId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }


                return (int)outputIdParam.Value;
            }
        }

        public static bool UpdateUser(UserDTO userDTO)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_UpdateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserId", userDTO.Id);
                    command.Parameters.AddWithValue("@PersonId", userDTO.PersonId);
                    command.Parameters.AddWithValue("@UserName", userDTO.UserName);
                    command.Parameters.AddWithValue("@Password", userDTO.Password);

                    var outputIdParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        connection.Close();
                    }
                    return (int)outputIdParam.Value > 0;

                }
            }


        }

        public static bool DeleteUser(int userId)
        {

            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_DeleteUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    int rowsAffected = 0;
                    try
                    {
                        connection.Open();
                        rowsAffected = (int)command.ExecuteScalar();

                    }
                    catch (SqlException ex)
                    {
                        rowsAffected = 0;
                    }

                    return (rowsAffected == 1);

                }
            }

        }
        
        public static bool CheckUserCredentials(string username, string password)
        {
            bool isValid = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckUserCredentials", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", username);
                    command.Parameters.AddWithValue("@Password", password);


                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                   

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        isValid = (int)returnParameter.Value != 0;
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }
            }
            return isValid;
        }
        

        public static bool IsUserExists(string userName)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckUserExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", userName);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        isExist = (int)returnParameter.Value != 0;
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            return isExist;

        }

        public static bool IsUserExists(int userId)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckUserExistsById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        isExist = (int)returnParameter.Value != 0;
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            return isExist;

        }


        public static bool IsPersonUser(int personId)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_IsPersonUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PersonId", personId);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        isExist = (int)returnParameter.Value != 0;
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            return isExist;

        }



    }
}
