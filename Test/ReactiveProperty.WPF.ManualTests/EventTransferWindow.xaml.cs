using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveProperty.Tests.Documentation;

namespace ReactiveProperty.WPF.ManualTests;

public partial class EventTransferWindow : Window
{
    private const string Chapter = "Event-transfer-to-ViewModel-from-View";

    public EventTransferWindow()
    {
        InitializeComponent();
        CommandEnglish.Content = DocumentationExamples.Create("docs", Chapter, 0);
        PropertyEnglish.Content = DocumentationExamples.Create("docs", Chapter, 1);
        CommandJapanese.Content = DocumentationExamples.Create("docs-ja", Chapter, 0);
        PropertyJapanese.Content = DocumentationExamples.Create("docs-ja", Chapter, 1);
    }

    protected override void OnClosed(EventArgs e)
    {
        foreach (var frame in new[] { CommandEnglish, PropertyEnglish, CommandJapanese, PropertyJapanese })
        {
            if (frame.Content is Page page && page.DataContext is object model)
            {
                foreach (var property in model.GetType().GetProperties())
                {
                    (property.GetValue(model) as IDisposable)?.Dispose();
                }
                page.DataContext = null;
                frame.Content = null;
            }
        }
        base.OnClosed(e);
    }
}
