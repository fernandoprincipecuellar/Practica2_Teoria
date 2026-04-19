using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica2_Teoria.Data;
using Practica2_Teoria.Models;

namespace Practica2_Teoria.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CursosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Inscribirse(int id)
    {
        var curso = await _context.Cursos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (curso == null)
        {
            return NotFound();
        }

        return View(curso);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inscribirse(int id, [Bind("Id")] Curso cursoModel)
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (curso == null)
        {
            TempData["Error"] = "El curso no existe o no está disponible.";
            return RedirectToAction("Index");
        }

        var matriculaExistente = await _context.Matriculas.AsNoTracking()
            .FirstOrDefaultAsync(m => m.CursoId == id && m.UsuarioId == usuario.Id);
        
        if (matriculaExistente != null)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Details", new { id });
        }

        var cupoUsado = await _context.Matriculas.AsNoTracking()
            .CountAsync(m => m.CursoId == id && (m.Estado == Estado.Confirmada || m.Estado == Estado.Pendiente));
        
        if (cupoUsado >= curso.CupoMaximo)
        {
            TempData["Error"] = "El curso alcanzó su cupo máximo.";
            return RedirectToAction("Details", new { id });
        }

        var cursosDelUsuario = await _context.Matriculas
            .Include(m => m.Curso)
            .AsNoTracking()
            .Where(m => m.UsuarioId == usuario.Id && (m.Estado == Estado.Confirmada || m.Estado == Estado.Pendiente))
            .ToListAsync();

        var solapamiento = cursosDelUsuario.Any(m => 
            m.Curso!.HorarioInicio < curso.HorarioFin && m.Curso.HorarioFin > curso.HorarioInicio
        );

        if (solapamiento)
        {
            TempData["Error"] = "Ya tienes un curso inscrito que se solapa con este horario.";
            return RedirectToAction("Details", new { id });
        }

        var nuevaMatricula = new Matricula
        {
            CursoId = id,
            UsuarioId = usuario.Id,
            FechaRegistro = DateTime.UtcNow,
            Estado = Estado.Pendiente
        };

        _context.Matriculas.Add(nuevaMatricula);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"Te has inscrito exitosamente en {curso.Nombre}. Tu estado es Pendiente.";
        return RedirectToAction("MisMatriculas");
    }

    [Authorize]
    public async Task<IActionResult> MisMatriculas()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var matriculas = await _context.Matriculas
            .Include(m => m.Curso)
            .AsNoTracking()
            .Where(m => m.UsuarioId == usuario.Id)
            .OrderByDescending(m => m.FechaRegistro)
            .ToListAsync();

        return View(matriculas);
    }
}
