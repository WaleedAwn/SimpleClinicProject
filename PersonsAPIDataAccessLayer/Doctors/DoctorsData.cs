using Microsoft.Data.SqlClient;
using PersonsAPIDataAccessLayer.Patients;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Doctors
{
    public class AllDoctorsInfoDTO
    {
        public AllDoctorsInfoDTO(int id,int PersonId, string specialization, string personName, DateTime dateOfBirth, string gender, string PhoneNumber, string email, string address)
        {
            this.Id = id;
            this.PersonId = PersonId;
            this.PersonName = personName;
            this.Specialization = specialization;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.PhoneNumber = PhoneNumber;
            this.Email = email;
            this.Address = address;

        }
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Specialization { get; set; }


        public string PersonName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

    }

    public class DoctorsDTO
    {
        public DoctorsDTO(int id, int Personid, string specialization)
        {
            this.Id = id;
            this.PersonId = Personid;
            Specialization = specialization;
        }

        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Specialization { get; set; }

    }

    public class DoctorsData
    {
        public static List<AllDoctorsInfoDTO> GetAllDoctors()
        {
            var DoctorsList = new List<AllDoctorsInfoDTO>();


            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllDoctorsInfo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DoctorsList.Add(new AllDoctorsInfoDTO
                            (

                                reader.GetInt32(reader.GetOrdinal("DoctorID")),
                                reader.GetInt32(reader.GetOrdinal("PersonID")),
                                reader.GetString(reader.GetOrdinal("Specialization")),
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

                return DoctorsList;
            }



        }


        public static AllDoctorsInfoDTO GetAllDoctorsDataByID(int PatientID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllDoctorsInfoByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DoctorID", PatientID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AllDoctorsInfoDTO
                             (
                                 reader.GetInt32(reader.GetOrdinal("DoctorID")),
                                reader.GetInt32(reader.GetOrdinal("PersonID")),
                                reader.GetString(reader.GetOrdinal("Specialization")),
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


        public static DoctorsDTO GetDoctorsByID(int DoctorID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetDoctorByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DoctorID", DoctorID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DoctorsDTO
                             (
                                reader.GetInt32(reader.GetOrdinal("DoctorID")),
                                reader.GetInt32(reader.GetOrdinal("PersonID")),
                                reader.GetString(reader.GetOrdinal("Specialization"))
                             );
                        }

                        else { return null; }
                    }
                }



            }

        }
        public static bool IsDoctorExist(int DoctorID)
        {
            int isFound = 0;
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckDoctorExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DoctorID", DoctorID);

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

        public static int AddNewDoctor(DoctorsDTO NewDoctorDTOInfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_AddNewDoctor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.AddWithValue("@PersonID", NewDoctorDTOInfo.PersonId);
                    command.Parameters.AddWithValue("@Specialization", NewDoctorDTOInfo.Specialization);



                    var outPutIdParm = new SqlParameter("@NewDoctorID", SqlDbType.Int)
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

        public static bool UpdateDoctor(DoctorsDTO UpdateDoctorDTOinfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_UpdateDoctor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@DoctorID", UpdateDoctorDTOinfo.Id);         

                    command.Parameters.AddWithValue("@PersonID", UpdateDoctorDTOinfo.PersonId);
                    command.Parameters.AddWithValue("@Specialization", UpdateDoctorDTOinfo.Specialization);




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

        public static bool DeleteDoctor(int DoctorId)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_DeleteDoctor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DoctorID", DoctorId);


                    connection.Open();

                    rowsAffected = (int)command.ExecuteScalar();
                    return rowsAffected == 1;
                }

            }




        }

        public static bool IsDoctorHasRelation(int DoctorId)
        {
            bool isFound = false;
            int returnValue = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckDoctorsRelations", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DoctorID", DoctorId);

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
