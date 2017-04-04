using Back.Api.Positivo.LineInfo.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Back.Api.Positivo.LineInfo.Models
{
    public class LineInfoModelFake : ILineInfo
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
            get { return _name; }
            private set { }
        }
        public int Length
        {
            get { return _length; }
            private set { }
        }
        public int Position
        {
            get { return _lineRank; }
            private set { }
        }
        public int EstimatedTime
        {
            get { return _estimatedWaitTime; }
            private set { }
        }

        #endregion

        #region Construction
        private LineInfoModelFake()
        {
        }
        #endregion

        #region Builders
        public static class Builder
        {

            private static LineInfoModelFake _lineInfo = null;
            private static IDataBase _db;

            public static LineInfoModelFake Build(IDataBase db)
            {
                _lineInfo = new LineInfoModelFake();

                _db = db;

                Dictionary<string, object> obj = _db.IOExecProc(GetSqlCommand()).First();


                //programação defensiva

                _lineInfo._name = obj["Name"].ToString();
                _lineInfo._length = Convert.ToUInt16(obj["Length"]);
                _lineInfo._lineRank = Convert.ToUInt16(obj["Rank"]);
                _lineInfo._clerkLength = Convert.ToInt16(ConfigurationManager.AppSettings["ClerkLength"]);
                _lineInfo._timeMedioResponse = Convert.ToInt16(ConfigurationManager.AppSettings["TimeResponse"]);

                _lineInfo.CalculateEstimatedWaitTime();

                return _lineInfo;
            }


            private static SqlCommand GetSqlCommand()
            {
                SqlCommand command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "nome da proc";

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