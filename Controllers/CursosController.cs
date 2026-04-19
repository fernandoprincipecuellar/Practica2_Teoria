using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica2_Teoria.Data;
using Practica2_Teoria.Models;

namespace Practica2_Teoria.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? nombre, int? creditosMin, int? creditosMax, TimeOnly? horarioInicio, TimeOnly? horarioFin)
    {
        if (creditosMin < 0)
        {
            return BadRequest("El valor mínimo de créditos no puede ser negativo.");
        }

        if (horarioInicio.HasValue && horarioFin.HasValue && horarioFin.Value < horarioInicio.Value)
        {
            return BadRequest("El horario de fin no puede ser anterior al horario de inicio.");
        }

        var cursos = _context.Cursos.AsNoTracking().Where(c => c.Activo);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            cursos = cursos.Where(c => c.Nombre.Contains(nombre));
        }

        if (creditosMin.HasValue)
        {
            cursos = cursos.Where(c => c.Creditos >= creditosMin.Value);
        }

        if (creditosMax.HasValue)
        {
            cursos = cursos.Where(c => c.Creditos <= creditosMax.Value);
        }

        if (horarioInicio.HasValue)
        {
            cursos = cursos.Where(c => c.HorarioInicio >= horarioInicio.Value);
        }

        if (horarioFin.HasValue)
        {
            cursos = cursos.Where(c => c.HorarioFin <= horarioFin.Value);
        }

        var lista = await cursos.OrderBy(c => c.Nombre).ToListAsync();
        return View(lista);
    }

    public async Task<IActionResult> Details(int id)
    {
        var curso = await _context.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (curso == null)
        {
            return NotFound();
        }

        return View(curso);
    }

    [Authorize]
    public IActionResult Inscribir(int id)
    {
        return RedirectToAction("Details", new { id });
    }
}
