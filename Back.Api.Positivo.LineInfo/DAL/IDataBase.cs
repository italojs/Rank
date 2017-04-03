using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace Back.Api.Positivo.LineInfo.DAL
{
    public interface IDataBase
    {
        void IExecNonQueryProc(DbCommand commands);
        List<Dictionary<string, object>> IOExecProc(DbCommand commands);
        List<object> OExecProc(DbCommand commands);
    }
}