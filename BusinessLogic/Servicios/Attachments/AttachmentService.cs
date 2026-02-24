using DataAccess.Modelos.DTOs.Attachments;
using DataAccess.Repositorios.Attachments;

namespace BusinessLogic.Servicios.Attachments
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _repository;
        public AttachmentService(IAttachmentRepository repository)
        {
            _repository = repository;
        }
        public async Task<AttachmentDownloadDto> DownloadAttachmentAsync(int attachmentId)
        {
            var attachment = await _repository.GetFileByAttachmentIdAsync(attachmentId);

            if (attachment == null)
                throw new KeyNotFoundException("Archivo no encontrado.");

            return attachment;
        }
    }
}
