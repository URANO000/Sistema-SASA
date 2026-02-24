namespace DataAccess.Modelos.DTOs.Attachments
{
    public class ListaAttachmentDto
    {
        public int IdAttachment { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public string UploadedBy { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }
}
