using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PersonsAPIDataAccessLayer;

namespace PersonsAPIDataAccessLayer.MedicalRecords
{
    
        public class MedicalRecordsDTO
        {
            public MedicalRecordsDTO(int medicalRecordID, string visitDescription, string diagnosis, string additionalNotes)
            {
                MedicalRecordID = medicalRecordID;
                VisitDescription = visitDescription;
                Diagnosis = diagnosis;
                this.AdditionalNotes = additionalNotes;

            }

            public int MedicalRecordID { get; set; }
            public string VisitDescription { get; set; }
            public string Diagnosis { get; set; }
            public string AdditionalNotes { get; set; }


        }
        public class MedicalRecordsData
        {

            public static List<MedicalRecordsDTO> GetAllMedicalRecords()
            {
                var MedicalRecordsList = new List<MedicalRecordsDTO>();


                using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetAllMedicalRecords", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MedicalRecordsList.Add(new MedicalRecordsDTO
                                (
                                    //(MedicalRecordID, Name, DateOfBirth, Diagnosis, AdditionalNotes, Email, Address
                                    reader.GetInt32(reader.GetOrdinal("MedicalRecordID")),
                                    reader.GetString(reader.GetOrdinal("VisitDescription")),

                                    reader.GetString(reader.GetOrdinal("Diagnosis")),
                                    reader.GetString(reader.GetOrdinal("AdditionalNotes"))



                                ));
                            }
                        }
                    }
                    conn.Close();

                    return MedicalRecordsList;
                }



            }

            public static MedicalRecordsDTO GetMedicalRecordByID(int MedicalRecordID)
            {
                using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetMedicalRecordByID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MedicalRecordID", MedicalRecordID);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new MedicalRecordsDTO
                                 (

                                     reader.GetInt32(reader.GetOrdinal("MedicalRecordID")),
                                     reader.GetString(reader.GetOrdinal("VisitDescription")),
                                     reader.GetString(reader.GetOrdinal("Diagnosis")),
                                     reader.GetString(reader.GetOrdinal("AdditionalNotes"))




                                 );
                            }

                            else { return null; }
                        }
                    }



                }



            }

            public static int AddNewMedicalRecord(MedicalRecordsDTO NewMedicalRecordsInfo)
            {
                using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
                {
                    using (var command = new SqlCommand("SP_AddNewMedicalRecord", connection))
                    {
                        //(MedicalRecordID, VisitDescription, DateOfBirth, Diagnosis, AdditionalNotes, Email, Address
                        command.CommandType = CommandType.StoredProcedure;
                        //command.Parameters.AddWithValue("@MedicalRecordID", NewMedicalRecordsInfo.MedicalRecordID);
                        command.Parameters.AddWithValue("@VisitDescription", NewMedicalRecordsInfo.VisitDescription);
                        command.Parameters.AddWithValue("@Diagnosis", NewMedicalRecordsInfo.Diagnosis);
                        command.Parameters.AddWithValue("@AdditionalNotes", NewMedicalRecordsInfo.AdditionalNotes);

                        var outPutIdParm = new SqlParameter("@NewMedicalRecordID", SqlDbType.Int)
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

            public static bool UpdateMedicalRecord(MedicalRecordsDTO UpdateMedicalRecordsinfo)
            {
                using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
                {
                    using (var command = new SqlCommand("SP_UpdateMedicalRecord", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@MedicalRecordID", UpdateMedicalRecordsinfo.MedicalRecordID);
                        command.Parameters.AddWithValue("@VisitDescription", UpdateMedicalRecordsinfo.VisitDescription);
                        command.Parameters.AddWithValue("@Diagnosis", UpdateMedicalRecordsinfo.Diagnosis);
                        command.Parameters.AddWithValue("@AdditionalNotes", UpdateMedicalRecordsinfo.AdditionalNotes);

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

            public static bool DeleteMedicalRecord(int MedicalRecordID)
            {
                int rowsAffected = 0;
                using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
                {
                    using (var command = new SqlCommand("SP_DeleteMedicalRecord", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MedicalRecordID", MedicalRecordID);


                        connection.Open();

                        rowsAffected = (int)command.ExecuteScalar();
                        return rowsAffected == 1;
                    }

                }




            }


            public static bool IsMedicalRecordExist(int MedicalRecordID)
            {
                int isFound = 0;
                bool isExist = false;

                using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_CheckMedicalRecordExists", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MedicalRecordID", MedicalRecordID);

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




        }
    
}
