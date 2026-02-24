using DataAccess.Modelos.DTOs.Attachments;
using DataAccess.Modelos.Entidades.ModTiquete;


namespace DataAccess.Repositorios.Attachments
{
    public interface IAttachmentRepository
    {
        //Para añadir múltiples archivos adjuntos si hay más de uno
        Task AddRangeAsync(List<Attachment> attachments);
        Task <List<ListaAttachmentDto>> ListarAttachmentsAsync(int idTiquete);

        Task<AttachmentDownloadDto?> GetFileByAttachmentIdAsync(int attachmentId);
    }
}
