using Microsoft.Data.SqlClient;
using PersonsAPIBusinessLayer.Users;
using PersonsAPIDataAccessLayer.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Payments
{
    public class PaymentData
    {
        public static List<PaymentDTOWithName> GetAllpayments()
        {
            List<PaymentDTOWithName> paymentsList = new List<PaymentDTOWithName>();

            using (SqlConnection conn = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPayments", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string notes;
                            if (reader.GetValue("AdditionalNotes") != DBNull.Value)
                                notes = reader.GetString(reader.GetOrdinal("AdditionalNotes"));
                            else
                                notes = null;

                            paymentsList.Add(new PaymentDTOWithName
                            (
                                reader.GetInt32(reader.GetOrdinal("PaymentId")),
                                reader.GetString(reader.GetOrdinal("PaidPatient")),
                                reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                                reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                reader.GetDecimal(reader.GetOrdinal("AmountPaid")),
                                notes
                            ));
                        }
                    }
                }

            }

            return paymentsList;
        }

        public static PaymentDTOWithName GetPaymentById(int paymentId)
        {

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPaymentById", connection))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PaymentId", paymentId);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string notes;
                            if (reader.GetValue("AdditionalNotes") != DBNull.Value)
                                notes = reader.GetString(reader.GetOrdinal("AdditionalNotes"));
                            else
                                notes = null;
                               
                            return new PaymentDTOWithName(
                                reader.GetInt32(reader.GetOrdinal("PaymentId")),
                                reader.GetString(reader.GetOrdinal("PaidPatient")),
                                reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                                reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                reader.GetDecimal(reader.GetOrdinal("AmountPaid")),
                                notes
                                );

                        }
                        else
                            return null;
                    }


                }
            }

        }

        public static int AddNewPayment(PaymentDTO pDTO)
        {
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            using (var command = new SqlCommand("SP_AddNewPayment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PaymentDate", pDTO.PaymentDate);
                command.Parameters.AddWithValue("@AmountPaid", pDTO.AmountPaid);
                command.Parameters.AddWithValue("@PaymentMethod", pDTO.PaymentMethod);
                if (pDTO.PaymentMethod  != null)
                    command.Parameters.AddWithValue("@AdditionalNotes", pDTO.PaymentMethod);
                else
                    command.Parameters.AddWithValue("@AdditionalNotes", DBNull.Value);




                var outputIdParam = new SqlParameter("@NewPaymentId", SqlDbType.Int)
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

        public static bool UpdatePayment(PaymentDTO pDTO)
        {
            int rowsAffected = 0;
            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            using (var command = new SqlCommand("SP_UpdatePayment", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@PaymentId", pDTO.Id);
                command.Parameters.AddWithValue("@PaymentDate", pDTO.PaymentDate);
                command.Parameters.AddWithValue("@AmountPaid", pDTO.AmountPaid);
                command.Parameters.AddWithValue("@PaymentMethod", pDTO.PaymentMethod);
                if (pDTO.PaymentMethod != null)
                    command.Parameters.AddWithValue("@AdditionalNotes", pDTO.AdditionalNotes);
                else
                    command.Parameters.AddWithValue("@AdditionalNotes", DBNull.Value);




                var outputIdParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    rowsAffected = (int)outputIdParam.Value;
                }
                catch (Exception ex)
                {

                }
            }
            return rowsAffected > 0;
        }

        public static bool DeletePayment(int paymentId)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(ConnectionClass.ConnectionString))
            {
                using (var command = new SqlCommand("SP_DeletePayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentId", paymentId);
                    try
                    {
                        connection.Open();
                        rowsAffected = (int)command.ExecuteScalar();

                    }
                    catch (SqlException ex)
                    {
                        rowsAffected = 0;
                    }

                }
            }
            return (rowsAffected == 1);
        }

        public static bool IsPaymentExists(int paymentId)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckPaymentExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentId", paymentId);

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

        public static bool IsPaymentHasRelations(int paymentId)
        {

            bool isExist = false;

            using (SqlConnection connection = new SqlConnection(ConnectionClass.ConnectionString))
            {

                using (SqlCommand command = new SqlCommand("SP_CheckPaymentRelations", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentId", paymentId);

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
