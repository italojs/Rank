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
        private int _position;
        private int _estimatedWaitTime;
        private int _TotalAgents ; 
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
            get{ return _position; }
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
            private static string[] Querys = {
                    "SELECT top 1 SkillsetName LineName, ContactID FROM vw.ContactsByContacts a WHERE a.ContactStatus = 'New' AND a.CustomerID = {0} order by a.ContactOpenTime desc",
                    "SELECT ContactId FROM vw.ContactsByContacts a WHERE a.ContactStatus = 'New' AND a.SkillsetName = '{0}' order by a.ContactOpenTime desc",
                    "SELECT top 100 ContactTotalOpenDuration FROM vw.ContactsByContacts a WHERE a.ContactStatus = 'Closed' AND a.SkillsetName = '{0}' order by a.ContactClosedTime desc"
                };

            public static LineInfoModel Build(IDataBase db, int customerId)
            {
                _lineInfo = new LineInfoModel();
                _db = db;

                try
                {

                    Dictionary<string, object> NameLine = _db.IOExecProc(GetSqlCommand(customerId, String.Format(Querys[0], customerId))).First();

                    List<Dictionary<string, object>> UsersInLine = _db.IOExecProc(GetSqlCommand(customerId, String.Format(Querys[1], NameLine["LineName"].ToString())));
                    int? position = UsersInLine.IndexOf(UsersInLine.Where(dct => Convert.ToInt32(dct["ContactID"]) == Convert.ToInt32(NameLine["ContactID"])).First());

                    List<Dictionary<string, object>> ConTotalOpenDuration = _db.IOExecProc(GetSqlCommand(customerId, String.Format(Querys[2], NameLine["LineName"].ToString())));

                    int MeanResponseTime = 0;
                    foreach (Dictionary<string, object> dct in ConTotalOpenDuration)
                    {
                        MeanResponseTime += Convert.ToInt32(dct["ContactTotalOpenDuration"]);
                    }
                    MeanResponseTime = MeanResponseTime != 0 ? MeanResponseTime / ConTotalOpenDuration.Count() : 0;

                    _lineInfo._name = NameLine["LineName"].ToString() ?? "NULL";
                    _lineInfo._length = UsersInLine.Any() ? UsersInLine.Count() : 0;
                    _lineInfo._position = position ?? 0;
                    _lineInfo._TotalAgents  = Convert.ToInt16(ConfigurationManager.AppSettings["TotalAgents "]);
                    _lineInfo._timeMedioResponse = MeanResponseTime;

                    _lineInfo.CalculateEstimatedWaitTime();

                    return _lineInfo;

                }
                catch (Exception ex)
                {
                    _lineInfo._name = "NULL";
                    _lineInfo._length = 0;
                    _lineInfo._position = 0;
                    _lineInfo._TotalAgents  = 0;
                    _lineInfo._timeMedioResponse = 0;
                    _lineInfo.EstimatedTime = 0;

                    return _lineInfo;
                }



            }

            
            private static OdbcCommand GetSqlCommand(int customerId, string query) {
                OdbcCommand command = new OdbcCommand();

                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = query;

                return command;
            }
            
        }

       
        #endregion

        #region Methods
        public void CalculateEstimatedWaitTime()
        {
            _estimatedWaitTime = (_position * _timeMedioResponse) / _TotalAgents ;
        }
        #endregion
    }
}