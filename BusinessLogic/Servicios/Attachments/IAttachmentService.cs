
using DataAccess.Modelos.DTOs.Attachments;

namespace BusinessLogic.Servicios.Attachments
{
    public interface IAttachmentService
    {
        Task<AttachmentDownloadDto> DownloadAttachmentAsync(int attachmentId);
    }
}
