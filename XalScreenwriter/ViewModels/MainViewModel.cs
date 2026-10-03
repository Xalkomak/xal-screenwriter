using System;
using System.Collections.Generic;
using XalScreenwriter.Models;

namespace XalScreenwriter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Document CurrentDocument { get; }
    public List<DocumentElementDefinition> DocumentElements => CurrentDocument.DocumentType?.Elements ?? [];
    public string? SelectedElementType { get; set; }
    public List<DocumentTypeDefinition> AvailableDocumentTypes { get; }
    public DocumentTypeDefinition? SelectedDocumentType { get; set; }

    public MainViewModel()
    {
        DocumentTypeDefinition screenplay = DocumentTypeLoader.Load("screenplay");

        string sample = """
        INT. Bedroom - night

        The room is completely dark.

        PENELOPE
        (cautious)
        Who's there?

        She reaches for the light. The room suddenly illuminates.
        """;

        CurrentDocument = new Document
        {
            DocumentTypeId = "screenplay",
            Content = sample,
            DocumentType = DocumentTypeLoader.Load("screenplay")
        };
        
        AvailableDocumentTypes = DocumentTypeLoader.GetAvailableTypes();
        SelectedDocumentType = AvailableDocumentTypes.Find(definition => definition.Id == CurrentDocument.DocumentTypeId);

        

        

        //CurrentDocument.Elements =
        //    DocumentRecognizer.Recognize(
        //        CurrentDocument.Content,
        //        screenplay);
        //foreach (DocumentElement element in CurrentDocument.Elements)
        //{
        //    string formatted =
        //    DocumentFormatter.FormatElement(
        //        element,
        //        screenplay);
//
        //    Console.WriteLine(
        //        $"{element.Type}: {formatted}");
        //}
    }
}