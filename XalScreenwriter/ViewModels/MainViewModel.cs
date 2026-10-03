using XalScreenwriter.Models;

namespace XalScreenwriter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Document CurrentDocument { get; } = new()
    {
        DocumentType = "screenplay",
        Content = "Hello world!\n\nThis is Xalkomak's Screenwriter."
    };
}