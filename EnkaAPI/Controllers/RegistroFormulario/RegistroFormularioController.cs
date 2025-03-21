using CapaDTO.Peticiones;
using CapaDTO.Respuestas;
using CapaNegocio.Interfaz.auth.interfaz;
using CapaNegocio.Interfaz.RegistroFormulario.Interface;
using CapaNegocio.Interfaz.RegistroTimpo.Interzas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

namespace EnkaAPI.Controllers.RegistroFormulario
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroFormularioController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private readonly IHttpClientFactory _clientFactory;

        private readonly IConfiguration _configuration;

        protected readonly IloginCapaNegocios _iloginCapaNegocios;
        protected readonly IRegistroFormularioCapaNegocio _RegistoFomulario;


        public RegistroFormularioController(IWebHostEnvironment env, IloginCapaNegocios iloginCapaNegocios, IConfiguration _configuration, IRegistroFormularioCapaNegocio RegistoFomulario, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            this._configuration = _configuration;
            this._iloginCapaNegocios = iloginCapaNegocios;
            this._RegistoFomulario = RegistoFomulario;
            _env = env;
        }


        [HttpGet]
        [Authorize]
        [Route("SolicitudNuevoFormulario")]
        public async Task<IActionResult> SolicitudNuevoFormulario()
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            int idusuario = await _iloginCapaNegocios.iduser(UserName);
            try
            {
                FormularioDto NuevoFormulario=new FormularioDto();
                 NuevoFormulario = await _RegistoFomulario.CrearNuenoFormulario(idusuario);
                return Ok(NuevoFormulario);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize]
        [Route("ReplicaFormulario")]
        public async Task<IActionResult> ReplicaFormulario(int IdFormulario)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            int idusuario = await _iloginCapaNegocios.iduser(UserName);
            try
            {

                FormularioDto NuevoFormulario = new FormularioDto();
                NuevoFormulario = await _RegistoFomulario.ReplicaFormulario(IdFormulario, idusuario);
                return Ok(NuevoFormulario);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }



        [HttpGet]
        [Authorize]
        [Route("CambiaEstadoFormulario")]
        public async Task<IActionResult> CambiaEstadoFormulario(int IdFormululario, int IdEstado)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            try
            {
                bool Resultado;
                Resultado =await _RegistoFomulario.CambiaEstadoFormulario(IdFormululario, IdEstado);
                return Ok(Resultado);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        [HttpGet]
        [Authorize]
        [Route("ApruebaFormulario")]
        public async Task<IActionResult> ApruebaFormulario(int IdFormululario, int IdEstado)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            try
            {
                bool Resultado;
                Resultado = await _RegistoFomulario.ApruebaFormulario(IdFormululario, IdEstado);
                return Ok(Resultado);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        [HttpGet]
        [Authorize]
        [Route("ListaFormularios")]
        public async Task<IActionResult> ListaFormularios(string Lang)
        {
            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            try
            {            
             CurrentUserInfo userinfo = new CurrentUserInfo();
            userinfo = await _iloginCapaNegocios.Isadmin(UserName);

                if (userinfo.Rol == "Proveedor/Cliente")
                {
                    int idUsuario = await _iloginCapaNegocios.iduser(UserName);
                    List<FormularioDto> LstFormualrios = new List<FormularioDto>();
                    LstFormualrios = await _RegistoFomulario.ListaFormulariosbyClienteProveedor(idUsuario, Lang);
                    return Ok(LstFormualrios);
                }
                else if (userinfo.Rol == "Contabilidad")
                {
                    List<FormularioDto> LstFormualrios = new List<FormularioDto>();
                    LstFormualrios = await _RegistoFomulario.ListaFormulariosContabilidad();
                    return Ok(LstFormualrios);
                }
                else if (userinfo.Rol == "Control Interno")
                {
                    List<FormularioDto> LstFormualrios = new List<FormularioDto>();
                    LstFormualrios = await _RegistoFomulario.ListaFormulariosControlInterno();
                    return Ok(LstFormualrios);
                }
                else if (userinfo.Rol == "Oficial de Cumplimiento")
                {
                    List<FormularioDto> LstFormualrios = new List<FormularioDto>();
                    LstFormualrios = await _RegistoFomulario.ListaFormulariosOficialCumplimiento();
                    return Ok(LstFormualrios);
                }
                else if (userinfo.Rol == "Comprador" || userinfo.Rol == "Vendedor")
                {
                    int idUsuario = await _iloginCapaNegocios.iduser(UserName);
                    List<FormularioDto> LstFormualrios = new List<FormularioDto>();
                    LstFormualrios = await _RegistoFomulario.ListaFormulariosCompradorVendedor(idUsuario);
                    return Ok(LstFormualrios);
                } else if (userinfo.Rol == "Usuario OEA")
                {                 
                  List<FormularioDto> LstFormualrios = new List<FormularioDto>();
                  LstFormualrios = await _RegistoFomulario.ListaFormulariosUsuarioOEA();
                  return Ok(LstFormualrios);

                }
                return null;
              
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }//ListaFormulariosbyClienteProveedor

        [HttpPost]
        [Authorize]
        [Route("GuardaDatosGenerales")]
        public async Task<IActionResult> GuardaDatosGenerales(DatosGeneralesDto objRegistro)
        {
            try {

                bool respuesya = await _RegistoFomulario.GuardarDatosGenerales(objRegistro);
                return Ok(respuesya);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("ConsultaDatosGenerales")]
        public async Task<IActionResult> ConsultaDatosGenerales(int IdFormulario)
        {

            DatosGeneralesDto objRegistro = new DatosGeneralesDto();
            try
            {
                objRegistro = await _RegistoFomulario.ConsultaDatosGenerales(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }



        [HttpPost]
        [Authorize]
        [Route("GuardaDatosContacto")]
        public async Task<IActionResult> GuardaDatosContacto(List<DatosContactoDto> objRegistro)
        {
            try
            {
                bool respuesya = await _RegistoFomulario.GuardaInformacionContactos(objRegistro);
                return Ok(respuesya);
            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("ConsultaDatosContactos")]
        public async Task<IActionResult> ConsultaDatosContactos(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {
                List < DatosContactoDto> lsitaContactos = new List<DatosContactoDto>(); 
                lsitaContactos = await _RegistoFomulario.ListaDatosContacto(IdFormulario);
                return Ok(lsitaContactos);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }


        [HttpPost]
        [Authorize]
        [Route("GuardaReferenciasComerciales")]
        public async Task<IActionResult> GuardaReferenciasComerciales(List<ReferenciaComercialesBancariasDtol> objRegistro)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {

                bool respuesya = await _RegistoFomulario.GuardaReferenciaComercialBanc(objRegistro);
                return Ok(respuesya);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }



        [HttpGet]
        [Authorize]
        [Route("ConsultaReferenciasFinancieras")]
        public async Task<IActionResult> ConsultaReferenciasFinancieras(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {
                List<ReferenciaComercialesBancariasDtol> objRegistro = new List<ReferenciaComercialesBancariasDtol>();
                objRegistro = await _RegistoFomulario.ListaReferenciasComercialesBan(IdFormulario);
                return Ok(objRegistro);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }



        [HttpPost]
        [Authorize]
        [Route("GuardaDatosPago")]
        public async Task<IActionResult> GuardaDatosPago(DatosPagosDto objRegistro)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {

                bool respuesya = await _RegistoFomulario.GuardaDatosPago(objRegistro);
                return Ok(respuesya);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Authorize]
        [Route("ConsultaDatosPago")]
        public async Task<IActionResult> ConsultaDatosPago(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}
            DatosPagosDto objRegistro = new DatosPagosDto();
            try
            {
                objRegistro = await _RegistoFomulario.ConsultaDatosPago(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Authorize]
        [Route("GuardaCumplimientoNormativo")]
        public async Task<IActionResult> GuardaCumplimientoNormativo(CumplimientoNormativoDto objRegistro)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {

                bool respuesya = await _RegistoFomulario.GuardaCumplimientoNormativo(objRegistro);
                return Ok(respuesya);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }



        [HttpGet]
        [Authorize]
        [Route("ConsultaCumplimientoNormativo")]
        public async Task<IActionResult> ConsultaCumplimientoNormativo(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}
            CumplimientoNormativoDto objRegistro = new CumplimientoNormativoDto();
            try
            {
                objRegistro = await _RegistoFomulario.ConsultaCumplimientoNormativo(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("GuardaDespachoMercancia")]
        public async Task<IActionResult> GuardaDespachoMercancia(DespachoMercanciaDto objRegistro)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {

                bool respuesya = await _RegistoFomulario.GuardaDespachoMercancia(objRegistro);
                return Ok(respuesya);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Authorize]
        [Route("ConsultaDespachoMercancia")]
        public async Task<IActionResult> ConsultaDespachoMercancia(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}
            DespachoMercanciaDto objRegistro = new DespachoMercanciaDto();
            try
            {
                objRegistro = await _RegistoFomulario.ConsulataDespachoMercancia(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //DespachoMercanciaDto
        [Authorize]
        [HttpPost("uploadfiles")]        
        public async Task<IActionResult> UploadFiles([FromForm] IFormCollection form)
        {
            var idFormulario = form["IdFormulario"].ToString();
            var uploadRoot = Path.Combine("ArchivosAdjuntos", idFormulario); // Carpeta base de almacenamiento

            // Verifica si la carpeta de destino existe, si no, la crea
            if (!Directory.Exists(uploadRoot))
            {
                Directory.CreateDirectory(uploadRoot);
            }

            //var archivosGuardados = new List<DatosAdjunto>();  // Lista para almacenar información de archivos

            foreach (var file in form.Files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(uploadRoot, file.FileName);

                    // Guarda el archivo físicamente en el sistema de archivos
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Crea una instancia para almacenar los detalles del archivo
                    var archivoInfo = new DatoAduntoDto
                    {
                        Id = 0,
                        IdFormulario = int.Parse(idFormulario),
                        NombreArchivo = file.FileName,
                        RutaArchivo = filePath,
                        TipoArchivo = file.ContentType,
                        FechaSubida = DateTime.Now.ToString()
                      };
                }

                   // archivosGuardados.Add(archivoInfo);  // Añade a la lista para insertar en la BD
              }
            

            // Inserta la información de archivos en la base de datos
           // await GuardarDetallesEnBaseDatos(archivosGuardados);

            return Ok(new { message = "Archivos subidos y detalles guardados exitosamente" });
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("upload2")]
        public async Task<IActionResult> UploadFile2(IFormFile file, [FromForm] string key, [FromForm] int idFormulario)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no encontrado.");

            var uploadRoot = Path.Combine("ArchivosAdjuntos", idFormulario.ToString()); // Carpeta base de almacenamiento

            try
            {
                // Verifica si la carpeta de destino existe, si no, la crea
                if (!Directory.Exists(uploadRoot))
                {
                    Directory.CreateDirectory(uploadRoot);
                }

                // Crear la ruta completa incluyendo el nombre del archivo
                var filePath = Path.Combine(uploadRoot, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }


                double tamanio = file.Length;
                tamanio = tamanio / 100000;
                tamanio = Math.Round(tamanio, 0);
                ArchivoDto archivo = new ArchivoDto();
                archivo.extencion = Path.GetExtension(file.FileName);
                archivo.NombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);
                archivo.peso = tamanio.ToString();
                archivo.Ubicacion = filePath;
                archivo.Key = key;
                archivo.IdFormulario=idFormulario;

                if (!await _RegistoFomulario.GuardaInfoAdjuntos(archivo))
                {
                    return BadRequest("Error al Guardar en base de datos");
                }

                if (key == "PDFEnviado")
                {
                    FormularioDto obj = new FormularioDto();
                    obj= await _RegistoFomulario.InfoFormulario(idFormulario);
                    if (obj.IdEstado == 3)
                    {
                        Task.Run(() => _RegistoFomulario.EnviarNotificacionClienteEnviaForm(idFormulario, filePath));


                        Task.Run(() => _RegistoFomulario.EnviarDocumentoFirmar(obj, filePath, file.FileName));

                    } else if (obj.IdEstado == 4) {
                        Task.Run(() => _RegistoFomulario.NotificacionFormularioCorregidoClienteForm(idFormulario, filePath));
                        Task.Run(() => _RegistoFomulario.EnviarDocumentoFirmar(obj, filePath, file.FileName));
                    }



                }
                
                return Ok(new { fileName = file.FileName, filePath = filePath, idFormulario });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize]
        [HttpGet("obtener-archivos/{idFormulario}")]
        public IActionResult ObtenerArchivos(string idFormulario)
        {
            var directoryPath = Path.Combine("ArchivosAdjuntos", idFormulario);
            if (!Directory.Exists(directoryPath))
            {
                return NotFound(new { message = "No se encontraron archivos." });
            }

            var archivos = Directory.GetFiles(directoryPath)
                .Select(filePath => new
                {
                    NombreArchivo = Path.GetFileName(filePath),
                    RutaArchivo = filePath,
                    // Aquí puedes agregar más propiedades si es necesario
                }).ToList();

            return Ok(archivos);
        }




        [HttpPost]
        [Authorize]
        [Route("GuardaInfoRepresentantesLegales/{IdFormulario}")]
        public async Task<IActionResult> GuardaInfoRepresentantesLegales(int IdFormulario, [FromBody] object objRegistro)
        {
            try
            {                 

                bool resutlado = await _RegistoFomulario.GuardaInformacionRepresentantesLegales(IdFormulario, objRegistro); 
               
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("ConsultaRepresentanteLegal")]
        public async Task<IActionResult> ConsultaRepresentanteLegal(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}
           
            try
            {
               object objRegistro = await _RegistoFomulario.ConsultaInfoRepresentanteslegales(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("GuardaInfoAccionistas/{IdFormulario}")]
        public async Task<IActionResult> GuardaInfoAccionistas(int IdFormulario, [FromBody] object objRegistro)
        {
            try
            {
                // Serializar el objeto JSON a una cadena


                // Guardar el registro en la base de datos
                bool resutlado = await _RegistoFomulario.GuardaInformacionAccionistas(IdFormulario, objRegistro);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("ConsultaAccionistas")]
        public async Task<IActionResult> ConsultaAccionistas(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {
                

             object objRegistro = await _RegistoFomulario.ConsultaInfoAccionistas(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("GuardaInfoJuntaDirectiva/{IdFormulario}")]
        public async Task<IActionResult> GuardaInfoJuntaDirectiva(int IdFormulario, [FromBody] object objRegistro)
        {
            try
            {
                // Serializar el objeto JSON a una cadena
                string jsonData = JsonConvert.SerializeObject(objRegistro);



                // Crear un nuevo registro y guardar el JSON en JsonData
                var registro = new accionistas
                {
                    JsonData = jsonData
                };

                // Guardar el registro en la base de datos
                bool resutlado = await _RegistoFomulario.GuardaInformacionJuntaDirectiva(IdFormulario, objRegistro);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("Consultajuntadirectiva")]
        public async Task<IActionResult> Consultajuntadirectiva(int IdFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}

            try
            {
                object objRegistro = await _RegistoFomulario.ConsultaInfoJuntaDirectiva(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("EliminaArchivoCargado")]
        public async Task<IActionResult> EliminaArchivoCargado(int IdFormulario, string Key)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            try
            {


                ArchivoDto archivo = new ArchivoDto();


                archivo = await _RegistoFomulario.ConsultaInfoArchivo(IdFormulario, Key);



                try
                {
                    // Verificar si el archivo existe
                    if (System.IO.File.Exists(archivo.Ubicacion))
                    {
                        // Eliminar el archivo
                        System.IO.File.Delete(archivo.Ubicacion);

                        bool eliminadt = await _RegistoFomulario.EliminaArchivoBasedatos(archivo.Id);

                        return Ok(eliminadt);
                    }
                    else
                    {
                        return BadRequest(false);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Ocurrió un error al intentar eliminar el archivo: {ex.Message}" });
                }



            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        [HttpGet]
        [Authorize]
        [Route("ListaArchivosCargadosxFormualrio")]
        public async Task<IActionResult> ListaArchivosCargadosxFormualrio(int IdFormulario)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }
            try
            {

                List<ArchivoDto> archivo = new List<ArchivoDto>();

                archivo = await _RegistoFomulario.ConsultaInfoArchivoCargados(IdFormulario);

                return Ok(archivo);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("descargararchivo")]
        public async Task<IActionResult> DescargarArchivo([FromQuery] string key, [FromQuery] int idFormulario)
        {
            // Buscar el archivo en la base de datos
            ArchivoDto archivo = new ArchivoDto();


            archivo = await _RegistoFomulario.ConsultaInfoArchivo(idFormulario, key);

            if (archivo == null)
            {
                return NotFound("Archivo no encontrado.");
            }

            var rutaArchivo = archivo.Ubicacion; // Suponemos que la ubicación completa está en esta propiedad

            if (!System.IO.File.Exists(rutaArchivo))
            {
                return NotFound("El archivo no existe en el servidor.");
            }

            var memoria = new MemoryStream();
            await using (var stream = new FileStream(rutaArchivo, FileMode.Open))
            {
                await stream.CopyToAsync(memoria);
            }

            memoria.Position = 0;
            string nombreArchivo = archivo.NombreArchivo + archivo.extencion;


            return File(memoria, "application/pdf", nombreArchivo);
        }


        [HttpPost]
        [Authorize]
        [Route("GuardaMotivoRechazo")]
        public async Task<IActionResult> GuardaMotivoRechazo(RechazoFormularioDto objRechazo)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }

            CurrentUserInfo userinfo = new CurrentUserInfo();
            userinfo = await _iloginCapaNegocios.Isadmin(UserName);
            int idUsuario = await _iloginCapaNegocios.iduser(UserName);
            int IdEstado = 0;
            if (userinfo.Rol == "Contabilidad")
            {
                IdEstado = 6;
            }
            else if (userinfo.Rol == "Control Interno")
            {
                IdEstado = 8;
            }
            else if (userinfo.Rol == "Oficial de Cumplimiento")
            {
                IdEstado = 9;
            }

            bool Respuesta = false;
            try
            {
                Respuesta = await _RegistoFomulario.GuardaMotivoRechazoFormulario(objRechazo,IdEstado,idUsuario);
                if (IdEstado == 6)
                {
                    Task.Run(() => _RegistoFomulario.EnviarNotificacionPendienteCorrecciones(objRechazo.IdFormulario, objRechazo));
                }
                else if (IdEstado == 8)
                { //EnviarNotificacionRechazadoporControlInterno
                    Task.Run(() => _RegistoFomulario.EnviarNotificacionRechazadoporControlInterno(objRechazo.IdFormulario, objRechazo));
                }
                else if (IdEstado == 9)
                {
                    Task.Run(() => _RegistoFomulario.EnviarNotificacionTerceroRechazadoOficialCumplimineto(objRechazo.IdFormulario, objRechazo));
                }
                return Ok(Respuesta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("ConsultaMotivoRechazo")]
        public async Task<IActionResult> ConsultaMotivoRechazo(int IdFormulario)
        {


            try
            {
                RechazoFormularioDto obj = new RechazoFormularioDto();

                 obj = await _RegistoFomulario.MuestraMotivoRechazo(IdFormulario);
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("ConsultaResultadosListas")]
        public async Task<IActionResult> ConsultaResultadosListas(int IdFormulario)
        {
            try
            {
                List<InformacionInspektorDto> listaResultados = new List<InformacionInspektorDto>();
                listaResultados = await _RegistoFomulario.ListaResultadosInspektor(IdFormulario);
                return Ok(listaResultados);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Authorize]
        [Route("descargaReporteInspektor")]
        public async Task<IActionResult> descargaReporteInspektor(int IdConsulta)
        {
            // Buscar el archivo en la base de datos
            var client = _clientFactory.CreateClient();
            var requestUri = _configuration["Inspektor:UrlReporteInspektor"].ToString();
            string authToken = _configuration["Inspektor:AuthToken"].ToString();

            var requestData = new ConsultaRequestDto {
                IdConsultaEmpresa = IdConsulta.ToString(),
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);


            var response = await client.PostAsJsonAsync(requestUri, requestData);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileNameStar ?? "reporte.pdf";

                return File(content, "application/pdf", fileName);
            }
            else
            {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }

        [HttpPost]//FormularioModelDTO
        [Authorize]
        [Route("GuardaInformacionOEA")]
        public async Task<IActionResult> GuardaInformacionOEA(FormularioModelDTO objRegistro)
        {

            string UserName = "";
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.Identity.Name;
            }
            else
            {
                return BadRequest();
            }

            try
            {
                int idUsuario = await _iloginCapaNegocios.iduser(UserName);
                bool respuesya = await _RegistoFomulario.GuardarInformacionOEA(objRegistro, idUsuario);
                return Ok(respuesya);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }


        [HttpGet]
        [Authorize]
        [Route("ConsultaInformacionOEA")]
        public async Task<IActionResult> ConsultaInformacionOEA(int IdFormulario)
        {

            FormularioModelDTO objRegistro = new FormularioModelDTO();
            try
            {
                objRegistro = await _RegistoFomulario.ConsultaDatosInformacionOEA(IdFormulario);
                return Ok(objRegistro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("GuardaDeclaracionesFormulario")]
        public async Task<IActionResult> GuardaDeclaracionesFormulario(DeclaracionesDto objRegistro)
        {
            try
            {
                bool respuesya = await _RegistoFomulario.GuardarDeclaraciones(objRegistro);
                return Ok(respuesya);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Authorize]
        [Route("ConsultaDeclaraciones")]
        public async Task<IActionResult> ConsultaDeclaraciones(int IdFormulario)
        {
            DeclaracionesDto objRegistro = new DeclaracionesDto();
            try
            {
                objRegistro = await _RegistoFomulario.ConsultaDeclaraciones(IdFormulario);
                return Ok(objRegistro);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }


        [HttpGet]
        [Authorize]
        [Route("ConsultaInformacionTriburaria")]
        public async Task<IActionResult> ConsultaInformacionTriburaria(int IdFormulario)
        {

            InformacionTributariaDTO objRegistro = new InformacionTributariaDTO();

            try
            {
                objRegistro = await _RegistoFomulario.ConsultaInformacionTributaria(IdFormulario);
                return Ok(objRegistro);

            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }


        }


        [HttpPost]
        [Authorize]
        [Route("GuardaInformacionTributaria")]
        public async Task<IActionResult> GuardaInformacionTributaria(InformacionTributariaDTO objRegistro)
        {
            try
            {
                bool respuesya = await _RegistoFomulario.GuardarInformacionTriburaria(objRegistro);
                return Ok(respuesya);
            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        //[Authorize]
        [Route("ConsultaInfoRamaJudicial")]
        public async Task<IActionResult> ConsultaInfoRamaJudicial(ListasAdicionalesDto objRegistro)
        {
            MemoryStream excelStream = await _RegistoFomulario.ConsultaRamaJudicial(objRegistro);

            // Devuelve el archivo Excel al cliente
            return File(excelStream.ToArray(), "application/pdf", "ReporteFormulario.pdf");
        }

        [HttpPost]
        //[Authorize]
        [Route("ConsultaInfoProcuraduria")]
        public async Task<IActionResult> ConsultaInfoProcuraduria(ListasAdicionalesDto objRegistro)
        {
            MemoryStream excelStream = await _RegistoFomulario.ConsultaInfoProcuraduria(objRegistro);
            // Devuelve el archivo Excel al cliente
            return File(excelStream.ToArray(), "application/pdf", "ReporteFormulario.pdf");
        }


        [HttpPost]
        //[Authorize]
        [Route("ConsultaInfoEjecucionPenas")]
        public async Task<IActionResult> ConsultaInfoEjecucionPenas(ListasAdicionalesDto objRegistro)
        {
            MemoryStream excelStream = await _RegistoFomulario.EjecucucionPenas(objRegistro);
            // Devuelve el archivo Excel al cliente
            return File(excelStream.ToArray(), "application/pdf", "ReporteFormulario.pdf");
        }




        /* [HttpPost]
         //[Authorize]
         [Route("ConsultaInfoRamaJudicial")]
         public async Task<IActionResult> ConsultaInfoRamaJudicial(ListasAdicionalesDto objRegistro)
         {
             try
             {
                 bool respuesya = await _RegistoFomulario.GuardarInformacionTriburaria(objRegistro);
                 return Ok(respuesya);
             }
             catch (Exception ex)

             {
                 return BadRequest(ex.Message);
             }
         }

         [HttpPost]
         //[Authorize]
         [Route("ConsultaInfoProcuraduria")]
         public async Task<IActionResult> ConsultaInfoProcuraduria(ListasAdicionalesDto objRegistro)
         {
             try
             {
                 bool respuesya = await _RegistoFomulario.GuardarInformacionTriburaria(objRegistro);
                 return Ok(respuesya);
             }
             catch (Exception ex)

             {
                 return BadRequest(ex.Message);
             }
         }

         */

        [HttpGet]
        [Route("validapaises")]
        public async Task<IActionResult> validapaises(int IdFormulario)
        {
            try
            {
                _RegistoFomulario.ValidaPaisesRiesgo(IdFormulario);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        [HttpGet]
        [Authorize]
        [Route("ValidaResultadosListas")]
        public async Task<IActionResult> ValidaResultadosListas(int IdFormulario)
        {
            try
            {
                bool resultado = await _RegistoFomulario.ValidaResultadosListas(IdFormulario);              

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("autenticacionnetco")]
        public async Task<IActionResult> autenticacionnetco(int idFormulario)
        {
            //string UserName = "";
            //if (User.Identity.IsAuthenticated)
            //{
            //    UserName = User.Identity.Name;
            //}
            //else
            //{
            //    return BadRequest();
            //}
            string cookei;
            try
            {

                FormularioDto obj = new FormularioDto();
                obj = await _RegistoFomulario.InfoFormulario(idFormulario);

                var uploadRoot = Path.Combine("ArchivosAdjuntos", idFormulario.ToString());

                var filePath = Path.Combine(uploadRoot, "PDFEnviado_Formulario_3.pdf");

                bool xxx = await _RegistoFomulario.ValidaSiDocumentoEstaFirmado(idFormulario);

               // cookei = await _RegistoFomulario.EnviarDocumentoFirmar(obj, filePath,"PdfJuan.pdf");
                return Ok(xxx);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Authorize]
        [Route("ValidaDocumentoFirmado")]
        public async Task<IActionResult> ValidaDocumentoFirmado(int idFormulario)
        {

            string cookei;
            try
            {
                bool xxx = await _RegistoFomulario.ValidaSiDocumentoEstaFirmado(idFormulario);
                return Ok(xxx);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
