using DataAccess.Modelos.Entidades.ModTiquete;


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
            await _context.SaveChangesAsync();
        }
    }
}
