using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProgramacionV_2026.Models;

namespace ProgramacionV_2026.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public UsuarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Index(UsuarioViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                //string passwordCrypted = BCrypt.Net.BCrypt.HashPassword(model.Password);

                string query = "SELECT Password FROM Users WHERE Username = @Username";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Username", model.Username);
                    //sqlCommand.Parameters.AddWithValue("@Password", passwordCrypted);
                    var results = sqlCommand.ExecuteScalar();

                    if (results != null)
                    {

                        string hashedPassword = results.ToString();
                        if (BCrypt.Net.BCrypt.Verify(model.Password, hashedPassword))
                        {
                            // Usuario autenticado correctamente
                            HttpContext.Session.SetString("UsuarioLogueado", model.Username);
                            return RedirectToAction("Index","Home");
                        }
                        //else
                        //{
                        // Credenciales incorrectas
                        // ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                        //}
                    }
                }
            }
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View(model);
        }




        // GET: UsuarioController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            ViewBag.UsuarioInsertado = HttpContext.Session.GetString("UsuarioInsertado");
            HttpContext.Session.Remove("UsuarioInsertado");
           
            var usuarioLogueado = HttpContext.Session.GetString("UsuarioLogueado");
            if (string.IsNullOrEmpty(usuarioLogueado))
            {
                //ViewBag.Usuario = usuarioLogueado;
                return RedirectToAction("Index", "Usuario");
            }
            return View();
        }



        [HttpPost]
        public IActionResult Create(UsuarioViewModel usuario) {

            string passwordCrypted = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = "INSERT INTO Users ([Username] ,[Password]) VALUES (@Username,@Password)";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Username", usuario.Username);
                    sqlCommand.Parameters.AddWithValue("@Password", passwordCrypted);
                    int userCount = (int)sqlCommand.ExecuteNonQuery();

                    if (userCount > 0)
                    {
                        // Usuario Creado correctamente
                        HttpContext.Session.SetString("UsuarioInsertado", usuario.Username);
                        return RedirectToAction("Create");
                    }
                }

            }  
            return View();
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
