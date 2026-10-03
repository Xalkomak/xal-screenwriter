using System.Text.Json;
using System;
using System.IO;

namespace XalScreenwriter.Models;

public static class DocumentTypeLoader
{
    public static DocumentTypeDefinition Load(string documentTypeId)
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "doc_type",
            documentTypeId,
            "format.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Document type definition not found: {path}");
        }

        string json = File.ReadAllText(path);

        DocumentTypeDefinition? definition =
            JsonSerializer.Deserialize<DocumentTypeDefinition>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (definition is null)
        {
            throw new InvalidDataException(
                $"Document type definition is empty or invalid: {path}");
        }

        return definition;
    }
}