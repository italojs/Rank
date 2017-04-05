using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Back.Api.Positivo.LineInfo.DAL
{
    public class DatabaseODBC : IDataBase
    {
        private OdbcConnection _connection = null;
        private OdbcCommand _command = null;

        public DatabaseODBC()
        {
            _connection = new OdbcConnection();
            string connectionString = ConfigurationManager.ConnectionStrings["BDMultimidia"].ConnectionString;
            _connection.ConnectionString = connectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("String de conexão com o banco não foi encontrada.");

            _command = new OdbcCommand();
            _command.Connection = (OdbcConnection)_connection;
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandTimeout = 90000;
        }

        private void SetParameters(DbCommand commands)
        {
            _command.CommandText = commands.CommandText;

            foreach (OdbcParameter command in commands.Parameters)
            {
                if (command.Value == null)
                {
                    OdbcParameter param = new OdbcParameter(command.ParameterName, command.OdbcType, command.Size);
                    param.Precision = command.Precision;
                    param.Scale = command.Scale;

                    _command.Parameters.Add(param).Direction = ParameterDirection.Output;
                }
                else
                {
                    _command.Parameters.AddWithValue(command.ParameterName, command.Value);
                }
            }
        }

        public List<object> OExecProc(DbCommand commands)
        {

            List<object> objs = new List<object>();
            try
            {
                if (commands != null)
                {
                    SetParameters(commands);

                    OpenConnection();
                    _command.ExecuteNonQuery();

                    foreach (OdbcParameter parameter in _command.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            objs.Add(parameter.Value);
                        }
                    }

                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Commands not found");

            }

            return objs;
        }

        public void IExecNonQueryProc(DbCommand commands)
        {

            List<object> objs = new List<object>();
            try
            {
                if (commands != null)
                {
                    SetParameters(commands);

                    OpenConnection();
                    _command.ExecuteNonQuery();
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public List<Dictionary<string, object>> IOExecProc(DbCommand commands)
        {
            List<Dictionary<string, object>> lDict = new List<Dictionary<string, object>>();

            try
            {
                //using (_connection)

                //{
                    OpenConnection();
                    using (commands)
                    {
                        SetParameters(commands);
                        OdbcDataReader reader = _command.ExecuteReader();

                        while (reader.Read())
                        {
                            Dictionary<string, object> line = new Dictionary<string, object>();

                            for (int x = 0; x < reader.FieldCount; x++)
                            {
                                if (!reader.IsDBNull(x))
                                    line.Add(reader.GetName(x), reader.GetValue(x).ToString());
                                else
                                    line.Add(reader.GetName(x), 0);
                            }
                            lDict.Add(line);
                        }
                        reader.Close();
                    }
                //}
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            _connection.Close();

            return lDict;
        }

        private void OpenConnection()
        {
            // _connection.ConnectionString = connectionString;
            _connection.Open();
        }

        private void CloseConnection()
        {
            _connection.Close();
            _connection.Dispose();
            _command.Dispose();
        }
    }
}