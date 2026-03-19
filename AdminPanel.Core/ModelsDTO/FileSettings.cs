namespace AdminPanel.Core.ModelsDto
{
    public class FileSettings
    {
        public string FolderName { get; set; }
        public string[] AllowedExtentions { get; set; }
        public int MaxSize { get; set; }
    }
}
