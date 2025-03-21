using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Respuestas
{
    public class RootProcuraduria
    {
        public bool TraeResultados { get; set; }
        public bool Error { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string html_response { get; set; }
        public bool not_criminal_records { get; set; }
        public List<Ciudadano> data { get; set; }
    }

    public class Ciudadano
    {
        public string identification { get; set; }
        public string name { get; set; }
        public string num_siri { get; set; }
        public List<Sancion> sanciones { get; set; }
        public List<Delito> delitos { get; set; }
        public List<Instancia> instancias { get; set; }
        public List<Evento> eventos { get; set; }
        public List<Inhabilidad> inhabilidades { get; set; }
    }

    public class Sancion
    {
        public string sancion { get; set; }
        public string termino { get; set; }
        public string clase { get; set; }
        public string suspendida { get; set; }
    }

    public class Delito
    {
        public string descripcion { get; set; }
    }

    public class Instancia
    {
        public string nombre { get; set; }
        public string autoridad { get; set; }
        public string fecha_provincia { get; set; }
        public string fecha_efecto_juridicos { get; set; }
    }

    public class Evento
    {
        public string nombre_causa { get; set; }
        public string entidad { get; set; }
        public string tipo_acto { get; set; }
        public string fecha_acto { get; set; }
    }

    public class Inhabilidad
    {
        public string siri { get; set; }
        public string modulo { get; set; }
        public string inhabilidad_legal { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
    }

}
