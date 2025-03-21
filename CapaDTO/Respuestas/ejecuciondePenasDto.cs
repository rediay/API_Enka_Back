using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class EjecucionPenas
    {
        public bool TraeResultados { get; set; }
        public bool Error { get; set; }
        public List<DataDto> Data { get; set; }
    }

    public class DataDto
    {
        public string QueryParam { get; set; }
        public string NameResult { get; set; }
        public string IdentificationNumberResult { get; set; }
        public string CityName { get; set; }
        public string Link { get; set; }
        public string License { get; set; }
        public DateTime QueryDate { get; set; }
        public bool IsSuccess { get; set; }
    }
}
