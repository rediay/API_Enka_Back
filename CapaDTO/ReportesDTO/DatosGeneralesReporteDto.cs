using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.ReportesDTO
{
    public class DatosGeneralesReporteDto
    {

        public int Id { get; set; }
        public int IdFormulario { get; set; }
        public string FechaDiligenciamiento { get; set; }
        public string Empresa { get; set; }
        public string TipoSolicitud { get; set; }
        public string ClaseTercero { get; set; }
        public string CategoriaTercero { get; set; }
        public string NombreRazonSocial { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string DigitoVarificacion { get; set; }
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public string TamanoTercero { get; set; }
        public string ActividadEconimoca { get; set; }
        public string DireccionPrincipal { get; set; }
        public string CodigoPostal { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public string ObligadoFE { get; set; }
        public string CorreoElectronicoFE { get; set; }
        public string TieneSucursalesOtrosPaises { get; set; }
        public string PaisesOtrasSucursales { get; set; }

        public object PreguntasAdicionales { get; set; }
    }
}
