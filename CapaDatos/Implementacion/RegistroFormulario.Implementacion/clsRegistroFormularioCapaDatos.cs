using CapaDatos.Interfaz.RegistroFormulario.Interface;
using CapaDatos.util;
using CapaDTO.ERP;
using CapaDTO.Peticiones;
using CapaDTO.ReportesDTO;
using CapaDTO.Respuestas;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.Implementacion.RegistroFormulario.Implementacion
{
    public class clsRegistroFormularioCapaDatos : IRegistroFormularioCapaDatos
    {


        private cDataBase cDataBase;
        private readonly IConfiguration _configuration;

        public clsRegistroFormularioCapaDatos(IConfiguration configuration)
        {
            _configuration = configuration;
            cDataBase = new cDataBase(_configuration);
        }



        public async Task<FormularioDto> CrearNuenoFormulario(int IdUsuario)
        {
            string strConsulta = string.Empty;

            FormularioDto NuevoFormulario = new FormularioDto();


            DataTable dtInformacion = new DataTable();

            DateTime FechaCreacion = DateTime.Now;

             string fechaFormateada = FechaCreacion.ToString("dd-MM-yyyy HH:mm");

            try
            {
                int scope = 0;
                strConsulta = string.Format("Insert into [dbo].[FormularioClienteProveedores] values ({0},1,'{1}') SELECT SCOPE_IDENTITY()", IdUsuario,fechaFormateada);

                cDataBase.conectar();
                dtInformacion = cDataBase.mtdEjecutarConsultaSQL(strConsulta);
                cDataBase.desconectar();

                if (dtInformacion.Rows.Count > 0)
                {
                    scope = Convert.ToInt32(dtInformacion.Rows[0][0]);

                    NuevoFormulario.Id= scope;
                    NuevoFormulario.IdUsuario = IdUsuario;
                    NuevoFormulario.NombreUsuario = "";
                    NuevoFormulario.IdEstado = 1;
                    NuevoFormulario.Estado = "Creado / Created";
                    NuevoFormulario.FechaFormulario = fechaFormateada;
                }
                else
                {
                    throw new InvalidOperationException("error al crear el Cliente");
                    NuevoFormulario = null;
                }
             


            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

            return NuevoFormulario;
        }


        public async Task<FormularioDto> ReplicaFormulario(int IdFormularioAnterior, int IdUsuario)
        {
            string strConsulta = string.Empty;

            FormularioDto NuevoFormulario = new FormularioDto();


            DataTable dtInformacion = new DataTable();

            DateTime FechaCreacion = DateTime.Now;

            string fechaFormateada = FechaCreacion.ToString("dd-MM-yyyy HH:mm");

            try
            {
                int scope = 0;
                strConsulta = string.Format("Insert into [dbo].[FormularioClienteProveedores] values ({0},1,'{1}') SELECT SCOPE_IDENTITY()", IdUsuario, fechaFormateada);

                cDataBase.conectar();
                dtInformacion = cDataBase.mtdEjecutarConsultaSQL(strConsulta);
                cDataBase.desconectar();

                if (dtInformacion.Rows.Count > 0)
                {
                    scope = Convert.ToInt32(dtInformacion.Rows[0][0]);

                    NuevoFormulario.Id = scope;
                    NuevoFormulario.IdUsuario = IdUsuario;
                    NuevoFormulario.NombreUsuario = "";
                    NuevoFormulario.IdEstado = 1;
                    NuevoFormulario.Estado = "Creado / Created";
                    NuevoFormulario.FechaFormulario = fechaFormateada;

                    bool copoatablas = await CopiaTablas(scope, IdFormularioAnterior);

                }
                else
                {
                    throw new InvalidOperationException("error al crear el Cliente");
                    NuevoFormulario = null;
                }
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

            return NuevoFormulario;
        }

        public async Task<bool> CopiaTablas(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            bool DatosGenerales =await CopiaDatosGenerarles(IdFormularioNuevo, IdFormularioAnterior);

            bool representantes = await CopiaRepresentantesLegales(IdFormularioNuevo, IdFormularioAnterior);



            bool juntadirectiva = await CopiaJuntadirectiva(IdFormularioNuevo, IdFormularioAnterior);


            bool accionistas = await CopiaAccionistas(IdFormularioNuevo, IdFormularioAnterior);



            bool datoscontacto = await CopiaDatosContacto(IdFormularioNuevo, IdFormularioAnterior);
            bool datospago = await CopiaDatosPgo(IdFormularioNuevo, IdFormularioAnterior);
            bool despachomercancia = await CopiaDespachoMercancia(IdFormularioNuevo, IdFormularioAnterior);

            bool cumplimiento = await CopiaCumplimientoNormativo(IdFormularioNuevo, IdFormularioAnterior);


            bool tributaria = await CopiaTibutaria(IdFormularioNuevo, IdFormularioAnterior);

            bool referencias = await CopiaReferenciasComerciales(IdFormularioNuevo, IdFormularioAnterior);



            bool declaraciones = await CopiaDeclaraciones(IdFormularioNuevo, IdFormularioAnterior);

     


            return true;
        }



        public async Task<bool> CopiaDatosGenerarles(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DatosGeneralesDto datosgenerarles = new DatosGeneralesDto();
            datosgenerarles = await ConsultaDatosGenerales(IdFormularioAnterior);

            datosgenerarles.IdFormulario=IdFormularioNuevo;
            datosgenerarles.Id = 0;
            return await GuardarDatosGenerales(datosgenerarles);  

        }


        public async Task<bool> CopiaRepresentantesLegales(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            object representantes = await ConsultaInfoRepresentanteslegales(IdFormularioAnterior);

            if (representantes != null)
            {
                return await GuardaInformacionRepresentantesLegales(IdFormularioNuevo, representantes);
            }

            return true;
        }

        public async Task<bool> CopiaJuntadirectiva(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            object representantes = await ConsultaInfoJuntaDirectiva(IdFormularioAnterior);

            if (representantes != null)
            {
                return await GuardaInformacionJuntaDirectiva(IdFormularioNuevo, representantes);
            }

            return true;
        }

        public async Task<bool> CopiaAccionistas(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            object representantes = await ConsultaInfoAccionistas(IdFormularioAnterior);

            if (representantes != null)
            {
                return await GuardaInformacionAccionistas(IdFormularioNuevo, representantes);
            }

            return true;
        }


        public async Task<bool> CopiaCumplimientoNormativo(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            CumplimientoNormativoDto cumplimiento = new CumplimientoNormativoDto();
            cumplimiento = await ConsultaCumplimientoNormativo(IdFormularioAnterior);   

            if (cumplimiento != null)
            {
                cumplimiento.Id = 0;
                cumplimiento.IdFormulario = IdFormularioNuevo;


                return await GuardaCumplimientoNormativo(cumplimiento);
            }

            return true;
        }


        public async Task<bool> CopiaDatosContacto(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            List<DatosContactoDto> lstContactos = new List<DatosContactoDto>();
            lstContactos = await ListaDatosContacto(IdFormularioAnterior);

            if (lstContactos != null)
            {
                foreach (DatosContactoDto contacto in lstContactos)
                {
                    contacto.Id = 0;
                    contacto.IdFormulario = IdFormularioNuevo;
                }
                return await GuardaInformacionContactos(lstContactos);
            }

            return true;
        }

        public async Task<bool> CopiaDatosPgo(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DatosPagosDto pago = new DatosPagosDto();
            pago = await ConsultaDatosPago(IdFormularioAnterior);

            if (pago != null)
            {
                pago.Id = 0;
                pago.IdFormulario = IdFormularioNuevo;


                return await GuardaDatosPago(pago);
            }

            return true;
        }

        public async Task<bool> CopiaDespachoMercancia(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DespachoMercanciaDto desoachos = new DespachoMercanciaDto();
            desoachos = await ConsulataDespachoMercancia(IdFormularioAnterior);

            if (desoachos != null)
            {
                desoachos.Id = 0;
                desoachos.IdFormulario = IdFormularioNuevo;


                return await GuardaDespachoMercancia(desoachos);
            }

            return true;
        }


        public async Task<bool> CopiaTibutaria(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            InformacionTributariaDTO tributaria = new InformacionTributariaDTO();
            tributaria = await ConsultaInformacionTributaria(IdFormularioAnterior);

            if (tributaria != null)
            {
                tributaria.Id = 0;
                tributaria.IdFormulario = IdFormularioNuevo;


                return await GuardarInformacionTriburaria(tributaria);
            }

            return true;
        }



        public async Task<bool> CopiaReferenciasComerciales(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            List<ReferenciaComercialesBancariasDtol> lstReferencia = new List<ReferenciaComercialesBancariasDtol>();
            lstReferencia = await ListaReferenciasComercialesBan(IdFormularioAnterior);

            if (lstReferencia != null)
            {
                foreach (ReferenciaComercialesBancariasDtol contacto in lstReferencia)
                {
                    contacto.Id = 0;
                    contacto.IdFormulario = IdFormularioNuevo;
                }
                return await GuardaReferenciaComercialBanc(lstReferencia);
            }

            return true;
        }

        public async Task<bool> CopiaDeclaraciones(int IdFormularioNuevo, int IdFormularioAnterior)
        {
            DeclaracionesDto declaraciones = new DeclaracionesDto();
            declaraciones = await ConsultaDeclaraciones(IdFormularioAnterior);

            if (declaraciones != null)
            {
                declaraciones.Id = 0;
                declaraciones.IdFormulario = IdFormularioNuevo;


                return await GuardarDeclaraciones(declaraciones);
            }

            return true;
        }





        public async Task<bool> CambiaEstadoFormulario(int IdFormulario, int IdEstado)
        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[FormularioClienteProveedores] " +
               "SET [IdEstado] = @IdEstado " +
               "WHERE [Id] = @Id";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                   new SqlParameter() { ParameterName = "@IdEstado ", SqlDbType = SqlDbType.Int, Value =  IdEstado },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar");
            }
            return respuesta;

        }


        public async Task<List<FormularioDto>> ListaFormulariosbyUser(int IdUsuario)
        {

            return null;
        
        }

        public async Task<List<FormularioDto>> ListaFormularios()
        {

            List<FormularioDto> ListaFormularios=new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormulario();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm=new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario= Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado= Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado= dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario= dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        public async Task<List<FormularioDto>> ListaFormulariosCompradorVendedor(int IdUsuario)
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioCompradorVendedor(IdUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioCompradorVendedor(int IdUsuario)
        {
            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) WHERE usu.IdCompradorVendedor={0} order by FPC.Id desc", IdUsuario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<List<FormularioDto>> ListaFormulariosUsuarioOEA()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioUsuarioOEA();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioUsuarioOEA()
        {
            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado>2 ORDER BY FPC.Id DESC;");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;

        }

        public async Task<List<FormularioDto>> ListaFormulariosContabilidad()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioContabilidad();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioContabilidad()
        {


            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado in (3,4,5,7,8,9) order by FPC.Id desc");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }


        public async Task<List<FormularioDto>> ListaFormulariosControlInterno()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioControlInterno();

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioControlInterno()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado in (7) order by FPC.Id desc");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }

        public async Task<List<FormularioDto>> ListaFormulariosOficialCumplimiento()
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormularioOficialCumplimiento();
            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado"].ToString();
                    objForm.Oea = dtInformacion.Rows[rows]["ExisteInformacionOEA"].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }

        private DataTable ConsultaListaFormularioOficialCumplimiento()
        {


            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) where FPC.IdEstado in (8) order by FPC.Id desc");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }



        public async Task<List<FormularioDto>> ListaFormulariosbyClienteProveedor(int IdUsuario, string Lang)
        {

            List<FormularioDto> ListaFormularios = new List<FormularioDto>();
            DataTable dtInformacion = ConsultaListaFormulariobyClienteProveedor(IdUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    FormularioDto objForm = new FormularioDto();
                    objForm.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objForm.IdUsuario = Convert.ToInt32(dtInformacion.Rows[rows]["IdUsuario"]);
                    objForm.NombreUsuario = dtInformacion.Rows[rows]["NombreUsuario"].ToString();
                    objForm.IdEstado = Convert.ToInt32(dtInformacion.Rows[rows]["IdEstado"]);
                    objForm.Estado = dtInformacion.Rows[rows]["Estado_"+Lang].ToString();
                    objForm.FechaFormulario = dtInformacion.Rows[rows]["FechaFormulario"].ToString();
                    ListaFormularios.Add(objForm);
                }
            }

            return ListaFormularios;
        }
        private DataTable ConsultaListaFormulariobyClienteProveedor(int IdUsuario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre,' ' , usu.Apellidos) as NombreUsuario, FPC.IdEstado,EF.Nombre_ES as Estado_ES, EF.Nombre_EN as Estado_EN,FPC.FechaFormulario from [dbo].[FormularioClienteProveedores] as FPC inner join [dbo].[tbl_Usuarios] as usu ON (FPC.IdUsuario=usu.Id) inner join [dbo].[EstadoFormulario] as EF ON (EF.Id=FPC.IdEstado) where FPC.IdUsuario={0} order by FPC.Id desc", IdUsuario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }



        public async Task<DatosGeneralesDto> ConsultaDatosGenerales(int IdFormulario)

        {

            string Consulta = string.Empty;
            DatosGeneralesDto objeto = new DatosGeneralesDto();
            Consulta = string.Format("SELECT [Id],[IdFormulario],[Empresa],[TipoSolicitud] ,[ClaseTercero] ,[CategoriaTercero] ,[NombreRazonSocial] ,[TipoIdentificacion] ,[NumeroIdentificacion],[DigitoVerificacion],[Pais] ,[Ciudad] ,[TamanoTercero] ,[RazonSocial],[DireccionPrincipal],[CodigoPostal] ,[CorreoElectronico],[Telefono],[ObligadoFacturarElectronicamente],[CorreoElectronicoFacturaEletronica],[SucursalOtroPais] ,[OtroPais],[JsonPreguntasPep] FROM [dbo].[DatosGenerales] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.FechaDiligenciamiento = "";
                objeto.Empresa = Convert.ToInt32(dtInformacion.Rows[0]["Empresa"]);
                objeto.TipoSolicitud = Convert.ToInt32(dtInformacion.Rows[0]["TipoSolicitud"]);
                objeto.ClaseTercero = Convert.ToInt32(dtInformacion.Rows[0]["ClaseTercero"]);
                objeto.CategoriaTercero = Convert.ToInt32(dtInformacion.Rows[0]["CategoriaTercero"]);
                objeto.NombreRazonSocial = dtInformacion.Rows[0]["NombreRazonSocial"].ToString();
                objeto.TipoIdentificacion = Convert.ToInt32(dtInformacion.Rows[0]["TipoIdentificacion"]);
                objeto.NumeroIdentificacion = dtInformacion.Rows[0]["NumeroIdentificacion"].ToString();
                objeto.DigitoVarificacion = dtInformacion.Rows[0]["DigitoVerificacion"].ToString();

                objeto.Pais = Convert.ToInt32(dtInformacion.Rows[0]["Pais"]);
                objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                objeto.TamanoTercero = Convert.ToInt32(dtInformacion.Rows[0]["TamanoTercero"]);
                objeto.ActividadEconimoca = Convert.ToInt32(dtInformacion.Rows[0]["RazonSocial"]);
                objeto.DireccionPrincipal = dtInformacion.Rows[0]["DireccionPrincipal"].ToString();
                objeto.CodigoPostal = dtInformacion.Rows[0]["CodigoPostal"].ToString();

                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                objeto.ObligadoFE = Convert.ToInt32(dtInformacion.Rows[0]["ObligadoFacturarElectronicamente"]);
                objeto.CorreoElectronicoFE = dtInformacion.Rows[0]["CorreoElectronicoFacturaEletronica"].ToString();
                objeto.TieneSucursalesOtrosPaises = Convert.ToInt32(dtInformacion.Rows[0]["SucursalOtroPais"]);
                objeto.PaisesOtrasSucursales = dtInformacion.Rows[0]["OtroPais"].ToString();
                objeto.PreguntasAdicionales = dtInformacion.Rows[0]["JsonPreguntasPep"];


                return objeto;
            }

            return null;

        }


        private DataTable ConsultaListaFormulario()
        {
            string Consulta = string.Empty;

            Consulta = string.Format("SELECT FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre, ' ', usu.Apellidos) AS NombreUsuario, FPC.IdEstado, EF.Nombre_ES AS Estado, FPC.FechaFormulario, CASE WHEN EXISTS (SELECT 1 FROM [dbo].[InformacionOEA] WHERE [dbo].[InformacionOEA].IdFormulario = FPC.Id) THEN 'Sí' ELSE 'No' END AS ExisteInformacionOEA FROM [dbo].[FormularioClienteProveedores] AS FPC INNER JOIN [dbo].[tbl_Usuarios] AS usu ON (FPC.IdUsuario = usu.Id) INNER JOIN [dbo].[EstadoFormulario] AS EF ON (EF.Id = FPC.IdEstado) ORDER BY FPC.Id DESC;");

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;



        }

        public async Task<bool> GuardarDatosGenerales(DatosGeneralesDto objRegistro)
        {
            bool Respeusta=false;

            if (ExisteDatosGenerales(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id !=0))
            {
                Respeusta= EditaDatosGenerales(objRegistro);

            }
            else
            {
                Respeusta = GuardaDatosGeneralesFist(objRegistro);
            }

        return Respeusta;
        }



        private bool GuardaDatosGeneralesFist(DatosGeneralesDto objRegistro)
        {
            bool respuesta = false;
            try
            {
              

                string query = "insert into [dbo].[DatosGenerales] " +
                          "VALUES (@IdFormulario, @Empresa, @TipoSolicitud, @ClaseTercero, @CategoriaTercero, @NombreRazonSocial, @TipoIdentificacion, @NumeroIdentificacion, @DigitoVerificacion, @Pais, @Ciudad, @TamanoTercero, @RazonSocial, @DireccionPrincipal, @CodigoPostal, @CorreoElectronico, @Telefono, @ObligadoFacturarElectronicamente, @CorreoElectronicoFacturaEletronica, @SucursalOtroPais,@OtroPais,@JsonPreguntasPep)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Empresa ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Empresa },
                    new SqlParameter() { ParameterName = "@TipoSolicitud ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoSolicitud },
                     new SqlParameter() { ParameterName = "@ClaseTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ClaseTercero },
                      new SqlParameter() { ParameterName = "@CategoriaTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.CategoriaTercero },

                       new SqlParameter() { ParameterName = "@NombreRazonSocial ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRazonSocial },
                       new SqlParameter() { ParameterName = "@TipoIdentificacion ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoIdentificacion },
                       new SqlParameter() { ParameterName = "@NumeroIdentificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroIdentificacion },
                       new SqlParameter() { ParameterName = "@DigitoVerificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DigitoVarificacion },
                       new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                       new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                       new SqlParameter() { ParameterName = "@TamanoTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TamanoTercero },
                       new SqlParameter() { ParameterName = "@RazonSocial ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ActividadEconimoca },
                              new SqlParameter() { ParameterName = "@DireccionPrincipal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionPrincipal },
                       new SqlParameter() { ParameterName = "@CodigoPostal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostal },
                       new SqlParameter() { ParameterName = "@CorreoElectronico", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                       new SqlParameter() { ParameterName = "@Telefono", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },

                        new SqlParameter() { ParameterName = "@ObligadoFacturarElectronicamente", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoFE },
                     new SqlParameter() { ParameterName = "@CorreoElectronicoFacturaEletronica", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronicoFE },
                    new SqlParameter() { ParameterName = "@SucursalOtroPais", SqlDbType = SqlDbType.Int, Value =  objRegistro.TieneSucursalesOtrosPaises },
                        new SqlParameter() { ParameterName = "@OtroPais", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.PaisesOtrasSucursales },
                        new SqlParameter() { ParameterName = "@JsonPreguntasPep", SqlDbType = SqlDbType.NVarChar, Value = objRegistro.PreguntasAdicionales.ToString()},
                       


                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                respuesta = false;
                 

            }

            return respuesta;

        }


        private bool EditaDatosGenerales(DatosGeneralesDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DatosGenerales] " +
               "SET [Empresa] = @Empresa, " +
               "[TipoSolicitud] = @TipoSolicitud, " +
               "[ClaseTercero] = @ClaseTercero, " +
               "[CategoriaTercero] = @CategoriaTercero, " +
               "[NombreRazonSocial] = @NombreRazonSocial, " +
               "[TipoIdentificacion] = @TipoIdentificacion, " +
               "[NumeroIdentificacion] = @NumeroIdentificacion, " +
               "[DigitoVerificacion]= @DigitoVerificacion, " +
               "[Pais] = @Pais, " +
               "[Ciudad] = @Ciudad, " +
               "[TamanoTercero] = @TamanoTercero, " +
               "[RazonSocial] = @RazonSocial, " +
               "[DireccionPrincipal] = @DireccionPrincipal, " +
               "[CodigoPostal] = @CodigoPostal, " +
               "[CorreoElectronico] = @CorreoElectronico, " +

               "[Telefono] = @Telefono, " +
               "[ObligadoFacturarElectronicamente] = @ObligadoFacturarElectronicamente, " +
               "[CorreoElectronicoFacturaEletronica] = @CorreoFE, " +
               "[SucursalOtroPais] =@SucursalOtroPais, " +
               "[OtroPais] = @OtroPais, " +
                "[JsonPreguntasPep] = @JsonPreguntasPep " +

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Empresa ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Empresa },
                    new SqlParameter() { ParameterName = "@TipoSolicitud ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoSolicitud },
                     new SqlParameter() { ParameterName = "@ClaseTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ClaseTercero },
                      new SqlParameter() { ParameterName = "@CategoriaTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.CategoriaTercero },

                       new SqlParameter() { ParameterName = "@NombreRazonSocial ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRazonSocial },
                       new SqlParameter() { ParameterName = "@TipoIdentificacion ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoIdentificacion },
                       new SqlParameter() { ParameterName = "@NumeroIdentificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroIdentificacion },
                       new SqlParameter() { ParameterName = "@DigitoVerificacion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DigitoVarificacion },
                       new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                       new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                       new SqlParameter() { ParameterName = "@TamanoTercero ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TamanoTercero },
                       new SqlParameter() { ParameterName = "@RazonSocial ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ActividadEconimoca },
                              new SqlParameter() { ParameterName = "@DireccionPrincipal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionPrincipal },
                       new SqlParameter() { ParameterName = "@CodigoPostal ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostal },
                       new SqlParameter() { ParameterName = "@CorreoElectronico", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                       new SqlParameter() { ParameterName = "@Telefono", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },

                        new SqlParameter() { ParameterName = "@ObligadoFacturarElectronicamente", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoFE },
                         new SqlParameter() { ParameterName = "@CorreoFE", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronicoFE },
                        new SqlParameter() { ParameterName = "@SucursalOtroPais", SqlDbType = SqlDbType.Int, Value =  objRegistro.TieneSucursalesOtrosPaises },
                        new SqlParameter() { ParameterName = "@OtroPais", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.PaisesOtrasSucursales },
                         new SqlParameter() { ParameterName = "@JsonPreguntasPep", SqlDbType = SqlDbType.NVarChar, Value = objRegistro.PreguntasAdicionales.ToString()},


                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        private bool ExisteDatosGenerales(int Id,int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DatosGenerales] where Id={0} and IdFormulario={1}",Id,IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else {
                return false;
                     }
         }


        public async Task<bool> GuardaInformacionContactos(List<DatosContactoDto> objRegistro)
        {
            bool resultado = false;

            try
            {
 

                foreach (DatosContactoDto contacto in objRegistro)
                {
                    if (existecontacto(contacto) || contacto.Id != 0)
                    {
                        EditaContacto(contacto);
                    }
                    else {
                        GuardaContacto(contacto);
                    }
                }
                resultado=true;
            }
            catch (Exception ex)
            {
                resultado=false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return resultado;
        }


        private bool existecontacto(DatosContactoDto objRegistro)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DatosContacto] where Id={0} and IdFormulario={1}", objRegistro.Id, objRegistro.IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GuardaContacto(DatosContactoDto objRegistro)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DatosContacto] where IdFormulario=@IdFormulario";


                string query = "insert into [dbo].[DatosContacto] " +
                          "VALUES (@IdFormulario, @NombreContacto, @Cargo, @Area, @Telefono, @CorreoElectronico)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreContacto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreContacto },
                    new SqlParameter() { ParameterName = "@Cargo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CargoContacto },
                    new SqlParameter() { ParameterName = "@Area ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AreaContacto },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TelefonoContacto },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }


        private void EliminarDatosContacto(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DatosContacto] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                   
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }
        private void EditaContacto(DatosContactoDto objRegistro)
        {
            string strConsulta;
            try
            {
                strConsulta = "UPDATE [dbo].[DatosContacto] " +
               "SET [NombreContacto] = @NombreContacto, " +
               "[Cargo] = @Cargo, " +
               "[Area] = @Area, " +
               "[Telefono] = @Telefono, " +
               "[CorreoElectronico] = @CorreoElectronico " +

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreContacto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreContacto },
                    new SqlParameter() { ParameterName = "@Cargo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CargoContacto },
                    new SqlParameter() { ParameterName = "@Area ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AreaContacto },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TelefonoContacto },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
              
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
        }


        public async Task<List<DatosContactoDto>> ListaDatosContacto(int IdFormulario)
        {

            string Consulta = string.Empty;
            List<DatosContactoDto> listobjeto = new List<DatosContactoDto>();
            Consulta = string.Format("SELECT [Id],[IdFormulario],[NombreContacto],[Cargo],[Area],[Telefono],[CorreoElectronico] FROM [dbo].[DatosContacto] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    DatosContactoDto objDato = new DatosContactoDto();

                    objDato.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objDato.IdFormulario = IdFormulario;
                    objDato.NombreContacto= dtInformacion.Rows[rows]["NombreContacto"].ToString().Trim();
                    objDato.CargoContacto = dtInformacion.Rows[rows]["Cargo"].ToString().Trim();
                    objDato.AreaContacto = dtInformacion.Rows[rows]["Area"].ToString().Trim();
                    objDato.TelefonoContacto = dtInformacion.Rows[rows]["Telefono"].ToString().Trim();
                    objDato.CorreoElectronico = dtInformacion.Rows[rows]["CorreoElectronico"].ToString().Trim();

                    listobjeto.Add(objDato);
                }             


                return listobjeto;
            }

            return null;


        }



        public async Task<bool> GuardaReferenciaComercialBanc(List<ReferenciaComercialesBancariasDtol> objRegistro)
        {
            bool resultado = false;

            try
            {
               

                foreach (ReferenciaComercialesBancariasDtol referencia in objRegistro)
                {

                    if (existeReferencia(referencia) || referencia.Id != 0)
                    {
                        EditaReferenciaComercial(referencia);
                    }
                    else
                    {
                        GuardaReferencia(referencia);
                    }

                }

                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return resultado;
        }

        private void EditaReferenciaComercial(ReferenciaComercialesBancariasDtol objRegistro)
        {
            string strConsulta;
            try
            {
                strConsulta = "UPDATE [dbo].[ReferenciasComercialesBancarias] " +
               "SET [TipoReferencia] = @TipoReferencia, " +
               "[NombreCompleto] = @NombreCompleto, " +
               "[Ciudad] = @Ciudad, " +
               "[Telefono] = @Telefono " +
             

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@TipoReferencia ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoReferencia },
                    new SqlParameter() { ParameterName = "@NombreCompleto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreCompleto },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);

                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Referencia");
            }
        }

        private void GuardaReferencia(ReferenciaComercialesBancariasDtol objRegistro)
        {
            try
            {
                 string query = "insert into [dbo].[ReferenciasComercialesBancarias] " +
                          "VALUES (@IdFormulario, @TipoReferencia, @NombreCompleto, @Ciudad, @Telefono)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@TipoReferencia ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoReferencia },
                    new SqlParameter() { ParameterName = "@NombreCompleto ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreCompleto },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                   
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private void EliminarReferenca(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[ReferenciasComercialesBancarias] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool existeReferencia(ReferenciaComercialesBancariasDtol objRegistro)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[ReferenciasComercialesBancarias] where Id={0} and IdFormulario={1}", objRegistro.Id, objRegistro.IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<List<ReferenciaComercialesBancariasDtol>> ListaReferenciasComercialesBan(int IdFormulario)
        {

            string Consulta = string.Empty;
            List<ReferenciaComercialesBancariasDtol> listobjeto = new List<ReferenciaComercialesBancariasDtol>();
            Consulta = string.Format("SELECT  [Id] ,[IdFormulario]  ,[TipoReferencia],[NombreCompleto] ,[Ciudad] ,[Telefono]  FROM [dbo].[ReferenciasComercialesBancarias] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    ReferenciaComercialesBancariasDtol objDato = new ReferenciaComercialesBancariasDtol();

                    objDato.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objDato.IdFormulario = IdFormulario;
                    objDato.TipoReferencia = Convert.ToInt32(dtInformacion.Rows[rows]["TipoReferencia"]);
                    objDato.NombreCompleto = dtInformacion.Rows[rows]["NombreCompleto"].ToString().Trim();
                    objDato.Ciudad = dtInformacion.Rows[rows]["Ciudad"].ToString().Trim();
                    objDato.Telefono = dtInformacion.Rows[rows]["Telefono"].ToString().Trim();
                    listobjeto.Add(objDato);
                }


                return listobjeto;
            }

            return null;


        }




        public async Task<bool> GuardaDatosPago(DatosPagosDto objRegistro)
        {
            bool Respuesta = false;

            try
            {

                if (existeDatosPagosDto(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
                {
                    Respuesta = editaDatosPago(objRegistro);
                }
                else
                {
                    Respuesta = GuardaDatosPgo(objRegistro);
                }

               
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;
        }

        private bool existeDatosPagosDto(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DatosDePagos] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private void EliminarDatosdePago(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DatosDePagos] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool GuardaDatosPgo(DatosPagosDto objRegistro)
        {
           
            try
            {
                string query = "insert into [dbo].[DatosDePagos] " +
                         "VALUES (@IdFormulario, @NombreBanco, @NumeroCuenta, @TipoCuenta, @CodigoSwift , @Ciudad , @Pais, @CorreoElectronico)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreBanco ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreBanco },
                    new SqlParameter() { ParameterName = "@NumeroCuenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroCuenta },
                    new SqlParameter() { ParameterName = "@TipoCuenta ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoCuenta },
                    new SqlParameter() { ParameterName = "@CodigoSwift ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoSwift },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },

                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                return false;
            }

        }


        private bool editaDatosPago(DatosPagosDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DatosDePagos] " +
               "SET [NombreBanco] = @NombreBanco, " +
               "[NumeroCuenta] = @NumeroCuenta, " +
               "[TipoCuenta] = @TipoCuenta, " +
               "[CodigoSwift] = @CodigoSwift, " +
               "[Ciudad] = @Ciudad, " +
               "[Pais] = @Pais, " +
               "[CorreoElectronico] = @CorreoElectronico " +
               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                     new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreBanco ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreBanco },
                    new SqlParameter() { ParameterName = "@NumeroCuenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroCuenta },
                    new SqlParameter() { ParameterName = "@TipoCuenta ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TipoCuenta },
                    new SqlParameter() { ParameterName = "@CodigoSwift ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoSwift },
                    new SqlParameter() { ParameterName = "@Ciudad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Ciudad },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        public async Task<DatosPagosDto> ConsultaDatosPago(int IdFormulario)
        {

            string Consulta = string.Empty;
            DatosPagosDto objeto = new DatosPagosDto();
            Consulta = string.Format("SELECT [Id],[IdFormulario] ,[NombreBanco]  ,[NumeroCuenta]  ,[TipoCuenta] ,[CodigoSwift]  ,[Ciudad] ,[Pais] ,[CorreoElectronico] FROM [dbo].[DatosDePagos] where IdFormulario={0}", IdFormulario);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.NombreBanco = dtInformacion.Rows[0]["NombreBanco"].ToString();
                objeto.NumeroCuenta = dtInformacion.Rows[0]["NumeroCuenta"].ToString();
                objeto.TipoCuenta = Convert.ToInt32(dtInformacion.Rows[0]["TipoCuenta"]);
                objeto.CodigoSwift = dtInformacion.Rows[0]["CodigoSwift"].ToString();
                objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                objeto.Pais = Convert.ToInt32(dtInformacion.Rows[0]["Pais"]);
                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();  
                return objeto;
            }

            return null;

        }


        public async Task<bool> GuardaCumplimientoNormativo(CumplimientoNormativoDto objRegistro)
        {
            bool Respuesta = false;
            try
            {
                if (existeCumplientoNormativo(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
                {
                    Respuesta = EditaCumpliomientoNormativo(objRegistro);
                }
                else
                {
                    Respuesta = GuardaCumplimientoNor(objRegistro);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;
        }

        public async Task<CumplimientoNormativoDto> ConsultaCumplimientoNormativo(int IdFormulario)

        {

            string Consulta = string.Empty;
            CumplimientoNormativoDto objeto = new CumplimientoNormativoDto();
            Consulta = string.Format("SELECT [Id] ,[IdFormulario],[ObligadoProgramaLA],[ObligadoProgramaEE],[TieneContratosSP],[ActividadesActivosVirtuales],[IntercambiosActivosMoneda],[IntercambioActivosVirtuales],[TransferenciasActivosVirtuales],[CustodiaAdministraAC],[ParticipacionSFAV],[ServiciosAV],[IngresosYearAnterior] FROM [dbo].[CumplimientoNormativo] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.ObligadoProgramaLA = Convert.ToInt32(dtInformacion.Rows[0]["ObligadoProgramaLA"]);
                objeto.ObligadoProgramaEE = Convert.ToInt32(dtInformacion.Rows[0]["ObligadoProgramaEE"]);
                objeto.TieneContratosSP = Convert.ToInt32(dtInformacion.Rows[0]["TieneContratosSP"]);
                objeto.ActividadesActivosVirtuales = Convert.ToInt32(dtInformacion.Rows[0]["ActividadesActivosVirtuales"]);

                objeto.IntercambiosActivosMoneda = Convert.ToInt32(dtInformacion.Rows[0]["IntercambiosActivosMoneda"]);
                objeto.IntercambioActivosVirtuales = Convert.ToInt32(dtInformacion.Rows[0]["IntercambioActivosVirtuales"]);
                objeto.TransferenciasActivosVirtuales = Convert.ToInt32(dtInformacion.Rows[0]["TransferenciasActivosVirtuales"]);
                objeto.CustodiaAdministraAC = Convert.ToInt32(dtInformacion.Rows[0]["CustodiaAdministraAC"]);
                objeto.ParticipacionSFAV = Convert.ToInt32(dtInformacion.Rows[0]["ParticipacionSFAV"]);
                objeto.ServiciosAV = Convert.ToInt32(dtInformacion.Rows[0]["ServiciosAV"]);
                objeto.IngresosYearAnterior = Convert.ToInt32(dtInformacion.Rows[0]["IngresosYearAnterior"]);

                return objeto;
            }

            return null;

        }

        private bool existeCumplientoNormativo(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[CumplimientoNormativo] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private void EliminarCumplimientoNormativo(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[CumplimientoNormativo] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool GuardaCumplimientoNor(CumplimientoNormativoDto objRegistro)
        {
            try
            {
                string query = "insert into [dbo].[CumplimientoNormativo] " +
                         "VALUES (@IdFormulario, @ObligadoProgramaLA, @ObligadoProgramaEE, @TieneContratosSP, @ActividadesActivosVirtuales,@IntercambiosActivosMoneda,@IntercambioActivosVirtuales,@TransferenciasActivosVirtuales,@CustodiaAdministraAC,@ParticipacionSFAV,@ServiciosAV,@IngresosYearAnterior)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@ObligadoProgramaLA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoProgramaLA },
                    new SqlParameter() { ParameterName = "@ObligadoProgramaEE ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoProgramaEE },
                    new SqlParameter() { ParameterName = "@TieneContratosSP ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TieneContratosSP },
                    new SqlParameter() { ParameterName = "@ActividadesActivosVirtuales ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ActividadesActivosVirtuales },

                    new SqlParameter() { ParameterName = "@IntercambiosActivosMoneda ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IntercambiosActivosMoneda },
                    new SqlParameter() { ParameterName = "@IntercambioActivosVirtuales ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IntercambioActivosVirtuales },
                    new SqlParameter() { ParameterName = "@TransferenciasActivosVirtuales ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TransferenciasActivosVirtuales },
                    new SqlParameter() { ParameterName = "@CustodiaAdministraAC ", SqlDbType = SqlDbType.Int, Value =  objRegistro.CustodiaAdministraAC },
                    new SqlParameter() { ParameterName = "@ParticipacionSFAV ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ParticipacionSFAV },
                    new SqlParameter() { ParameterName = "@ServiciosAV ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ServiciosAV },
                    new SqlParameter() { ParameterName = "@IngresosYearAnterior ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IngresosYearAnterior },
                  
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();

                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                return false;

            }

        }

        private bool EditaCumpliomientoNormativo(CumplimientoNormativoDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[CumplimientoNormativo] " +
               "SET [ObligadoProgramaLA] = @ObligadoProgramaLA, " +
               "[ObligadoProgramaEE] = @ObligadoProgramaEE, " +
               "[TieneContratosSP] = @TieneContratosSP, " +
               "[ActividadesActivosVirtuales] = @ActividadesActivosVirtuales, " +

               "[IntercambiosActivosMoneda] = @IntercambiosActivosMoneda, " +
               "[IntercambioActivosVirtuales] = @IntercambioActivosVirtuales, " +
               "[TransferenciasActivosVirtuales] = @TransferenciasActivosVirtuales, " +
               "[CustodiaAdministraAC] = @CustodiaAdministraAC, " +
               "[ParticipacionSFAV] = @ParticipacionSFAV, " +
               "[ServiciosAV] = @ServiciosAV, " +
               "[IngresosYearAnterior] = @IngresosYearAnterior " +

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                     new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@ObligadoProgramaLA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoProgramaLA },
                    new SqlParameter() { ParameterName = "@ObligadoProgramaEE ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ObligadoProgramaEE },
                    new SqlParameter() { ParameterName = "@TieneContratosSP ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TieneContratosSP },
                    new SqlParameter() { ParameterName = "@ActividadesActivosVirtuales ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ActividadesActivosVirtuales },

                    new SqlParameter() { ParameterName = "@IntercambiosActivosMoneda ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IntercambiosActivosMoneda },
                    new SqlParameter() { ParameterName = "@IntercambioActivosVirtuales ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IntercambioActivosVirtuales },
                    new SqlParameter() { ParameterName = "@TransferenciasActivosVirtuales ", SqlDbType = SqlDbType.Int, Value =  objRegistro.TransferenciasActivosVirtuales },
                    new SqlParameter() { ParameterName = "@CustodiaAdministraAC ", SqlDbType = SqlDbType.Int, Value =  objRegistro.CustodiaAdministraAC },
                    new SqlParameter() { ParameterName = "@ParticipacionSFAV ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ParticipacionSFAV },
                    new SqlParameter() { ParameterName = "@ServiciosAV ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ServiciosAV },
                    new SqlParameter() { ParameterName = "@IngresosYearAnterior ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IngresosYearAnterior },


                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Cumplimiento");
            }
            return respuesta;

        }



        public async Task<bool> GuardaDespachoMercancia(DespachoMercanciaDto objRegistro)
        {
            bool Respuesta = false;
            try
            {
                if (existeDespachoMercancia(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
                {
                    Respuesta = EditaDespachoMercancia(objRegistro);
                }
                else
                {
                    Respuesta = GuardaDespachoMerc(objRegistro);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;

        }
        private bool existeDespachoMercancia(int Id, int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DespachoMercancia] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private void EliminarDespachoMercancia(int IdFormulario)
        {
            try
            {
                string querydelete = "Delete from [dbo].[DespachoMercancia] where IdFormulario=@IdFormulario";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(querydelete, parametros);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
            }

        }

        private bool GuardaDespachoMerc(DespachoMercanciaDto objRegistro)
        {
            try
            {
                string query = "insert into [dbo].[DespachoMercancia] " +
                         "VALUES (@IdFormulario, @DireccionDespacho, @Pais, @Cuidad, @CodigoPostalEnvio, @Telefono)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@DireccionDespacho ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionDespacho },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@Cuidad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Cuidad },
                    new SqlParameter() { ParameterName = "@CodigoPostalEnvio ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostalEnvio },
                     new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                
            }

        }

        private bool EditaDespachoMercancia(DespachoMercanciaDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DespachoMercancia] " +
               "SET [DireccionDespacho] = @DireccionDespacho, " +
               "[Pais] = @Pais, " +
               "[Cuidad] = @Cuidad, " +
               "[CodigoPostalEnvio] = @CodigoPostalEnvio, " +
               "[Telefono] = @Telefono " +
               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@DireccionDespacho ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.DireccionDespacho },
                    new SqlParameter() { ParameterName = "@Pais ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Pais },
                    new SqlParameter() { ParameterName = "@Cuidad ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Cuidad },
                    new SqlParameter() { ParameterName = "@CodigoPostalEnvio ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CodigoPostalEnvio },
                     new SqlParameter() { ParameterName = "@Telefono ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Telefono },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Cumplimiento");
            }
            return respuesta;


        }

        public async Task<DespachoMercanciaDto> ConsulataDespachoMercancia(int IdFormulario)
        {

            string Consulta = string.Empty;
            DespachoMercanciaDto objeto = new DespachoMercanciaDto();
            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[DireccionDespacho],[Pais],[Cuidad] ,[CodigoPostalEnvio] ,[Telefono] FROM [dbo].[DespachoMercancia] where IdFormulario={0}", IdFormulario);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.DireccionDespacho = dtInformacion.Rows[0]["DireccionDespacho"].ToString();
                objeto.Pais = Convert.ToInt32(dtInformacion.Rows[0]["Pais"]);
                objeto.Cuidad = dtInformacion.Rows[0]["Cuidad"].ToString();
                objeto.CodigoPostalEnvio = dtInformacion.Rows[0]["CodigoPostalEnvio"].ToString();
                objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                return objeto;
            }

            return null;

        }

        public async Task<bool> GuardaInformacionRepresentantesLegales(int IdFormulario, object objrepresetantes)
        {
            bool Respuesta = false;
            try
            {
                if (ExisteInformacionRepresentantes(IdFormulario))
                {
                    Respuesta = EditaRepresentanteLegal(IdFormulario, objrepresetantes);
                }
                else
                {
                    Respuesta = GuardaRepresentantes(IdFormulario, objrepresetantes);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al CreaR DatosContacto");
            }
            return Respuesta;


        }

        public bool GuardaRepresentantes(int IdFormulario, object objrepresetantes)
        {

            try
            {
                string query = "insert into [dbo].[RepresentanteLegal] " +
                         "VALUES (@IdFormulario, @JsonRepresentanteLegal)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonRepresentanteLegal ", SqlDbType = SqlDbType.NVarChar, Value =  objrepresetantes.ToString() },
                  
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");

            }

        }


        private bool EditaRepresentanteLegal(int IdFormulario, object objrepresetantes)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[RepresentanteLegal] " +
               "SET [JsonRepresentanteLegal] = @JsonRepresentanteLegal " +
               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonRepresentanteLegal ", SqlDbType = SqlDbType.NVarChar, Value =  objrepresetantes.ToString() },
                     
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el Cumplimiento");
            }
            return respuesta;


        }

        private bool ExisteInformacionRepresentantes(int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[RepresentanteLegal] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<object> ConsultaInfoRepresentanteslegales(int IdFormulario)

        {
            object objetojson;
            string Consulta = string.Empty;
       
     
            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[JsonRepresentanteLegal] FROM [dbo].[RepresentanteLegal] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objetojson = dtInformacion.Rows[0]["JsonRepresentanteLegal"];
                return objetojson;
            }

            return null;

        }


        public async Task<bool> GuardaInformacionAccionistas(int IdFormulario, object objrepresetantes)
        {
            bool Respuesta = false;
            try
            {
                if (ExisteInformacionAccionistas(IdFormulario))
                {
                    Respuesta = EditaAccionistas(IdFormulario, objrepresetantes);
                }
                else
                {
                    Respuesta = GuardaAccionistas(IdFormulario, objrepresetantes);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al Crea accionista");
            }
            return Respuesta;


        }//GuardaInformacionAccionistas

        private bool ExisteInformacionAccionistas(int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[Accionistas] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EditaAccionistas(int IdFormulario, object objacionista)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[Accionistas] " +
               "SET [JsonAccionistas] = @JsonAccionistas  " +

               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonAccionistas ", SqlDbType = SqlDbType.NVarChar, Value =  objacionista.ToString() },
                   
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el accionista");
            }
            return respuesta;


        }

        public bool GuardaAccionistas(int IdFormulario, object objacionista)
        {

            try
            {
                string query = "insert into [dbo].[Accionistas] " +
                         "VALUES (@IdFormulario, @JsonAccionistas)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonAccionistas ", SqlDbType = SqlDbType.NVarChar, Value =  objacionista.ToString() },
                    
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");

            }

        }

        public async Task<object> ConsultaInfoAccionistas(int IdFormulario)

        {

            string Consulta = string.Empty;
            object objetojson = null;

            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[JsonAccionistas] FROM [dbo].[Accionistas] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objetojson  = dtInformacion.Rows[0]["JsonAccionistas"];

                return objetojson;
            }

            return null;

        }


        public async Task<bool> GuardaInformacionJuntaDirectiva(int IdFormulario, object objjuntadirectiva)
        {
            bool Respuesta = false;
            try
            {
                if (ExisteInformacionJuntaDirectiva(IdFormulario))
                {
                    Respuesta = EditaJuntaDirectiva(IdFormulario, objjuntadirectiva);
                }
                else
                {
                    Respuesta = GuardaJuntaDirectiva(IdFormulario, objjuntadirectiva);
                }
            }
            catch (Exception ex)
            {
                Respuesta = false;
                throw new InvalidOperationException("error al Crea accionista");
            }
            return Respuesta;


        }//GuardaInformacionAccionistas

        private bool ExisteInformacionJuntaDirectiva(int IdFormulario)
        {

            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[JuntaDirectiva] where IdFormulario={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EditaJuntaDirectiva(int IdFormulario, object objJuntaDirectiva)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[JuntaDirectiva] " +
               "SET [JsonJuntaDirectiva] = @JsonJuntaDirectiva " +
               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonJuntaDirectiva ", SqlDbType = SqlDbType.NVarChar, Value =  objJuntaDirectiva.ToString() },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el accionista");
            }
            return respuesta;


        }

        public bool GuardaJuntaDirectiva(int IdFormulario, object objJuntadirectiva)
        {

            try
            {
                string query = "insert into [dbo].[JuntaDirectiva] " +
                         "VALUES (@IdFormulario, @JsonJuntaDirectiva)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  IdFormulario },
                    new SqlParameter() { ParameterName = "@JsonJuntaDirectiva ", SqlDbType = SqlDbType.NVarChar, Value =  objJuntadirectiva.ToString() },
                     
                };
                cDataBase.conectar();


                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");

            }

        }

        public async Task<object> ConsultaInfoJuntaDirectiva(int IdFormulario)

        {

            string Consulta = string.Empty;
            object objetojson;

            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[JsonJuntaDirectiva] FROM [dbo].[JuntaDirectiva] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objetojson = dtInformacion.Rows[0]["JsonJuntaDirectiva"];
                return objetojson;
            }

            return null;

        }

        public async Task<bool> GuardaInfoAdjuntos(ArchivoDto objAdjunto)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;
            try
            {
                strConsulta = string.Format("insert into [dbo].[AdjuntoFormulario] ([NombreArchivo],[Extencion],[Peso],[Ubicacion],[Key],[IdFormulario]) values ('{0}','{1}','{2}','{3}','{4}',{5})", objAdjunto.NombreArchivo, objAdjunto.extencion, objAdjunto.peso, objAdjunto.Ubicacion, objAdjunto.Key, objAdjunto.IdFormulario);


                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            return respuesta;
        }


        public async Task<ArchivoDto> ConsultaInfoArchivo(int IdFormualrio, string Key)
        {

            ArchivoDto archivo = new ArchivoDto();
            DataTable dtInformacion = new DataTable();
            try
            {

                string strConsulta = string.Format("SELECT [Id], [NombreArchivo],[Extencion],[Peso] ,[Ubicacion],[Key],[IdFormulario]  FROM [dbo].[AdjuntoFormulario] where [IdFormulario] = {0} and [Key] ='{1}'", IdFormualrio, Key);

                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            if (dtInformacion.Rows.Count > 0)
            {

                archivo.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                archivo.NombreArchivo = dtInformacion.Rows[0]["NombreArchivo"].ToString();
                archivo.extencion = dtInformacion.Rows[0]["Extencion"].ToString();
                archivo.peso = dtInformacion.Rows[0]["Peso"].ToString();
                archivo.Ubicacion = dtInformacion.Rows[0]["Ubicacion"].ToString();
                archivo.Key = dtInformacion.Rows[0]["Key"].ToString();
                archivo.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                return archivo;
            }

            return null;
        }



        public async Task<List<ArchivoDto>> ConsultaInfoArchivoCargados(int IdFormualrio)
        {

            List<ArchivoDto> lstarchivo = new List<ArchivoDto>();
            DataTable dtInformacion = new DataTable();
            try
            {

                string strConsulta = string.Format("SELECT [Id], [NombreArchivo],[Extencion],[Peso] ,[Ubicacion],[Key],[IdFormulario]  FROM [dbo].[AdjuntoFormulario] where [IdFormulario] = {0}", IdFormualrio);

                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            if (dtInformacion.Rows.Count > 0)
            {

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    ArchivoDto archivo = new ArchivoDto();
                    archivo.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    archivo.NombreArchivo = dtInformacion.Rows[rows]["NombreArchivo"].ToString();
                    archivo.extencion = dtInformacion.Rows[rows]["Extencion"].ToString();
                    archivo.peso = dtInformacion.Rows[rows]["Peso"].ToString();
                    archivo.Ubicacion = dtInformacion.Rows[rows]["Ubicacion"].ToString();
                    archivo.Key = dtInformacion.Rows[rows]["Key"].ToString();
                    archivo.IdFormulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    lstarchivo.Add(archivo);
                }
                return lstarchivo;
               
            }

            return null;
        }


        public async Task<bool> EliminaArchivoBasedatos(int idArchivo)
        {
            string strConsulta = string.Empty;
            bool respuesta = false;
            try
            {
                strConsulta = string.Format("delete from [dbo].[AdjuntoFormulario] where Id={0}",idArchivo);


                cDataBase.conectar();
                cDataBase.ejecutarQuery(strConsulta);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            return respuesta;
        }

        public async Task<bool> GuardaMotivoRechazoFormulario(RechazoFormularioDto objRechazo, int IdEstado, int IdUsuario)
        {


            try
            {
                string query = "insert into [dbo].[RechazoFormulario] " +
                         "VALUES (@IdFormulario,@MotivoRechazo,GETDATE(),@IdUsuarioRechaza)";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRechazo.IdFormulario },
                    new SqlParameter() { ParameterName = "@MotivoRechazo ", SqlDbType = SqlDbType.NVarChar, Value =  objRechazo.MotivoRechazo.ToString() },
                     new SqlParameter() { ParameterName = "@IdUsuarioRechaza ", SqlDbType = SqlDbType.Int, Value =  IdUsuario },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);


                string query2 = "Update [dbo].[FormularioClienteProveedores] " +
                    "Set IdEstado=@IdEstado where Id=@IdFormulario";
                List<SqlParameter> parametros2 = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRechazo.IdFormulario },
                     new SqlParameter() { ParameterName = "@IdEstado ", SqlDbType = SqlDbType.Int, Value =  IdEstado },

                };
                cDataBase.EjecutarSPParametrosSinRetornonuew(query2, parametros2);



                cDataBase.desconectar();
                return true;
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");

            }


        }


        public async Task<RechazoFormularioDto> MuestraMotivoRechazo(int IdFormulario)
        {
            string strConsulta = string.Empty;
            DataTable dtInformacion = new DataTable();
            RechazoFormularioDto objRechazo = new RechazoFormularioDto();
            try
            {
                strConsulta = string.Format("select Id,IdFormulario,MotivoRechazo,FechaRechazo from [dbo].[RechazoFormulario] where IdFormulario={0} order by Id desc", IdFormulario);
                 cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el ususario");
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objRechazo.IdFormulario = IdFormulario;
                objRechazo.MotivoRechazo= dtInformacion.Rows[0]["MotivoRechazo"].ToString();
                objRechazo.FechaRechazo = dtInformacion.Rows[0]["FechaRechazo"].ToString();

                return objRechazo;
            }
            else {
                return null;
            }
            return null;

        }

        public async Task GuardarConsultaInspektor(InformacionInspektorDto obj)
        {
            try
            {
                string query = "INSERT INTO [dbo].[inspektor_respuesta_ws] ([IdFormulario],[Tipo_Tercero],[Tipo_Identificacion],[Numero_Identificacion],[Nombre],[Numero_Consulta],[Coincidencias],[Fecha_Consulta]) " +
                         "VALUES (@IdFormulario,@Tipo_Tercero,@Tipo_Identificacion,@Numero_Identificacion,@Nombre,@Numero_Consulta,@Coincidencias,GETDATE())";
                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  obj.IdFomulario },
                    new SqlParameter() { ParameterName = "@Tipo_Tercero ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Tipo_Tercero.ToString() },
                    new SqlParameter() { ParameterName = "@Tipo_Identificacion ", SqlDbType = SqlDbType.Int, Value =  obj.Tipo_Identificacion.ToString() },
                    new SqlParameter() { ParameterName = "@Numero_Identificacion ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Numero_Identificacion.ToString() },
                    new SqlParameter() { ParameterName = "@Nombre ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Nombre.ToString() },
                    new SqlParameter() { ParameterName = "@Numero_Consulta ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Numero_Consulta.ToString() },
                    new SqlParameter() { ParameterName = "@Coincidencias ", SqlDbType = SqlDbType.NVarChar, Value =  obj.Coincidencias.ToString() },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                cDataBase.desconectar();
 
            }
            catch (Exception ex)    {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el accionista");
            }

        }

        public async Task<List<InformacionInspektorDto>> ListaResultadosInspektor(int IdFormulario)
        {
            List<InformacionInspektorDto> ListaResultados = new List<InformacionInspektorDto>();
            DataTable dtInformacion = ConsultaListaResultadosFomrualrio(IdFormulario);

            if (dtInformacion.Rows.Count > 0)
            {
                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {

                    InformacionInspektorDto objRespuesta = new InformacionInspektorDto();
                    objRespuesta.Id = Convert.ToInt32(dtInformacion.Rows[rows]["Id"]);
                    objRespuesta.IdFomulario = Convert.ToInt32(dtInformacion.Rows[rows]["IdFormulario"]);
                    objRespuesta.Tipo_Tercero = dtInformacion.Rows[rows]["Tipo_Tercero"].ToString();
                    objRespuesta.Tipo_Identificacion = Convert.ToInt32(dtInformacion.Rows[rows]["Tipo_Identificacion"]);
                    objRespuesta.TipoIdentificacion = dtInformacion.Rows[rows]["TipoDocumento"].ToString();
                    objRespuesta.Numero_Identificacion = dtInformacion.Rows[rows]["Numero_Identificacion"].ToString();
                    objRespuesta.Nombre = dtInformacion.Rows[rows]["Nombre"].ToString();
                    objRespuesta.Numero_Consulta = dtInformacion.Rows[rows]["Numero_Consulta"].ToString();
                    objRespuesta.Coincidencias = dtInformacion.Rows[rows]["Coincidencias"].ToString();
                    objRespuesta.Fecha_Consulta = dtInformacion.Rows[rows]["Fecha_Consulta"].ToString();

                    ListaResultados.Add(objRespuesta);
                }
            }

            return ListaResultados;

        }

        private DataTable ConsultaListaResultadosFomrualrio(int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("SELECT a.[Id],a.[IdFormulario],a.[Tipo_Tercero],a.[Tipo_Identificacion],b.Nombre_es as TipoDocumento ,a.[Numero_Identificacion],a.[Nombre],a.[Numero_Consulta],a.[Coincidencias],a.[Fecha_Consulta]  FROM .[dbo].[inspektor_respuesta_ws] as a inner join [dbo].[TipoDocumentos] as b on a.Tipo_Identificacion=b.Id where a.IdFormulario ={0}", IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<List<string>> CorreosControlInterno()//cambiaraqui
        {
            List<string> ListaCorreos = new List<string>();
            DataTable CorreoControlInterno = await ConsultaCorreosControlInternoAsync();
            if (CorreoControlInterno.Rows.Count > 0)
            {
                for (int rows = 0; rows < CorreoControlInterno.Rows.Count; rows++)
                {
                    ListaCorreos.Add(CorreoControlInterno.Rows[rows]["Email"].ToString());
                }
            }
            return ListaCorreos;
        }

        private async Task<DataTable> ConsultaCorreosControlInternoAsync()
        {
            string consulta = "SELECT Email FROM [dbo].[tbl_Usuarios] WHERE TipoUsuario = 4";
            DataTable dtInformacion = new DataTable();

            string connString = _configuration.GetConnectionString("SQLConnectionString");
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar correos de control interno: " + ex.Message);
            }

            return dtInformacion;
        }


        public async Task<List<string>> CorreosOficialCumplimineto()
        {
            List<string> ListaCorreos = new List<string>();
            DataTable CorreoControlInterno = ConsultaCorreosOficialCumplimiento();
            if (CorreoControlInterno.Rows.Count > 0)
            {
                ListaCorreos.Add(CorreoControlInterno.Rows[0]["Email"].ToString());
            }
            return ListaCorreos;
        }

        private DataTable ConsultaCorreosOficialCumplimiento()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select Email from  [dbo].[tbl_Usuarios] where TipoUsuario=5");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }



        public async Task<List<string>> CorreosEnvioFormualrio(int IdFormulario)
        {
            List<string> ListaCorreos = new List<string>();

            DataTable CorreosContabilidad =  ConsultaCorreosContabilidad();

            if (CorreosContabilidad.Rows.Count > 0)
            {
                foreach (DataRow fila in CorreosContabilidad.Rows)
                {
                    ListaCorreos.Add(fila["Email"].ToString());
                }               

            }

            FormularioDto objform = new FormularioDto();

            objform =await InfoFormulario(IdFormulario);

            int idCompradorVendedor = await DevulveIdCompradorVendedor(objform.IdUsuario);

            ListaCorreos.Add(await DevuelveCorreoCompradorVendedor(idCompradorVendedor));
            return ListaCorreos;

        }

        private async Task<int> DevulveIdCompradorVendedor(int IdUsuarioClienteProveedor)
        {

            int Idusuario = 0;

            string Consulta = string.Empty;
            Consulta = string.Format("select * from [dbo].[tbl_Usuarios] where Id={0}", IdUsuarioClienteProveedor);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                Idusuario = Convert.ToInt32(dtInformacion.Rows[0]["IdCompradorVendedor"]);

            }

            return Idusuario;
        }




        public async Task<List<string>> CorreosCorreccionFormulario(int IdFormulario)
        {
            List<string> ListaCorreos = new List<string>();


            DataTable CorreoProveedorcliente = ConsultaCorreosUsuario(IdFormulario);
            if (CorreoProveedorcliente.Rows.Count > 0)
            {
                ListaCorreos.Add(CorreoProveedorcliente.Rows[0]["Email"].ToString());
            }

            FormularioDto objform = new FormularioDto();

            objform = await InfoFormulario(IdFormulario);

            int idCompradorVendedor = await DevulveIdCompradorVendedor(objform.IdUsuario);

            ListaCorreos.Add(await DevuelveCorreoCompradorVendedor(idCompradorVendedor));
            return ListaCorreos;

        }

        private DataTable ConsultaCorreosContabilidad()
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select * from [dbo].[tbl_Usuarios] where TipoUsuario=3");
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        private DataTable ConsultaCorreosUsuario(int IdFormulario)
        {
            string Consulta = string.Empty;
            Consulta = string.Format("select a.* from [dbo].[tbl_Usuarios] as a inner join [dbo].[FormularioClienteProveedores] as b on (b.IdUsuario=a.Id) where b.Id = {0}",IdFormulario);
            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }


        public async Task<string> DevuelveCorreoCompradorVendedor(int IdUsuario)
        {
            string CorreoCompradorVendedor = string.Empty;

            DataTable dtInformacion = ConsultaCorreoVendedorComprador(IdUsuario);

            if (dtInformacion.Rows.Count > 0)
            {
                CorreoCompradorVendedor = dtInformacion.Rows[0]["Email"].ToString();
            }

            return CorreoCompradorVendedor;

        }


        private DataTable ConsultaCorreoVendedorComprador(int idUsuario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string consonsulta = string.Format("select Email from  [dbo].[tbl_Usuarios] where Id={0}", idUsuario);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consonsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }



        public async Task<FormularioDto> InfoFormulario(int IdFormualrio)
        {

            FormularioDto objformualrio=new FormularioDto();
            DataTable dtInformacion = ConsultaInfoFormualrio(IdFormualrio);


            objformualrio.Id= IdFormualrio; ;
            objformualrio.IdUsuario = Convert.ToInt32(dtInformacion.Rows[0]["IdUsuario"]);
            objformualrio.NombreUsuario = dtInformacion.Rows[0]["NombreUsuario"].ToString();
            objformualrio.IdEstado= Convert.ToInt32(dtInformacion.Rows[0]["IdEstado"]);
            objformualrio.Estado = dtInformacion.Rows[0]["Estado"].ToString();
            objformualrio.FechaFormulario= dtInformacion.Rows[0]["FechaFormulario"].ToString();


            return objformualrio;
                

        }

        private DataTable ConsultaInfoFormualrio(int IdFormulario)
        {
            DataTable dtInformacion = new DataTable();
            try
            {
                string consonsulta = string.Format("select FPC.Id, FPC.IdUsuario, CONCAT(usu.Nombre,' ' , usu.Apellidos) as NombreUsuario, FPC.IdEstado,EF.Nombre_ES as Estado,FPC.FechaFormulario from [dbo].[FormularioClienteProveedores] as FPC inner join [dbo].[tbl_Usuarios] as usu ON (FPC.IdUsuario=usu.Id) inner join [dbo].[EstadoFormulario] as EF ON (EF.Id=FPC.IdEstado) where FPC.Id={0} order by FPC.Id desc", IdFormulario);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(consonsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            return dtInformacion;
        }



        public async Task<bool> GuardarInformacionOEA(FormularioModelDTO objRegistro,int IdUsuario)
        {
            bool Respeusta = false;

            if (ExisteDatosOEA(objRegistro.IdFormulario))
            {
                Respeusta = EditaInformacionOEA(objRegistro);

            }
            else
            {
                Respeusta = GuardaInformacionOEA(objRegistro, IdUsuario);
            }

            return Respeusta;
        }

        private bool ExisteDatosOEA(int IdFormulario)
        {
            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[InformacionOEA] where IdFormulario={0}",  IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EditaInformacionOEA(FormularioModelDTO objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[InformacionOEA] " +
               "SET [Uen] = @Uen, " +
               "[ResponsableVenta] = @ResponsableVenta, " +
               "[CorreoElectronico] = @CorreoElectronico, " +
               "[ResponsableCartera] = @ResponsableCartera, " +
               "[ResponsableTecnico] = @ResponsableTecnico, " +
               "[Moneda] = @Moneda, " +
               "[FormaPago] = @FormaPago, " +
               "[NumeroDias]= @NumeroDias, " +
               "[CadenaLogistica] = @CadenaLogistica, " +
               "[ListasRiesgo] = @ListasRiesgo, " +
               "[SustanciasNarcoticos] = @SustanciasNarcoticos, " +
               "[Certificaciones] = @Certificaciones, " +
               "[ProveedorCadenaLogistica] = @ProveedorCadenaLogistica, " +
               "[RiesgoPais] = @RiesgoPais, " +
               "[AntiguedadEmpresa] = @AntiguedadEmpresa, " +
               "[RiesgoSeguridad] = @RiesgoSeguridad, " +
               "[Valoracion] = @Valoracion, " +
               "[ListasRiesgoCliente] = @ListasRiesgoCliente, " +
               "[TipoNegociacion] = @TipoNegociacion, " +
               "[VistoBuenoAseguradora] = @VistoBuenoAseguradora, " +
               "[RiesgoPaisCliente] = @RiesgoPaisCliente, " +
               "[CertificacionesInstitucionalidad] = @CertificacionesInstitucionalidad, " +
               "[RiesgoSeguridadCliente] = @RiesgoSeguridadCliente, " +
               "[ValoracionCliente] = @ValoracionCliente, " +
               "[SegmentacionRiesgo] = @SegmentacionRiesgo " +
               "WHERE [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {                  
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Uen ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Uen },
                    new SqlParameter() { ParameterName = "@ResponsableVenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableVenta },
                     new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                      new SqlParameter() { ParameterName = "@ResponsableCartera ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableCartera },
                       new SqlParameter() { ParameterName = "@ResponsableTecnico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableTecnico },
                       new SqlParameter() { ParameterName = "@Moneda ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Moneda },
                       new SqlParameter() { ParameterName = "@FormaPago ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FormaPago },
                       new SqlParameter() { ParameterName = "@NumeroDias ", SqlDbType = SqlDbType.Int, Value =  objRegistro.NumeroDias },
                       new SqlParameter() { ParameterName = "@CadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CadenaLogistica },
                       new SqlParameter() { ParameterName = "@ListasRiesgo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgo },
                       new SqlParameter() { ParameterName = "@SustanciasNarcoticos ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SustanciasNarcoticos },
                       new SqlParameter() { ParameterName = "@Certificaciones ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Certificaciones },
                       new SqlParameter() { ParameterName = "@ProveedorCadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ProveedorCadenaLogistica },
                       new SqlParameter() { ParameterName = "@RiesgoPais ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPais },
                       new SqlParameter() { ParameterName = "@AntiguedadEmpresa", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AntiguedadEmpresa },
                       new SqlParameter() { ParameterName = "@RiesgoSeguridad", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridad },
                        new SqlParameter() { ParameterName = "@Valoracion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Valoracion },
                        new SqlParameter() { ParameterName = "@ListasRiesgoCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgoCliente },
                        new SqlParameter() { ParameterName = "@TipoNegociacion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TipoNegociacion },
                        new SqlParameter() { ParameterName = "@VistoBuenoAseguradora", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.VistoBuenoAseguradora },
                        new SqlParameter() { ParameterName = "@RiesgoPaisCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPaisCliente },
                        new SqlParameter() { ParameterName = "@CertificacionesInstitucionalidad", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CertificacionesInstitucionalidad },
                        new SqlParameter() { ParameterName = "@RiesgoSeguridadCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridadCliente },
                        new SqlParameter() { ParameterName = "@ValoracionCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ValoracionCliente },
                        new SqlParameter() { ParameterName = "@SegmentacionRiesgo", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SegmentacionRiesgo },
                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }

        private bool GuardaInformacionOEA(FormularioModelDTO objRegistro, int IdUsuario)
        {
            bool respuesta = false;
            try
            {

                string query = "insert into [dbo].[InformacionOEA] " +
                          "VALUES (@IdFormulario, @Uen, @ResponsableVenta, @CorreoElectronico, @ResponsableCartera, @ResponsableTecnico, @Moneda, @FormaPago, @NumeroDias, @CadenaLogistica, @ListasRiesgo, @SustanciasNarcoticos, @Certificaciones, @ProveedorCadenaLogistica, @RiesgoPais, @AntiguedadEmpresa, @RiesgoSeguridad, @Valoracion, @ListasRiesgoCliente, @TipoNegociacion,@VistoBuenoAseguradora,@RiesgoPaisCliente,@CertificacionesInstitucionalidads,@RiesgoSeguridadCliente,@ValoracionCliente,@IdUsuario,getdate(),@SegmentacionRiesgo)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                     new SqlParameter() { ParameterName = "@IdUsuario ", SqlDbType = SqlDbType.Int, Value =  IdUsuario },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Uen ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Uen },
                    new SqlParameter() { ParameterName = "@ResponsableVenta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableVenta },
                     new SqlParameter() { ParameterName = "@CorreoElectronico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoElectronico },
                      new SqlParameter() { ParameterName = "@ResponsableCartera ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableCartera },
                       new SqlParameter() { ParameterName = "@ResponsableTecnico ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ResponsableTecnico },
                       new SqlParameter() { ParameterName = "@Moneda ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Moneda },
                       new SqlParameter() { ParameterName = "@FormaPago ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FormaPago },
                       new SqlParameter() { ParameterName = "@NumeroDias ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumeroDias },
                       new SqlParameter() { ParameterName = "@CadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CadenaLogistica },
                       new SqlParameter() { ParameterName = "@ListasRiesgo ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgo },
                       new SqlParameter() { ParameterName = "@SustanciasNarcoticos ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SustanciasNarcoticos },
                       new SqlParameter() { ParameterName = "@Certificaciones ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Certificaciones },
                       new SqlParameter() { ParameterName = "@ProveedorCadenaLogistica ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ProveedorCadenaLogistica },
                       new SqlParameter() { ParameterName = "@RiesgoPais ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPais },
                       new SqlParameter() { ParameterName = "@AntiguedadEmpresa", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.AntiguedadEmpresa },
                       new SqlParameter() { ParameterName = "@RiesgoSeguridad", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridad },
                        new SqlParameter() { ParameterName = "@Valoracion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Valoracion },
                        new SqlParameter() { ParameterName = "@ListasRiesgoCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ListasRiesgoCliente },
                        new SqlParameter() { ParameterName = "@TipoNegociacion", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.TipoNegociacion },
                        new SqlParameter() { ParameterName = "@VistoBuenoAseguradora", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.VistoBuenoAseguradora },
                        new SqlParameter() { ParameterName = "@RiesgoPaisCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoPaisCliente },
                        new SqlParameter() { ParameterName = "@CertificacionesInstitucionalidads", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CertificacionesInstitucionalidad },
                        new SqlParameter() { ParameterName = "@RiesgoSeguridadCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.RiesgoSeguridadCliente },
                        new SqlParameter() { ParameterName = "@ValoracionCliente", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.ValoracionCliente },
                        new SqlParameter() { ParameterName = "@SegmentacionRiesgo", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.SegmentacionRiesgo },

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear OEA");
                respuesta = false;


            }

            return respuesta;

        }

        public async Task<FormularioModelDTO> ConsultaDatosInformacionOEA(int IdFormulario)

        {

            string Consulta = string.Empty;
            FormularioModelDTO objeto = new FormularioModelDTO();
            Consulta = string.Format("sELECT * FROM [dbo].[InformacionOEA] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.IdFormulario = IdFormulario;
                objeto.Uen = dtInformacion.Rows[0]["Uen"].ToString();
                objeto.ResponsableVenta = dtInformacion.Rows[0]["ResponsableVenta"].ToString();
                objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                objeto.ResponsableCartera = dtInformacion.Rows[0]["ResponsableCartera"].ToString();
                objeto.ResponsableTecnico = dtInformacion.Rows[0]["ResponsableTecnico"].ToString();
                objeto.Moneda = dtInformacion.Rows[0]["Moneda"].ToString();
                objeto.FormaPago = dtInformacion.Rows[0]["FormaPago"].ToString();
                objeto.NumeroDias = dtInformacion.Rows[0]["NumeroDias"].ToString(); 
                objeto.CadenaLogistica = dtInformacion.Rows[0]["CadenaLogistica"].ToString();
                objeto.ListasRiesgo = dtInformacion.Rows[0]["ListasRiesgo"].ToString();
                objeto.SustanciasNarcoticos = dtInformacion.Rows[0]["SustanciasNarcoticos"].ToString();
                objeto.Certificaciones = dtInformacion.Rows[0]["Certificaciones"].ToString();
                objeto.ProveedorCadenaLogistica = dtInformacion.Rows[0]["ProveedorCadenaLogistica"].ToString();
                objeto.RiesgoPais = dtInformacion.Rows[0]["RiesgoPais"].ToString();
                objeto.AntiguedadEmpresa = dtInformacion.Rows[0]["AntiguedadEmpresa"].ToString();
                objeto.RiesgoSeguridad = dtInformacion.Rows[0]["RiesgoSeguridad"].ToString();
                objeto.Valoracion = dtInformacion.Rows[0]["Valoracion"].ToString();
                objeto.ListasRiesgoCliente = dtInformacion.Rows[0]["ListasRiesgoCliente"].ToString();
                objeto.TipoNegociacion = dtInformacion.Rows[0]["TipoNegociacion"].ToString();
                objeto.VistoBuenoAseguradora = dtInformacion.Rows[0]["VistoBuenoAseguradora"].ToString();
                objeto.RiesgoPaisCliente = dtInformacion.Rows[0]["RiesgoPaisCliente"].ToString();
                objeto.CertificacionesInstitucionalidad = dtInformacion.Rows[0]["CertificacionesInstitucionalidad"].ToString();
                objeto.RiesgoSeguridadCliente = dtInformacion.Rows[0]["RiesgoSeguridadCliente"].ToString();
                objeto.ValoracionCliente = dtInformacion.Rows[0]["ValoracionCliente"].ToString();
                objeto.SegmentacionRiesgo = dtInformacion.Rows[0]["SegmentacionRiesgo"].ToString();
                return objeto;
            }

            return null;

        }


        public async Task<UserFormInformationDTO> Userinfo(int IdFormulario)
        {
            DataTable dtInformacion = new DataTable();
            string strConsulta;
            strConsulta = String.Format("SELECT u.id,u.Nombre,u.Apellidos,u.Email,b.Nombre,u.Identificacion as Identificacion,u.ActualizarPass FROM tbl_Usuarios u  inner join [dbo].[tbl_TipoUsuario] b on (u.TipoUsuario=b.Id) inner join [dbo].[FormularioClienteProveedores] AS FCP ON (FCP.IdUsuario=u.Id) where FCP.Id={0} GROUP BY u.Id, u.Nombre, u.Apellidos,u.Email,b.Nombre,u.IdArea,u.ActualizarPass,u.Identificacion", IdFormulario);

            string connString = _configuration.GetConnectionString("SQLConnectionString");
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(strConsulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                UserFormInformationDTO userinfo = new UserFormInformationDTO();
                userinfo.Nombre = dtInformacion.Rows[0]["Nombre"].ToString().Trim();
                userinfo.Apellido = dtInformacion.Rows[0]["Apellidos"].ToString().Trim();
                userinfo.Usuario = dtInformacion.Rows[0]["Email"].ToString().Trim();
                userinfo.CorreoElectronico = dtInformacion.Rows[0]["Email"].ToString().Trim();
                userinfo.Identificacion = dtInformacion.Rows[0]["Identificacion"].ToString().Trim(); 

                return userinfo;

            }
            else
            {
                return null;
            }

        }


        public async Task<string> DevulveNombrePais(int Pais)
        {
            DataTable dtInformacion = new DataTable();
            string Respuesta = string.Empty;
            string strConsulta;
            try
            {
                strConsulta = String.Format("SELECT [Id],[Nombre_es] ,[Nombre_en] FROM [dbo].[Paises] where Id={0}", Pais);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
            }

            if (dtInformacion.Rows.Count > 0)
            {
               
               Respuesta= dtInformacion.Rows[0]["Nombre_es"].ToString();

            }

            return Respuesta;
        }


        public void GuardaPeticionRespuestaErp(PeticionRespuestaERPDTO objRegistro)
        {
            bool respuesta = false;
            try
            {   string query = "insert into [dbo].[tblConsumoERP] " +
                          "VALUES (@Peticion, @Respuesta, Getdate(), @IdFormulario)";


                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Peticion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Peticion.ToString() },
                    new SqlParameter() { ParameterName = "@Respuesta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Respuesta.ToString() },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();

            }

       
        }

        public void GuardaPeticionRespuestaFirma(PeticionRespuestaFirmaNetco objRegistro)
        {
            bool respuesta = false;
            try
            {
                string query = "insert into [dbo].[tblConsumoFirmaNetco] " +
                          "VALUES (@Peticion, @Respuesta, Getdate(),@UID, @IdFormulario,0)";


                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@Peticion ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Peticion.ToString() },
                    new SqlParameter() { ParameterName = "@Respuesta ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Respuesta.ToString() },
                    new SqlParameter() { ParameterName = "@UID ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.UID.ToString() },
                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();

            }


        }

        public async Task<bool> GuardarDeclaraciones(DeclaracionesDto objRegistro)
        {
            bool Respeusta = false;

            if (ExisteDeclaraciones(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
            {
                Respeusta = EditaDeclaraciones(objRegistro);

            }
            else
            {
                Respeusta = GuardaDeclaracionesFist(objRegistro);
            }

            return Respeusta;
        }



        private bool GuardaDeclaracionesFist(DeclaracionesDto objRegistro)
        {
            bool respuesta = false;
            try
            {

                string query = "insert into [dbo].[DeclaracionFormulario] " +
                          "VALUES (@IdFormulario, @NombreRepresentanteFirma, @CorreoRepresentante)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreRepresentanteFirma ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRepresentanteFirma },
                    new SqlParameter() { ParameterName = "@CorreoRepresentante ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoRepresentante },
               };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                respuesta = false;


            }

            return respuesta;

        }


        private bool EditaDeclaraciones(DeclaracionesDto objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[DeclaracionFormulario] " +
               "SET [NombreRepresentanteFirma] = @NombreRepresentanteFirma, " +
               "[CorreoRepresentante] = @CorreoRepresentante " +               

               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@NombreRepresentanteFirma ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NombreRepresentanteFirma },
                    new SqlParameter() { ParameterName = "@CorreoRepresentante ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.CorreoRepresentante },
               };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        private bool ExisteDeclaraciones(int Id, int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[DeclaracionFormulario] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<DeclaracionesDto> ConsultaDeclaraciones(int IdFormulario)

        {

            string Consulta = string.Empty;
            DeclaracionesDto objeto = new DeclaracionesDto();
            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[NombreRepresentanteFirma] ,[CorreoRepresentante] FROM [dbo].[DeclaracionFormulario] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                objeto.NombreRepresentanteFirma = dtInformacion.Rows[0]["NombreRepresentanteFirma"].ToString();
                objeto.CorreoRepresentante = dtInformacion.Rows[0]["CorreoRepresentante"].ToString();
                return objeto;
            }

            return null;
        }



        public async Task<bool> GuardarInformacionTriburaria(InformacionTributariaDTO objRegistro)
        {
            bool Respeusta = false;

            if (ExisteInformacionTriburaria(objRegistro.Id, objRegistro.IdFormulario) || (objRegistro.Id != 0))
            {
                Respeusta = EditaInformacionTributaria(objRegistro);

            }
            else
            {
                Respeusta = GuardaInformacionTriburaria(objRegistro);
            }

            return Respeusta;
        }



        private bool GuardaInformacionTriburaria(InformacionTributariaDTO objRegistro)
        {
            bool respuesta = false;
            try
            {

                string query = "insert into [dbo].[InformacionTributaria]" +
                          "VALUES (@IdFormulario, @GranContribuyente,@NumResolucionGranContribuyente,@FechaResolucionGranContribuyente,@Autorretenedor,@NumResolucionAutorretenedor,@FechaResolucionAutorretenedor,@ResponsableICA,@MunicipioRetener,@Tarifa,@ResponsableIVA,@AgenteRetenedorIVA)";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                   new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@GranContribuyente ", SqlDbType = SqlDbType.Int, Value =  objRegistro.GranContribuyente },
                    new SqlParameter() { ParameterName = "@NumResolucionGranContribuyente ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumResolucionGranContribuyente },
                     new SqlParameter() { ParameterName = "@FechaResolucionGranContribuyente ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FechaResolucionGranContribuyente },
                      new SqlParameter() { ParameterName = "@Autorretenedor ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Autorretenedor },

                       new SqlParameter() { ParameterName = "@NumResolucionAutorretenedor ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumResolucionAutorretenedor },
                       new SqlParameter() { ParameterName = "@FechaResolucionAutorretenedor ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FechaResolucionAutorretenedor },
                       new SqlParameter() { ParameterName = "@ResponsableICA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ResponsableICA },
                       new SqlParameter() { ParameterName = "@MunicipioRetener ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.MunicipioRetener },
                       new SqlParameter() { ParameterName = "@Tarifa ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Tarifa },
                       new SqlParameter() { ParameterName = "@ResponsableIVA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ResponsableIVA },
                       new SqlParameter() { ParameterName = "@AgenteRetenedorIVA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.AgenteRetenedorIVA },                     

                };
                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();



            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al crear el Cliente");
                respuesta = false;


            }

            return respuesta;

        }


        private bool EditaInformacionTributaria(InformacionTributariaDTO objRegistro)

        {
            bool respuesta = false;
            string strConsulta = string.Empty;
            try
            {
                strConsulta = "UPDATE [dbo].[InformacionTributaria] " +
               "SET [GranContribuyente] = @GranContribuyente, " +
               "[NumResolucionGranContribuyente] = @NumResolucionGranContribuyente, " +
               "[FechaResolucionGranContribuyente] = @FechaResolucionGranContribuyente, " +
               "[Autorretenedor] = @Autorretenedor, " +
               "[NumResolucionAutorretenedor] = @NumResolucionAutorretenedor, " +
               "[FechaResolucionAutorretenedor] = @FechaResolucionAutorretenedor, " +
               "[ResponsableICA] = @ResponsableICA, " +
               "[MunicipioRetener]= @MunicipioRetener, " +
               "[Tarifa] = @Tarifa, " +
               "[ResponsableIVA] = @ResponsableIVA, " +
               "[AgenteRetenedorIVA] = @AgenteRetenedorIVA " +
               "WHERE [Id] = @Id and [IdFormulario]= @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>() {
                    new SqlParameter() { ParameterName = "@Id ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Id },
                         new SqlParameter() { ParameterName = "@IdFormulario ", SqlDbType = SqlDbType.Int, Value =  objRegistro.IdFormulario },
                    new SqlParameter() { ParameterName = "@GranContribuyente ", SqlDbType = SqlDbType.Int, Value =  objRegistro.GranContribuyente },
                    new SqlParameter() { ParameterName = "@NumResolucionGranContribuyente ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumResolucionGranContribuyente },
                     new SqlParameter() { ParameterName = "@FechaResolucionGranContribuyente ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FechaResolucionGranContribuyente },
                      new SqlParameter() { ParameterName = "@Autorretenedor ", SqlDbType = SqlDbType.Int, Value =  objRegistro.Autorretenedor },

                       new SqlParameter() { ParameterName = "@NumResolucionAutorretenedor ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.NumResolucionAutorretenedor },
                       new SqlParameter() { ParameterName = "@FechaResolucionAutorretenedor ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.FechaResolucionAutorretenedor },
                       new SqlParameter() { ParameterName = "@ResponsableICA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ResponsableICA },
                       new SqlParameter() { ParameterName = "@MunicipioRetener ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.MunicipioRetener },
                       new SqlParameter() { ParameterName = "@Tarifa ", SqlDbType = SqlDbType.VarChar, Value =  objRegistro.Tarifa },
                       new SqlParameter() { ParameterName = "@ResponsableIVA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.ResponsableIVA },
                       new SqlParameter() { ParameterName = "@AgenteRetenedorIVA ", SqlDbType = SqlDbType.Int, Value =  objRegistro.AgenteRetenedorIVA },

                };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(strConsulta, parametros);
                respuesta = true;
                cDataBase.desconectar();

            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                throw new InvalidOperationException("error al Editar el DatosGenerales");
            }
            return respuesta;


        }



        private bool ExisteInformacionTriburaria(int Id, int IdFormulario)
        {


            string Consulta = string.Empty;

            Consulta = string.Format("select * from [dbo].[InformacionTributaria] where Id={0} and IdFormulario={1}", Id, IdFormulario);

            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }
            if (dtInformacion.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<InformacionTributariaDTO> ConsultaInformacionTributaria(int IdFormulario)

        {

            string Consulta = string.Empty;
            InformacionTributariaDTO objeto = new InformacionTributariaDTO();
            Consulta = string.Format("SELECT [Id],[IdFormulario] ,[GranContribuyente],[NumResolucionGranContribuyente] ,[FechaResolucionGranContribuyente],[Autorretenedor] ,[NumResolucionAutorretenedor],[FechaResolucionAutorretenedor] ,[ResponsableICA] ,[MunicipioRetener] ,[Tarifa] ,[ResponsableIVA] ,[AgenteRetenedorIVA]  FROM [dbo].[InformacionTributaria] where IdFormulario={0}", IdFormulario);


            DataTable dtInformacion = new DataTable();
            try
            {
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(Consulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception(ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                objeto.IdFormulario = IdFormulario;
                objeto.GranContribuyente = Convert.ToInt32(dtInformacion.Rows[0]["GranContribuyente"]);
                objeto.NumResolucionGranContribuyente = dtInformacion.Rows[0]["NumResolucionGranContribuyente"].ToString();
                objeto.FechaResolucionGranContribuyente = dtInformacion.Rows[0]["FechaResolucionGranContribuyente"].ToString();
                objeto.Autorretenedor = Convert.ToInt32(dtInformacion.Rows[0]["Autorretenedor"]);
                objeto.NumResolucionAutorretenedor = dtInformacion.Rows[0]["NumResolucionAutorretenedor"].ToString();
                objeto.FechaResolucionAutorretenedor = dtInformacion.Rows[0]["FechaResolucionAutorretenedor"].ToString();
                objeto.ResponsableICA = Convert.ToInt32(dtInformacion.Rows[0]["ResponsableICA"]);
                objeto.MunicipioRetener = dtInformacion.Rows[0]["MunicipioRetener"].ToString();
                objeto.Tarifa = dtInformacion.Rows[0]["Tarifa"].ToString();
                objeto.ResponsableIVA = Convert.ToInt32(dtInformacion.Rows[0]["ResponsableIVA"]);
                objeto.AgenteRetenedorIVA = Convert.ToInt32(dtInformacion.Rows[0]["AgenteRetenedorIVA"]);



                return objeto;
            }

            return null;

        }

        public async Task<DatosGeneralesReporteDto> ConsultaDatosGeneralesAlertaPaises(int IdFormulario)
        {

            string Consulta = string.Empty;
            string where = String.Format("Where tblDG.IdFormulario={0}",IdFormulario); 
            Consulta = string.Format("SELECT tblDG.[Id],tblDG.[IdFormulario],tblDG.[Empresa],tblTSolicitud.Nombre_es TipoSolicitud,tblTerc.Nombre_es ClaseTercero ,tblCatTer.Nombre_es CategoriaTercero,tblDG.[NombreRazonSocial], " +
                "tblTipDocon.Nombre_es TipoIdentificacion ,tblDG.[NumeroIdentificacion],tblDG.[DigitoVerificacion],tblPais.Nombre_es Pais ,tblDG.[Ciudad] ,tblTamTerc.Nombre_es TamanoTercero ,tblActEco.Nombre_es RazonSocial,tblDG.[DireccionPrincipal],tblDG.[CodigoPostal] ,tblDG.[CorreoElectronico],tblDG.[Telefono],Ofactu.Nombre_es ObligadoFacturarElectronicamente,tblDG.[CorreoElectronicoFacturaEletronica],sucOPais.Nombre_es SucursalOtroPais  ,tblDG.[OtroPais], tblDG.[JsonPreguntasPep] FROM [dbo].[DatosGenerales] as tblDG " +
                "inner join [dbo].[TipoSolicitud] as tblTSolicitud on (tblDG.TipoSolicitud=tblTSolicitud.Id) inner join [dbo].[ClaseTercero] as tblTerc on (tblDG.ClaseTercero=tblTerc.Id) inner join [dbo].[CategoriaTercero] as tblCatTer on (tblDG.CategoriaTercero=tblCatTer.Id) " +
                "inner join [dbo].[TipoDocumentos] as tblTipDocon on (tblDG.TipoIdentificacion=tblTipDocon.Id) inner join [dbo].[Paises] as tblPais  on (tblDG.Pais=tblPais.Id) left join [dbo].[TamañoTercero] as tblTamTerc  on (tblDG.TamanoTercero=tblTamTerc.Id) left join [dbo].[ActividadEconomicaCiiu] as tblActEco  on (tblDG.RazonSocial=tblActEco.Id) " +
                "inner join [dbo].[SINO] as Ofactu on (tblDG.ObligadoFacturarElectronicamente=Ofactu.Id) inner join [dbo].[SINO] as sucOPais on (tblDG.SucursalOtroPais=sucOPais.Id)" +
                "inner join [dbo].[FormularioClienteProveedores] AS fcp ON (fcp.Id=tblDG.IdFormulario) {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                    DatosGeneralesReporteDto objeto = new DatosGeneralesReporteDto();
                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                    objeto.FechaDiligenciamiento = "";
                    objeto.Empresa = dtInformacion.Rows[0]["Empresa"].ToString();
                    objeto.TipoSolicitud = dtInformacion.Rows[0]["TipoSolicitud"].ToString();
                    objeto.ClaseTercero = dtInformacion.Rows[0]["ClaseTercero"].ToString();
                    objeto.CategoriaTercero = dtInformacion.Rows[0]["CategoriaTercero"].ToString(); objeto.NombreRazonSocial = dtInformacion.Rows[0]["NombreRazonSocial"].ToString();
                    objeto.TipoIdentificacion = dtInformacion.Rows[0]["TipoIdentificacion"].ToString();
                    objeto.NumeroIdentificacion = dtInformacion.Rows[0]["NumeroIdentificacion"].ToString();
                    objeto.DigitoVarificacion = dtInformacion.Rows[0]["DigitoVerificacion"].ToString();
                    objeto.Pais = dtInformacion.Rows[0]["Pais"].ToString();
                    objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                    objeto.TamanoTercero = dtInformacion.Rows[0]["TamanoTercero"].ToString();
                    objeto.ActividadEconimoca = dtInformacion.Rows[0]["RazonSocial"].ToString();
                    objeto.DireccionPrincipal = dtInformacion.Rows[0]["DireccionPrincipal"].ToString();
                    objeto.CodigoPostal = dtInformacion.Rows[0]["CodigoPostal"].ToString();
                    objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                    objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();
                    objeto.ObligadoFE = dtInformacion.Rows[0]["ObligadoFacturarElectronicamente"].ToString();
                    objeto.CorreoElectronicoFE = dtInformacion.Rows[0]["CorreoElectronicoFacturaEletronica"].ToString();
                    objeto.TieneSucursalesOtrosPaises = dtInformacion.Rows[0]["SucursalOtroPais"].ToString();
                    objeto.PaisesOtrasSucursales = dtInformacion.Rows[0]["OtroPais"].ToString();
                    objeto.PreguntasAdicionales = dtInformacion.Rows[0]["JsonPreguntasPep"];
                
                return objeto;
            }

            return null;

        }


        public async Task<RepJunAccDTO> ConsultaInfoRepresentanteslegalesAlertaPaises(int IdFormulario)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);
          

            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonRepresentanteLegal]  FROM [dbo].[RepresentanteLegal] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                    RepJunAccDTO obj = new RepJunAccDTO();
                    obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                    obj.Persona = dtInformacion.Rows[0]["JsonRepresentanteLegal"];

                return obj;
            }

            return null;

        }

        public async Task<RepJunAccDTO> ConsultaInfoJuntaDirectivalegales(int IdFormulario)

        {
         
            string Consulta = string.Empty;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);          

            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonJuntaDirectiva]  FROM [dbo].[JuntaDirectiva] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);

            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                    RepJunAccDTO obj = new RepJunAccDTO();
                    obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                    obj.Persona = dtInformacion.Rows[0]["JsonJuntaDirectiva"];
                
                return obj;
            }

            return null;

        }

        public async Task<RepJunAccDTO> ConsultaInfoAccionistasAlertaPaises(int IdFormulario)

        {
            List<RepJunAccDTO> objetolsit = new List<RepJunAccDTO>();
            string Consulta = string.Empty;

            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);
            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[JsonAccionistas]  FROM [dbo].[Accionistas] as a inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                    RepJunAccDTO obj = new RepJunAccDTO();
                    obj.IdFomrulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                    obj.Persona = dtInformacion.Rows[0]["JsonAccionistas"];
                  
                return obj;
            }

            return null;

        }


        public async Task<DatosPagosReporteDto> ConsultaDatosPagoAlertaPaises(int IdFormulario)
        {

            string Consulta = string.Empty;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);

          
            Consulta = string.Format("SELECT a.[Id], a.[IdFormulario], a.[NombreBanco], a.[NumeroCuenta],b.Nombre_es TipoCuenta, a.[CodigoSwift], a.[Ciudad],c.Nombre_es Pais, a.[CorreoElectronico] FROM [dbo].[DatosDePagos] as a inner join [dbo].[TipoCuentaBanco] as b on (b.Id=a.TipoCuenta) inner join [dbo].[Paises] as c on (a.Pais=c.Id) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);
            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                    DatosPagosReporteDto objeto = new DatosPagosReporteDto();
                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                    objeto.NombreBanco = dtInformacion.Rows[0]["NombreBanco"].ToString();
                    objeto.NumeroCuenta = dtInformacion.Rows[0]["NumeroCuenta"].ToString();
                    objeto.TipoCuenta = dtInformacion.Rows[0]["TipoCuenta"].ToString();
                    objeto.CodigoSwift = dtInformacion.Rows[0]["CodigoSwift"].ToString();
                    objeto.Ciudad = dtInformacion.Rows[0]["Ciudad"].ToString();
                    objeto.Pais = dtInformacion.Rows[0]["Pais"].ToString();
                    objeto.CorreoElectronico = dtInformacion.Rows[0]["CorreoElectronico"].ToString();
                return objeto;
            }

            return null;

        }


        public async Task<DespachoMercanciaReporteDto> ConsulataDespachoMercanciaAlertaPaises(int IdFormulario)
        {
            string Consulta = string.Empty;
            string where = String.Format("Where a.IdFormulario={0}", IdFormulario);


            Consulta = string.Format("SELECT a.[Id] ,a.[IdFormulario] ,a.[DireccionDespacho],b.Nombre_es Pais,a.[Cuidad] ,a.[CodigoPostalEnvio] ,a.[Telefono] FROM [dbo].[DespachoMercancia] as a inner join [dbo].[Paises] as b on(b.Id=a.Pais) inner join [dbo].[FormularioClienteProveedores] fcp on (a.IdFormulario=fcp.Id) {0}", where);
            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {
                    DespachoMercanciaReporteDto objeto = new DespachoMercanciaReporteDto();
                    objeto.Id = Convert.ToInt32(dtInformacion.Rows[0]["Id"]);
                    objeto.IdFormulario = Convert.ToInt32(dtInformacion.Rows[0]["IdFormulario"]);
                    objeto.DireccionDespacho = dtInformacion.Rows[0]["DireccionDespacho"].ToString();
                    objeto.Pais = dtInformacion.Rows[0]["Pais"].ToString();
                    objeto.Cuidad = dtInformacion.Rows[0]["Cuidad"].ToString();
                    objeto.CodigoPostalEnvio = dtInformacion.Rows[0]["CodigoPostalEnvio"].ToString();
                    objeto.Telefono = dtInformacion.Rows[0]["Telefono"].ToString();

                return objeto;
            }
            return null;

        }


        public async Task<bool> ValidaResultadosListas(int IdFormulario)
        {

            bool respuesta = false;
            string Consulta = string.Format("SELECT  [Id],[IdFormulario],[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre] ,[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta] FROM [dbo].[inspektor_respuesta_ws] Where IdFormulario={0}", IdFormulario);
            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al validar consulta en listas: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                int contador = 0;

                for (int rows = 0; rows < dtInformacion.Rows.Count; rows++)
                {
                    contador += Convert.ToInt32(dtInformacion.Rows[rows]["Coincidencias"]);
                }

                if (contador > 0)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            return respuesta;

        }


        public async Task GuardaInfoServiciosAdicionales(int IdFormulario,string NumeroConsulta, object jsonProcuraduria, object jsonRamaJudicial, object ejecucionPenas)
        {
            object jsonProcura = jsonProcuraduria?.ToString() ?? (object)DBNull.Value;
            object jsonRamaJu = jsonRamaJudicial?.ToString() ?? (object)DBNull.Value;
            object jsonEjecuPenas = ejecucionPenas?.ToString() ?? (object)DBNull.Value;

            if (jsonProcura == DBNull.Value && jsonRamaJu == DBNull.Value && jsonEjecuPenas == DBNull.Value)
            {
                return;
            }

            string strConsulta = "UPDATE [dbo].[inspektor_respuesta_ws] " +
                                 "SET [JsonInfoProcuraduria] = @jsonProcura, " +
                                 "[JsonRamaJudicial] = @jsonRamaJu, " +
                                 "[JsonEjecucionPenas] = @jsonEjecuPenas " +
                                 "WHERE [Numero_Consulta] = @NumeroConsulta AND [IdFormulario] = @IdFormulario";

            List<SqlParameter> parametros = new List<SqlParameter>()
    {
        new SqlParameter("@NumeroConsulta", SqlDbType.VarChar) { Value = NumeroConsulta },
        new SqlParameter("@IdFormulario", SqlDbType.Int) { Value = IdFormulario },
        new SqlParameter("@jsonProcura", SqlDbType.NVarChar) { Value = jsonProcura },
        new SqlParameter("@jsonRamaJu", SqlDbType.NVarChar) { Value = jsonRamaJu },
        new SqlParameter("@jsonEjecuPenas", SqlDbType.NVarChar) { Value = jsonEjecuPenas }
    };

            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(strConsulta, connection))
                    {
                        command.Parameters.AddRange(parametros.ToArray());
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al editar los datos generales", ex);
            }
        }

        public async Task<object> ConsultaInfoProcuraduria(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws][Numero_Consulta] {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = dtInformacion.Rows[0]["JsonInfoProcuraduria"];

                return objetojson;
            }

            return objetojson;

        }


        public async Task<object> ConsultaRamaJudicial(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws][Numero_Consulta] {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = dtInformacion.Rows[0]["JsonRamaJudicial"];

                return objetojson;
            }

            return objetojson;

        }


        public async Task<object> ConsultaEjecucionPenas(ListasAdicionalesDto objRegistro)

        {
            string Consulta = string.Empty;
            object objetojson = null;
            string where = String.Format("Where IdFormulario={0} and [Numero_Consulta]='{1}' and Numero_Identificacion='{2}' ", objRegistro.IdFormulario, objRegistro.NumeroConsulta, objRegistro.NumeroIdentificacion);


            Consulta = string.Format("SELECT [Id] ,[IdFormulario] ,[Tipo_Tercero] ,[Tipo_Identificacion] ,[Numero_Identificacion],[Nombre],[Numero_Consulta] ,[Coincidencias] ,[Fecha_Consulta],[JsonInfoProcuraduria] ,[JsonRamaJudicial],[JsonEjecucionPenas] FROM [dbo].[inspektor_respuesta_ws][Numero_Consulta] {0}", where);


            DataTable dtInformacion = new DataTable();
            string connString = _configuration.GetConnectionString("SQLConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(Consulta, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dtInformacion));
                    }
                }
            }
            catch (Exception ex)
            {
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
                throw new Exception("Error al consultar datos generales de alerta de países: " + ex.Message);
            }

            if (dtInformacion.Rows.Count > 0)
            {

                objetojson = dtInformacion.Rows[0]["JsonEjecucionPenas"];

                return objetojson;
            }

            return objetojson;

        }



        public async Task<DataTable> DevuleveUIDOcumento(int IdFormualrio)
        {
            DataTable dtInformacion = new DataTable();
            string Respuesta = string.Empty;
            string strConsulta;
            try
            {
                strConsulta = String.Format("select top(1)Id, UID,Firmado  from tblConsumoFirmaNetco where IdFormulario={0} order by Id desc", IdFormualrio);
                cDataBase.conectar();
                dtInformacion = cDataBase.ejecutarConsulta(strConsulta);
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();
                dtInformacion.Rows.Clear();
                dtInformacion.Columns.Clear();
            }

            if (dtInformacion.Rows.Count > 0)
            {

                return dtInformacion;

            }

            return null;
        }



        public void ActualizaPeticionRespuestaFirma(int Id, int idformulaio)
        {
            bool respuesta = false;
            try
            {
     
                string query = "UPDATE [dbo].[tblConsumoFirmaNetco] " +
                   "SET Firmado = @Firmado " +
                   "WHERE Id = @Id AND IdFormulario = @IdFormulario";

                List<SqlParameter> parametros = new List<SqlParameter>()
    {
        new SqlParameter() { ParameterName = "@IdFormulario", SqlDbType = SqlDbType.Int, Value = idformulaio },
        new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id },
        new SqlParameter() { ParameterName = "@Firmado", SqlDbType = SqlDbType.Bit, Value = 1},
    };

                cDataBase.conectar();
                cDataBase.EjecutarSPParametrosSinRetornonuew(query, parametros);
                respuesta = true;
                cDataBase.desconectar();
            }
            catch (Exception ex)
            {
                cDataBase.desconectar();

            }


        }


    }
}
