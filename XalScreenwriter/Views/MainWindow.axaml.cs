using System;
using Avalonia.Controls;
using XalScreenwriter.ViewModels;
using XalScreenwriter.Models;

namespace XalScreenwriter.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        Editor.Text = _viewModel.CurrentDocument.Content;

        Editor.Document.TextChanged += Editor_Document_TextChanged;
        Editor.TextArea.Caret.PositionChanged += Caret_PositionChanged;

        UpdateCaretPosition();
    }

    private void Editor_Document_TextChanged(object? sender, EventArgs e)
    {
        _viewModel.CurrentDocument.Content = Editor.Text;
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
}