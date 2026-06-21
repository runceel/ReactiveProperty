using R3MigrationSample;

// Drives the migrated RegistrationViewModel end-to-end and verifies the migrated behavior.
// Returns a non-zero exit code if any assertion fails so the sample can run as a smoke test.
using var viewModel = new RegistrationViewModel();

var failures = 0;

void Check(string description, bool condition)
{
    var status = condition ? "PASS" : "FAIL";
    Console.WriteLine($"[{status}] {description}");
    if (!condition)
    {
        failures++;
    }
}

// Validation: an empty name is invalid and blocks the command.
Check("Empty name reports a validation error", viewModel.Name.HasErrors);
Check("Command disabled while the name is invalid", !viewModel.RegisterCommand.CanExecute());

// Validation clears once a valid name is provided.
viewModel.Name.Value = "Alice";
viewModel.Age.Value = 30;
Check("Valid name clears the validation error", !viewModel.Name.HasErrors);
Check("Command enabled once the form is valid", viewModel.RegisterCommand.CanExecute());

// Async command + notifier: the command runs, toggles Busy, and refills the collection.
var registerTask = viewModel.RegisterCommand.ExecuteAsync();
await SpinWaitAsync(() => viewModel.Busy.IsBusy);
Check("BusyNotifier reports busy while registering", viewModel.Busy.IsBusy);
Check("Command disabled while busy", !viewModel.RegisterCommand.CanExecute());

await registerTask;
Check("BusyNotifier clears after registering", !viewModel.Busy.IsBusy);

// Collection: the read-only reactive collection mirrors the registered users.
Check("ReadOnlyReactiveCollection mirrors the registration", viewModel.RegisteredUsers.Count == 1);
Check(
    "Registered user is projected through the converter",
    viewModel.RegisteredUsers[0].DisplayName == "Alice (30)");

// After registering, the form resets and becomes invalid again.
Check("Form resets to an invalid state after registering", viewModel.Name.HasErrors);

Console.WriteLine();
Console.WriteLine(failures == 0
    ? "All checks passed. The migrated ViewModel runs on R3 + ReactiveProperty.R3."
    : $"{failures} check(s) failed.");

return failures == 0 ? 0 : 1;

static async Task SpinWaitAsync(Func<bool> condition)
{
    for (var i = 0; i < 100; i++)
    {
        if (condition())
        {
            return;
        }

        await Task.Delay(10).ConfigureAwait(false);
    }
}
