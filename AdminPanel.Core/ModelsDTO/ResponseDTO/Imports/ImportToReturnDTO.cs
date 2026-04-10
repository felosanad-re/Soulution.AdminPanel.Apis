namespace AdminPanel.Core.ModelsDto.ResponseDTO.Imports
{
    public class ImportToReturnDTO<T>
    {
        public int TotalRows { get; set; }
        public int AddedCount { get; set; }
        public int SkippedDuplicates { get; set; }
        public List<string> Errors { get; set; }
        public string Message => $"Total Rows is {TotalRows}, Added {AddedCount} And skipped {SkippedDuplicates}";
        public IReadOnlyList<T> Data { get; set; }
    }
}
