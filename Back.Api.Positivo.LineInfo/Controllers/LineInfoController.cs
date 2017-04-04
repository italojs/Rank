using Back.Api.Positivo.LineInfo.Business;
using Back.Api.Positivo.LineInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Back.Api.Positivo.LineInfo.Controllers
{
    
    public class LineInfoController : ApiController
    {

        [HttpGet]
        [Route("api/LineInfo/{customerId}")]
        public ILineInfo LineRank(int customerId)
        {
            LineInfoBSS lineInfo = new LineInfoBSS(customerId);

            return lineInfo.getLineInfo();
        }

    }
}
