using Back.Api.Positivo.LineInfo.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Back.Api.Positivo.LineInfo.Models
{
    public  class LineInfoModel : ILineInfo
    {
        #region Fields
        private string _name;
        private int _length;
        private int _lineRank;
        private int _estimatedWaitTime;
        private int _clerkLength; 
        private int _timeMedioResponse;
        #endregion

        #region Properties
        public string Name
        {
            get{return _name;}
            private set { }
        }
        public int Length
        {
            get{ return _length; }
            private set { }
        }
        public int Position
        {
            get{ return _lineRank; }
            private set { }
        }
        public int EstimatedTime
        {
            get{return _estimatedWaitTime;}
            private set { }
        }
        
        #endregion

        #region Construction
        private LineInfoModel()
        {
        }
        #endregion

        #region Builders
        public static class Builder
        {

            private static LineInfoModel _lineInfo = null;
            private static IDataBase _db;

            public static LineInfoModel Build(IDataBase db, int CustumerId)
            {
                _lineInfo = new LineInfoModel();

                _db = db;

                Dictionary<string, object> obj = _db.IOExecProc(GetSqlCommand(CustumerId)).First();
 
                _lineInfo._name = obj["Name"].ToString() ?? "null";
                _lineInfo._length = Convert.ToUInt16(obj["Length"]);
                _lineInfo._lineRank = Convert.ToUInt16(obj["Rank"]);
                _lineInfo._clerkLength = Convert.ToInt16(ConfigurationManager.AppSettings["ClerkLength"]);
                _lineInfo._timeMedioResponse = Convert.ToInt16(ConfigurationManager.AppSettings["TimeResponse"]);

                _lineInfo.CalculateEstimatedWaitTime();

                return _lineInfo;
            }

            
            private static OdbcCommand GetSqlCommand(int customerId) {
                OdbcCommand command = new OdbcCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = $"Procedure";
                //Query Procedure bellow
                //SELECT SkillsetName FROM vw.ContactsByContacts a WHERE a.ContactStatus = 'New' AND a.CustomerID = {customerId}
                //command.Parameters.AddWithValue("@customerId", customerId);

                return command;
            }
            
        }

       
        #endregion

        #region Methods
        public void CalculateEstimatedWaitTime()
        {
            _estimatedWaitTime = (_lineRank * _timeMedioResponse) / _clerkLength;
        }
        #endregion
    }
}