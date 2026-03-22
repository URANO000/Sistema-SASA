namespace DataAccess.Modelos.DTOs.Attachments
{
    public class AttachmentDownloadDto
    {
        public int IdAttachment { get; set; }
        public byte[] File { get; set; } = default!;
        public string FileName { get; set; } = default!;
    }
}
