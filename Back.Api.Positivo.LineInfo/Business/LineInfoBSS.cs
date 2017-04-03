using Back.Api.Positivo.LineInfo.DAL;
using Back.Api.Positivo.LineInfo.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Back.Api.Positivo.LineInfo.Business
{
    public class LineInfoBSS
    {
        private int _custumerId;
        public LineInfoBSS(int customerId)
        {
            _custumerId = customerId;
        }

        public ILineInfo getLineInfo()
        {

            ILineInfo lineInfo = null;

            try
            {
                IDataBase db = new DatabaseODBC();
                lineInfo = LineInfoModel.Builder.Build(db, _custumerId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return lineInfo;
        }
    }
}