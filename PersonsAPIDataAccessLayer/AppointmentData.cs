using Microsoft.Data.SqlClient;
using PersonsAPIDataAccessLayer.DTOs;
using PersonsAPIDataAccessLayer.People;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer
{
    public class AppointmentData
    {
        public static List<AllAppointmentDTO> GetAllAppointments()
        {
            var appointmentsList = new List<AllAppointmentDTO>();


            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllAppointments", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int? medicalRecord ;
                        int? paymentId ;
                        while (reader.Read())
                        {
                            if (reader.GetValue("MedicalRecordId") != DBNull.Value)
                                medicalRecord = reader.GetInt32(reader.GetOrdinal("DoctorId"));
                            else
                                medicalRecord = null;

                            if (reader.GetValue("PaymentId") != DBNull.Value)
                                paymentId = reader.GetInt32(reader.GetOrdinal("PaymentId"));
                            else
                                paymentId = null;

                            appointmentsList.Add(new AllAppointmentDTO
                            (
                                
                                reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                                reader.GetInt32(reader.GetOrdinal("PatientId")),
                                reader.GetString(reader.GetOrdinal("PersonName")),
                                reader.GetInt32(reader.GetOrdinal("DoctorId")),
                                reader.GetString(reader.GetOrdinal("DoctorName")),
                                reader.GetString(reader.GetOrdinal("Specialization")),
                                reader.GetDateTime(reader.GetOrdinal("AppointmentDateTime")),
                                reader.GetString(reader.GetOrdinal("AppointmentStatus")),
                                medicalRecord,
                                paymentId

                            ));
                        }
                    }
                }
            }
            return appointmentsList;

        }

        public static AllAppointmentDTO GetAppointmentByID(int appointmentId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAppointmentByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int? medicalRecord;
                        int? paymentId;
                        if (reader.Read())
                        {
                            if (reader.GetValue("MedicalRecordId") != DBNull.Value)
                                medicalRecord = reader.GetInt32(reader.GetOrdinal("DoctorId"));
                            else
                                medicalRecord = null;

                            if (reader.GetValue("PaymentId") != DBNull.Value)
                                paymentId = reader.GetInt32(reader.GetOrdinal("PaymentId"));
                            else
                                paymentId = null;

                            return(new AllAppointmentDTO
                            (

                                reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                                reader.GetInt32(reader.GetOrdinal("PatientId")),
                                reader.GetString(reader.GetOrdinal("PersonName")),
                                reader.GetInt32(reader.GetOrdinal("DoctorId")),
                                reader.GetString(reader.GetOrdinal("DoctorName")),
                                reader.GetString(reader.GetOrdinal("Specialization")),
                                reader.GetDateTime(reader.GetOrdinal("AppointmentDateTime")),
                                reader.GetString(reader.GetOrdinal("AppointmentStatus")),
                                medicalRecord,
                                paymentId

                            ));
                        }

                        else { return null; }
                    }
                }



            }



        }

        public static int AddNewAppointment(AppointmentDTO aDTO)
        {
            int appointmentId = -1;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_AddNewAppointment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@PersonID", aDTO.PersonID);
                    command.Parameters.AddWithValue("@PatientID", aDTO.PatientId);
                    command.Parameters.AddWithValue("@DoctorId", aDTO.DoctorId);
                    command.Parameters.AddWithValue("@AppointmentDate", aDTO.AppointmentDate);
                    command.Parameters.AddWithValue("@AppointmentStatus", aDTO.AppointmentStatus);
                    if(aDTO.MedicalRecordId == null)
                        command.Parameters.AddWithValue("@MedicalRecordID", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@MedicalRecordID", aDTO.MedicalRecordId);
                    if (aDTO.PaymentId == null)
                        command.Parameters.AddWithValue("@PaymentId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@PaymentId", aDTO.PaymentId);

                    var outPutIdParm = new SqlParameter("@NewAppointmentId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutIdParm);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        appointmentId = (int)outPutIdParm.Value;
                    }
                    catch (Exception ex)
                    {
                        
                    }                    
                    

                }

            }
            return appointmentId;
        }

        public static bool UpdateAppointment(AppointmentDTO aDTO)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_UpdateAppointment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AppointmentId", aDTO.Id);

                    command.Parameters.AddWithValue("@PatientID", aDTO.PatientId);
                    command.Parameters.AddWithValue("@DoctorId", aDTO.DoctorId);
                    command.Parameters.AddWithValue("@AppointmentDate", aDTO.AppointmentDate);
                    command.Parameters.AddWithValue("@AppointmentStatus", aDTO.AppointmentStatus);
                    if (aDTO.MedicalRecordId == null)
                        command.Parameters.AddWithValue("@MedicalRecordID", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@MedicalRecordID", aDTO.MedicalRecordId);
                    if (aDTO.PaymentId == null)
                        command.Parameters.AddWithValue("@PaymentId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@PaymentId", aDTO.PaymentId);


                    var outPutIdParm = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutIdParm);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        rowsAffected = (int)outPutIdParm.Value;
                    }
                    catch(Exception ex)
                    {

                    }
                    
                   

                }

            }
            return rowsAffected > 0;
        }

        public static bool DeleteAppointment(int appointmentId)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_DeleteAppointment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    try
                    {
                        connection.Open();
                        rowsAffected = (int)command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                    }
                    
                }

            }
            return rowsAffected > 0;
        }

        public static bool IsAppointmentExist(int appointmentId)
        {
            
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckAppointmentExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    connection.Open();
                    command.ExecuteNonQuery();

                    isExist = (int)returnParameter.Value != 0;    
                }
            }
            return isExist;

        }

        public static bool IsAppointmentHasRelations(int appointmentId)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckAppointmentRelations", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    connection.Open();
                    command.ExecuteNonQuery();

                    isExist = (int)returnParameter.Value != 0;
                }
            }
            return isExist;

        }

        public static bool IsPatientHasActiveAppointmentWithDoctor(int paitnetId, int doctorId)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_IsPatientHasActiveAppointmentWithSameDr", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PatientId", paitnetId);
                    command.Parameters.AddWithValue("@DoctorId", doctorId);


                    SqlParameter returnParameter = new SqlParameter();
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    connection.Open();
                    command.ExecuteNonQuery();

                    isExist = (int)returnParameter.Value != 0;
                }
            }
            return isExist;

        }

        public static List<AllAppointmentDTO> GetAllPatientAppointments(int patientId)
        {
            var appointmentsList = new List<AllAppointmentDTO>();


            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPatientAppointments", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PatientId", patientId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int? medicalRecord;
                        int? paymentId;
                        while (reader.Read())
                        {
                            if (reader.GetValue("MedicalRecordId") != DBNull.Value)
                                medicalRecord = reader.GetInt32(reader.GetOrdinal("DoctorId"));
                            else
                                medicalRecord = null;

                            if (reader.GetValue("PaymentId") != DBNull.Value)
                                paymentId = reader.GetInt32(reader.GetOrdinal("PaymentId"));
                            else
                                paymentId = null;

                            appointmentsList.Add(new AllAppointmentDTO
                            (

                                reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                                reader.GetInt32(reader.GetOrdinal("PatientId")),
                                reader.GetString(reader.GetOrdinal("PersonName")),
                                reader.GetInt32(reader.GetOrdinal("DoctorId")),
                                reader.GetString(reader.GetOrdinal("DoctorName")),
                                reader.GetString(reader.GetOrdinal("Specialization")),
                                reader.GetDateTime(reader.GetOrdinal("AppointmentDateTime")),
                                reader.GetString(reader.GetOrdinal("AppointmentStatus")),
                                medicalRecord,
                                paymentId

                            ));
                        }
                    }
                }
            }
            return appointmentsList;

        }



    }
}
