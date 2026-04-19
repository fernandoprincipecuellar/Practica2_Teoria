using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Practica2_Teoria.Data;
using Practica2_Teoria.Models;

namespace Practica2_Teoria.Controllers;

[Authorize(Roles = "Coordinador")]
[Route("Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;

    public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("Cursos/Index")]
    public async Task<IActionResult> CursosIndex()
    {
        var cursos = await _context.Cursos.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();
        return View("Cursos/Index", cursos);
    }

    [HttpGet("Cursos/Create")]
    public IActionResult CursosCreate() => View("Cursos/Create", new Curso { Activo = true });

    [HttpPost("Cursos/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CursosCreate(Curso curso)
    {
        if (curso.Creditos <= 0)
        {
            ModelState.AddModelError(nameof(curso.Creditos), "Los créditos deben ser mayores a cero.");
        }

        if (curso.HorarioInicio >= curso.HorarioFin)
        {
            ModelState.AddModelError(nameof(curso.HorarioFin), "El horario de fin debe ser posterior al horario de inicio.");
        }

        if (!ModelState.IsValid)
        {
            return View("Cursos/Create", curso);
        }

        curso.Activo = true;
        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync("cursos_activos");

        return RedirectToAction(nameof(CursosIndex));
    }

    [HttpGet("Cursos/Edit/{id}")]
    public async Task<IActionResult> CursosEdit(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
        {
            return NotFound();
        }
        return View("Cursos/Edit", curso);
    }

    [HttpPost("Cursos/Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CursosEdit(int id, Curso curso)
    {
        if (id != curso.Id)
        {
            return BadRequest();
        }

        if (curso.Creditos <= 0)
        {
            ModelState.AddModelError(nameof(curso.Creditos), "Los créditos deben ser mayores a cero.");
        }

        if (curso.HorarioInicio >= curso.HorarioFin)
        {
            ModelState.AddModelError(nameof(curso.HorarioFin), "El horario de fin debe ser posterior al horario de inicio.");
        }

        if (!ModelState.IsValid)
        {
            return View("Cursos/Edit", curso);
        }

        var cursoExistente = await _context.Cursos.FindAsync(id);
        if (cursoExistente == null)
        {
            return NotFound();
        }

        cursoExistente.Codigo = curso.Codigo;
        cursoExistente.Nombre = curso.Nombre;
        cursoExistente.Creditos = curso.Creditos;
        cursoExistente.CupoMaximo = curso.CupoMaximo;
        cursoExistente.HorarioInicio = curso.HorarioInicio;
        cursoExistente.HorarioFin = curso.HorarioFin;
        cursoExistente.Activo = curso.Activo;

        await _context.SaveChangesAsync();
        await _cache.RemoveAsync("cursos_activos");

        return RedirectToAction(nameof(CursosIndex));
    }

    [HttpPost("Cursos/Desactivar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CursosDesactivar(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
        {
            return NotFound();
        }

        curso.Activo = false;
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync("cursos_activos");

        return RedirectToAction(nameof(CursosIndex));
    }

    [HttpGet("Matriculas/Index")]
    public async Task<IActionResult> MatriculasIndex()
    {
        var matriculas = await _context.Matriculas
            .Include(m => m.Curso)
            .Include(m => m.Usuario)
            .AsNoTracking()
            .OrderBy(m => m.Curso!.Nombre)
            .ThenBy(m => m.FechaRegistro)
            .ToListAsync();

        return View("Matriculas/Index", matriculas);
    }

    [HttpPost("Matriculas/Confirmar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MatriculasConfirmar(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula == null)
        {
            return NotFound();
        }

        matricula.Estado = Estado.Confirmada;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MatriculasIndex));
    }

    [HttpPost("Matriculas/Cancelar/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MatriculasCancelar(int id)
    {
        var matricula = await _context.Matriculas.FindAsync(id);
        if (matricula == null)
        {
            return NotFound();
        }

        matricula.Estado = Estado.Cancelada;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MatriculasIndex));
    }

    [AllowAnonymous]
    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied() => View();
}
