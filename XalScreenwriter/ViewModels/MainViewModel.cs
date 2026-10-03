using System;
using System.Collections.Generic;
using XalScreenwriter.Models;

namespace XalScreenwriter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Document CurrentDocument { get; }
    public List<DocumentElementDefinition> DocumentElements => CurrentDocument.DocumentType?.Elements ?? [];

    public MainViewModel()
    {
        DocumentTypeDefinition screenplay =
            DocumentTypeLoader.Load("screenplay");

        string sample = """
INT. Bedroom - night

The room is completely dark.

Penelope
(cautious)
Who's there?

She reaches for the light. The room suddenly illuminates.
""";

        CurrentDocument = new Document
        {
            DocumentType = screenplay,
            Content = sample
        };

        CurrentDocument.Elements =
            DocumentRecognizer.Recognize(
                CurrentDocument.Content,
                screenplay);
        foreach (DocumentElement element in CurrentDocument.Elements)
        {
            string formatted =
            DocumentFormatter.FormatElement(
                element,
                screenplay);

            Console.WriteLine(
                $"{element.Type}: {formatted}");
        }
    }
}