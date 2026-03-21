using DataAccess.Modelos.Entidades;

namespace DataAccess.Repositorios.Auditorias
{
    public interface IAuditoriaRepository
    {
        Task AgregarAuditoria(Auditoria model);
    }
}
