using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using restaurante.Models;

namespace restaurante.Controllers
{
    public class cajaController : Controller
    {
        private readonly restauranteDbContext _context;

        public cajaController(restauranteDbContext context)
        {
            _context = context;
        }

        // GET: caja
        public async Task<IActionResult> Index()
        {
            var mesasDisponibles = _context.mesa.Where(r => r.estado== "Ocupada").ToList();
            return View(mesasDisponibles);
        }

        // GET: caja/Details/5 (mostrar platos consumidos)
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listadoDePedidosPorMesa = (from e in _context.detallePedido
                                           join p in _context.plato on e.id_Plato equals p.id_Plato
                                           where e.id_Mesa == id && e.estado != "cancelado"  // Filtrar por ID de Mesa y platos no cancelados
                                           select new
                                           {
                                               precio = p.precio,
                                               nombrePlato = p.nombre,
                                               cantidadPlato = e.cantidadPlato,
                                               total = p.precio * e.cantidadPlato
                                           }).ToList();

            ViewData["listadoDePedidosPorMesa"] = listadoDePedidosPorMesa;

            var montoTotalConsumido = (from e in _context.detallePedido
                                       join p in _context.plato on e.id_Plato equals p.id_Plato
                                       where e.id_Mesa == id && e.estado != "cancelado" // Filtrar por ID de Mesa igual a 1 y estado distinto de "cancelado"
                                       select e.cantidadPlato * p.precio).Sum();

            ViewData["montoTotalConsumido"] = montoTotalConsumido;

            ViewData["idMesa"] = id;


            return View();
        }


        public async Task<IActionResult> Edit(int id)
        {
            var listadoDePedidosPorMesa = (from e in _context.detallePedido
                                           join p in _context.plato on e.id_Plato equals p.id_Plato
                                           where e.id_Mesa == id && e.estado != "cancelado"  // Filtrar por ID de Mesa y platos no cancelados
                                           select new
                                           {
                                               precio = p.precio,
                                               nombrePlato = p.nombre,
                                               cantidadPlato = e.cantidadPlato,
                                               total = p.precio * e.cantidadPlato
                                           }).ToList();

            ViewData["listadoDePedidosPorMesa"] = listadoDePedidosPorMesa;

            var montoTotalConsumido = (from e in _context.detallePedido
                                       join p in _context.plato on e.id_Plato equals p.id_Plato
                                       where e.id_Mesa == id && e.estado != "cancelado" // Filtrar por ID de Mesa igual a 1 y estado distinto de "cancelado"
                                       select e.cantidadPlato * p.precio).Sum();

            ViewData["montoTotalConsumido"] = montoTotalConsumido;

            ViewData["idMesa"] = id;




            var mesa = _context.mesa.FirstOrDefault(p => p.id_Mesa == id);
            mesa.estado = "Disponible";
            _context.SaveChanges();






            return View("~/Views/caja/Edit.cshtml");
        }



        private bool mesaExists(int id)
        {
            return _context.mesa.Any(e => e.id_Mesa == id);
        }
    }
}
