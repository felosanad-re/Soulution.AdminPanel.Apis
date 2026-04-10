namespace AdminPanel.Core.ModelsDto.RequestDTO
{
    public class ImportExcelConfiguration<TDTO>
    {
        public string SheetName { get; set; } = "sheet 1";
        public int StartRow { get; set; } = 2;
        public bool HasHeader { get; set; } = true;

        public Dictionary<string, string> ColumnMapping { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
