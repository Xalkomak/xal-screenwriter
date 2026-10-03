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

    public RecognitionDefinition? Recognition { get; set; }

    public List<string> AllowedAfter { get; set; } = [];

    public PresentationDefinition? Presentation { get; set; }
    public string DisplayName
    {
        get
        {
            string[] words = Name.Split('_');
    
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] =
                        char.ToUpper(words[i][0]) +
                        words[i][1..];
                }
            }
    
            return string.Join(" ", words);
        }
    }
}

public class RecognitionDefinition
{
    public RecognitionPatterns? Patterns { get; set; }

    public bool Default { get; set; }
}

public class RecognitionPatterns
{
    public List<string> Prefixes { get; set; } = [];
    public string? Case { get; set; }
    public string? StartsWith { get; set; }
    public string? EndsWith { get; set; }
}

public class PresentationDefinition
{
    public string? Case { get; set; }

    public AlignmentDefinition? Alignment { get; set; }
}

public class AlignmentDefinition
{
    public string? Horizontal { get; set; }

    public string? Vertical { get; set; }
}