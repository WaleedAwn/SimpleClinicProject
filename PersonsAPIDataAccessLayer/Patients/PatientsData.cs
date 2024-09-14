using Microsoft.Data.SqlClient;
using PersonsAPIDataAccessLayer.People;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Patients
{
    public class PatientAllInfoDTO
    {
        public PatientAllInfoDTO(int id,int personId,  string personName, DateTime dateOfBirth, string gender, string PhoneNumber, string email, string address)
        {
            this.Id = id;
            this.personId = personId;
            
            this.PersonName = personName;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.PhoneNumber = PhoneNumber;
            this.Email = email;
            this.Address = address;

        }
        public int Id { get; set; }
        public int personId { get; set; }

        public string PersonName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

    }
      public class PatientsDTO
        {
            public PatientsDTO(int id, int Personid)
            {
                this.Id = id;
                this.PersonId = Personid;
              
            }
      

            public int Id { get; set; }
            public int PersonId { get; set; }
      

    }

    public class PatientsData
    {

        public static List<PatientAllInfoDTO> GetAllPatients()
        {
            var PatientsList = new List<PatientAllInfoDTO>();


            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPatientsInfo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PatientsList.Add(new PatientAllInfoDTO
                            (
                                //(PatientID,PersonID)
                                reader.GetInt32(reader.GetOrdinal("PatientID")),
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

                return PatientsList;
            }



        }

        public static PatientsDTO GetPatientsByID(int PatientID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPatientByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PatientsDTO
                             (
                              reader.GetInt32(reader.GetOrdinal("PatientID")),
                              reader.GetInt32(reader.GetOrdinal("PersonID"))
                             );
                        }

                        else { return null; }
                    }
                }



            }



        }


        public static PatientAllInfoDTO GetAllPatientsDataByID(int PatientID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPatientInfoByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PatientAllInfoDTO
                             (
                               reader.GetInt32(reader.GetOrdinal("PatientID")),
                                reader.GetInt32(reader.GetOrdinal("PersonID")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
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



        public static bool IsPatientExist(int patientId)
        {
            int isFound = 0;
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckPatientExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PatientID", patientId);

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

        public static int AddNewPatient(PatientsDTO NewPatientDTOInfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_AddNewPatient", connection))
                {
                                        command.CommandType = CommandType.StoredProcedure;
                    
                 
                    command.Parameters.AddWithValue("@PersonID", NewPatientDTOInfo.PersonId);
                  

                    var outPutIdParm = new SqlParameter("@NewPatientID", SqlDbType.Int)
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

        public static bool UpdatePatient(PatientsDTO UpdatePatientDTOinfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_UpdatePatient", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PatientID", UpdatePatientDTOinfo.Id);
                    command.Parameters.AddWithValue("@PersonID", UpdatePatientDTOinfo.PersonId);
                   

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

         public static bool DeletePatient(int patientId)
            {
                int rowsAffected = 0;
                using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
                {
                    using (var command = new SqlCommand("SP_DeletePatient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PatientID", patientId);


                        connection.Open();

                        rowsAffected = (int)command.ExecuteScalar();
                        return rowsAffected == 1;
                    }

                }




            }

          public static bool IsPatientHasRelation(int PatientId)
            {
                bool isFound = false;
                int returnValue = 0;

                using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_CheckPatientsRelations", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PatientID", PatientId);

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
