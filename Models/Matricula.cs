using Microsoft.AspNetCore.Identity;

namespace Practica2_Teoria.Models;

public enum Estado
{
    Pendiente,
    Confirmada,
    Cancelada
}

public class Matricula
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public Estado Estado { get; set; }

    public Curso? Curso { get; set; }
    public IdentityUser? Usuario { get; set; }
}
