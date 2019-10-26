using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Vsts.Dtos
{
    internal sealed class WiqlQueryResultDto
    {
        public IList<WiqlWorkItemDto> WorkItems { get; set; }
        
        public class WiqlWorkItemDto
        {
            public int Id { get; set; }
        }
    }
}
