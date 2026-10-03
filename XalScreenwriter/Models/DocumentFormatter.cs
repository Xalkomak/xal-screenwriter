using System;

namespace XalScreenwriter.Models;

public static class DocumentFormatter
{
    public static string FormatElement(
        DocumentElement element,
        DocumentTypeDefinition documentType)
    {
        DocumentElementDefinition? definition =
            FindDefinition(element.Type, documentType);

        if (definition?.Presentation?.Case is string caseRule)
        {
            return ApplyCase(element.Content, caseRule);
        }

        return element.Content;
    }

    private static DocumentElementDefinition? FindDefinition(
        string type,
        DocumentTypeDefinition documentType)
    {
        foreach (DocumentElementDefinition element in documentType.Elements)
        {
            if (string.Equals(
                    element.Name,
                    type,
                    StringComparison.OrdinalIgnoreCase))
            {
                return element;
            }
        }

        return null;
    }

    private static string ApplyCase(
        string text,
        string caseRule)
    {
        return caseRule.ToLowerInvariant() switch
        {
            "uppercase" => text.ToUpperInvariant(),

            "lowercase" => text.ToLowerInvariant(),

            _ => text
        };
    }
}