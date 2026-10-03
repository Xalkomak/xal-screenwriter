using System;
using Avalonia.Controls;
using XalScreenwriter.ViewModels;
using Avalonia.Interactivity;
using Avalonia.Input;
using XalScreenwriter.Models;
using XalScreenwriter.Views;
using System.Collections.Generic;
using AvaloniaEdit.Document;

namespace XalScreenwriter.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private Button? _selectedElementButton;
    private readonly Dictionary<string, Button> _elementButtons = [];

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        Editor.Text = _viewModel.CurrentDocument.Content;

        Editor.Document.TextChanged += Editor_Document_TextChanged;
        Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        Editor.TextArea.TextEntered += Editor_TextEntered;

        UpdateCaretPosition();
    }

    private void Editor_Document_TextChanged(
        object? sender,
        EventArgs e)
    {
        _viewModel.CurrentDocument.Content = Editor.Text;

        _viewModel.CurrentDocument.Elements =
            DocumentRecognizer.Recognize(
                Editor.Text,
                _viewModel.CurrentDocument.DocumentType!);
    }

    private void Caret_PositionChanged(object? sender, EventArgs e)
    {
        UpdateCaretPosition();
    }

    private void UpdateCaretPosition()
    {
        var caret = Editor.TextArea.Caret;

        StatusText.Text =
            $"Ln {caret.Line}, Col {caret.Column}";
    }

    private void ElementButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not DocumentElementDefinition definition)
        {
            return;
        }

        _elementButtons[definition.Name] = button;

        SelectElement(definition.Name, button);

        StatusText.Text =
            $"Element: {definition.DisplayName}";

        Editor.Focus();
    }

    private DocumentElement? GetCurrentElement()
    {
        int lineNumber = Editor.TextArea.Caret.Line - 1;

        foreach (DocumentElement element in _viewModel.CurrentDocument.Elements)
        {
            if (element.LineNumber == lineNumber)
            {
                return element;
            }
        }

        return null;
    }

    private void Editor_TextEntered(
        object? sender,
        TextInputEventArgs e)
    {
        switch (_viewModel.SelectedElementType)
        {
            case "character":
                ApplyUppercaseInput(e);
                break;

            case "scene_heading":
                ApplyUppercaseInput(e);
                break;

            case "action":
                ApplySentenceCapitalization(e);
                break;

            case "parenthetical":
                break;

            case "dialogue":
                ApplySentenceCapitalization(e);
                break;
        }

        TryCreateParenthetical(e);
    }

    private void ApplyUppercaseInput(TextInputEventArgs e)
    {
        string uppercase = e.Text.ToUpperInvariant();

        if (e.Text == uppercase)
        {
            return;
        }

        int offset = Editor.CaretOffset;

        Editor.Document.Replace(
            offset - e.Text.Length,
            e.Text.Length,
            uppercase);
    }

    private void ApplySentenceCapitalization(TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        char firstCharacter = e.Text[0];

        if (!char.IsLetter(firstCharacter)
            || !char.IsLower(firstCharacter))
        {
            return;
        }

        int insertedLength = e.Text.Length;
        int offset = Editor.CaretOffset;

        if (offset < insertedLength)
        {
            return;
        }

        int lineNumber = Editor.TextArea.Caret.Line;

        var line = Editor.Document.GetLineByNumber(lineNumber);

        int textBeforeLength =
            offset - insertedLength - line.Offset;

        if (textBeforeLength < 0)
        {
            return;
        }

        string textBefore = Editor.Document.GetText(
            line.Offset,
            textBeforeLength);

        bool startOfSentence =
            string.IsNullOrWhiteSpace(textBefore)
            || EndsSentence(textBefore);

        if (!startOfSentence)
        {
            return;
        }

        string replacement =
            char.ToUpperInvariant(firstCharacter) +
            e.Text[1..];

        Editor.Document.Replace(
            offset - insertedLength,
            insertedLength,
            replacement);
    }   

    private static bool EndsSentence(string text)
    {
        string trimmed = text.TrimEnd();

        if (trimmed.Length == 0)
        {
            return true;
        }

        char lastCharacter = trimmed[^1];

        return lastCharacter is '.' or '!' or '?';
    }

    private void TryCreateParenthetical(TextInputEventArgs e)
    {
        if (e.Text != "(")
        {
            return;
        }

        int currentLineNumber = Editor.TextArea.Caret.Line;

        if (currentLineNumber <= 1)
        {
            return;
        }

        var currentLine =
            Editor.Document.GetLineByNumber(currentLineNumber);

        var previousLine =
            Editor.Document.GetLineByNumber(currentLineNumber - 1);

        string currentText =
            Editor.Document.GetText(
                currentLine.Offset,
                currentLine.Length);

        string previousText =
            Editor.Document.GetText(
                previousLine.Offset,
                previousLine.Length);

        if (currentText != "(")
        {
            return;
        }

        if (!IsCharacterLine(previousText))
        {
            return;
        }

        int offset = Editor.CaretOffset;

        Editor.Document.Insert(offset, ")");

        Editor.CaretOffset = offset;

        SelectElement("parenthetical");
    }

    private bool IsCharacterLine(string text)
    {
        DocumentTypeDefinition? documentType =
            _viewModel.CurrentDocument.DocumentType;

        if (documentType is null)
        {
            return false;
        }

        foreach (DocumentElementDefinition element
            in documentType.Elements)
        {
            if (element.Name != "character")
            {
                continue;
            }

            if (element.Recognition?.Patterns?.Case == "uppercase")
            {
                return text == text.ToUpperInvariant();
            }
        }

        return false;
    }

    private void SelectElement(
        string elementType,
        Button? button = null)
    {
        if (_selectedElementButton is not null)
        {
            _selectedElementButton.Classes.Remove("selected");
        }

        if (button is null
            && _elementButtons.TryGetValue(
                elementType,
                out Button? storedButton))
        {
            button = storedButton;
        }

        if (button is not null)
        {
            button.Classes.Add("selected");
            _selectedElementButton = button;
        }

        _viewModel.SelectedElementType = elementType;
    }

    private async void CreateNew_Click(
        object? sender,
        RoutedEventArgs e)
    {
        var dialog = new NewDocumentWindow(
            _viewModel.AvailableDocumentTypes);

        bool? result = await dialog.ShowDialog<bool?>(this);

        if (result != true)
        {
            return;
        }

        DocumentTypeDefinition? definition =
            dialog.SelectedDocumentType;

        if (definition is null)
        {
            return;
        }

        StatusText.Text =
            $"New {definition.Name}: {dialog.ProjectName}";
    }
}