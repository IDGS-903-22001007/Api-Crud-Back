using prjAPI3.Models.catEmpleados;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration; // Se necesita el namespace para IConfiguration

namespace prjAPI3.Data
{
    // Esta clase maneja toda la interacción con la base de datos para los empleados.
    public class clsCatEmpleadosData
    {
        private readonly string conexion;

        // Constructor que inyecta la configuración para obtener la cadena de conexión
        public clsCatEmpleadosData(IConfiguration configuration)
        {
            // Se asume que "CadenaSQL" está definida en appsettings.json
            conexion = configuration.GetConnectionString("CadenaSQL");
        }

        // 1. OBTENER LISTA DE EMPLEADOS
        // CORRECCIÓN: El tipo de retorno fue cambiado a List<clsCatEmpleados>
        public async Task<List<clsCatEmpleados>> ListaEmpleados()
        {
            List<clsCatEmpleados> lista = new List<clsCatEmpleados>();
            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaEmpleados", con);
                cmd.CommandType = CommandType.StoredProcedure;
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // CORRECCIÓN: Se inicializa correctamente el objeto clsCatEmpleados
                        lista.Add(new clsCatEmpleados()
                        {
                            intIdEmpleado = Convert.ToInt32(reader["IDEMPLEADO"]),
                            strNombreCompleto = reader["NOMBRECOMPLETO"].ToString(),
                            strCorreo = reader["CORREO"].ToString(),
                            dblSueldo = Convert.ToDouble(reader["SUELDO"]),
                            strFechaContratacion = reader["FECHACONTRATO"] == DBNull.Value ? null : Convert.ToDateTime(reader["FECHACONTRATO"]),
                            strEstatus = reader["ESTATUS"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        // 2. OBTENER EMPLEADO INDIVIDUAL
        public async Task<clsCatEmpleados> ObtenerEmpleado(int intIdEmpleado)
        {
            clsCatEmpleados objEmpleado = null; // Inicializar a null si no se encuentra
            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerEmpleado", con);
                // Usar el nombre de parámetro correcto
                cmd.Parameters.AddWithValue("@IdEmpleado", intIdEmpleado); 
                // CORRECCIÓN: Se usa CommandType.StoredProcedure
                cmd.CommandType = CommandType.StoredProcedure; 
                
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync()) // Usar 'if' en lugar de 'while' para un solo registro
                    {
                        objEmpleado = new clsCatEmpleados()
                        {
                            intIdEmpleado = Convert.ToInt32(reader["IDEMPLEADO"]),
                            strNombreCompleto = reader["NOMBRECOMPLETO"].ToString(),
                            strCorreo = reader["CORREO"].ToString(),
                            dblSueldo = Convert.ToDouble(reader["SUELDO"]),
                            strFechaContratacion = reader["FECHACONTRATO"] == DBNull.Value ? null : Convert.ToDateTime(reader["FECHACONTRATO"]),
                            strEstatus = reader["ESTATUS"].ToString()
                        };
                    }
                }
            } // CORRECCIÓN: Se cerró el bloque 'using' correctamente
            return objEmpleado;
        }

        // 3. CREAR NUEVO EMPLEADO
        public async Task<bool> CrearEmpleado(clsCatEmpleados pobjCatEmpleado)
        {
            bool bolRespuesta = false;
            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_crearEmpleado", con);
                cmd.Parameters.AddWithValue("@NombreCompleto", pobjCatEmpleado.strNombreCompleto);
                cmd.Parameters.AddWithValue("@Correo", pobjCatEmpleado.strCorreo);
                cmd.Parameters.AddWithValue("@Sueldo", pobjCatEmpleado.dblSueldo);
                cmd.Parameters.AddWithValue("@FechaContrato", pobjCatEmpleado.strFechaContratacion); 
                cmd.Parameters.AddWithValue("@Estatus", pobjCatEmpleado.strEstatus);

                cmd.CommandType = CommandType.StoredProcedure; 

                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    bolRespuesta = true; // Si no hay excepción, la operación fue exitosa
                }
                catch(Exception ex)
                {
                    // Puedes añadir logging del 'ex' aquí
                    bolRespuesta = false; 
                }
            }
            return bolRespuesta;
        }

        // 4. EDITAR EMPLEADO EXISTENTE
        public async Task<bool> EditarEmpleado(clsCatEmpleados pobjCatEmpleado)
        {
            bool bolRespuesta = false;
            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_editarEmpleado", con);

                cmd.Parameters.AddWithValue("@IdEmpleado", pobjCatEmpleado.intIdEmpleado);
                cmd.Parameters.AddWithValue("@NombreCompleto", pobjCatEmpleado.strNombreCompleto);
                cmd.Parameters.AddWithValue("@Correo", pobjCatEmpleado.strCorreo);
                cmd.Parameters.AddWithValue("@Sueldo", pobjCatEmpleado.dblSueldo);
                cmd.Parameters.AddWithValue("@FechaContrato", pobjCatEmpleado.strFechaContratacion);
                cmd.Parameters.AddWithValue("@Estatus", pobjCatEmpleado.strEstatus);

                // CORRECCIÓN: Se usa CommandType.StoredProcedure
                cmd.CommandType = CommandType.StoredProcedure; 

                try
                {
                    await con.OpenAsync();
                    // CORRECCIÓN: Se añade la ejecución del comando.
                    if (await cmd.ExecuteNonQueryAsync() > 0) 
                    {
                        bolRespuesta = true;
                    }
                }
                catch (Exception ex)
                {
                    // Puedes añadir logging del 'ex' aquí
                    bolRespuesta = false;
                }
            }
            return bolRespuesta;
        }

        // 5. ELIMINAR EMPLEADO
        // CORRECCIÓN: Se recibe solo el IdEmpleado
        public async Task<bool> EliminarEmpleado(int intIdEmpleado)
        {
            bool bolRespuesta = false;
            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();

                // Verifica si el empleado existe
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM catEmpleados WHERE IdEmpleado = @IdEmpleado", con);
                checkCmd.Parameters.AddWithValue("@IdEmpleado", intIdEmpleado);
                int existe = (int)await checkCmd.ExecuteScalarAsync();

                if (existe > 0)
                {
                    SqlCommand cmd = new SqlCommand("sp_eliminarEmpleado", con);
                    cmd.Parameters.AddWithValue("@IdEmpleado", intIdEmpleado);
                    cmd.CommandType = CommandType.StoredProcedure;

                    await cmd.ExecuteNonQueryAsync();
                    bolRespuesta = true; // Considera éxito si el registro existe
                }
                else
                {
                    bolRespuesta = false;
                }
            }
            return bolRespuesta;
        }
    } 
}