using System.Collections.ObjectModel;
using R3;
using Reactive.Bindings.R3;
using Reactive.Bindings.R3.Extensions;
using Reactive.Bindings.R3.Notifiers;

namespace R3MigrationSample;

/// <summary>
/// A small registration ViewModel migrated from ReactiveProperty to R3 plus the
/// <c>ReactiveProperty.R3</c> bridge. It exercises every gap category called out by the
/// acceptance criteria: notifiers, validation, async commands, and collections.
/// </summary>
/// <remarks>
/// See <c>README.md</c> for the original ReactiveProperty (namespace <c>Reactive.Bindings</c>)
/// source this was migrated from, and the rule that drove each rewrite.
/// </remarks>
public sealed class RegistrationViewModel : IDisposable
{
    private readonly ObservableCollection<string> _registrations = [];

    // Validation gap: ValidatableReactiveProperty<T> exposes R3 error streams.
    public ValidatableReactiveProperty<string> Name { get; }

    // R3-direct: ReactiveProperty<int> is R3's read/write property (was ReactivePropertySlim<int>).
    public ReactiveProperty<int> Age { get; }

    // Notifier gap: BusyNotifier signals work in progress via a thread-safe reference count.
    public BusyNotifier Busy { get; } = new();

    // Collection gap: ReadOnlyReactiveCollection<T> mirrors the source collection with a converter.
    public ReadOnlyReactiveCollection<RegisteredUser> RegisteredUsers { get; }

    // Async-command gap: AsyncReactiveCommand disables itself while running.
    public AsyncReactiveCommand RegisterCommand { get; }

    public RegistrationViewModel()
    {
        Name = new ValidatableReactiveProperty<string>("")
            .SetValidateNotifyError(static x =>
                string.IsNullOrWhiteSpace(x) ? "Name is required." : null);

        Age = new ReactiveProperty<int>(0);

        RegisteredUsers = _registrations.ToReadOnlyReactiveCollection(
            static name => new RegisteredUser(name));

        // Rx composition stays R3-native: the command is enabled only when the name is valid
        // and no registration is in flight.
        var canRegister = Busy.CombineLatest(
            Name.ObserveHasErrors,
            static (isBusy, hasErrors) => !isBusy && !hasErrors);

        RegisterCommand = canRegister
            .ToAsyncReactiveCommand()
            .WithSubscribe(RegisterAsync);
    }

    private async Task RegisterAsync()
    {
        using (Busy.ProcessStart())
        {
            // Simulate an asynchronous persistence call.
            await Task.Delay(50).ConfigureAwait(false);
            _registrations.Add($"{Name.Value} ({Age.Value})");
        }

        Name.Value = "";
        Age.Value = 0;
    }

    public void Dispose()
    {
        RegisterCommand.Dispose();
        RegisteredUsers.Dispose();
        Name.Dispose();
        Age.Dispose();
    }
}

/// <summary>
/// A projected, read-only view of a stored registration string.
/// </summary>
public sealed record RegisteredUser(string DisplayName);
