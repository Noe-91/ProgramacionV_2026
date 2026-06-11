using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProgramacionV_2026.Models;

namespace ProgramacionV_2026.Controllers
{
    public class ActividadController : Controller
    {
        private readonly IConfiguration _configuration;

        public ActividadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var actividades = new List<ActividadViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query = @"
                    SELECT A.IdActividad, A.Titulo, A.Descripcion, A.Fecha, A.Hora,
                           A.Lugar, A.Responsable, A.Estado, A.IdTipoActividad,
                           T.Nombre AS TipoActividad
                    FROM Actividades A
                    INNER JOIN TipoActividad T ON A.IdTipoActividad = T.IdTipoActividad
                    ORDER BY A.Fecha DESC";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actividades.Add(MapearActividad(reader));
                    }
                }
            }

            return View(actividades);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ActividadViewModel actividad)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query = @"
                    INSERT INTO Actividades
                    (Titulo, Descripcion, Fecha, Hora, Lugar, Responsable, Estado, IdTipoActividad)
                    VALUES
                    (@Titulo, @Descripcion, @Fecha, @Hora, @Lugar, @Responsable, @Estado, @IdTipoActividad)";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    AgregarParametros(sqlCommand, actividad);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "Actividad creada correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View(ObtenerActividadPorId(id));
        }

        public IActionResult Edit(int id)
        {
            return View(ObtenerActividadPorId(id));
        }

        [HttpPost]
        public IActionResult Edit(ActividadViewModel actividad)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query = @"
                    UPDATE Actividades
                    SET Titulo = @Titulo,
                        Descripcion = @Descripcion,
                        Fecha = @Fecha,
                        Hora = @Hora,
                        Lugar = @Lugar,
                        Responsable = @Responsable,
                        Estado = @Estado,
                        IdTipoActividad = @IdTipoActividad
                    WHERE IdActividad = @IdActividad";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@IdActividad", actividad.IdActividad);
                    AgregarParametros(sqlCommand, actividad);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "Actividad modificada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query = "DELETE FROM Actividades WHERE IdActividad = @IdActividad";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@IdActividad", id);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "Actividad eliminada correctamente.";
            return RedirectToAction("Index");
        }

        private ActividadViewModel ObtenerActividadPorId(int id)
        {
            var actividad = new ActividadViewModel();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query = @"
                    SELECT A.IdActividad, A.Titulo, A.Descripcion, A.Fecha, A.Hora,
                           A.Lugar, A.Responsable, A.Estado, A.IdTipoActividad,
                           T.Nombre AS TipoActividad
                    FROM Actividades A
                    INNER JOIN TipoActividad T ON A.IdTipoActividad = T.IdTipoActividad
                    WHERE A.IdActividad = @IdActividad";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@IdActividad", id);

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            actividad = MapearActividad(reader);
                        }
                    }
                }
            }

            return actividad;
        }

        private ActividadViewModel MapearActividad(SqlDataReader reader)
        {
            return new ActividadViewModel
            {
                IdActividad = Convert.ToInt32(reader["IdActividad"]),
                Titulo = reader["Titulo"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                Fecha = Convert.ToDateTime(reader["Fecha"]),
                Hora = reader["Hora"] == DBNull.Value ? null : (TimeSpan)reader["Hora"],
                Lugar = reader["Lugar"].ToString(),
                Responsable = reader["Responsable"].ToString(),
                Estado = reader["Estado"].ToString(),
                IdTipoActividad = Convert.ToInt32(reader["IdTipoActividad"]),
                TipoActividad = reader["TipoActividad"].ToString()
            };
        }

        private void AgregarParametros(SqlCommand sqlCommand, ActividadViewModel actividad)
        {
            sqlCommand.Parameters.AddWithValue("@Titulo", actividad.Titulo);
            sqlCommand.Parameters.AddWithValue("@Descripcion", actividad.Descripcion ?? "");
            sqlCommand.Parameters.AddWithValue("@Fecha", actividad.Fecha);
            sqlCommand.Parameters.AddWithValue("@Hora", actividad.Hora.HasValue ? actividad.Hora.Value : DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@Lugar", actividad.Lugar ?? "");
            sqlCommand.Parameters.AddWithValue("@Responsable", actividad.Responsable ?? "");
            sqlCommand.Parameters.AddWithValue("@Estado", actividad.Estado);
            sqlCommand.Parameters.AddWithValue("@IdTipoActividad", actividad.IdTipoActividad);
        }
    }
}