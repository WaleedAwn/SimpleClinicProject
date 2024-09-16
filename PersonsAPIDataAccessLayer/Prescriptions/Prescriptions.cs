using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PersonsAPIDataAccessLayer;

namespace PersonsAPIDataAccessLayer.Prescriptions
{
    public class PrescriptionsDTO
    {
        public PrescriptionsDTO(int prescriptionID, int MedicalRecordID, string MedicationName, string Dosage, string Frequency, DateTime StartDate, DateTime EndDate, string? SpecialInstructions)
        {
            this.PrescriptionID = prescriptionID;
            this.MedicalRecordID = MedicalRecordID;
            this.MedicationName = MedicationName;
            this.Dosage = Dosage;
            this.Frequency = Frequency;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.SpecialInstructions = SpecialInstructions;

        }

        public int PrescriptionID { get; set; }
        public int MedicalRecordID { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? SpecialInstructions { get; set; }

    }

    public class PrescriptionsData
    {

        public static List<PrescriptionsDTO> GetAllPrescriptions()
        {
            var sList = new List<PrescriptionsDTO>();


            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPrescriptions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sList.Add(new PrescriptionsDTO
                            (
                                //(PrescriptionID, MedicalRecordID, MedicationName, Dosage, Frequency, StartDate, EndDate
                                reader.GetInt32(reader.GetOrdinal("PrescriptionID")),
                                reader.GetInt32(reader.GetOrdinal("MedicalRecordID")),
                                reader.GetString(reader.GetOrdinal("MedicationName")),
                                reader.GetString(reader.GetOrdinal("Dosage")),
                                reader.GetString(reader.GetOrdinal("Frequency")),
                                reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                reader.GetString(reader.GetOrdinal("SpecialInstructions"))


                            ));
                        }
                    }
                }
                conn.Close();

                return sList;
            }



        }

        public static PrescriptionsDTO GetByPrescriptionID(int PrescriptionID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPrescriptionByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PrescriptionsDTO
                             (

                                 reader.GetInt32(reader.GetOrdinal("PrescriptionID")),
                                reader.GetInt32(reader.GetOrdinal("MedicalRecordID")),
                                reader.GetString(reader.GetOrdinal("MedicationName")),
                                reader.GetString(reader.GetOrdinal("Dosage")),
                                reader.GetString(reader.GetOrdinal("Frequency")),
                                reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                reader.GetString(reader.GetOrdinal("SpecialInstructions"))


                             );
                        }

                        else { return null; }
                    }
                }



            }



        }

        public static int AddNewPrescription(PrescriptionsDTO NewDTOInfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_AddNewPrescription", connection))
                {
                    //(PrescriptionID, Name, MedicationName, Dosage, Frequency, StartDate, EndDate
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue("@PrescriptionID", NewDTOInfo.PrescriptionID);
                    command.Parameters.AddWithValue("MedicalRecordID", NewDTOInfo.MedicalRecordID);
                    command.Parameters.AddWithValue("@MedicationName", NewDTOInfo.MedicationName);
                    command.Parameters.AddWithValue("@Dosage", NewDTOInfo.Dosage);
                    command.Parameters.AddWithValue("@Frequency", NewDTOInfo.Frequency);
                    command.Parameters.AddWithValue("@StartDate", NewDTOInfo.StartDate.Date);
                    command.Parameters.AddWithValue("@EndDate", NewDTOInfo.EndDate.Date);
                    command.Parameters.AddWithValue("@SpecialInstructions", NewDTOInfo.SpecialInstructions);

                    var outPutPrescriptionIDParm = new SqlParameter("@NewPrescriptionID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutPrescriptionIDParm);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)outPutPrescriptionIDParm.Value;

                }

            }

        }

        public static bool UpdatePrescription(PrescriptionsDTO UpdateDTOinfo)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_UpdatePrescription", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PrescriptionID", UpdateDTOinfo.PrescriptionID);
                    command.Parameters.AddWithValue("MedicalRecordID", UpdateDTOinfo.MedicalRecordID);
                    command.Parameters.AddWithValue("@MedicationName", UpdateDTOinfo.MedicationName);
                    command.Parameters.AddWithValue("@Dosage", UpdateDTOinfo.Dosage);
                    command.Parameters.AddWithValue("@Frequency", UpdateDTOinfo.Frequency);
                    command.Parameters.AddWithValue("@StartDate", UpdateDTOinfo.StartDate);
                    command.Parameters.AddWithValue("@EndDate", UpdateDTOinfo.EndDate);
                    command.Parameters.AddWithValue("@SpecialInstructions", UpdateDTOinfo.SpecialInstructions);

                    var outPutPrescriptionIDParm = new SqlParameter("@RowsAffected", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outPutPrescriptionIDParm);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return (int)outPutPrescriptionIDParm.Value > 0;

                }

            }
        }

        public static bool DeletePrescription(int PrescriptionID)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_DeletePrescription", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);


                    connection.Open();

                    rowsAffected = (int)command.ExecuteScalar();
                    return rowsAffected == 1;
                }

            }




        }


        public static bool IsExistPrescription(int PrescriptionID)
        {
            int isFound = 0;
            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckPrescriptionExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);

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

        //public static bool IsPrescriptionHasRelation(int PrescriptionID)
        //{
        //    bool isFound = false;
        //    int returnValue = 0;

        //    using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
        //    {

        //        using (SqlCommand command = new SqlCommand("SP_CheckPrescriptionRelations", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@PrescriptionID", PrescriptionID);

        //            SqlParameter returnParameter = new SqlParameter();
        //            returnParameter.Direction = ParameterDirection.ReturnValue;
        //            command.Parameters.Add(returnParameter);

        //            connection.Open();
        //            command.ExecuteNonQuery();
        //            returnValue = (int)returnParameter.Value;
        //            isFound = returnValue != 0;
        //        }
        //    }
        //    return isFound;

        //}



    }

}
