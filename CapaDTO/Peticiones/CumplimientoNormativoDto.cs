using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDTO.Peticiones
{
    public class CumplimientoNormativoDto
    {
        public int Id { get; set; }

        public int ObligadoProgramaLA { get; set; }

        public int ObligadoProgramaEE { get; set; }
        public int TieneContratosSP { get; set; }
        public int ActividadesActivosVirtuales { get; set; }
        public int IntercambiosActivosMoneda { get; set; }
        public int IntercambioActivosVirtuales { get; set; }
        public int TransferenciasActivosVirtuales { get; set; }

        public int CustodiaAdministraAC { get; set; }
        public int ParticipacionSFAV { get; set; }
        public int ServiciosAV { get; set; }
        public int IngresosYearAnterior { get; set; }
        public int IdFormulario { get; set; }
    }
}
