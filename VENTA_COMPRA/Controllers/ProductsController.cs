using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using VENTA_COMPRA.Data;
using VENTA_COMPRA.Models;
namespace VENTA_COMPRA.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(AppDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var products = context.Product.ToList();
            return View(products);

        }

        public IActionResult Buscar(string query, string ordenar)
        {
            var productos = context.Product.AsQueryable();

          
                if (!string.IsNullOrEmpty(query))
                {
                    productos = productos.Where(p =>
                        p.NombreAuto.Contains(query) ||
                        p.Combustible.Contains(query) ||
                        p.AñoModelo.ToString().Contains(query) ||
                        p.Color.Contains(query) ||
                        p.Transmision.Contains(query) ||
                        p.Tipo.Contains(query)||
                        p.AñoModelo.Equals(query)||
                        p.Motor.Equals(query)
                     
                    );
                }
            switch (ordenar)
            {
                case "añoAsc":
                    productos = productos.OrderBy(p => p.AñoModelo);
                    break;
                case "añoDesc":
                    productos = productos.OrderByDescending(p => p.AñoModelo);
                    break;
                case "precioAsc":
                    productos = productos.OrderBy(p => p.Precio);
                    break;
                case "precioDesc":
                    productos = productos.OrderByDescending(p => p.Precio);
                    break;
                default:
                    // Orden por defecto si no se selecciona nada
                    productos = productos.OrderBy(p => p.NombreAuto);
                    break;
            }
            if (!productos.Any())
                {
                    return RedirectToAction("Index", "Products");
                }
    
                 return View("Buscar", productos.ToList()); // Devuelve la vista "Buscar.cshtml"
        }
        public IActionResult Vender()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Vender(ProductDto productDto)
        {
            if (productDto.ImageFileName==null)
            {
                ModelState.AddModelError("ImageFileName","The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFileName!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFileName.CopyTo(stream);
            }
            Product product = new Product()
            {
                  Nombres =productDto.Nombres,
                  Apellidos =productDto.Apellidos,
                  DNI =productDto.DNI,
                 Telefono=productDto.Telefono,
                  Email =productDto.Email,
                  Departamento =productDto.Departamento,
                  Provincia =productDto.Provincia,      
                  Distrito =productDto.Distrito,   
                   NombreAuto=productDto.NombreAuto, 
                  Precio =productDto.Precio,
                    Kilometros=productDto.Kilometros, 
                    Combustible =productDto.Combustible,
                    Motor=productDto.Motor,
                    Color =productDto.Color,
                     AñoModelo =productDto.AñoModelo,
                     Transmision =productDto.Transmision,
                    Tipo=productDto.Tipo,
                    Puertas=productDto.Puertas,
                    Descripcion=productDto.Descripcion,
                     ImageFileName = newFileName,
                     FechaVenta =DateTime.Now,
                    CreateAt =DateTime.Now,
                   
             };

            context.Product.Add(product);
            context.SaveChanges();

            TempData["SuccessMessage"] = "Auto registrado con éxito.";

            return RedirectToAction("Index", "Products");
        }


        public IActionResult Detalle(int id)
        {
            var product = context.Product.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email,string dni) {

            ViewBag.Email = email;
            ViewBag.DNI = dni;

            // Verificar si el usuario existe en la base de datos
            var user = context.Product.FirstOrDefault(u => u.Email == email && u.DNI == dni);

            if (user == null)
            {
                // Enviar mensaje de error si los datos no existen
                ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
                return View();
            }

            // Enviar mensaje de éxito si el usuario es encontrado
            ViewBag.SuccessMessage = "¡Usuario logueado correctamente!";
            return RedirectToAction("Index", "Home"); // Redirigir a la acción deseada
        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Eliminar datos del usuario de la sesión
            HttpContext.Session.Remove("UserEmail");

            // Redirigir a la página de login
            return RedirectToAction("Login");
        }
    }
}
