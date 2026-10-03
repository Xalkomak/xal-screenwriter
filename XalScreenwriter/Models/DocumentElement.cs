namespace XalScreenwriter.Models;

public class DocumentElement
{
    public string Type { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int LineNumber { get; set; }

    public int StartOffset { get; set; }

    public int Length { get; set; }
}