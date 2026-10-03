using System.Collections.Generic;
namespace XalScreenwriter.Models;

public class Document
{
    public string Content { get; set; } = string.Empty;

    public string DocumentTypeId { get; set; } = string.Empty;

    public DocumentTypeDefinition? DocumentType { get; set; }

    public List<DocumentElement> Elements { get; set; } = [];
}