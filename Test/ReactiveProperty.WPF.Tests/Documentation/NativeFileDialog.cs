using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReactiveProperty.Tests.Documentation;

internal static class NativeFileDialog
{
    public static void VerifySelectAndCancel(DocumentationWindow example)
    {
        var path = Path.Combine(Path.GetTempPath(), $"ReactiveProperty-{Guid.NewGuid():N}.snippet");
        File.WriteAllText(path, "<CodeSnippets />");
        try
        {
            var button = example.Controls<Button>()[0];
            var uiThread = GetCurrentThreadId();
            var select = Operate(uiThread, path);
            example.Click(button);
            select.GetAwaiter().GetResult();
            DocumentationWindow.Until(() => example.Controls<TextBlock>()[^1].Text == path);

            var cancel = Operate(uiThread, null);
            example.Click(button);
            cancel.GetAwaiter().GetResult();
            Assert.AreEqual(path, example.Controls<TextBlock>()[^1].Text, "Cancel must not publish a new file name.");
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static Task Operate(uint uiThread, string path)
    {
        return Task.Run(async () =>
        {
            var elapsed = Stopwatch.StartNew();
            var dialog = IntPtr.Zero;
            var completed = false;
            try
            {
                while (elapsed.Elapsed < TimeSpan.FromSeconds(30))
                {
                    EnumThreadWindows(uiThread, (handle, _) =>
                    {
                        var className = new StringBuilder(128);
                        GetClassName(handle, className, className.Capacity);
                        if (className.ToString() == "#32770")
                        {
                            dialog = handle;
                        }
                        return true;
                    }, IntPtr.Zero);
                    if (dialog != IntPtr.Zero)
                    {
                        if (path == null)
                        {
                            PostMessage(dialog, 0x0111, new IntPtr(2), IntPtr.Zero); // IDCANCEL
                            completed = true;
                            return;
                        }

                        var window = AutomationElement.FromHandle(dialog);
                        var host = window.FindFirst(TreeScope.Descendants,
                            new PropertyCondition(AutomationElement.AutomationIdProperty, "1148"));
                        var input = host?.FindFirst(TreeScope.Descendants,
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                        if (input != null && input.TryGetCurrentPattern(ValuePattern.Pattern, out var valuePattern))
                        {
                            ((ValuePattern)valuePattern).SetValue(path);
                            PostMessage(dialog, 0x0111, new IntPtr(1), IntPtr.Zero); // IDOK
                            completed = true;
                            return;
                        }
                    }
                    await Task.Delay(50).ConfigureAwait(false);
                }
                Assert.Fail($"Could not operate the real file dialog on test UI thread {uiThread}.");
            }
            finally
            {
                if (!completed && dialog != IntPtr.Zero)
                {
                    PostMessage(dialog, 0x0010, IntPtr.Zero, IntPtr.Zero); // WM_CLOSE, only this test's dialog
                }
            }
        });
    }

    private delegate bool EnumWindowsCallback(IntPtr handle, IntPtr parameter);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumThreadWindows(uint threadId, EnumWindowsCallback callback, IntPtr parameter);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr handle, StringBuilder className, int maximumCount);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam);
}
