using System.Collections.Generic;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MSSPAPI.Models;
using Newtonsoft.Json;
using System.Xml.Linq;
using Microsoft.Ajax.Utilities;
using Antlr.Runtime.Misc;
using Antlr.Runtime;
using System.Reflection;
using System.Diagnostics.Contracts;
using System.IdentityModel.Protocols.WSTrust;
using System.Xml.Serialization;
using Twilio.Types;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.Design;
using System.Windows.Markup;

namespace MSSPAPI.Helpers
{
    /// <summary>
    /// Clase dedicada a las operaciones en base de datos
    /// </summary>
    public static class DBOperations
    {
        private static SqlConnection cn = new SqlConnection(EncDec.Decript(ConfigurationManager.ConnectionStrings["MSSPDB"].ToString()));
        private static SqlConnection cnp = new SqlConnection(EncDec.Decript(ConfigurationManager.ConnectionStrings["PIFDB"].ToString()));
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool InsertTrackingSisNet(SisNetTracking snt)
        {
            try
            {
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de Tracking SisNet Data";
                string chis = JsonConvert.SerializeObject(ch);
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertSisNetTracking", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Command", SqlDbType.VarChar);
                    param.Value = snt.Command;
                    param = sqlCommandInsert.Parameters.Add("@StatusCode", SqlDbType.VarChar);
                    param.Value = snt.StCo;
                    param = sqlCommandInsert.Parameters.Add("@Request", SqlDbType.VarChar);
                    param.Value = snt.Rqt;
                    param = sqlCommandInsert.Parameters.Add("@Response", SqlDbType.VarChar);
                    param.Value = snt.Rsn;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public static List<Marcas> GetAllMarcas()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<Marcas> list = new List<Marcas>();
            try
            {
                SqlCommand sqlCommand = new SqlCommand("sp_GetAllBranches", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string configura = string.Empty;
                    Marcas marca = new Marcas();
                    marca.Id = (int)reader["Id"];
                    marca.IdCliente = (int)reader["IdCliente"];
                    marca.Branch = reader["Brand"] == DBNull.Value ? string.Empty : (string)reader["Brand"];
                    configura = reader["Configuration"] == DBNull.Value ? null : (string)reader["Configuration"];
                    marca.CrudHistoyList = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    marca.TokenEnrollment = reader["Token"] == DBNull.Value ? string.Empty : (string)reader["Token"];
                    marca.FechaInicio = reader["FechaInicio"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["FechaInicio"];
                    marca.FechaFin = reader["FechaFin"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["FechaFin"];
                    marca.Active = (bool)reader["Active"];
                    marca.Configuration = JsonConvert.DeserializeObject<ConfigurationBranch>(configura);
                    list.Add(marca);
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static Marcas GetOneBranch(string m)
        {
            Marcas marca = new Marcas();
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                ConfigurationBranch cb = new ConfigurationBranch();

                SqlCommand sqlCommand = new SqlCommand("sp_GetOneBranch", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = sqlCommand.Parameters.Add("@Branch", SqlDbType.NVarChar);
                param.Value = m;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string configura = string.Empty;

                    marca.Id = (int)reader["Id"];
                    marca.IdCliente = (int)reader["IdCliente"];
                    marca.Branch = reader["Brand"] == DBNull.Value ? string.Empty : (string)reader["Brand"];
                    configura = reader["Configuration"] == DBNull.Value ? null : (string)reader["Configuration"];
                    marca.CrudHistoyList = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    marca.TokenEnrollment = reader["Token"] == DBNull.Value ? string.Empty : (string)reader["Token"];
                    marca.FechaInicio = reader["FechaInicio"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["FechaInicio"];
                    marca.FechaFin = reader["FechaFin"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["FechaFin"];
                    marca.Active = (bool)reader["Active"];
                    marca.Configuration = JsonConvert.DeserializeObject<ConfigurationBranch>(configura);

                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return marca;
        }

        /// <summary>
        /// Creacion de una nueva marca
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        internal static bool InsertBranches(Marcas mc)
        {
            try
            {
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de nueva Marca Data";
                string chis = JsonConvert.SerializeObject(ch);
                string config = JsonConvert.SerializeObject(mc.Configuration);
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertBranches", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = mc.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@Branches", SqlDbType.VarChar);
                    param.Value = mc.Branch;
                    param = sqlCommandInsert.Parameters.Add("@Configuration", SqlDbType.VarChar);
                    param.Value = config;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }
        /// <summary>
        /// Update Branches 
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        internal static int UpdateBranches(Marcas cf)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateBranches", cn))
                {
                    CrudHistory ch = new CrudHistory();
                    ch.Date = DateTime.Now;
                    ch.Type = 'U';
                    ch.IdUser = 0;
                    ch.Comments = "Update de Branch Data";
                    string chis = JsonConvert.SerializeObject(ch);
                    string config = JsonConvert.SerializeObject(cf.Configuration);

                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@Id", SqlDbType.Int);
                    param.Value = cf.Id;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = cf.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@Branches", SqlDbType.VarChar);
                    param.Value = cf.Branch;
                    param = sqlCommandInsert.Parameters.Add("@Configuration", SqlDbType.VarChar);
                    param.Value = config;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = cf.Active;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Actualiza el token de login de enrollment
        /// </summary>
        /// <param name="tkn"></param>
        /// <param name="fInicio"></param>
        /// <param name="fFin"></param>
        /// <returns></returns>
        internal static int UpdateTokenEnrollment(string tkn, DateTime fInicio, DateTime fFin)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateTokenEnrollment", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@Token", SqlDbType.NVarChar);
                    param.Value = tkn;
                    param = sqlCommandInsert.Parameters.Add("@FechaInicio", SqlDbType.Date);
                    param.Value = fInicio;
                    param = sqlCommandInsert.Parameters.Add("@FechaFin", SqlDbType.Date);
                    param.Value = fFin;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Delete Branches 
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        internal static int DeleteBranches(int bra)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_DeleteBranches", cn))
                {
                    CrudHistory ch = new CrudHistory();
                    ch.Date = DateTime.Now;
                    ch.Type = 'D';
                    ch.IdUser = 0;
                    ch.Comments = "Delete Branch Data";
                    string chis = JsonConvert.SerializeObject(ch);

                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@brand", SqlDbType.Int);
                    param.Value = bra;
                    param.Value = bra;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cvs"></param>
        /// <returns></returns>
        internal static bool InsertClaves(Claves cvs)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertClave", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = cvs.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@Clave", SqlDbType.VarChar);
                    param.Value = cvs.Clave;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<AIZPay> GetAllAIZPay()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<AIZPay> list = new List<AIZPay>();
            try
            {
                SqlCommand sqlCommand = new SqlCommand("sp_GetAllAIZPay", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    AIZPay clave = new AIZPay();
                    clave.Id = (int)reader["Id"];
                    clave.IdVendor = (int)reader["IdVendor"];
                    clave.AIZPayData = reader["AIZPayData"] == DBNull.Value ? string.Empty : (string)reader["AIZPayData"];
                    clave.CrudHistoryList = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    clave.Active = (bool)reader["Active"];
                    list.Add(clave);
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<AIZPay> GetAIZPayByVendor(int IdVendor)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<AIZPay> list = new List<AIZPay>();
            try
            {


                SqlCommand sqlCommand = new SqlCommand("sp_GetAIZPayByVendor", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = sqlCommand.Parameters.Add("@IdVendor", SqlDbType.Int);
                param.Value = IdVendor;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    AIZPay clave = new AIZPay();
                    clave.Id = (int)reader["Id"];
                    clave.IdVendor = (int)reader["IdVendor"];
                    clave.AIZPayData = reader["AIZPayData"] == DBNull.Value ? string.Empty : (string)reader["AIZPayData"];
                    clave.CrudHistoryList = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    clave.Active = (bool)reader["Active"];
                    list.Add(clave);
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cvs"></param>
        /// <returns></returns>
        internal static bool InsertAIZPay(AIZPay cvs)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de Issuance Data";
                string chis = JsonConvert.SerializeObject(ch);

                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertAIZPay", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdVendor", SqlDbType.Int);
                    param.Value = cvs.IdVendor;
                    param = sqlCommandInsert.Parameters.Add("@AIZPayData", SqlDbType.VarChar);
                    param.Value = cvs.AIZPayData;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// updatw de cliente facturacion
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        internal static int UpdateAIZPay(AIZPay cf)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateAIZPay", cn))
                {
                    CrudHistory ch = new CrudHistory();
                    ch.Date = DateTime.Now;
                    ch.Type = 'U';
                    ch.IdUser = 0;
                    ch.Comments = "Update de AIZPay Data";
                    string chis = JsonConvert.SerializeObject(ch);

                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@Id", SqlDbType.Int);
                    param.Value = cf.Id;
                    param = sqlCommandInsert.Parameters.Add("@IdVendor", SqlDbType.Int);
                    param.Value = cf.IdVendor;
                    param = sqlCommandInsert.Parameters.Add("@AIZPayData", SqlDbType.VarChar);
                    param.Value = cf.AIZPayData;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = cf.Active;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// updatw de cliente facturacion
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        internal static int DeleteAIZPay(int cf)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            try
            {
                string issuance = JsonConvert.SerializeObject(cf);
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'U';
                ch.IdUser = 0;
                ch.Comments = "Delete de AIZPay Data";
                string chis = JsonConvert.SerializeObject(ch);
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_DeleteAIZPay", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Id", SqlDbType.VarChar);
                    param.Value = cf;
                    int rowsAffected = (int)sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        /// <summary>
        /// Insercion de cliente facturacion
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        internal static int InsertClienteFact(ClienteFacturacion cf)
        {
            try
            {

                string issuance = JsonConvert.SerializeObject(cf);
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de Issuance Data";
                string chis = JsonConvert.SerializeObject(ch);
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertClienteFacturacion", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@IssuanceData", SqlDbType.VarChar);
                    param.Value = issuance;
                    param = sqlCommandInsert.Parameters.Add("@XMLResponse", SqlDbType.VarChar);
                    param.Value = cf.XMLResponse;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteScalar();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// updatw de cliente facturacion
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        internal static int UpdateClienteFact(ClienteFacturacion cf)
        {
            int rowsAffected = 0;
            try
            {

                string issuance = JsonConvert.SerializeObject(cf);
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de Issuance Data";
                string chis = JsonConvert.SerializeObject(ch);
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClienteFacturacion", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlCommandBuilder.DeriveParameters(sqlCommandInsert);


                    sqlCommandInsert.Parameters["@Id"].Value = cf.Id;
                    sqlCommandInsert.Parameters["@URLCertificate"].Value = cf.URLCertificate;
                    sqlCommandInsert.Parameters["@InvoiceXML"].Value = cf.InvoiceXML;
                    sqlCommandInsert.Parameters["@InvoicePDF"].Value = cf.InvoicePDF;

                    sqlCommandInsert.ExecuteNonQuery();

                    rowsAffected = (int)sqlCommandInsert.Parameters["@RETURN_VALUE"].Value;

                    //SqlParameter param;

                    //param = sqlCommandInsert.Parameters.Add("@Id", SqlDbType.VarChar);
                    //param.Value = cf.Id;
                    //param = sqlCommandInsert.Parameters.Add("@URLCertificate", SqlDbType.VarChar);
                    //param.Value = cf.URLCertificate;
                    //param = sqlCommandInsert.Parameters.Add("@InvoiceXML", SqlDbType.VarChar);
                    //param.Value = cf.InvoiceXML;
                    //param = sqlCommandInsert.Parameters.Add("@InvoicePDF", SqlDbType.VarChar);
                    //param.Value = cf.InvoicePDF;

                    //int rowsAffected = (int)sqlCommandInsert.ExecuteScalar();
                    //if (rowsAffected > 0) return rowsAffected;
                    //else return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return rowsAffected;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<ClienteFacturacion> GetAllClientesFact(DateTime f1, DateTime f2)
        {

            List<ClienteFacturacion> list = new List<ClienteFacturacion>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                ClienteFacturacion clave = new ClienteFacturacion();
                SqlCommand sqlCommand = new SqlCommand("sp_GetAllClientesFact", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = sqlCommand.Parameters.Add("@F1", SqlDbType.Date);
                param.Value = f1;
                param = sqlCommand.Parameters.Add("@F2", SqlDbType.Date);
                param.Value = f2;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    string issuance = String.Empty;
                    string xmlResp = String.Empty;
                    string crud = String.Empty;
                    bool active = true;
                    int idIssuance = 0;
                    idIssuance = (int)reader["Id"];
                    issuance = reader["IssuanceData"] == DBNull.Value ? string.Empty : (string)reader["IssuanceData"];
                    xmlResp = reader["XMLResponse"] == DBNull.Value ? string.Empty : (string)reader["XMLResponse"];
                    crud = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    active = (bool)reader["Active"];
                    clave = JsonConvert.DeserializeObject<ClienteFacturacion>(issuance);
                    list.Add(clave);
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        public static List<Vehicles> GetAllVehicles()
        {

            List<Vehicles> list = new List<Vehicles>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();

                SqlCommand sqlCommand = new SqlCommand("sp_GetAllVehicles", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Vehicles clave = new Vehicles();
                    clave.Id = (int)reader["Id"];
                    clave.IdCliente = (int)reader["IIdCliented"];
                    clave.IdProgram = (int)reader["IdProgram"];
                    clave.Cliente = reader["Cliente"] == DBNull.Value ? string.Empty : (string)reader["Cliente"];
                    clave.IdModel = (int)reader["IdProgram"];
                    clave.Model = reader["Model"] == DBNull.Value ? string.Empty : (string)reader["Model"];
                    clave.IdVehicleVersion = (int)reader["IdVehicleVersion"];
                    clave.VehicleVersion = reader["VehicleVersion"] == DBNull.Value ? string.Empty : (string)reader["VehicleVersion"];
                    clave.IdYear = (int)reader["IdYear"];
                    clave.Year = reader["Year"] == DBNull.Value ? string.Empty : (string)reader["Year"];
                    clave.EngineNumber = reader["EngineNumber"] == DBNull.Value ? string.Empty : (string)reader["EngineNumber"];
                    clave.Kilometers = (int)reader["Kilometers"];
                    clave.VIN = reader["VIN"] == DBNull.Value ? string.Empty : (string)reader["VIN"];
                    clave.IdVehicleUse = (int)reader["IdVehicleUse"];
                    clave.VehicleUse = reader["VehicleUse"] == DBNull.Value ? string.Empty : (string)reader["VehicleUse"];
                    clave.Active = (bool)reader["Active"];
                    list.Add(clave);
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        internal static Vehicles GetVehiclesById(int v)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            Vehicles cl = new Vehicles();
            try
            {
                string SQL = "sp_GetVehiclesById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@vei", SqlDbType.Int);
                param.Value = v;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cl.Id = (int)reader["Id"];
                    cl.IdCliente = (int)reader["IIdCliented"];
                    cl.IdProgram = (int)reader["IdProgram"];
                    cl.Cliente = reader["Cliente"] == DBNull.Value ? string.Empty : (string)reader["Cliente"];
                    cl.IdModel = (int)reader["IdProgram"];
                    cl.Model = reader["Model"] == DBNull.Value ? string.Empty : (string)reader["Model"];
                    cl.IdVehicleVersion = (int)reader["IdVehicleVersion"];
                    cl.VehicleVersion = reader["VehicleVersion"] == DBNull.Value ? string.Empty : (string)reader["VehicleVersion"];
                    cl.IdYear = (int)reader["IdYear"];
                    cl.Year = reader["Year"] == DBNull.Value ? string.Empty : (string)reader["Year"];
                    cl.EngineNumber = reader["EngineNumber"] == DBNull.Value ? string.Empty : (string)reader["EngineNumber"];
                    cl.Kilometers = (int)reader["Kilometers"];
                    cl.VIN = reader["VIN"] == DBNull.Value ? string.Empty : (string)reader["VIN"];
                    cl.IdVehicleUse = (int)reader["IdVehicleUse"];
                    cl.VehicleUse = reader["VehicleUse"] == DBNull.Value ? string.Empty : (string)reader["VehicleUse"];
                    cl.Active = (bool)reader["Active"];
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        internal static Model GetModelsById(int mdl)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            Model cl = new Model();
            try
            {
                int ids = 0;
                string desc = String.Empty;
                string SQL = "sp_GetModelsById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@mdl", SqlDbType.Int);
                param.Value = mdl;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids = (int)reader["Id"];
                    desc = reader["Description"] == DBNull.Value ? string.Empty : (string)reader["Description"];
                }
                cl.IdModel = ids;
                cl.Models = desc;
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        internal static VehicleUse GetVehicleUseById(int vus)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            VehicleUse cl = new VehicleUse();
            try
            {
                int ids = 0;
                string desc = String.Empty;
                string SQL = "sp_GetVehicleUseById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@vei", SqlDbType.Int);
                param.Value = vus;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids = (int)reader["Id"];
                    desc = reader["Description"] == DBNull.Value ? string.Empty : (string)reader["Description"];
                }
                cl.IdVehicleUse = ids;
                cl.VehicleUses = desc;
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        internal static VehicleVersion GetVehicleVersionById(int vers, int mod)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            VehicleVersion cl = new VehicleVersion();
            try
            {
                int ids = 0;
                string desc = String.Empty;
                string SQL = "sp_GetVehicleUseById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@vei", SqlDbType.Int);
                param.Value = vers;
                param = cmd.Parameters.Add("@mdl", SqlDbType.Int);
                param.Value = mod;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids = (int)reader["Id"];
                    desc = reader["Description"] == DBNull.Value ? string.Empty : (string)reader["Description"];
                }
                cl.IdVehicleVersion = ids;
                cl.VehicleVersions = desc;
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }


        /// <summary>
        /// obtencion del cliente facturacion
        /// </summary>
        /// <param name="fac"></param>
        /// <returns></returns>
        internal static ClienteFacturacion GetClientesFactById(int fac)
        {
            string issuance = String.Empty;
            string xmlResp = String.Empty;
            string crud = String.Empty;
            bool active = true;
            int idIssuance = 0;
            if (cn.State == ConnectionState.Closed) cn.Open();
            ClienteFacturacion cl = new ClienteFacturacion();
            try
            {
                string SQL = "sp_GetClienteFactById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = fac;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    idIssuance = (int)reader["Id"];
                    issuance = reader["IssuanceData"] == DBNull.Value ? string.Empty : (string)reader["IssuanceData"];
                    xmlResp = reader["XMLResponse"] == DBNull.Value ? string.Empty : (string)reader["XMLResponse"];
                    crud = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    active = (bool)reader["Active"];
                }
                cl = JsonConvert.DeserializeObject<ClienteFacturacion>(issuance);

                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        internal static int InsertSenderFact(EmisoresFactura cf)
        {
            try
            {
                string senderdata = JsonConvert.SerializeObject(cf);
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de Sender Data";
                string chis = JsonConvert.SerializeObject(ch);
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertSenderFacturacion", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@SenderData", SqlDbType.VarChar);
                    param.Value = senderdata;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteScalar();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        internal static int InsertDataEvaluationKitt(DataEvaluation cf)
        {
            try
            {
                string daev = JsonConvert.SerializeObject(cf.DataEvaluations);
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = "Insercion de Evaluation Data";
                string chis = JsonConvert.SerializeObject(ch);
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertDataEvaluation", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@DataEvaluation", SqlDbType.VarChar);
                    param.Value = daev;
                    param = sqlCommandInsert.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                    param.Value = chis;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteScalar();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        internal static int InsertVehicles(Vehicles cf)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertVehicles", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = cf.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@IdProgram", SqlDbType.Int);
                    param.Value = cf.IdProgram;
                    param = sqlCommandInsert.Parameters.Add("@Cliente", SqlDbType.VarChar);
                    param.Value = cf.Cliente;
                    param = sqlCommandInsert.Parameters.Add("@IdModel", SqlDbType.Int);
                    param.Value = cf.IdModel;
                    param = sqlCommandInsert.Parameters.Add("@Model", SqlDbType.VarChar);
                    param.Value = cf.Model;
                    param = sqlCommandInsert.Parameters.Add("@IdVehicleVersion", SqlDbType.Int);
                    param.Value = cf.IdVehicleVersion;
                    param = sqlCommandInsert.Parameters.Add("@VehicleVersion", SqlDbType.VarChar);
                    param.Value = cf.VehicleVersion;
                    param = sqlCommandInsert.Parameters.Add("@IdYear", SqlDbType.Int);
                    param.Value = cf.IdYear;
                    param = sqlCommandInsert.Parameters.Add("@Year", SqlDbType.VarChar);
                    param.Value = cf.Year;
                    param = sqlCommandInsert.Parameters.Add("@EngineNumber", SqlDbType.VarChar);
                    param.Value = cf.EngineNumber;
                    param = sqlCommandInsert.Parameters.Add("@Kilometers", SqlDbType.Int);
                    param.Value = cf.Kilometers;
                    param = sqlCommandInsert.Parameters.Add("@VIN", SqlDbType.VarChar);
                    param.Value = cf.VIN;
                    param = sqlCommandInsert.Parameters.Add("@IdVehicleUse", SqlDbType.Int);
                    param.Value = cf.IdVehicleUse;
                    param = sqlCommandInsert.Parameters.Add("@VehicleUse", SqlDbType.VarChar);
                    param.Value = cf.VehicleUse;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteScalar();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        internal static int DeleteVehicles(int vehi)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_DeleteVehicles", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;

                    param = sqlCommandInsert.Parameters.Add("@vei", SqlDbType.Int);
                    param.Value = vehi;

                    int rowsAffected = (int)sqlCommandInsert.ExecuteScalar();
                    if (rowsAffected > 0) return rowsAffected;
                    else return rowsAffected;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Obtiene la data evaluation
        /// </summary>
        /// <returns></returns>
        public static List<DataEvaluation> GetDataEvaluationKitt()
        {

            List<DataEvaluation> list = new List<DataEvaluation>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDataEvaluationKitt", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataEvaluation obj = new DataEvaluation();
                            obj.Id = (int)reader["Id"];
                            obj.DataEvaluations = reader["DataEvaluation"] == DBNull.Value ? string.Empty : reader["DataEvaluation"].ToString();
                            obj.CrudHistryLust = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : reader["CrudHistoryList"].ToString();
                            obj.Active = (bool)reader["Active"];
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Obtiene la data evaluation
        /// </summary>
        /// <returns></returns>
        public static List<DataEvaluation> GetOneDataEvaluationKitt(int daev)
        {

            List<DataEvaluation> list = new List<DataEvaluation>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                SqlParameter param;

                using (SqlCommand cmd = new SqlCommand("sp_GetOneDataEvaluationKitt", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    param = cmd.Parameters.Add("@daev", SqlDbType.Int);
                    param.Value = daev;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataEvaluation obj = new DataEvaluation();
                            obj.Id = (int)reader["Id"];
                            obj.DataEvaluations = reader["DataEvaluation"] == DBNull.Value ? string.Empty : reader["DataEvaluation"].ToString();
                            obj.CrudHistryLust = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : reader["CrudHistoryList"].ToString();
                            obj.Active = (bool)reader["Active"];
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }


        internal static EmisoresFactura GetEmisoresFactById(int emis)
        {
            string emisorData = String.Empty;
            string crudH = String.Empty;
            bool active = true;
            int idEmi = 0;
            if (cn.State == ConnectionState.Closed) cn.Open();
            EmisoresFactura cl = new EmisoresFactura();
            try
            {
                string SQL = "sp_GetEmisoresFactById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@emi", SqlDbType.Int);
                param.Value = emis;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    idEmi = (int)reader["IdSender"];
                    emisorData = reader["SenderData"] == DBNull.Value ? string.Empty : (string)reader["SenderData"];
                    crudH = reader["CrudHistoryList"] == DBNull.Value ? string.Empty : (string)reader["CrudHistoryList"];
                    active = (bool)reader["Active"];
                }
                cl = JsonConvert.DeserializeObject<EmisoresFactura>(emisorData);
                cl.IdEmisores = idEmi;
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<Encuesta> GetEncuestaKitt()
        {

            List<Encuesta> list = new List<Encuesta>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetEncuestaKitt", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Encuesta obj = new Encuesta();
                            obj.Id = (int)reader["Id"];
                            obj.IdCteEnc = (int)reader["IdClienteEncuesta"];
                            obj.UURL = (bool)reader["UseURL"];
                            obj.URLPgts = reader["URL"] == DBNull.Value ? string.Empty : reader["URL"].ToString();
                            obj.Prgts = reader["Preguntas"] == DBNull.Value ? string.Empty : reader["Preguntas"].ToString();
                            obj.Respts = reader["Respuestas"] == DBNull.Value ? string.Empty : reader["Respuestas"].ToString();
                            obj.Active = (bool)reader["Active"];
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="cte"></param>
        /// <returns></returns>
        public static List<Encuesta> GetEncuestaKittByCliente(int cte)
        {

            List<Encuesta> list = new List<Encuesta>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetEncuestaByCliente", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = cmd.Parameters.Add("@cte", SqlDbType.Int);
                    param.Value = cte;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Encuesta obj = new Encuesta();
                            obj.Id = (int)reader["Id"];
                            obj.IdCteEnc = (int)reader["IdClienteEncuesta"];
                            obj.UURL = (bool)reader["UseURL"];
                            obj.URLPgts = reader["URL"] == DBNull.Value ? string.Empty : reader["URL"].ToString();
                            obj.Prgts = reader["Preguntas"] == DBNull.Value ? string.Empty : reader["Preguntas"].ToString();
                            obj.Respts = reader["Respuestas"] == DBNull.Value ? string.Empty : reader["Respuestas"].ToString();
                            obj.Active = (bool)reader["Active"];
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Inserta dealer code
        /// </summary>
        /// <returns></returns>
        public static bool InsertEncuestaKitt(Encuesta dc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertEncuestaKitt", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdClienteEncuesta", SqlDbType.Int);
                    param.Value = dc.IdCteEnc;
                    param = sqlCommandInsert.Parameters.Add("@UseURL", SqlDbType.Bit);
                    param.Value = dc.UURL;
                    param = sqlCommandInsert.Parameters.Add("@URL", SqlDbType.VarChar);
                    param.Value = dc.URLPgts;
                    param = sqlCommandInsert.Parameters.Add("@Preguntas", SqlDbType.VarChar);
                    param.Value = dc.Prgts;
                    param = sqlCommandInsert.Parameters.Add("@Respuestas", SqlDbType.VarChar);
                    param.Value = dc.Respts;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = dc.Active;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Actualiza el la tabla de dealer codes
        /// </summary>
        /// <returns></returns>
        public static bool UpdateEncuestaKitt(Encuesta dc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandUpdate = new SqlCommand("sp_UpdateEncuestaKitt", cn))
                {
                    sqlCommandUpdate.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandUpdate.Parameters.Add("@Id", SqlDbType.Int);
                    param.Value = dc.Id;
                    param = sqlCommandUpdate.Parameters.Add("@cte", SqlDbType.Int);
                    param.Value = dc.IdCteEnc;
                    param = sqlCommandUpdate.Parameters.Add("@UseUereele", SqlDbType.Bit);
                    param.Value = dc.UURL;
                    param = sqlCommandUpdate.Parameters.Add("@Uereele", SqlDbType.VarChar);
                    param.Value = dc.URLPgts;
                    param = sqlCommandUpdate.Parameters.Add("@Pgt", SqlDbType.VarChar);
                    param.Value = dc.Prgts;
                    param = sqlCommandUpdate.Parameters.Add("@Rps", SqlDbType.VarChar);
                    param.Value = dc.Respts;
                    param = sqlCommandUpdate.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = dc.Active;
                    int rowsAffected = sqlCommandUpdate.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Desactiva al cliente
        /// </summary>
        /// <returns></returns>
        public static bool DeleteEncuestaKitt(int enc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_DeleteEncuestaKitt", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@enc", SqlDbType.VarChar);
                    param.Value = enc;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<AvisoPrivacidad> GetAvisoPrivacidadKitt()
        {

            List<AvisoPrivacidad> list = new List<AvisoPrivacidad>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetAvisoPrivacidadKitt", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AvisoPrivacidad obj = new AvisoPrivacidad();
                            obj.Id = (int)reader["Id"];
                            obj.IdClienteAviso = (int)reader["IdClienteAviso"];
                            obj.UseURL = (bool)reader["UseURL"];
                            obj.URL = reader["URL"] == DBNull.Value ? string.Empty : reader["URL"].ToString();
                            obj.Aviso = reader["Aviso"] == DBNull.Value ? string.Empty : reader["Aviso"].ToString();
                            obj.Active = (bool)reader["Active"];
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="Idcte"></param>
        /// <returns></returns>
        public static List<AvisoPrivacidad> GetAvisoPrivacidadKittByCliente(int cte)
        {

            List<AvisoPrivacidad> list = new List<AvisoPrivacidad>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetAvisoPrivacidadKittByCliente", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = cmd.Parameters.Add("@cte", SqlDbType.Int);
                    param.Value = cte;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AvisoPrivacidad obj = new AvisoPrivacidad();
                            obj.Id = (int)reader["Id"];
                            obj.IdClienteAviso = (int)reader["IdClienteAviso"];
                            obj.UseURL = (bool)reader["UseURL"];
                            obj.URL = reader["URL"] == DBNull.Value ? string.Empty : reader["URL"].ToString();
                            obj.Aviso = reader["Aviso"] == DBNull.Value ? string.Empty : reader["Aviso"].ToString();
                            obj.Active = (bool)reader["Active"];
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Inserta dealer code
        /// </summary>
        /// <returns></returns>
        public static bool InsertAvisoPrivacidadKitt(AvisoPrivacidad dc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertAvisoPrivacidadKitt", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdClienteAviso", SqlDbType.Int);
                    param.Value = dc.IdClienteAviso;
                    param = sqlCommandInsert.Parameters.Add("@UseURL", SqlDbType.Bit);
                    param.Value = dc.UseURL;
                    param = sqlCommandInsert.Parameters.Add("@URL", SqlDbType.VarChar);
                    param.Value = dc.URL;
                    param = sqlCommandInsert.Parameters.Add("@Aviso", SqlDbType.VarChar);
                    param.Value = dc.Aviso;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = dc.Active;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Actualiza el la tabla de dealer codes
        /// </summary>
        /// <returns></returns>
        public static bool UpdateAvisoPrivacidadKitt(AvisoPrivacidad dc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandUpdate = new SqlCommand("sp_UpdateAvisoPrivacidadKitt", cn))
                {
                    sqlCommandUpdate.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandUpdate.Parameters.Add("@Id", SqlDbType.Int);
                    param.Value = dc.Id;
                    param = sqlCommandUpdate.Parameters.Add("@IdClienteAviso", SqlDbType.Int);
                    param.Value = dc.IdClienteAviso;
                    param = sqlCommandUpdate.Parameters.Add("@UseURL", SqlDbType.Bit);
                    param.Value = dc.UseURL;
                    param = sqlCommandUpdate.Parameters.Add("@URL", SqlDbType.VarChar);
                    param.Value = dc.URL;
                    param = sqlCommandUpdate.Parameters.Add("@Aviso", SqlDbType.VarChar);
                    param.Value = dc.Aviso;
                    param = sqlCommandUpdate.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = dc.Active;
                    int rowsAffected = sqlCommandUpdate.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Desactiva al cliente
        /// </summary>
        /// <returns></returns>
        public static bool DeleteAvisoPrivacidadKitt(int avs)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_DeleteAvisoPrivacidadKitt", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@avs", SqlDbType.VarChar);
                    param.Value = avs;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// obtencion del cliente facturacion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static ApiKeyKitt GetApiKeyKitt(int kitt)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            ApiKeyKitt cl = new ApiKeyKitt();
            try
            {
                string SQL = "sp_GetApiKeyKittByIdCliente";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@cte", SqlDbType.Int);
                param.Value = kitt;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cl.Id = (int)reader["Id"];
                    cl.IdCliente = (int)reader["IdCliente"];
                    cl.TenantId = (int)reader["TenantId"];
                    cl.ApiKey = reader["ApiKey"] == DBNull.Value ? string.Empty : (string)reader["ApiKey"];
                    cl.Usernamemail = reader["Usernamemail"] == DBNull.Value ? string.Empty : (string)reader["Usernamemail"];
                    cl.Pwd = reader["Pwd"] == DBNull.Value ? string.Empty : (string)reader["Pwd"];
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<Claves> GetClaves(int IdCte)
        {

            List<Claves> list = new List<Claves>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                Claves clave = new Claves();
                SqlCommand sqlCommand = new SqlCommand("sp_GetClaveByCliente", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = sqlCommand.Parameters.Add("@cte", SqlDbType.Int);
                param.Value = IdCte;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    clave.IdClaveCliente = (int)reader["IdClaveCliente"];
                    clave.IdCliente = (int)reader["IdCliente"];
                    clave.Clave = (string)reader["Clave"];
                    list.Add(clave);
                }

                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tkn"></param>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public static bool InsertNewMSSPToken(string tkn, int idCliente)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommand = new SqlCommand("sp_InsertMSSPToken", cn))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    sqlCommand.Parameters.AddWithValue("@Tkn", tkn);
                    sqlCommand.Parameters.AddWithValue("@Cliente", idCliente);
                    int rowsAffected = sqlCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                        return true;
                    }
                    else
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// OBTIENE EL TOKEN PARA LA SESION DE USUARIO
        public static List<Logs> GetLogs()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<Logs> logs = new List<Logs>();
            try
            {
                SqlCommand sqlCommand = new SqlCommand("exec dbo.sp_GetLog", cn);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Logs log = new Logs();
                    log.IdLog = (int)reader["IdLog"];
                    log.Date = (DateTime)reader["Date"];
                    log.Thread = (string)reader["Thread"];
                    log.Level = (string)reader["Level"];
                    log.Logger = (string)reader["Logger"];
                    log.Message = (string)reader["Message"];
                    log.Request = (string)reader["Request"];
                    log.Response = (string)reader["Response"];
                    log.IP = (string)reader["IP"];
                    log.Browser = (string)reader["Browser"];
                    //log.Exception = null;
                    logs.Add(log);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return logs;
        }

        /// <summary>
        /// Get Token by cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        internal static string GetTokenByCliente(int idCliente)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            string token = string.Empty;
            try
            {
                string SQL = "sp_GetTokenByIdCliente";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = idCliente;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Token cl = new Token();
                    cl.IdSesion = (int)reader["IdSesion"];
                    cl.IdCliente = (int)reader["IdCliente"];
                    cl.Tokens = (string)reader["Token"];
                    cl.FechaInicio = (DateTime)reader["FechaInicio"];
                    cl.FechaFin = (DateTime)reader["FechaFin"];
                    cl.Active = (bool)reader["Active"];
                    token = cl.Tokens;
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return token;
        }

        /// <summary>
        /// get user login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        internal static bool GetUserLogin(string user, string pass)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            bool sesion = false;
            try
            {
                string SQL = "sp_Login";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@User", SqlDbType.VarChar);
                param.Value = user;
                param = cmd.Parameters.Add("@Password", SqlDbType.VarChar);
                param.Value = pass;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LoginModel cl = new LoginModel();
                    cl.IdLogin = (int)reader["IdLogin"];
                    cl.Usr = (string)reader["Nombre"];
                    cl.Pd = (string)reader["Password"];
                    cl.Timeout = (int)reader["Timeout"];
                    cl.Active = (bool)reader["Active"];
                    sesion = true;
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return sesion;
        }

        /// <summary>
        /// get user login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pswd"></param>
        /// <returns></returns>
        internal static bool GetLogin(string user, string pswd)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            bool tokens = false;
            try
            {
                string SQL = "sp_GetLogin";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@User", SqlDbType.VarChar);
                param.Value = user;
                param = cmd.Parameters.Add("@Password", SqlDbType.VarChar);
                param.Value = pswd;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LoginModel cl = new LoginModel();
                    cl.IdLogin = (int)reader["IdLogin"];
                    cl.Usr = (string)reader["Nombre"];
                    cl.Pd = (string)reader["Password"];
                    cl.Active = (bool)reader["Active"];
                    tokens = true;
                }
                Console.WriteLine("Se cerro");
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                cn.Close();
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return tokens;
        }

        /// <summary>
        /// get token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal static bool GetToken(string token)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            bool tokens = false;
            try
            {
                string SQL = "sp_GetToken";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Token", SqlDbType.VarChar);
                param.Value = token;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Token cl = new Token();
                    cl.IdSesion = (int)reader["IdSesion"];
                    cl.IdCliente = (int)reader["IdCliente"];
                    cl.Tokens = (string)reader["Token"];
                    cl.FechaInicio = (DateTime)reader["FechaInicio"];
                    cl.FechaFin = (DateTime)reader["FechaFin"];
                    cl.Active = (bool)reader["Active"];
                    tokens = true;
                }
                Console.WriteLine("Se cerro");
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                cn.Close();
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return tokens;
        }

        /// <summary>
        /// get cliente by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static Cliente GetClientesById(int id)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            Cliente cl = new Cliente();
            try
            {
                string SQL = "sp_GetClienteById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = id;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cl.IdCliente = (int)reader["IdCliente"];
                    cl.NombreCliente = (string)reader["NombreCliente"];
                    cl.Configuraciones = (string)reader["Configuraciones"];
                    cl.Apikey = reader["Apikey"] == DBNull.Value ? string.Empty : (string)reader["Apikey"];
                    cl.Authorization = reader["Authorization"] == DBNull.Value ? string.Empty : (string)reader["Authorization"];
                    cl.RiskTypeCode = reader["RiskTypeCode"] == DBNull.Value ? string.Empty : (string)reader["RiskTypeCode"];
                    cl.Active = (bool)reader["Active"];
                    cl.Payments = reader["Payments"] == DBNull.Value ? string.Empty : (string)reader["Payments"];
                }

                FlujoAlterno Fa = DBOperations.GetFlujoAlterno(id);

                cl.Login = Fa.EnabledLogin;
                cl.Flujo = Fa.Enabled;
                cl.Multiple = Fa.EnabledMultiple;


                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public static EmailTemplate GetEmailTemplate(string tipo)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            EmailTemplate et = new EmailTemplate();
            try
            {
                string SQL = "sp_GetEmailTemplateById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Tipo", SqlDbType.VarChar);
                param.Value = tipo;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    et.IdEmail = (int)reader["IdEmail"];
                    et.Descripcion = (string)reader["Descripcion"];
                    et.BodyName = (string)reader["BodyName"];
                    et.Body = (string)reader["Body"];
                    et.SubjectPre = (string)reader["SubjectPre"];
                    et.SubjectPost = (string)reader["SubjectPost"];
                    et.CCEmails = (string)reader["CCEmails"];
                    et.BCCEmails = (string)reader["BCCEmails"];
                    et.Active = (bool)reader["Active"];
                    et.IdCliente = (int)reader["IdCliente"];
                    et.EmailFrom = (string)reader["EmailFrom"];
                    //els.Add(et);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return et;
        }

        /// <summary>
        /// insert in bitacora table
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        internal static bool InsertBitacora(Bitacora bt)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertBitacora", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IP", SqlDbType.VarChar);
                    param.Value = bt.IP;
                    param = sqlCommandInsert.Parameters.Add("@Navegador", SqlDbType.VarChar);
                    param.Value = bt.Navegador;
                    param = sqlCommandInsert.Parameters.Add("@Usuario", SqlDbType.VarChar);
                    param.Value = bt.Usuario;
                    param = sqlCommandInsert.Parameters.Add("@Fecha", SqlDbType.DateTime);
                    param.Value = bt.Fecha;
                    param = sqlCommandInsert.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                    param.Value = bt.Descripcion;
                    param = sqlCommandInsert.Parameters.Add("@Plataforma", SqlDbType.VarChar);
                    param.Value = bt.Plataforma;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Insert claim
        /// </summary>
        /// <param name="caso"></param>
        /// <param name="personal"></param>
        /// <param name="nombre"></param>
        /// <param name="mail"></param>
        /// <param name="cliente"></param>
        /// <returns></returns> @CertificadoPadre
        internal static bool InsertClaim(string caso, string personal, string nombre, string mail, int cliente, string CertificadoPadre)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertClaim", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@NumeroCaso", SqlDbType.VarChar);
                    param.Value = caso;
                    param = sqlCommandInsert.Parameters.Add("@RespInfoPersonal", SqlDbType.VarChar);
                    param.Value = personal;
                    param = sqlCommandInsert.Parameters.Add("@NombreCliente", SqlDbType.VarChar);
                    param.Value = nombre;
                    param = sqlCommandInsert.Parameters.Add("@CorreoCliente", SqlDbType.VarChar);
                    param.Value = mail;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = cliente;
                    //param = sqlCommandInsert.Parameters.Add("@CertificadoPadre", SqlDbType.VarChar);
                    //param.Value = CertificadoPadre;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// insert folio
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="nombre"></param>
        /// <param name="direccion"></param>
        /// <param name="telefono"></param>
        /// <param name="estado"></param>
        /// <param name="delegacion"></param>
        /// <param name="cp"></param>
        /// <param name="preguntas"></param>
        /// <param name="mail"></param>
        /// <param name="articulo"></param>
        /// <param name="poliza"></param>
        /// <param name="documento"></param>
        /// <param name="claimnumber"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        internal static bool InsertClaimFolio(string folio, string nombre, string direccion, string telefono, string estado, string delegacion, string cp, string preguntas, string mail, string articulo, string poliza, string documento, string claimnumber, string certificado)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertClaimFolio", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Folio", SqlDbType.VarChar);
                    param.Value = folio;
                    param = sqlCommandInsert.Parameters.Add("@Nombre", SqlDbType.VarChar);
                    param.Value = nombre;
                    param = sqlCommandInsert.Parameters.Add("@Articulo", SqlDbType.VarChar);
                    param.Value = articulo;
                    param = sqlCommandInsert.Parameters.Add("@Poliza", SqlDbType.VarChar);
                    param.Value = poliza;
                    param = sqlCommandInsert.Parameters.Add("@Direccion", SqlDbType.VarChar);
                    param.Value = direccion;
                    param = sqlCommandInsert.Parameters.Add("@Telefono", SqlDbType.VarChar);
                    param.Value = telefono;
                    param = sqlCommandInsert.Parameters.Add("@Estado", SqlDbType.VarChar);
                    param.Value = estado;
                    param = sqlCommandInsert.Parameters.Add("@Delegacion", SqlDbType.VarChar);
                    param.Value = delegacion;
                    param = sqlCommandInsert.Parameters.Add("@CP", SqlDbType.VarChar);
                    param.Value = cp;
                    param = sqlCommandInsert.Parameters.Add("@Preguntas", SqlDbType.VarChar);
                    param.Value = preguntas;
                    param = sqlCommandInsert.Parameters.Add("@Mail", SqlDbType.VarChar);
                    param.Value = mail;
                    param = sqlCommandInsert.Parameters.Add("@Documento", SqlDbType.VarChar);
                    param.Value = documento;
                    param = sqlCommandInsert.Parameters.Add("@ClaimNumber", SqlDbType.VarChar);
                    param.Value = claimnumber;
                    param = sqlCommandInsert.Parameters.Add("@Certificado", SqlDbType.VarChar);
                    param.Value = certificado;
                    param = sqlCommandInsert.Parameters.Add("@FechaCreacion", SqlDbType.DateTime);
                    param.Value = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// update folio
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="direccion"></param>
        /// <param name="estado"></param>
        /// <param name="delegacion"></param>
        /// <param name="cp"></param>
        /// <param name="preguntas"></param>
        /// <param name="mail"></param>
        /// <param name="articulo"></param>
        /// <param name="poliza"></param>
        /// <param name="documento"></param>
        /// <param name="claimnumber"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolio(string nombre, string direccion, string estado, string delegacion, string cp, string preguntas, string mail, string articulo, string poliza, string documento, string claimnumber)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolio", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Nombre", SqlDbType.VarChar);
                    param.Value = nombre;
                    param = sqlCommandInsert.Parameters.Add("@Articulo", SqlDbType.VarChar);
                    param.Value = articulo;
                    param = sqlCommandInsert.Parameters.Add("@Poliza", SqlDbType.VarChar);
                    param.Value = poliza;
                    param = sqlCommandInsert.Parameters.Add("@Direccion", SqlDbType.VarChar);
                    param.Value = direccion;
                    param = sqlCommandInsert.Parameters.Add("@Estado", SqlDbType.VarChar);
                    param.Value = estado;
                    param = sqlCommandInsert.Parameters.Add("@Delegacion", SqlDbType.VarChar);
                    param.Value = delegacion;
                    param = sqlCommandInsert.Parameters.Add("@CP", SqlDbType.VarChar);
                    param.Value = cp;
                    param = sqlCommandInsert.Parameters.Add("@Preguntas", SqlDbType.VarChar);
                    param.Value = preguntas;
                    param = sqlCommandInsert.Parameters.Add("@Mail", SqlDbType.VarChar);
                    param.Value = mail;
                    param = sqlCommandInsert.Parameters.Add("@Documento", SqlDbType.VarChar);
                    param.Value = documento;
                    param = sqlCommandInsert.Parameters.Add("@ClaimNumber", SqlDbType.VarChar);
                    param.Value = claimnumber;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// update folio begin
        /// </summary>
        /// <param name="CertificateNumber"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolioFromBeginClaim(string Poliza, string Certificado)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolioFromBeginClaim", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Poliza", SqlDbType.VarChar);
                    param.Value = Poliza == null ? "" : Poliza;
                    param = sqlCommandInsert.Parameters.Add("@Certificado", SqlDbType.VarChar);
                    param.Value = Certificado == null ? "" : Certificado;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// udpdate folio device
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolioFromDeviceSelection(string SerialNumber)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolioFromDeviceSelection", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Articulo", SqlDbType.VarChar);
                    string Serial = SerialNumber == null ? "" : SerialNumber;
                    param.Value = Serial;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// update folio submit Q
        /// </summary>
        /// <param name="Preguntas"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolioFromSubmitQuestion(string Preguntas)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolioFromSubmitQuestion", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Preguntas", SqlDbType.VarChar);
                    param.Value = Preguntas == null ? "" : Preguntas;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// update folio shipping
        /// </summary>
        /// <param name="Address1"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="PostalCode"></param>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="EmailAddress"></param>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolioFromSubmitShipping(string Address1, string City, string State, string PostalCode, string FirstName, string LastName, string EmailAddress, string PhoneNumber, string Articulo)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolioFromSubmitShipping", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Address1", SqlDbType.VarChar);
                    param.Value = Address1 == null ? "" : EncDec.Encript(Address1);
                    param = sqlCommandInsert.Parameters.Add("@City", SqlDbType.VarChar);
                    param.Value = City == null ? "" : EncDec.Encript(City);
                    param = sqlCommandInsert.Parameters.Add("@State", SqlDbType.VarChar);
                    param.Value = State == null ? "" : EncDec.Encript(State);
                    param = sqlCommandInsert.Parameters.Add("@PostalCode", SqlDbType.VarChar);
                    param.Value = PostalCode == null ? "" : EncDec.Encript(PostalCode);
                    param = sqlCommandInsert.Parameters.Add("@Name", SqlDbType.VarChar);
                    string Fir = FirstName == null ? "" : FirstName;
                    string Las = LastName == null ? "" : LastName;
                    param.Value = EncDec.Encript(Fir + " " + Las);
                    param = sqlCommandInsert.Parameters.Add("@EmailAddress", SqlDbType.VarChar);
                    param.Value = EmailAddress == null ? "" : EncDec.Encript(EmailAddress);
                    param = sqlCommandInsert.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                    param.Value = PhoneNumber == null ? "" : EncDec.Encript(PhoneNumber);
                    param = sqlCommandInsert.Parameters.Add("@Articulo", SqlDbType.VarChar);
                    param.Value = Articulo == null ? "" : Articulo;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// update folio document
        /// </summary>
        /// <param name="Documento"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolioFromAttachDocument(string Documento)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolioFromAttachDocument", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Documento", SqlDbType.VarChar);
                    param.Value = Documento == null ? "" : Documento;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// update folio details
        /// </summary>
        /// <param name="ClaimNumber"></param>
        /// <returns></returns>
        internal static bool UpdateClaimFolioFromDetailsClaim(string ClaimNumber)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimFolioFromDetailsClaim", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@ClaimNumber", SqlDbType.VarChar);
                    param.Value = ClaimNumber == null ? "" : ClaimNumber;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ClaimFolio GetLastClaimFolio()
        {
            if (cnp.State == ConnectionState.Closed) cnp.Open();
            ClaimFolio Cf = new ClaimFolio();
            try
            {
                string SQL = "sp_GetLastClaimFolio";
                SqlCommand cmd = new SqlCommand(SQL, cnp);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cf.IdFolio = (int)reader["IdFolio"];
                    Cf.Folio = reader["Folio"] == DBNull.Value ? "" : (string)reader["Folio"];
                    Cf.Nombre = reader["Nombre"] == DBNull.Value ? "" : (string)reader["Nombre"];
                    Cf.Articulo = reader["Articulo"] == DBNull.Value ? "" : (string)reader["Articulo"];
                    Cf.Poliza = reader["Poliza"] == DBNull.Value ? "" : (string)reader["Poliza"];
                    Cf.Direccion = reader["Direccion"] == DBNull.Value ? "" : (string)reader["Direccion"];
                    Cf.Telefono = reader["Telefono"] == DBNull.Value ? "" : (string)reader["Telefono"];
                    Cf.Estado = reader["Estado"] == DBNull.Value ? "" : (string)reader["Estado"];
                    Cf.Delegacion = reader["Delegacion"] == DBNull.Value ? "" : (string)reader["Delegacion"];
                    Cf.CP = reader["CP"] == DBNull.Value ? "" : (string)reader["CP"];
                    Cf.Preguntas = reader["Preguntas"] == DBNull.Value ? "" : (string)reader["Preguntas"];
                    Cf.Mail = reader["Mail"] == DBNull.Value ? "" : (string)reader["Mail"];
                    Cf.Documento = reader["Documento"] == DBNull.Value ? "" : (string)reader["Documento"];
                    Cf.ClaimNumber = reader["ClaimNumber"] == DBNull.Value ? "" : (string)reader["ClaimNumber"];
                    Cf.FechaCreacion = (DateTime)reader["FechaCreacion"];
                    Cf.Certificado = reader["Certificado"] == DBNull.Value ? "" : (string)reader["Certificado"];
                    //els.Add(et);
                }
                reader.Close();
                cnp.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
            return Cf;
        }

        internal static bool InsertElegiblesData(string Poliza, int IdCliente, string ElegibleData, string ElegibleHistoryList, bool Active)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertElegibleData", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Poliza", SqlDbType.VarChar);
                    param.Value = Poliza;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@ElegibleData", SqlDbType.VarChar);
                    param.Value = ElegibleData;
                    param = sqlCommandInsert.Parameters.Add("@ElegibleHistoryList", SqlDbType.VarChar);
                    param.Value = ElegibleHistoryList;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.VarChar);
                    param.Value = Active;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        public static MethodRepair GetMethorRepair(string dealer, string risk, string city, string state, string scc, string marca)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            MethodRepair Cf = new MethodRepair();
            try
            {
                string SQL = "sp_GetMethorRepair";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                param.Value = dealer;
                param = cmd.Parameters.Add("@RiskType", SqlDbType.VarChar);
                param.Value = risk;
                param = cmd.Parameters.Add("@City", SqlDbType.VarChar);
                param.Value = city;
                param = cmd.Parameters.Add("@State", SqlDbType.VarChar);
                param.Value = state;
                param = cmd.Parameters.Add("@ServiceCenterCode", SqlDbType.VarChar);
                param.Value = scc;
                param = cmd.Parameters.Add("@Marca", SqlDbType.VarChar);
                param.Value = marca;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cf.IdMethodRepair = (int)reader["IdMethodRepair"];
                    Cf.DealerCode = reader["DealerCode"] == DBNull.Value ? "" : (string)reader["DealerCode"];
                    Cf.RiskType = reader["RiskType"] == DBNull.Value ? "" : (string)reader["RiskType"];
                    Cf.Marca = reader["Marca"] == DBNull.Value ? "" : (string)reader["Marca"];
                    Cf.StateProvidence = reader["StateProvidence"] == DBNull.Value ? "" : (string)reader["StateProvidence"];
                    Cf.StateCode = reader["StateCode"] == DBNull.Value ? "" : (string)reader["StateCode"];
                    Cf.City = reader["City"] == DBNull.Value ? "" : (string)reader["City"];
                    Cf.ServiceCenterCode = reader["ServiceCenterCode"] == DBNull.Value ? "" : (string)reader["ServiceCenterCode"];
                    Cf.ServiceCenterName = reader["ServiceCenterName"] == DBNull.Value ? "" : (string)reader["ServiceCenterName"];
                    Cf.Output = reader["Output"] == DBNull.Value ? "" : (string)reader["Output"];
                    Cf.CreationDate = reader["CreationDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["CreationDate"];
                    Cf.UserCreator = reader["UserCreator"] == DBNull.Value ? "" : (string)reader["UserCreator"];
                    Cf.Active = reader["Active"] == DBNull.Value ? false : (bool)reader["Active"];
                    Cf.HorarioS = reader["HorarioS"] == DBNull.Value ? "" : (string)reader["HorarioS"];
                    Cf.HorarioF = reader["HorarioF"] == DBNull.Value ? "" : (string)reader["HorarioF"];
                    Cf.Email1 = reader["Email1"] == DBNull.Value ? "" : (string)reader["Email1"];
                    Cf.Email2 = reader["Email2"] == DBNull.Value ? "" : (string)reader["Email2"];
                    Cf.Bairro = reader["Bairro"] == DBNull.Value ? "" : (string)reader["Bairro"];
                    Cf.CEP = reader["CEP"] == DBNull.Value ? "" : (string)reader["CEP"];
                    Cf.Phone1 = reader["Phone1"] == DBNull.Value ? "" : (string)reader["Phone1"];
                    Cf.Phone2 = reader["Phone2"] == DBNull.Value ? "" : (string)reader["Phone2"];
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return Cf;
        }

        public static ClaimData GetCertificateClaim(string certificado)
        {
            if (cnp.State == ConnectionState.Closed) cnp.Open();
            ClaimData Cf = new ClaimData();
            try
            {
                string SQL = "sp_GetCertificateClaim";
                SqlCommand cmd = new SqlCommand(SQL, cnp);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Certificado", SqlDbType.VarChar);
                param.Value = certificado;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cf.IdClaim = (int)reader["IdClaim"];
                    Cf.Poliza = reader["Poliza"] == DBNull.Value ? "" : (string)reader["Poliza"];
                    Cf.IdCliente = reader["IdCliente"] == DBNull.Value ? 0 : (int)reader["IdCliente"];
                    Cf.ClaimDatas = reader["ClaimData"] == DBNull.Value ? "" : (string)reader["ClaimData"];
                    Cf.ClaimHistoryList = reader["ClaimHistoryList"] == DBNull.Value ? "" : (string)reader["ClaimHistoryList"];
                    Cf.DealerCode = reader["DealerCode"] == DBNull.Value ? "" : (string)reader["DealerCode"];
                    Cf.Active = (bool)reader["Active"];
                    //els.Add(et);
                }
                reader.Close();
                cnp.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
            return Cf;
        }

        internal static bool InsertClaimData(string Poliza, int IdCliente, string ClaimData, string ClaimHistoryList, bool Active, string dealer)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertClaimData", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Poliza", SqlDbType.VarChar);
                    param.Value = Poliza;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@ClaimData", SqlDbType.VarChar);
                    param.Value = ClaimData;
                    param = sqlCommandInsert.Parameters.Add("@ClaimHistoryList", SqlDbType.VarChar);
                    param.Value = ClaimHistoryList;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.VarChar);
                    param.Value = Active;
                    param = sqlCommandInsert.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                    param.Value = dealer;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        internal static bool UpdateClaimData(int id, string Poliza, int IdCliente, string ClaimData, string ClaimHistoryList, bool Active, string dealer)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimData", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdClaim", SqlDbType.VarChar);
                    param.Value = id;
                    param = sqlCommandInsert.Parameters.Add("@Poliza", SqlDbType.VarChar);
                    param.Value = Poliza;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@ClaimData", SqlDbType.VarChar);
                    param.Value = ClaimData;
                    param = sqlCommandInsert.Parameters.Add("@ClaimHistoryList", SqlDbType.VarChar);
                    param.Value = ClaimHistoryList;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.VarChar);
                    param.Value = Active;
                    param = sqlCommandInsert.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                    param.Value = dealer;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// Obtiene el flujo alterno
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static FlujoAlterno GetFlujoAlterno(int IdCliente)
        {
            if (cnp.State == ConnectionState.Closed) cnp.Open();
            FlujoAlterno Fa = new FlujoAlterno();
            try
            {
                string SQL = "sp_GetFlujoAlterno";
                SqlCommand cmd = new SqlCommand(SQL, cnp);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = IdCliente;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Fa.IdCliente = (int)reader["IdCliente"];
                    Fa.Enabled = (bool)reader["Enabled"];
                    Fa.EnabledLogin = (bool)reader["EnabledLogin"];
                    Fa.EnabledMultiple = (bool)reader["EnabledMultiple"];
                }
                reader.Close();
                //cnp.Close();
            }
            catch (Exception)
            {

                throw;
            }
            finally { if (cnp.State == ConnectionState.Open) cnp.Close(); }
            return Fa;
        }

        internal static bool UpdateClaimDetailsQuestions(string questions, string claimnumber, string cert, string caso, string desc)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimDetailsQuestions", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Questions", SqlDbType.VarChar);
                    param.Value = questions;
                    param = sqlCommandInsert.Parameters.Add("@Description", SqlDbType.VarChar);
                    param.Value = desc;
                    param = sqlCommandInsert.Parameters.Add("@Claim", SqlDbType.VarChar);
                    param.Value = claimnumber;
                    param = sqlCommandInsert.Parameters.Add("@Certificate", SqlDbType.VarChar);
                    param.Value = cert;
                    param = sqlCommandInsert.Parameters.Add("@Case", SqlDbType.VarChar);
                    param.Value = caso;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        internal static bool UpdateClaimDetailsShipping(string caso, string shipping)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimDetailsShipping", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Shipping", SqlDbType.VarChar);
                    param.Value = shipping;
                    param = sqlCommandInsert.Parameters.Add("@Caso", SqlDbType.VarChar);
                    param.Value = caso;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        internal static bool UpdateClaimDetailsDevice(string serial, string caso, string device)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimDetailsDevice", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Serial", SqlDbType.VarChar);
                    param.Value = serial;
                    param = sqlCommandInsert.Parameters.Add("@Caso", SqlDbType.VarChar);
                    param.Value = caso;
                    param = sqlCommandInsert.Parameters.Add("@Device", SqlDbType.VarChar);
                    param.Value = device;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        internal static bool UpdateClaimDetails(string cs, string claimstatus, string caso)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateClaimDetails", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@ClaimStatus", SqlDbType.VarChar);
                    param.Value = cs;
                    param = sqlCommandInsert.Parameters.Add("@ExtendedClaimStatus", SqlDbType.VarChar);
                    param.Value = claimstatus;
                    param = sqlCommandInsert.Parameters.Add("@Caso", SqlDbType.VarChar);
                    param.Value = caso;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        /// Obtiene las traducciones por cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable GetQuestionTemplate(int id, string setcode)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            DataTable dt = new DataTable();
            try
            {
                string SQL = "sp_GetQuestionByIdClient_2";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;

                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = id;
                param = cmd.Parameters.Add("@SetCode", SqlDbType.VarChar);
                param.Value = setcode;
                SqlDataReader datareader = cmd.ExecuteReader();
                dt.Load(datareader);
                // questiontemplate = JsonConvert.SerializeObject(dt);
                datareader.Close();

                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return dt;
        }

        /// <summary>
        /// Obtiene el codigo de traduccion por cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable GetCodeTraslate(int id)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            DataTable dt = new DataTable();
            try
            {
                string SQL = "sp_CodeByIdClient";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = id;
                SqlDataReader datareader = cmd.ExecuteReader();

                dt.Load(datareader);
                // questiontemplate = JsonConvert.SerializeObject(dt);
                datareader.Close();

                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return dt;
        }

        /// <summary>
        /// Obtiene el api key por dealer code
        /// </summary>
        /// <param name="dealercode"></param>
        /// <returns></returns>
        public static Cliente GetApikey(string dealercode)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            Cliente et = new Cliente();
            try
            {
                string SQL = "sp_GetClienteByDealerCode";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                param.Value = dealercode;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    et.IdCliente = (int)reader["IdCliente"];
                    et.NombreCliente = (string)reader["NombreCliente"];
                    et.Configuraciones = (string)reader["Configuraciones"];
                    et.Apikey = (string)reader["Apikey"];
                    et.Authorization = (string)reader["Authorization"];
                    et.RiskTypeCode = (string)reader["RiskTypeCode"];
                    et.Active = (bool)reader["Active"];
                    et.URLHeader = reader["URLHeader"] == DBNull.Value ? string.Empty : reader["URLHeader"].ToString();
                    et.URLFooter = reader["URLFooter"] == DBNull.Value ? string.Empty : reader["URLFooter"].ToString();

                    //els.Add(et);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return et;
        }



        /// <summary>
        /// Obtiene la descripcion del issue
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static DataTable GetDescriptionIssue(string certificate)
        {
            if (cnp.State == ConnectionState.Closed) cnp.Open();
            DataTable dt = new DataTable();
            try
            {
                string SQL = "sp_GetSearchClaimbyCertificate";
                SqlCommand cmd = new SqlCommand(SQL, cnp);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Certificate", SqlDbType.VarChar);
                param.Value = certificate;
                SqlDataReader datareader = cmd.ExecuteReader();

                dt.Load(datareader);
                datareader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
            return dt;
        }

        /// <summary>
        /// Obtiene el token
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTokens()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            DataTable dt = new DataTable();
            try
            {
                string SQL = "sp_get_alltokens";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader datareader = cmd.ExecuteReader();
                if (datareader.HasRows)
                {
                    dt.Load(datareader);
                    datareader.Close();
                    cn.Close();
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return dt;
        }

        /// <summary>
        /// Obtiene el template del mail a enviar
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EmailTemplate GetEmailTemplate(string tipo, int id)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            EmailTemplate et = new EmailTemplate();
            try
            {
                string SQL = "sp_GetEmailTemplateById";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Tipo", SqlDbType.VarChar);
                param.Value = tipo;
                param = cmd.Parameters.Add("@Cliente", SqlDbType.Int);
                param.Value = id;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    et.IdEmail = (int)reader["IdEmail"];
                    et.Descripcion = (string)reader["Descripcion"];
                    et.BodyName = (string)reader["BodyName"];
                    et.Body = (string)reader["Body"];
                    et.SubjectPre = (string)reader["SubjectPre"];
                    et.SubjectPost = (string)reader["SubjectPost"];
                    et.CCEmails = (string)reader["CCEmails"];
                    et.BCCEmails = (string)reader["BCCEmails"];
                    et.Active = (bool)reader["Active"];
                    et.IdCliente = (int)reader["IdCliente"];
                    et.EmailFrom = (string)reader["EmailFrom"];
                    et.Server = (string)reader["Server"];
                    et.Port = (string)reader["Port"];
                    et.EmailSenderName = (string)reader["EmailSenderName"];
                    et.MessageTitle = (string)reader["MessageTitle"];
                    et.MessageMail = (string)reader["MessageMail"];
                    et.MessageBody = (string)reader["MessageBody"];
                    //els.Add(et);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return et;
        }

        /// <summary>
        /// Obtiene un claim
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static ClaimDetails GetOneClaimDetails(string claim)
        {
            if (cnp.State == ConnectionState.Closed) cnp.Open();
            ClaimDetails et = new ClaimDetails();
            try
            {
                string SQL = "sp_GetOneClaimDetails";
                SqlCommand cmd = new SqlCommand(SQL, cnp);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Claim", SqlDbType.VarChar);
                param.Value = claim;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    et.IdClaim = (int)reader["IdClaim"];
                    et.NumeroCaso = (string)reader["NumeroCaso"];
                    et.ClaimNumber = (string)reader["ClaimNumber"];
                    et.NumeroSerie = (string)reader["NumeroSerie"];
                    et.RespInfoPersonal = (string)reader["RespInfoPersonal"];
                    et.RespInfoDispositivo = (string)reader["RespInfoDispositivo"];
                    et.RespInfoPersonal = (string)reader["RespInfoPersonal"];
                    et.RespInfoPreguntas = (string)reader["RespInfoPreguntas"];
                    et.RespInfoClaimDescription = reader["RespInfoClaimDescription"] == null ? null : reader["RespInfoClaimDescription"].ToString();
                    et.RespInfoCLaimDetails = reader["RespInfoCLaimDetails"] == null ? null : reader["RespInfoCLaimDetails"].ToString();
                    et.CertificateNumber = (string)reader["CertificateNumber"];
                    et.NombreCliente = reader["NombreCliente"] == null ? null : reader["NombreCliente"].ToString();
                    et.CorreoCliente = (string)reader["CorreoCliente"];
                    et.StatusType = reader["StatusType"] == DBNull.Value ? string.Empty : (string)reader["StatusType"];
                    et.IdCliente = reader["IdCliente"] == DBNull.Value ? 0 : (int)reader["IdCliente"];
                    et.CertificadoPadre = reader["CertificadoPadre"] == DBNull.Value ? string.Empty : (string)reader["CertificadoPadre"];

                    //els.Add(et);
                }
                reader.Close();
                cnp.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
            return et;
        }

        /// <summary>
        /// Obtiene los paises
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string GetCountry(int Id)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();

            DataTable dt = new DataTable();
            string json = string.Empty;
            try
            {
                string SQL = "sp_data_countries";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@iD", SqlDbType.Int);
                param.Value = Id;
                SqlDataReader datareader = cmd.ExecuteReader();

                dt.Load(datareader);

                json = DatatabletoJson(dt);

                datareader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return json;
        }

        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public static List<DealerCodes> GetDealerCodesByIdCliente(int IdCliente)
        {

            List<DealerCodes> list = new List<DealerCodes>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDealerCodesByIdCliente", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = IdCliente;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DealerCodes obj = new DealerCodes();
                            obj.IdDealerCode = (int)reader["IdDealerCode"];
                            obj.IdCliente = (int)reader["IdCliente"];
                            obj.DealerCode = reader["DealerCode"] == DBNull.Value ? string.Empty : reader["DealerCode"].ToString();
                            obj.TerminosDeUso = reader["TerminosDeUso"] == DBNull.Value ? string.Empty : reader["TerminosDeUso"].ToString();
                            obj.TerminosDeGarantia = reader["TerminosDeGarantia"] == DBNull.Value ? string.Empty : reader["TerminosDeGarantia"].ToString();
                            obj.FrequentlyAskedQuestions = reader["FrequentlyAskedQuestions"] == DBNull.Value ? string.Empty : reader["FrequentlyAskedQuestions"].ToString();
                            obj.DownloadCertificate = reader["DownloadCertificate"] == DBNull.Value ? string.Empty : reader["DownloadCertificate"].ToString();
                            obj.DownloadDocument = reader["DownloadDocument"] == DBNull.Value ? string.Empty : reader["DownloadDocument"].ToString();
                            obj.ContactUs = reader["ContactUs"] == DBNull.Value ? string.Empty : reader["ContactUs"].ToString();
                            obj.TextSurvey = reader["TextSurvey"] == DBNull.Value ? string.Empty : reader["TextSurvey"].ToString();
                            obj.Okta = "";
                            obj.Idioma = reader["Idioma"] == DBNull.Value ? string.Empty : reader["Idioma"].ToString();
                            obj.ServiceOption = reader["ServiceOption"] == DBNull.Value ? string.Empty : reader["ServiceOption"].ToString();
                            obj.ExternalURL = reader["ExternalURL"] == DBNull.Value ? string.Empty : reader["ExternalURL"].ToString();
                            obj.PoliticaPrivacidad = reader["PoliticaPrivacidad"] == DBNull.Value ? string.Empty : reader["PoliticaPrivacidad"].ToString();
                            obj.TerminosCondiciones = reader["TerminosCondiciones"] == DBNull.Value ? string.Empty : reader["TerminosCondiciones"].ToString();
                            obj.DatosRequeridos = reader["DatosRequeridos"] == DBNull.Value ? string.Empty : reader["DatosRequeridos"].ToString();
                            obj.IsSelected = reader["IsSelected"] == DBNull.Value ? false : (bool)reader["IsSelected"];
                            obj.ProductCodeIcon = reader["ProductCodeIcon"] == DBNull.Value ? string.Empty : reader["ProductCodeIcon"].ToString();
                            obj.VariablesBusquedaNS = reader["VariablesBusquedaNS"] == DBNull.Value ? string.Empty : reader["VariablesBusquedaNS"].ToString();
                            obj.MultipleBusquedaNS = reader["MultipleBusquedaNS"] == DBNull.Value ? false : (bool)reader["MultipleBusquedaNS"];
                            obj.DealerGroup = reader["DealerGroup"] == DBNull.Value ? string.Empty : reader["DealerGroup"].ToString();
                            obj.CompanyCode = reader["CompanyCode"] == DBNull.Value ? string.Empty : reader["CompanyCode"].ToString();
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }


        /// <summary>
        /// Obtiene los dealer codes por cliente
        /// </summary>
        /// <param name="dealer"></param>
        /// <returns></returns>
        public static List<DealerCodes> GetDealerCodesByDealerCode(string dealer)
        {

            List<DealerCodes> list = new List<DealerCodes>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetDealerCodesByDealerCode", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = cmd.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                    param.Value = dealer;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DealerCodes obj = new DealerCodes();
                            obj.IdDealerCode = (int)reader["IdDealerCode"];
                            obj.IdCliente = (int)reader["IdCliente"];
                            obj.DealerCode = reader["DealerCode"] == DBNull.Value ? string.Empty : reader["DealerCode"].ToString();
                            obj.TerminosDeUso = reader["TerminosDeUso"] == DBNull.Value ? string.Empty : reader["TerminosDeUso"].ToString();
                            obj.TerminosDeGarantia = reader["TerminosDeGarantia"] == DBNull.Value ? string.Empty : reader["TerminosDeGarantia"].ToString();
                            obj.FrequentlyAskedQuestions = reader["FrequentlyAskedQuestions"] == DBNull.Value ? string.Empty : reader["FrequentlyAskedQuestions"].ToString();
                            obj.DownloadCertificate = reader["DownloadCertificate"] == DBNull.Value ? string.Empty : reader["DownloadCertificate"].ToString();
                            obj.DownloadDocument = reader["DownloadDocument"] == DBNull.Value ? string.Empty : reader["DownloadDocument"].ToString();
                            obj.ContactUs = reader["ContactUs"] == DBNull.Value ? string.Empty : reader["ContactUs"].ToString();
                            obj.TextSurvey = reader["TextSurvey"] == DBNull.Value ? string.Empty : reader["TextSurvey"].ToString();
                            obj.Okta = "";
                            obj.Idioma = reader["Idioma"] == DBNull.Value ? string.Empty : reader["Idioma"].ToString();
                            obj.ServiceOption = reader["ServiceOption"] == DBNull.Value ? string.Empty : reader["ServiceOption"].ToString();
                            obj.ExternalURL = reader["ExternalURL"] == DBNull.Value ? string.Empty : reader["ExternalURL"].ToString();
                            obj.PoliticaPrivacidad = reader["PoliticaPrivacidad"] == DBNull.Value ? string.Empty : reader["PoliticaPrivacidad"].ToString();
                            obj.TerminosCondiciones = reader["TerminosCondiciones"] == DBNull.Value ? string.Empty : reader["TerminosCondiciones"].ToString();
                            obj.DatosRequeridos = reader["DatosRequeridos"] == DBNull.Value ? string.Empty : reader["DatosRequeridos"].ToString();
                            obj.IsSelected = reader["IsSelected"] == DBNull.Value ? false : (bool)reader["IsSelected"];
                            obj.ProductCodeIcon = reader["ProductCodeIcon"] == DBNull.Value ? string.Empty : reader["ProductCodeIcon"].ToString();
                            obj.VariablesBusquedaNS = reader["VariablesBusquedaNS"] == DBNull.Value ? string.Empty : reader["VariablesBusquedaNS"].ToString();
                            obj.MultipleBusquedaNS = reader["MultipleBusquedaNS"] == DBNull.Value ? false : (bool)reader["MultipleBusquedaNS"];
                            obj.DealerGroup = reader["DealerGroup"] == DBNull.Value ? string.Empty : reader["DealerGroup"].ToString();
                            obj.CompanyCode = reader["CompanyCode"] == DBNull.Value ? string.Empty : reader["CompanyCode"].ToString();
                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }

        /// <summary>
        /// Inserta dealer code
        /// </summary>
        /// <returns></returns>
        public static bool InsertDealerCodes(DealerCodes dc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertDealerCodes", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = dc.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                    param.Value = dc.DealerCode;
                    param = sqlCommandInsert.Parameters.Add("@TerminosDeUso", SqlDbType.VarChar);
                    param.Value = dc.TerminosDeUso;
                    param = sqlCommandInsert.Parameters.Add("@TerminosDeGarantia", SqlDbType.VarChar);
                    param.Value = dc.TerminosDeGarantia;
                    param = sqlCommandInsert.Parameters.Add("@FrequentlyAskedQuestions", SqlDbType.VarChar);
                    param.Value = dc.FrequentlyAskedQuestions;
                    param = sqlCommandInsert.Parameters.Add("@DownloadCertificate", SqlDbType.VarChar);
                    param.Value = dc.DownloadCertificate;
                    param = sqlCommandInsert.Parameters.Add("@DownloadDocument", SqlDbType.VarChar);
                    param.Value = dc.DownloadDocument;
                    param = sqlCommandInsert.Parameters.Add("@ContactUs", SqlDbType.VarChar);
                    param.Value = dc.ContactUs;
                    param = sqlCommandInsert.Parameters.Add("@TextSurvey", SqlDbType.VarChar);
                    param.Value = dc.TextSurvey;
                    param = sqlCommandInsert.Parameters.Add("@ExternalURL", SqlDbType.VarChar);
                    param.Value = dc.ExternalURL;
                    param = sqlCommandInsert.Parameters.Add("@Idioma", SqlDbType.VarChar);
                    param.Value = dc.Idioma;
                    param = sqlCommandInsert.Parameters.Add("@Device", SqlDbType.VarChar);
                    param.Value = dc.Device;
                    param = sqlCommandInsert.Parameters.Add("@Okta", SqlDbType.VarChar);
                    param.Value = dc.Okta;
                    param = sqlCommandInsert.Parameters.Add("@ServiceOptions", SqlDbType.VarChar);
                    param.Value = dc.ServiceOption;
                    param = sqlCommandInsert.Parameters.Add("@PoliticaPrivacidad", SqlDbType.VarChar);
                    param.Value = dc.PoliticaPrivacidad;
                    param = sqlCommandInsert.Parameters.Add("@TerminosCondiciones", SqlDbType.VarChar);
                    param.Value = dc.TerminosCondiciones;
                    param = sqlCommandInsert.Parameters.Add("@DatosRequeridos", SqlDbType.VarChar);
                    param.Value = "";
                    param = sqlCommandInsert.Parameters.Add("@IsSelected", SqlDbType.Bit);
                    param.Value = true;
                    param = sqlCommandInsert.Parameters.Add("@ProductCodeIcon", SqlDbType.VarChar);
                    param.Value = "";
                    param = sqlCommandInsert.Parameters.Add("@VariablesBusquedaNS", SqlDbType.VarChar);
                    param.Value = "";
                    param = sqlCommandInsert.Parameters.Add("@MultipleBusquedaNS", SqlDbType.VarChar);
                    param.Value = false;
                    param = sqlCommandInsert.Parameters.Add("@DealerGroup", SqlDbType.VarChar);
                    param.Value = "";
                    param = sqlCommandInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar);
                    param.Value = "";
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }



        /// <summary>
        /// Actualiza el la tabla de dealer codes
        /// </summary>
        /// <returns></returns>
        public static bool UpdateDealerCodes(DealerCodes dc)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandUpdate = new SqlCommand("sp_UpdateDealerCodes", cn))
                {
                    sqlCommandUpdate.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandUpdate.Parameters.Add("@IdDealerCode", SqlDbType.VarChar);
                    param.Value = dc.IdDealerCode;
                    param = sqlCommandUpdate.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = dc.IdCliente;
                    param = sqlCommandUpdate.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                    param.Value = dc.DealerCode;
                    param = sqlCommandUpdate.Parameters.Add("@TerminosDeUso", SqlDbType.VarChar);
                    param.Value = dc.TerminosDeUso;
                    param = sqlCommandUpdate.Parameters.Add("@TerminosDeGarantia", SqlDbType.VarChar);
                    param.Value = dc.TerminosDeGarantia;
                    param = sqlCommandUpdate.Parameters.Add("@FrequentlyAskedQuestions", SqlDbType.VarChar);
                    param.Value = dc.FrequentlyAskedQuestions;
                    param = sqlCommandUpdate.Parameters.Add("@DownloadCertificate", SqlDbType.VarChar);
                    param.Value = dc.DownloadCertificate;
                    param = sqlCommandUpdate.Parameters.Add("@DownloadDocument", SqlDbType.VarChar);
                    param.Value = dc.DownloadDocument;
                    param = sqlCommandUpdate.Parameters.Add("@ContactUs", SqlDbType.VarChar);
                    param.Value = dc.ContactUs;
                    param = sqlCommandUpdate.Parameters.Add("@TextSurvey", SqlDbType.VarChar);
                    param.Value = dc.TextSurvey;
                    param = sqlCommandUpdate.Parameters.Add("@ExternalURL", SqlDbType.VarChar);
                    param.Value = dc.ExternalURL;
                    param = sqlCommandUpdate.Parameters.Add("@Idioma", SqlDbType.VarChar);
                    param.Value = dc.Idioma;

                    param = sqlCommandUpdate.Parameters.Add("@Okta", SqlDbType.VarChar);
                    param.Value = dc.Okta;
                    param = sqlCommandUpdate.Parameters.Add("@ServiceOption", SqlDbType.VarChar);
                    param.Value = dc.ServiceOption;
                    param = sqlCommandUpdate.Parameters.Add("@PoliticaPrivacidad", SqlDbType.VarChar);
                    param.Value = dc.PoliticaPrivacidad;
                    param = sqlCommandUpdate.Parameters.Add("@TerminosCondiciones", SqlDbType.VarChar);
                    param.Value = dc.TerminosCondiciones;
                    param = sqlCommandUpdate.Parameters.Add("@DatosRequeridos", SqlDbType.VarChar);
                    param.Value = dc.DatosRequeridos;
                    param = sqlCommandUpdate.Parameters.Add("@IsSelected", SqlDbType.Bit);
                    param.Value = dc.IsSelected;
                    param = sqlCommandUpdate.Parameters.Add("@ProductCodeIcon", SqlDbType.VarChar);
                    param.Value = dc.ProductCodeIcon;

                    param = sqlCommandUpdate.Parameters.Add("@VariablesBusquedaNS", SqlDbType.VarChar);
                    param.Value = dc.VariablesBusquedaNS;
                    param = sqlCommandUpdate.Parameters.Add("@MultipleBusquedaNS", SqlDbType.VarChar);
                    param.Value = dc.MultipleBusquedaNS;
                    param = sqlCommandUpdate.Parameters.Add("@DealerGroup", SqlDbType.VarChar);
                    param.Value = dc.DealerGroup;
                    param = sqlCommandUpdate.Parameters.Add("@CompanyCode", SqlDbType.VarChar);
                    param.Value = dc.CompanyCode;
                    int rowsAffected = sqlCommandUpdate.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Agrega un nuevo cliente
        /// </summary>
        /// <returns></returns>
        public static bool InsertCliente(Cliente cl)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertCliente", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@NombreCliente", SqlDbType.VarChar);
                    param.Value = cl.NombreCliente;
                    param = sqlCommandInsert.Parameters.Add("@Configuraciones", SqlDbType.VarChar);
                    param.Value = cl.Configuraciones;
                    param = sqlCommandInsert.Parameters.Add("@Apikey", SqlDbType.VarChar);
                    param.Value = cl.Apikey;
                    param = sqlCommandInsert.Parameters.Add("@Authorization", SqlDbType.VarChar);
                    param.Value = cl.Authorization;
                    param = sqlCommandInsert.Parameters.Add("@RiskTypeCode", SqlDbType.VarChar);
                    param.Value = cl.RiskTypeCode;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = true;
                    param = sqlCommandInsert.Parameters.Add("@Payments", SqlDbType.VarChar);
                    param.Value = cl.Payments;
                    param = sqlCommandInsert.Parameters.Add("@URL", SqlDbType.VarChar);
                    param.Value = cl.URL;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Actualiza datos del cliente
        /// </summary>
        /// <returns></returns>
        public static bool UpdateCliente(Cliente cl)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateCliente", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = cl.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@NombreCliente", SqlDbType.VarChar);
                    param.Value = cl.NombreCliente;
                    param = sqlCommandInsert.Parameters.Add("@Configuraciones", SqlDbType.VarChar);
                    param.Value = cl.Configuraciones;
                    param = sqlCommandInsert.Parameters.Add("@Apikey", SqlDbType.VarChar);
                    param.Value = cl.Apikey;
                    param = sqlCommandInsert.Parameters.Add("@Authorization", SqlDbType.VarChar);
                    param.Value = cl.Authorization;
                    param = sqlCommandInsert.Parameters.Add("@RiskTypeCode", SqlDbType.VarChar);
                    param.Value = cl.RiskTypeCode;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = cl.Active;
                    param = sqlCommandInsert.Parameters.Add("@Payments", SqlDbType.VarChar);
                    param.Value = cl.Payments;
                    param = sqlCommandInsert.Parameters.Add("@Url", SqlDbType.VarChar);
                    param.Value = cl.URL;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Desactiva al cliente
        /// </summary>
        /// <returns></returns>
        public static bool DesactivaCliente(int id)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_DisableCliente", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = id;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }



        /// <summary>
        ///
        /// </summary>
        /// <param name="fa"></param>
        /// <returns></returns>
        public static bool InsertFlujoAlterno(FlujoAlterno fa)
        {
            try
            {
                if (cnp.State == ConnectionState.Closed) cnp.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertFlujoAlterno", cnp))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = fa.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@Enabled", SqlDbType.Bit);
                    param.Value = fa.Enabled;
                    param = sqlCommandInsert.Parameters.Add("@EnabledLogin", SqlDbType.Bit);
                    param.Value = fa.EnabledLogin;
                    param = sqlCommandInsert.Parameters.Add("@EnabledMultiple", SqlDbType.Bit);
                    param.Value = fa.EnabledMultiple;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnp.State == ConnectionState.Open) cnp.Close();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal static Cliente GetClientesLast()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            Cliente cl = new Cliente();
            try
            {
                string SQL = "sp_GetClienteLast";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cl.IdCliente = (int)reader["IdCliente"];
                    cl.NombreCliente = (string)reader["NombreCliente"];
                    cl.Configuraciones = (string)reader["Configuraciones"];
                    cl.Apikey = (string)reader["Apikey"];
                    cl.Authorization = (string)reader["Authorization"];
                    cl.RiskTypeCode = (string)reader["RiskTypeCode"];
                    cl.Active = (bool)reader["Active"];
                    cl.Payments = (string)reader["Payments"];
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return cl;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        /// <returns></returns>
        public static List<Cliente> GetAllClientes()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<Cliente> list = new List<Cliente>();
            try
            {
                string SQL = "sp_GetAllClientes";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Cliente obj = new Cliente();
                    obj.IdCliente = (int)reader["IdCliente"];
                    obj.NombreCliente = (string)reader["NombreCliente"];
                    obj.Configuraciones = (string)reader["Configuraciones"];
                    obj.Apikey = (string)reader["Apikey"];
                    obj.Authorization = (string)reader["Authorization"];
                    obj.RiskTypeCode = (string)reader["RiskTypeCode"];
                    obj.Active = (bool)reader["Active"];
                    obj.Payments = (string)reader["Payments"];
                    list.Add(obj);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return list;
        }

        /// <summary>
        /// Obtiene cliente por Id
        /// </summary>
        /// <returns></returns>
        public static Cliente GetCliente(int Id)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            Cliente obj = new Cliente();
            try
            {
                string SQL = "sp_GetCliente";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Id", SqlDbType.Int);
                param.Value = Id;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    obj.IdCliente = (int)reader["IdCliente"];
                    obj.NombreCliente = (string)reader["NombreCliente"];
                    obj.Configuraciones = (string)reader["Configuraciones"];
                    obj.Apikey = reader["Apikey"] == DBNull.Value ? string.Empty : reader["Apikey"].ToString();
                    obj.RiskTypeCode = reader["RiskTypeCode"] == DBNull.Value ? string.Empty : reader["RiskTypeCode"].ToString();
                    obj.Active = (bool)reader["Active"];
                    obj.URL = (string)reader["urlclient"];

                }
                reader.Close();
                cn.Close();

                if (cn.State == ConnectionState.Closed) cn.Open();
                List<DealerCodes> dealerCodes = new List<DealerCodes>();
                dealerCodes = GetDealerCodesByIdCliente(Id);
                obj.DealerCode = dealerCodes;
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return obj;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <param name="risk"></param>
        /// <returns></returns>
        public static List<RiskTypeCode> GetRiskTypeByClienteRisk(int IdCliente, string risk)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<RiskTypeCode> list = new List<RiskTypeCode>();
            try
            {
                string SQL = "sp_GetRiskTypeByClienteRisk";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = IdCliente;
                param = cmd.Parameters.Add("@RiskType", SqlDbType.VarChar);
                param.Value = risk;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RiskTypeCode obj = new RiskTypeCode();
                    obj.IdRiskTypeCode = (int)reader["IdRiskTypeCode"];
                    obj.IdCliente = (int)reader["IdCliente"];
                    obj.RiskType = reader["RiskType"] == DBNull.Value ? string.Empty : reader["RiskType"].ToString();
                    obj.Marca = reader["Marca"] == DBNull.Value ? string.Empty : reader["Marca"].ToString();

                    list.Add(obj);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return list;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
        public static bool InsertRiskType(RiskTypeCode cl)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertRiskTypeCode", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = cl.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@RiskType", SqlDbType.VarChar);
                    param.Value = cl.RiskType;
                    param = sqlCommandInsert.Parameters.Add("@Marca", SqlDbType.VarChar);
                    param.Value = cl.Marca;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Actualiza datos del cliente
        /// </summary>
        /// <returns></returns>
        public static bool UpdateRiskTypeCode(RiskTypeCode cl)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_UpdateRiskTypeCode", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdRiskTypeCode", SqlDbType.Int);
                    param.Value = cl.IdRiskTypeCode;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = cl.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@RiskType", SqlDbType.VarChar);
                    param.Value = cl.RiskType;
                    param = sqlCommandInsert.Parameters.Add("@Marca", SqlDbType.VarChar);
                    param.Value = cl.Marca;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        /// <summary>
        /// Reemplaza \r\n por vacios
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DatatabletoJson(DataTable dt)
        {
            string JsonString = string.Empty;
            JsonString = JsonConvert.SerializeObject(dt, Formatting.Indented);
            return JsonString.Replace("\r\n", string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numerotransaccion"></param>
        /// <param name="transaccionJson"></param>
        /// <returns></returns>

        internal static bool InsertTransaction(string numerotransaccion, string transaccionJson)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertTransaccion", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@NumeroTransaccion", SqlDbType.VarChar);
                    param.Value = numerotransaccion;
                    param = sqlCommandInsert.Parameters.Add("@TransaccionJson", SqlDbType.VarChar);
                    param.Value = transaccionJson;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numerotransaccion"></param>
        /// <returns></returns>
        public static TransaccionData GetTransaccionByNumeroTransaccion(string numerotransaccion)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            TransaccionData obj = new TransaccionData();
            try
            {
                string SQL = "sp_GetTransaccionByTransaccion";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@NumeroTransaccion", SqlDbType.VarChar);
                param.Value = numerotransaccion;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    obj.IdTransaccion = (int)reader["IdTransaccion"];
                    obj.NumeroTransaccion = (string)reader["NumeroTransaccion"];
                    obj.TransaccionJson = (string)reader["TransaccionJson"];
                }
                reader.Close();
                cn.Close();

                if (cn.State == ConnectionState.Closed) cn.Open();
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return obj;
        }


        public static TransaccionData GetTransaccionByPolicy(string policy)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            TransaccionData obj = new TransaccionData();
            try
            {
                string SQL = "sp_GetTransaccionByPolicy";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@Policy", SqlDbType.VarChar);
                param.Value = policy;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    obj.IdTransaccion = (int)reader["IdTransaccion"];
                    obj.NumeroTransaccion = (string)reader["NumeroTransaccion"];
                    obj.TransaccionJson = (string)reader["TransaccionJson"];
                }
                reader.Close();
                cn.Close();

                if (cn.State == ConnectionState.Closed) cn.Open();
                reader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return obj;
        }

        public static List<EmailTemplate> GetEmailByCliente(int IdCliente)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<EmailTemplate> list = new List<EmailTemplate>();
            try
            {
                string SQL = "sp_GetEmailByIdCliente";
                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                param.Value = IdCliente;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    EmailTemplate obj = new EmailTemplate();
                    obj.IdEmail = (int)reader["IdEmail"];
                    obj.Descripcion = reader["Descripcion"] == DBNull.Value ? string.Empty : (string)reader["Descripcion"];
                    obj.BodyName = reader["BodyName"] == DBNull.Value ? string.Empty : (string)reader["BodyName"];
                    obj.Body = reader["Body"] == DBNull.Value ? string.Empty : (string)reader["Body"];
                    obj.SubjectPre = reader["SubjectPre"] == DBNull.Value ? string.Empty : (string)reader["SubjectPre"];
                    obj.SubjectPost = reader["SubjectPost"] == DBNull.Value ? string.Empty : (string)reader["SubjectPost"];
                    obj.CCEmails = reader["CCEmails"] == DBNull.Value ? string.Empty : (string)reader["CCEmails"];
                    obj.BCCEmails = reader["BCCEmails"] == DBNull.Value ? string.Empty : (string)reader["BCCEmails"];
                    obj.Active = (bool)reader["Active"];
                    obj.IdCliente = (int)reader["IdCliente"];
                    obj.EmailFrom = reader["EmailFrom"] == DBNull.Value ? string.Empty : (string)reader["EmailFrom"];
                    obj.Server = reader["Server"] == DBNull.Value ? string.Empty : (string)reader["Server"];
                    obj.Port = reader["Port"] == DBNull.Value ? string.Empty : (string)reader["Port"];
                    obj.EmailSenderName = reader["EmailSenderName"] == DBNull.Value ? string.Empty : (string)reader["EmailSenderName"];
                    obj.MessageTitle = reader["MessageTitle"] == DBNull.Value ? string.Empty : (string)reader["MessageTitle"];
                    obj.MessageMail = reader["MessageMail"] == DBNull.Value ? string.Empty : (string)reader["MessageMail"];
                    obj.MessageBody = reader["MessageBody"] == DBNull.Value ? string.Empty : (string)reader["MessageBody"];

                    list.Add(obj);
                }
                reader.Close();
                cn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return list;
        }

        internal static bool InsertEmailTemplate(EmailTemplate mail)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_Insert_EmailTemplate", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                    param.Value = mail.Descripcion;
                    param = sqlCommandInsert.Parameters.Add("@SubjectPre", SqlDbType.VarChar);
                    param.Value = mail.SubjectPre;
                    param = sqlCommandInsert.Parameters.Add("@SubjectPost", SqlDbType.VarChar);
                    param.Value = mail.SubjectPost;
                    param = sqlCommandInsert.Parameters.Add("@CCMails", SqlDbType.VarChar);
                    param.Value = mail.CCEmails;
                    param = sqlCommandInsert.Parameters.Add("@BCCEmails", SqlDbType.VarChar);
                    param.Value = mail.BCCEmails;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = mail.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@EmailFrom", SqlDbType.VarChar);
                    param.Value = mail.EmailFrom;
                    param = sqlCommandInsert.Parameters.Add("@Server", SqlDbType.VarChar);
                    param.Value = mail.Server;
                    param = sqlCommandInsert.Parameters.Add("@Port", SqlDbType.VarChar);
                    param.Value = mail.Port;
                    param = sqlCommandInsert.Parameters.Add("@EmailSenderName", SqlDbType.VarChar);
                    param.Value = mail.EmailSenderName;
                    param = sqlCommandInsert.Parameters.Add("@MessageTitle", SqlDbType.VarChar);
                    param.Value = mail.MessageTitle;
                    param = sqlCommandInsert.Parameters.Add("@MessageBody", SqlDbType.VarChar);
                    param.Value = mail.MessageBody;
                    param = sqlCommandInsert.Parameters.Add("@MessageMail", SqlDbType.VarChar);
                    param.Value = mail.MessageMail;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        internal static bool UpdateEmailTemplate(EmailTemplate mail)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_Update_EmailTemplate", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdEmail", SqlDbType.VarChar);
                    param.Value = mail.IdEmail;
                    param = sqlCommandInsert.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                    param.Value = mail.Descripcion;
                    param = sqlCommandInsert.Parameters.Add("@SubjectPre", SqlDbType.VarChar);
                    param.Value = mail.SubjectPre;
                    param = sqlCommandInsert.Parameters.Add("@SubjectPost", SqlDbType.VarChar);
                    param.Value = mail.SubjectPost;
                    param = sqlCommandInsert.Parameters.Add("@CCMails", SqlDbType.VarChar);
                    param.Value = mail.CCEmails;
                    param = sqlCommandInsert.Parameters.Add("@BCCEmails", SqlDbType.VarChar);
                    param.Value = mail.BCCEmails;
                    param = sqlCommandInsert.Parameters.Add("@Active", SqlDbType.Bit);
                    param.Value = mail.Active;
                    param = sqlCommandInsert.Parameters.Add("@IdCliente", SqlDbType.VarChar);
                    param.Value = mail.IdCliente;
                    param = sqlCommandInsert.Parameters.Add("@EmailFrom", SqlDbType.VarChar);
                    param.Value = mail.EmailFrom;
                    param = sqlCommandInsert.Parameters.Add("@Server", SqlDbType.VarChar);
                    param.Value = mail.Server;
                    param = sqlCommandInsert.Parameters.Add("@Port", SqlDbType.VarChar);
                    param.Value = mail.Port;
                    param = sqlCommandInsert.Parameters.Add("@EmailSenderName", SqlDbType.VarChar);
                    param.Value = mail.EmailSenderName;
                    param = sqlCommandInsert.Parameters.Add("@MessageTitle", SqlDbType.VarChar);
                    param.Value = mail.MessageTitle;
                    param = sqlCommandInsert.Parameters.Add("@MessageBody", SqlDbType.VarChar);
                    param.Value = mail.MessageBody;
                    param = sqlCommandInsert.Parameters.Add("@MessageMail", SqlDbType.VarChar);
                    param.Value = mail.MessageMail;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static bool DesactivaEmail(int id)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_Disable_EmailTemplate", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@IdEmail", SqlDbType.VarChar);
                    param.Value = id;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        internal static bool InsertSaveDecoToken(string decoTokenJson)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertDecoToken", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@DecoTokenJson", SqlDbType.VarChar);
                    param.Value = decoTokenJson;
                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static List<TiposBusqueda> GetTipoBusquedaByCliente()
        {

            List<TiposBusqueda> list = new List<TiposBusqueda>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetTipoBusquedaByIdCliente", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    //param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                    //param.Value = IdCliente;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TiposBusqueda obj = new TiposBusqueda();
                            obj.IdMetodoBusqueda = (int)reader["IdMetodoBusqueda"];
                            obj.MetodoBusqueda = reader["MetodoBusqueda"] == DBNull.Value ? string.Empty : reader["MetodoBusqueda"].ToString();
                            obj.IdCliente = (int)reader["IdCliente"];
                            obj.Parametros = reader["Parametros"] == DBNull.Value ? string.Empty : reader["Parametros"].ToString();

                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }


        public static List<NextStep> GetNextStepByClient(string IdCliente, string DealerCode)
        {

            List<NextStep> list = new List<NextStep>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetNextStep", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = Convert.ToInt32(IdCliente);
                    param = cmd.Parameters.Add("@DealerCode", SqlDbType.VarChar);
                    param.Value = DealerCode;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NextStep obj = new NextStep();
                            obj.IdCliente = (int)reader["IdCliente"];
                            obj.DealerCode = reader["DealerCode"] == DBNull.Value ? string.Empty : reader["DealerCode"].ToString();
                            obj.CoverageType = reader["CoverageType"] == DBNull.Value ? string.Empty : reader["CoverageType"].ToString();
                            obj.MethodOfRepairCode = reader["MethodOfRepairCode"] == DBNull.Value ? string.Empty : reader["MethodOfRepairCode"].ToString();
                            obj.Icon = reader["Icon"] == DBNull.Value ? string.Empty : reader["Icon"].ToString();
                            obj.Text = reader["Text"] == DBNull.Value ? string.Empty : reader["Text"].ToString();
                            obj.Title = reader["Title"] == DBNull.Value ? string.Empty : reader["Title"].ToString();
                            obj.RiskType = reader["RiskType"] == DBNull.Value ? string.Empty : reader["RiskType"].ToString();
                            obj.Languaje = reader["Lenguaje"] == DBNull.Value ? string.Empty : reader["Lenguaje"].ToString();

                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }


        public static List<TrackingCode> GetTrackingCode(int IdCliente)
        {

            List<TrackingCode> list = new List<TrackingCode>();
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetTrackingSteps", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = cmd.Parameters.Add("@IdCliente", SqlDbType.Int);
                    param.Value = Convert.ToInt32(IdCliente);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TrackingCode obj = new TrackingCode();
                            obj.Id = (int)reader["Id"];
                            obj.IdCliente = (int)reader["Id_Cliente"];
                            obj.Code = reader["Code"] == DBNull.Value ? string.Empty : reader["Code"].ToString();
                            obj.Text = reader["Text"] == DBNull.Value ? string.Empty : reader["Text"].ToString();
                            obj.Visible = reader["Visible"] == DBNull.Value ? false : (bool)reader["Visible"];
                            obj.comment_extra = reader["comment_extra"] == DBNull.Value ? false : (bool)reader["comment_extra"];

                            list.Add(obj);
                        }

                        reader.Close();
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally { if (cn.State == ConnectionState.Open) cn.Close(); }
            return list;
        }


        public static bool InsertMethodOfRepaired(DataTable dt, string Id)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertMethodOfRepair", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Id", SqlDbType.VarChar);
                    param.Value = Convert.ToInt32(Id);
                    param = sqlCommandInsert.Parameters.AddWithValue("@dt", dt);


                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    return true;

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        public static bool InsertTextDinamic(DataTable dt, string Id)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                using (SqlCommand sqlCommandInsert = new SqlCommand("sp_InsertNextSteps", cn))
                {
                    sqlCommandInsert.CommandType = CommandType.StoredProcedure;
                    SqlParameter param;
                    param = sqlCommandInsert.Parameters.Add("@Id", SqlDbType.VarChar);
                    param.Value = Convert.ToInt32(Id);
                    param = sqlCommandInsert.Parameters.AddWithValue("@dt", dt);


                    int rowsAffected = sqlCommandInsert.ExecuteNonQuery();
                    return true;

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static List<ApiKeyKitt> GetAllApiKeys()
        {
            if (cn.State == ConnectionState.Closed) cn.Open();
            List<ApiKeyKitt> apiKeys = new List<ApiKeyKitt>();
            try
            {
                SqlCommand command = new SqlCommand("sp_GetAllApiKeys", cn);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ApiKeyKitt apiKey = new ApiKeyKitt();
                    apiKey.Id = (int)reader["Id"];
                    apiKey.IdCliente = (int)reader["IdCliente"];
                    apiKey.TenantId = (int)reader["TenantId"];
                    apiKey.ApiKey = (reader["ApiKey"] == DBNull.Value) ? "" : (string)reader["ApiKey"];
                    apiKey.Usernamemail = (reader["Usernamemail"] == DBNull.Value) ? "" : (string)reader["Usernamemail"];
                    apiKey.Pwd = (reader["Pwd"] == DBNull.Value) ? "" : (string)reader["Pwd"];

                    apiKeys.Add(apiKey);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }


            return apiKeys;
        }

        public static List<ServiceProviderConfiguration> GetServiceProviderConfiguration(int? Id = null, string ProviderName = null)
        {
            List<ServiceProviderConfiguration> serviceProviders = new List<ServiceProviderConfiguration>();

            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();

                SqlCommand command = new SqlCommand("sp_GetServiceProviderConfiguration", cn);
                command.CommandType = CommandType.StoredProcedure;


                if (!string.IsNullOrEmpty(ProviderName))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@ProviderName", SqlDbType = SqlDbType.NVarChar, Size = 1000, Value = ProviderName, Direction = ParameterDirection.Input });
                }

                if (Id.HasValue)
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = Id });
                }

                
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ServiceProviderConfiguration providerConfiguration = new ServiceProviderConfiguration();

                    providerConfiguration.Id = (int)reader["Id"];
                    providerConfiguration.ProviderName = (reader["ProviderName"] == DBNull.Value) ? "" : (string)reader["ProviderName"];
                    providerConfiguration.Configuration = (reader["Configuration"] == DBNull.Value) ? "" : (string)reader["Configuration"];
                    providerConfiguration.BaseUrl = (string)reader["BaseUrl"];
                    providerConfiguration.ApiKey = (reader["ApiKey"] == DBNull.Value) ? "" : (string)reader["ApiKey"];
                    providerConfiguration.SvcUsr = (reader["ServiceUser"] == DBNull.Value) ? "" : (string)reader["ServiceUser"];
                    providerConfiguration.SvcPwd = (reader["ServicePwd"] == DBNull.Value) ? "" : (string)reader["ServicePwd"];
                    providerConfiguration.SvcType = (reader["ServiceType"] == DBNull.Value) ? "" : (string)reader["ServiceType"];
                    providerConfiguration.CreationDate = (DateTime)reader["CreationDate"];
                    providerConfiguration.ModifiedDate = (reader["ModifiedDate"] == DBNull.Value) ? new DateTime() : (DateTime)reader["ModifiedDate"];
                    providerConfiguration.Active = (bool)reader["Active"];

                    serviceProviders.Add(providerConfiguration);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return serviceProviders;

        }


        public static void CRUDServiceProviderConfiguration(ref ServiceProviderConfiguration providerConfiguration, string operation)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                SqlCommand command = new SqlCommand("sp_CRUDServiceProviceConfiguration", cn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter() { ParameterName = "@IdProviderConfig", SqlDbType = SqlDbType.Int, Value = providerConfiguration.Id, Direction = ParameterDirection.InputOutput });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Configuration", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.Configuration, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ProviderName", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.ProviderName, Size = 1000, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@BaseUrl", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.BaseUrl, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ApiKey", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.ApiKey, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ServiceUser", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.SvcUsr, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ServicePwd", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.SvcPwd, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ServiceType", SqlDbType = SqlDbType.NVarChar, Value = providerConfiguration.SvcType, Size = 20, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Active", SqlDbType = SqlDbType.Bit, Value = providerConfiguration.Active, Direction = ParameterDirection.Input });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Opertation", SqlDbType = SqlDbType.Char, Value = operation, Size = 1, Direction = ParameterDirection.Input });

                command.ExecuteNonQuery();

                if (operation == "C")
                {
                    providerConfiguration.Id = (int)command.Parameters["@IdProviderConfig"].Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static bool InsertUpdateProgram(ref Program Program, string Action)
        {
            bool isSuccessfullyExecuted = false;


            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = Action == "C" ? "Insert Program" : "Update Porgram";
                string chis = JsonConvert.SerializeObject(ch);
                Program.CrudHistoryList = chis;
                SqlCommand command = new SqlCommand("sp_CRUDProgram", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

                foreach (PropertyInfo property in Program.GetType().GetProperties())
                {

                    if (!property.PropertyType.IsGenericType)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = string.Format("@{0}", property.Name);
                        param.Value = property.GetValue(Program);
                        param.Direction = ParameterDirection.Input;

                        switch (property.PropertyType.Name)
                        {
                            case "Int32":
                                param.SqlDbType = SqlDbType.Int;
                                break;
                            case "String":
                                param.SqlDbType = SqlDbType.VarChar;
                                break;
                            case "Boolean":
                                param.SqlDbType = SqlDbType.Bit;
                                break;
                        }

                        command.Parameters.Add(param);
                    }
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "@Action", Direction = ParameterDirection.Input, SqlDbType = SqlDbType.Char, Size = 1, Value = Action });

                command.ExecuteNonQuery();

                if (Action == "C")
                {
                    Program.Id = (int)command.Parameters["@RETURN_VALUE"].Value;
                }

                isSuccessfullyExecuted = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return isSuccessfullyExecuted;
        }

        public static bool InsertUpdateDeleteProduct(Product product, string action, out int IdProduct)
        {
            bool isSuccess = false;

            try
            {

                if (cn.State == ConnectionState.Closed) cn.Open();
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = action == "C" ? "Insert Product" : "Update Porgram";
                string chis = JsonConvert.SerializeObject(ch);
                product.CrudHistoryList = chis;
                SqlCommand command = new SqlCommand("sp_CRUDProduct", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });


                foreach (PropertyInfo property in product.GetType().GetProperties())
                {
                    if (!property.PropertyType.IsGenericType)
                    {

                        SqlParameter parameter = new SqlParameter();
                        parameter.ParameterName = string.Format("@{0}", property.Name);
                        parameter.Direction = ParameterDirection.Input;
                        parameter.Value = property.GetValue(product);

                        switch (property.PropertyType.Name)
                        {
                            case "Int32":
                                parameter.SqlDbType = SqlDbType.Int;
                                break;
                            case "String":
                                parameter.SqlDbType = SqlDbType.VarChar;
                                break;
                            case "Boolean":
                                parameter.SqlDbType = SqlDbType.Bit;
                                break;
                        }

                        command.Parameters.Add(parameter);
                    }
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "@Action", Direction = ParameterDirection.Input, SqlDbType = SqlDbType.Char, Size = 1, Value = action });


                command.ExecuteNonQuery();

                IdProduct = (action == "C") ? (int)command.Parameters["@RETURN_VALUE"].Value : 0;


                isSuccess = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return isSuccess;
        }

        public static List<Rate> GetRates(int IdProduct, int IdProgram)
        {
            List<Rate> rates = new List<Rate>();

            try
            {

                if (cn.State == ConnectionState.Closed) cn.Open();
                int idpro = Convert.ToInt32(IdProduct);
                int idprog = Convert.ToInt32(IdProgram);
                SqlCommand cmd = new SqlCommand("sp_CRUDRate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IdProduct", SqlDbType = SqlDbType.Int, Value = idpro });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@IdProgram", SqlDbType = SqlDbType.Int, Value = idprog });
                cmd.Parameters.Add(new SqlParameter() { ParameterName = "@Operation", SqlDbType = SqlDbType.Char, Value = "R", Size = 1 });

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Rate rate = new Rate();

                    rate.Id = (int)reader["Id"];
                    rate.Term = (int)reader["Term"];
                    rate.IdProduct = (int)reader["IdProduct"];
                    rate.IdClassType = (int)reader["IdClassType"];
                    rate.ClassCode = (string)reader["ClassCode"];
                    rate.MaxKm = (int)reader["MaxKm"];
                    rate.Deductible = (decimal)reader["Deductible"];
                    rate.PriceCert = (decimal)reader["PriceCert"];
                    rate.PriceEW = (decimal)reader["PriceEW"];
                    rate.PriceWP = (decimal)reader["PriceWP"];
                    rate.MKT = (reader["MKT"] == DBNull.Value) ? 0 : (decimal)reader["MKT"];
                    rate.PlanCodeEW = (int)reader["PlanCodeEW"];
                    rate.PriceRA = (decimal)reader["PriceRA"];
                    rate.PlanCodeRA = (int)reader["PlanCodeRA"];
                    rate.Prime = (decimal)reader["Prime"];
                    rate.AgencyCommission = (decimal)reader["AgencyCommission"];
                    rate.VendorCommission = (decimal)reader["VendorCommission"];
                    rate.DealerCommission = (decimal)reader["DealerCommission"];
                    rate.AdminFee = (decimal)reader["AdminFee"];
                    rate.InsurancePolicy = (decimal)reader["InsurancePolicy"];
                    rate.CommAnzen = (decimal)reader["CommAnzen"];
                    rate.Rewards = (decimal)reader["Rewards"];
                    rate.CommNR = (decimal)reader["CommNR"];
                    rate.CommPlant = (decimal)reader["CommPlant"];
                    rate.RiskPremium = (decimal)reader["RiskPremium"];
                    rate.Profit = (decimal)reader["Profit"];
                    rate.IdProgram = (int)reader["IdProgram"];
                    rate.Taxes_Percent = (reader["Taxes_Percent"] == DBNull.Value) ? 0 : (decimal)reader["Taxes_Percent"];
                    rate.CrudHistoryList = (string)reader["CrudHistoryList"];
                    rate.Active = (bool)reader["Active"];

                    rates.Add(rate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }


            return rates;
        }


        public static bool InsertUpdateRate(Rate Rate, string Operation, out int IdRate)
        {
            bool isSuccess = false;

            try
            {

                if (cn.State == ConnectionState.Closed) cn.Open();
                CrudHistory ch = new CrudHistory();
                ch.Date = DateTime.Now;
                ch.Type = 'C';
                ch.IdUser = 0;
                ch.Comments = Operation == "C" ? "Insert Rate" : "Update Porgram";
                string chis = JsonConvert.SerializeObject(ch);
                Rate.CrudHistoryList = chis;
                SqlCommand command = new SqlCommand("sp_CRUDRate", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

                foreach (PropertyInfo property in Rate.GetType().GetProperties())
                {
                    switch (property.PropertyType.Name)
                    {
                        case "Int32":
                            command.Parameters.Add(new SqlParameter() { ParameterName = string.Format("@{0}", property.Name), Direction = ParameterDirection.Input, SqlDbType = SqlDbType.Int, Value = property.GetValue(Rate) });
                            break;
                        case "String":
                            command.Parameters.Add(new SqlParameter() { ParameterName = string.Format("@{0}", property.Name), Direction = ParameterDirection.Input, SqlDbType = SqlDbType.VarChar, Value = property.GetValue(Rate) });
                            break;
                        case "Decimal":
                            command.Parameters.Add(new SqlParameter() { ParameterName = string.Format("@{0}", property.Name), Direction = ParameterDirection.Input, SqlDbType = SqlDbType.Decimal, Value = property.GetValue(Rate) });
                            break;
                        case "Boolean":
                            command.Parameters.Add(new SqlParameter() { ParameterName = string.Format("@{0}", property.Name), Direction = ParameterDirection.Input, SqlDbType = SqlDbType.Bit, Value = property.GetValue(Rate) });
                            break;
                    }
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "@Operation", SqlDbType = SqlDbType.Char, Size = 1, Value = Operation });

                command.ExecuteNonQuery();


                IdRate = (Operation == "C") ? (int)command.Parameters["@RETURN_VALUE"].Value : 0;


                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return isSuccess;
        }


        public static void CRUDBDEOInspeciton(ref BDEOCaseInspection caseInspection, string action)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();

                SqlCommand command = new SqlCommand("sp_CRUDBDEOInspection", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RETURN_VALUE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

                if (action == "C" || action == "U")
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@RequestData", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input, Value = caseInspection.RequestData });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@CrudHistoryList", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input, Value = caseInspection.CrudHistoryList });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@Active", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input, Value = caseInspection.Active });

                    if (action == "U")
                    {
                        command.Parameters.Add(new SqlParameter() { ParameterName = "@CaseData", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input, Value = caseInspection.CaseData });
                    }

                }
                else if (action == "R" || action == "D")
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = caseInspection.Id });
                }

                command.Parameters.Add(new SqlParameter() { ParameterName = "@Action", SqlDbType = SqlDbType.Char, Size = 1, Direction = ParameterDirection.Input, Value = action });

                if (action == "R")
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        caseInspection.Id = (int)reader["Id"];
                        caseInspection.RequestData = (string)reader["RequestData"];
                        caseInspection.CaseData = (reader["CaseData"] == DBNull.Value) ? "" : (string)reader["CaseData"];
                        caseInspection.CrudHistoryList = (string)reader["CrudHistoryList"];
                        caseInspection.Active = (bool)reader["Active"];
                    }
                }
                else
                {
                    command.ExecuteNonQuery();

                    if (action == "C")
                    {
                        caseInspection.Id = (int)command.Parameters["@RETURN_VALUE"].Value;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

        }

        public static void CRUDCase(ref Case _case, string action)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                int id = Convert.ToInt32(_case.Id);
                string ci = _case.CaseIdentifier;
                string pn = _case.PhoneNumber;
                int sti = Convert.ToInt32(_case.StatusId);
                string cd = _case.CustomerData;
                string ch = _case.CrudHistoryList;
                string ac = action;
                bool act = _case.Active;

                if (action == "C")
                {
                    List<CrudHistory> crudHistoryList = new List<CrudHistory>() { new CrudHistory() { Date = DateTime.Now, Comments = "Creación inicial", IdUser = 0, Type = 'C' } };
                    ch = JsonConvert.SerializeObject(crudHistoryList);
                }

                SqlCommand command = new SqlCommand("sp_CRUDCase", cn);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = command.Parameters.Add("@Id", SqlDbType.Int);
                param.Direction = ParameterDirection.InputOutput;
                param.Value = id;                
                param = command.Parameters.Add("@CaseIdentifier", SqlDbType.VarChar);
                param.Value = ci;
                param.Size = 1000;
                param.Direction = ParameterDirection.InputOutput;
                param = command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                param.Value = pn;
                param.Size = 20;
                param = command.Parameters.Add("@StatusId", SqlDbType.Int);
                param.Value = sti;
                param = command.Parameters.Add("@Status", SqlDbType.VarChar);
                param.Direction = ParameterDirection.Output;
                param.Size = 50;
                param = command.Parameters.Add("@CustomerData", SqlDbType.VarChar);
                param.Value = cd;
                param = command.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                param.Value = ch;
                param = command.Parameters.Add("@Active", SqlDbType.Bit);
                param.Value = act;
                param = command.Parameters.Add("@Action", SqlDbType.Char);
                param.Value = ac;
                param.Size = 1;

                command.ExecuteNonQuery();

                if (action == "C")
                {
                    _case.Id = (int)command.Parameters["@Id"].Value;
                    _case.CaseIdentifier = (string)command.Parameters["@CaseIdentifier"].Value;
                }

                _case.Status = (command.Parameters["@Status"].Value == DBNull.Value) ? "" :  (string)command.Parameters["@Status"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static List<Case> getCaseByIdentifierOrPhoneNumber(string caseIdentifier, string phoneNumber = null)
        {
            List<Case> _caseList = new List<Case>();

            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();

                SqlCommand command = new SqlCommand("sp_CRUDCase", cn);
                command.CommandType = CommandType.StoredProcedure;
                string pn = (phoneNumber);
                string ci = (caseIdentifier);
                SqlParameter param;
                param = command.Parameters.Add("@Id", SqlDbType.Int);
                param.Value = 0;
                param.Direction = ParameterDirection.InputOutput;
                param = command.Parameters.Add("@CaseIdentifier", SqlDbType.VarChar);
                param.Value = ci;
                param.Direction = ParameterDirection.InputOutput;
                param.Size = 1000;
                if (!string.IsNullOrEmpty(pn))
                {
                    param = command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                    param.Size = 20;
                    param.Value = pn;
                }
                param = command.Parameters.Add("@Action", SqlDbType.VarChar);
                param.Value = "R";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Case _case = new Case();
                    _case.Id = (int)reader["Id"];
                    _case.CaseIdentifier = (string)reader["CaseIdentifier"];
                    _case.PhoneNumber = (string)reader["PhoneNumber"];
                    _case.StatusId = (int)reader["StatusId"];
                    _case.Status = (reader["Status"] == DBNull.Value) ? "" : (string)reader["Status"];
                    _case.CustomerData = (reader["CustomerData"] == DBNull.Value) ? "" : (string)reader["CustomerData"];
                    _case.CrudHistoryList = (string)reader["CrudHistoryList"];
                    _case.Active = (bool)reader["Active"];

                    _caseList.Add(_case);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return _caseList;
        }

        public static void CreateTrackingSisNet(SisNetTracking data)
        {
            try
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                int sc = Convert.ToInt32(data.StCo);
                string rq = (data.Rqt);
                string rs = (data.Rsn);
                SqlCommand command = new SqlCommand("sp_CRUDSisNetOperations", cn);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                param.Direction = ParameterDirection.ReturnValue;
                param = command.Parameters.Add("@Command", SqlDbType.VarChar);
                param.Value = data.Command;
                param = command.Parameters.Add("@StatusCode", SqlDbType.Int);
                param.Value = sc;
                param = command.Parameters.Add("@Request", SqlDbType.VarChar);
                param.Value = rq;
                param = command.Parameters.Add("@Response", SqlDbType.VarChar);
                param.Value = rs;
                param = command.Parameters.Add("@Action", SqlDbType.Char);
                param.Value = "C";

                command.ExecuteNonQuery();

            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<List<CaseStatus>> GetCaseStatus(int? Id, string Description = null)
        {
            List<CaseStatus> caseStatusList = new List<CaseStatus>();

            try
            {
                if (cn.State == ConnectionState.Closed) await cn.OpenAsync();

                int? valueParamId = (!Id.HasValue) ? 0 : Id;
                int sc = Convert.ToInt32(Id);
                string rs = (Description);
                SqlCommand command = new SqlCommand("sp_CRUDCaseStatus", cn);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = command.Parameters.Add("@Id", SqlDbType.Int);
                param.Value = sc;
                if (!string.IsNullOrEmpty(rs))
                {
                    param = command.Parameters.Add("@Description", SqlDbType.VarChar);
                    param.Value = Description;
                }
                param = command.Parameters.Add("@Action", SqlDbType.Char);
                param.Value = "R";

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while(reader.Read()) 
                {
                    CaseStatus caseStatus = new CaseStatus();
                    caseStatus.Id = (int)reader["Id"];
                    caseStatus.Description = (string)reader["Description"];
                    caseStatus.CrudHistoryList = (reader["CrudHistoryList"] == DBNull.Value) ? "" : (string)reader["CrudHistoryList"];
                    caseStatus.Active = (bool)reader["Active"];

                    caseStatusList.Add(caseStatus);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }

            return caseStatusList;
        }

        public static async Task<CaseStatus> CaseStatusCrud(CaseStatus caseStatus, string action)
        {
            List<CrudHistory> crudHistoryList = null;

            try
            {
                if (action == "C")
                {
                    crudHistoryList = new List<CrudHistory>() { new CrudHistory() { Date = DateTime.Now, IdUser = 0, Comments = "Creación inicial", Type = 'C' } };
                } 
                else if (action == "U")
                {
                    crudHistoryList = JsonConvert.DeserializeObject<List<CrudHistory>>(caseStatus.CrudHistoryList);

                    if (crudHistoryList.Count == 1)
                    {
                        crudHistoryList.Add(new CrudHistory() { Date = DateTime.Now, IdUser = 0, Comments = "Actualizacion del estatus", Type = 'U' });
                    } 
                    else if (crudHistoryList.Count == 2)
                    {
                        CrudHistory history = crudHistoryList.ElementAt(1);
                        history.Date = DateTime.Now;
                    }

                }
                else if (action == "D")
                {
                    List<CaseStatus> statusList = await GetCaseStatus(caseStatus.Id, null);
                    string crudHistoryListStr = statusList.ElementAt(0).CrudHistoryList;

                    crudHistoryList = JsonConvert.DeserializeObject<List<CrudHistory>>(crudHistoryListStr);

                    crudHistoryList.Add(new CrudHistory() { Date = DateTime.Now, IdUser = 0, Comments = "Deshabilitado del estatus", Type = 'D', });
                }

                caseStatus.CrudHistoryList = (crudHistoryList != null) ? JsonConvert.SerializeObject(crudHistoryList) : "";
                

                if (cn.State == ConnectionState.Closed) await cn.OpenAsync();

                int sc = Convert.ToInt32(caseStatus.Id);
                string rq = (action);

                SqlCommand command = new SqlCommand("sp_CRUDCaseStatus", cn);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter param;
                param = command.Parameters.Add("@Id", SqlDbType.Int);
                param.Value = sc;               

                if (rq == "C" || rq == "U")
                {
                    param = command.Parameters.Add("@Description", SqlDbType.VarChar);
                    param.Value = caseStatus.Description;
                }
                param = command.Parameters.Add("@CrudHistoryList", SqlDbType.VarChar);
                param.Value = caseStatus.CrudHistoryList;
                param = command.Parameters.Add("@Action", SqlDbType.VarChar);
                param.Value = action;

                await command.ExecuteNonQueryAsync();

                if (action == "C")
                {
                    caseStatus.Id = (int)command.Parameters["@Id"].Value;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return caseStatus;

        }
    }
}
