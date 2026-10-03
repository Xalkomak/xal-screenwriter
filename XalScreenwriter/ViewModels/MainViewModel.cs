using XalScreenwriter.Models;

namespace XalScreenwriter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Document CurrentDocument { get; }

    public MainViewModel()
    {
        DocumentTypeDefinition screenplay =
            DocumentTypeLoader.Load("screenplay");

        CurrentDocument = new Document
        {
            DocumentType = screenplay,
            Content = "Hello world!\n\nThis is Xalkomak's Screenwriter."
        };
    }
}