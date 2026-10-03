using System;
using System.Collections.Generic;

namespace XalScreenwriter.Models;

public static class DocumentRecognizer
{
    public static List<DocumentElement> Recognize(
        string content,
        DocumentTypeDefinition documentType)
    {
        var elements = new List<DocumentElement>();

        string[] lines = content.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None);

        string? previousType = null;
        int currentOffset = 0;

        for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
        {
            string line = lines[lineNumber];

            if (string.IsNullOrWhiteSpace(line))
            {
                previousType = null;
                currentOffset += line.Length + 1;
                continue;
            }

            DocumentElementDefinition? definition =
                FindElement(line, previousType, documentType);

            string type = definition?.Name ?? "unknown";

            elements.Add(new DocumentElement
            {
                Type = type,
                Content = line,
                LineNumber = lineNumber,
                StartOffset = currentOffset,
                Length = line.Length
            });

            previousType = type;

            currentOffset += line.Length + 1;
        }

        return elements;
    }

    private static DocumentElementDefinition? FindElement(
        string line,
        string? previousType,
        DocumentTypeDefinition documentType)
    {
        foreach (DocumentElementDefinition element in documentType.Elements)
        {
            if (element.Recognition?.Patterns?.Prefixes is { Count: > 0 } prefixes)
            {
                foreach (string prefix in prefixes)
                {
                    if (line.StartsWith(
                            prefix,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return element;
                    }
                }
            }

            if (element.Recognition?.Patterns?.StartsWith is string startsWith
                && line.StartsWith(
                    startsWith,
                    StringComparison.Ordinal))
            {
                if (element.Recognition.Patterns.EndsWith is string endsWith
                    && !line.EndsWith(
                        endsWith,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                return element;
            }

            if (element.Recognition?.Patterns?.Case == "uppercase"
                && line == line.ToUpperInvariant())
            {
                if (element.AllowedAfter.Count == 0
                    || (previousType is not null
                        && element.AllowedAfter.Contains(previousType)))
                {
                    return element;
                }
            }
        }

        foreach (DocumentElementDefinition element in documentType.Elements)
        {
            if (element.AllowedAfter.Contains(previousType ?? string.Empty))
            {
                return element;
            }
        }

        foreach (DocumentElementDefinition element in documentType.Elements)
        {
            if (element.Recognition?.Default == true)
            {
                return element;
            }
        }

        return null;
    }
}