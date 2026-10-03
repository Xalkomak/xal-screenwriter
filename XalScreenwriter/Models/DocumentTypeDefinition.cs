using System.Collections.Generic;
namespace XalScreenwriter.Models;

public class DocumentTypeDefinition
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Version { get; set; }

    public List<DocumentElementDefinition> Elements { get; set; } = [];
}

public class DocumentElementDefinition
{
    public string Name { get; set; } = string.Empty;

    public List<string> Format { get; set; } = [];
}