using System.Text.Json;
using System;
using System.IO;
using System.Collections.Generic;

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

    public static DocumentTypeDefinition LoadForDocument(
        Document document)
    {
        if (string.IsNullOrWhiteSpace(document.DocumentTypeId))
        {
            throw new InvalidOperationException(
                "Document type identifier is not set.");
        }

        return Load(document.DocumentTypeId);
    }

    public static List<DocumentTypeDefinition> GetAvailableTypes()
    {
        string rootPath = Path.Combine(
            AppContext.BaseDirectory,
            "doc_type");

        if (!Directory.Exists(rootPath))
        {
            return [];
        }

        var definitions = new List<DocumentTypeDefinition>();

        foreach (string directory in Directory.GetDirectories(rootPath))
        {
            string formatPath =
                Path.Combine(directory, "format.json");

            if (!File.Exists(formatPath))
            {
                continue;
            }

            string json = File.ReadAllText(formatPath);

            DocumentTypeDefinition? definition =
                JsonSerializer.Deserialize<DocumentTypeDefinition>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (definition is not null)
            {
                definitions.Add(definition);
            }
        }

        return definitions;
    }

    public static DocumentTypeDefinition LoadFromDirectory(
        string directoryPath)
    {
        string formatPath =
            Path.Combine(directoryPath, "format.json");
    
        if (!File.Exists(formatPath))
        {
            throw new FileNotFoundException(
                "The selected directory does not contain a format.json.",
                formatPath);
        }
    
        string json = File.ReadAllText(formatPath);
    
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
                $"The document type definition is invalid: {formatPath}");
        }
    
        return definition;
    }
}