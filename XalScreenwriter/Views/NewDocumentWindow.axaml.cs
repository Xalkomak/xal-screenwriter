using System.Collections.Generic;
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using XalScreenwriter.Models;

namespace XalScreenwriter.Views;

public partial class NewDocumentWindow : Window
{
    private readonly List<DocumentTypeDefinition> _documentTypes;

    public string ProjectName { get; private set; } = string.Empty;

    public DocumentTypeDefinition? SelectedDocumentType
    {
        get
        {
            return DocumentTypeComboBox.SelectedItem
                as DocumentTypeDefinition;
        }
    }

    public NewDocumentWindow(
        IEnumerable<DocumentTypeDefinition> documentTypes)
    {
        InitializeComponent();

        _documentTypes = new List<DocumentTypeDefinition>(
            documentTypes);

        DocumentTypeComboBox.ItemsSource =
            _documentTypes;

        DocumentTypeComboBox.SelectedIndex = 0;
    }

    private void Cancel_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(false);
    }

    private void Create_Click(
        object? sender,
        RoutedEventArgs e)
    {
        ProjectName =
            ProjectNameBox.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(ProjectName))
        {
            return;
        }

        if (SelectedDocumentType is null)
        {
            return;
        }

        Close(true);
    }

    private async void LoadCustomType_Click(
        object? sender,
        RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFolder> folders =
            await StorageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions
                {
                    Title = "Select Document Type",
                    AllowMultiple = false
                });

        if (folders.Count == 0)
        {
            return;
        }

        IStorageFolder folder = folders[0];

        string? localPath = folder.TryGetLocalPath();

        if (string.IsNullOrWhiteSpace(localPath))
        {
            return;
        }

        try
        {
            DocumentTypeDefinition definition =
                DocumentTypeLoader.LoadFromDirectory(localPath);

            _documentTypes.Add(definition);

            DocumentTypeComboBox.ItemsSource = null;
            DocumentTypeComboBox.ItemsSource = _documentTypes;

            DocumentTypeComboBox.SelectedItem = definition;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Unable to load custom document type: {ex.Message}");
        }
    }
}