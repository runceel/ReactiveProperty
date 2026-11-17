using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;

namespace ReactiveProperty.Tests.Documentation;

internal sealed class DocumentationWindow : TraceListener
{
    private readonly IScheduler _previousScheduler;
    private readonly SourceLevels _previousTraceLevel;
    private readonly StringBuilder _bindingErrors = new();
    private readonly HashSet<object> _disposed = new(ReferenceEqualityComparer.Instance);

    public Window Window { get; }
    public Page Page { get; }

    public DocumentationWindow(string locale, string chapter, int page)
    {
        _previousScheduler = ReactivePropertyScheduler.Default;
        ReactivePropertyScheduler.SetDefault(new SynchronizationContextScheduler(SynchronizationContext.Current));
        _previousTraceLevel = PresentationTraceSources.DataBindingSource.Switch.Level;
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;
        PresentationTraceSources.DataBindingSource.Listeners.Add(this);
        try
        {
            Page = DocumentationExamples.Create(locale, chapter, page);
            Assert.IsNotNull(Page.DataContext, $"{locale}/{chapter}/{page}: missing DataContext");
            Window = new Window
            {
                Title = $"Documentation: {locale}/{chapter}/{page}",
                Content = Page,
                Width = 800,
                Height = 600,
                Left = -10000,
                ShowActivated = false,
                ShowInTaskbar = false
            };
            Window.Show();
            Drain();
        }
        catch
        {
            Window?.Close();
            DisposeModel(Page?.DataContext);
            RestoreEnvironment();
            throw;
        }
    }

    public T[] Controls<T>() where T : DependencyObject
    {
        Window.UpdateLayout();
        return Descendants(Page).OfType<T>().ToArray();
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            yield return child;
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }

    public void Click(Button button)
    {
        Assert.IsTrue(button.IsEnabled, $"Disabled button: {button.Content}");
        var peer = new ButtonAutomationPeer(button);
        var invoke = (IInvokeProvider)peer.GetPattern(PatternInterface.Invoke);
        invoke.Invoke();
        Drain();
    }

    public static void Enter(TextBox input, string value)
    {
        input.SetCurrentValue(TextBox.TextProperty, value);
        Drain();
    }

    public static void Drain()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    public static void Until(Func<bool> condition)
    {
        if (condition())
        {
            Drain();
            return;
        }

        var frame = new DispatcherFrame();
        var timeout = Stopwatch.StartNew();
        var timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };
        timer.Tick += (_, _) =>
        {
            if (condition() || timeout.Elapsed > TimeSpan.FromSeconds(30))
            {
                frame.Continue = false;
            }
        };
        timer.Start();
        try
        {
            Dispatcher.PushFrame(frame);
            Assert.IsTrue(condition(), "The documented UI did not reach the expected state within the watchdog timeout.");
            Drain();
        }
        finally
        {
            timer.Stop();
        }
    }

    public static T Property<T>(object instance, string name)
    {
        var property = instance.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(property, $"{instance.GetType().Name}.{name}");
        var value = property.GetValue(instance);
        Assert.IsInstanceOfType<T>(value);
        return (T)value;
    }

    public void AssertNoBindingErrors()
    {
        Drain();
        Assert.AreEqual("", _bindingErrors.ToString(), "WPF binding diagnostics");
    }

    private void DisposeModel(object value)
    {
        if (value == null || value is string || !_disposed.Add(value))
        {
            return;
        }
        if (value is IEnumerable collection)
        {
            foreach (var item in collection)
            {
                DisposeModel(item);
            }
        }
        if (value.GetType().Namespace?.StartsWith("ReactiveProperty.Tests.Documentation.Generated.", StringComparison.Ordinal) == true)
        {
            foreach (var property in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.GetIndexParameters().Length == 0 && property.GetMethod != null)
                {
                    DisposeModel(property.GetValue(value));
                }
            }
        }
        (value as IDisposable)?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                Window.Close();
                DisposeModel(Page.DataContext);
                Page.DataContext = null;
                Drain();
            }
            finally
            {
                RestoreEnvironment();
            }
        }
        base.Dispose(disposing);
    }

    private void RestoreEnvironment()
    {
        PresentationTraceSources.DataBindingSource.Listeners.Remove(this);
        PresentationTraceSources.DataBindingSource.Switch.Level = _previousTraceLevel;
        ReactivePropertyScheduler.SetDefault(_previousScheduler);
    }

    public override void Write(string message)
    {
        _bindingErrors.Append(message);
    }

    public override void WriteLine(string message)
    {
        _bindingErrors.AppendLine(message);
    }
}
