using DataAccess.Modelos.DTOs.Attachments;
using DataAccess.Modelos.Entidades.ModTiquete;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repositorios.Attachments
{

    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;
        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddRangeAsync(List<Attachment> attachments)
        {
            await _context.Attachments.AddRangeAsync(attachments);
        }

        public async Task<List<ListaAttachmentDto>> ListarAttachmentsAsync(int idTiquete)
        {
            return await _context.Attachments
                   .AsNoTracking()
                   .Where(a => a.IdTiquete == idTiquete)
                   .OrderByDescending(a => a.UploadedAt)
                   .Select(a => new ListaAttachmentDto
                   {
                       IdAttachment = a.IdAttachment,
                       FileName = a.FileName,
                       FileSize = a.FileSize,
                       UploadedBy = a.UploadedBy,
                       UploadedAt = a.UploadedAt
                   })
                   .ToListAsync();
        }

        //Para un download
        public async Task<AttachmentDownloadDto?> GetFileByAttachmentIdAsync(int attachmentId)
        {
            return await _context.Attachments
                .AsNoTracking()
                .Where(a => a.IdAttachment == attachmentId)
                .Select(a => new AttachmentDownloadDto
                {
                    IdAttachment = a.IdAttachment,
                    File = a.File!,
                    FileName = a.FileName!
                })
                .FirstOrDefaultAsync();
        }
    }
}
