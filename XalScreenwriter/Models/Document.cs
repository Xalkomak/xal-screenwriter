namespace XalScreenwriter.Models;

public class Document
{
    public string Content { get; set; } = string.Empty;

    public DocumentTypeDefinition? DocumentType { get; set; }
}