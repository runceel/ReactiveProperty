using System.Windows.Input;

namespace Reactive.Bindings;

/// <summary>
/// Command extension methods for Blazor.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Return a boolean value that is inverted CanExecute method.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>Inverted value of CanExecute method.</returns>
    public static bool IsDisabled(this ICommand command) => 
        !command.CanExecute(null);
}
