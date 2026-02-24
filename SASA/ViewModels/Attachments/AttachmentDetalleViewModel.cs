namespace SASA.ViewModels.Attachments
{
    public class AttachmentDetalleViewModel
    {
        public int IdAttachment { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public string UploadedBy { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
    }
}
