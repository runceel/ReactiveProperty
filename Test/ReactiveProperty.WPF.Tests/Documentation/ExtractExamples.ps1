param(
    [Parameter(Mandatory = $true)][string]$DocumentationRoot,
    [Parameter(Mandatory = $true)][string]$OutputDirectory,
    [string]$Chapter
)

$ErrorActionPreference = 'Stop'
$encoding = New-Object System.Text.UTF8Encoding($true)

function Write-GeneratedFile($Path, $Content) {
    if (-not (Test-Path -LiteralPath $Path) -or [IO.File]::ReadAllText($Path) -cne $Content) {
        [IO.File]::WriteAllText($Path, $Content, $encoding)
    }
}

# Indices select the complete C# blocks accompanying each Page, including earlier
# code-behind and model definitions reused by later examples in the same chapter.
$examples = @(
    @{ Chapter = 'Collections'; Page = 0; Code = @(0, 1) },
    @{ Chapter = 'Collections'; Page = 1; Code = @(2, 1) },
    @{ Chapter = 'Collections'; Page = 2; Code = @(3, 4, 5, 1) },
    @{ Chapter = 'Collections'; Page = 3; Code = @(7, 1) },
    @{ Chapter = 'Collections'; Page = 4; Code = @(8, 1) },
    @{ Chapter = 'Commanding'; Page = 0; Code = @(3, 4) },
    @{ Chapter = 'Commanding'; Page = 1; Code = @(12, 4) },
    @{ Chapter = 'Commanding'; Page = 2; Code = @(14, 4) },
    @{ Chapter = 'Commanding'; Page = 3; Code = @(15, 16) },
    @{ Chapter = 'ReactiveProperty'; Page = 0; Code = @(8, 9) },
    @{ Chapter = 'ReactiveProperty'; Page = 1; Code = @(11, 9) },
    @{ Chapter = 'Event-transfer-to-ViewModel-from-View'; Page = 0; Code = @(0, 1) },
    @{ Chapter = 'Event-transfer-to-ViewModel-from-View'; Page = 1; Code = @(0, 2) },
    @{ Chapter = 'Work-together-with-plane-model-layer-objects'; Page = 0; Code = @(0, 2, 3) }
)

if ($Chapter) {
    $examples = @($examples | Where-Object { $_.Chapter -eq $Chapter })
    if ($examples.Count -eq 0) { throw "No executable documentation examples for chapter '$Chapter'." }
}

$usings = @'
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
'@

$factories = New-Object 'System.Collections.Generic.List[string]'
foreach ($locale in @('docs', 'docs-ja')) {
    foreach ($example in $examples) {
        $id = ($example.Chapter -replace '-', '') + $example.Page
        $language = if ($locale -eq 'docs') { 'English' } else { 'Japanese' }
        $namespace = "ReactiveProperty.Tests.Documentation.Generated.$language.$id"
        $directory = Join-Path $OutputDirectory "$language\$id"
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
        $source = Join-Path $DocumentationRoot "$locale\features\$($example.Chapter).md"
        $text = [IO.File]::ReadAllText($source)
        $pages = [regex]::Matches($text, '(?s)```xml\s*(<Page\b.*?</Page>)\s*```')
        $blocks = [regex]::Matches($text, '(?s)```csharp\s*(.*?)```')
        $expectedPages = @($examples | Where-Object { $_.Chapter -eq $example.Chapter }).Count
        if ($pages.Count -ne $expectedPages) {
            throw "Update the executable example map for ${source}: expected $expectedPages Pages, found $($pages.Count)."
        }

        # Isolate examples with repeated type names; do not replace their behavior,
        # clocks, bindings, converters, or model implementations.
        $xaml = $pages[$example.Page].Groups[1].Value
        $xaml = [regex]::Replace($xaml, 'x:Class="[^"]+"', "x:Class=`"$namespace.MainPage`"")
        $xaml = $xaml.Replace('clr-namespace:App1', "clr-namespace:$namespace")
        $xaml = $xaml.Replace('clr-namespace:ViewModels', "clr-namespace:$namespace")
        Write-GeneratedFile (Join-Path $directory 'MainPage.xaml') $xaml
        foreach ($index in $example.Code) {
            if ($index -ge $blocks.Count) { throw "Missing C# block $index in $source" }
            $code = $blocks[$index].Groups[1].Value
            if ($code -match '(?m)^namespace (App1|RPSample)\s*$') {
                $code = [regex]::Replace($code, '(?m)^namespace (App1|RPSample)\s*$', "namespace $namespace")
            } else {
                $code = "namespace $namespace {`n$code`n}"
            }
            $existingUsings = @([regex]::Matches($code, '(?m)^using [^;]+;') | ForEach-Object { $_.Value })
            $additionalUsings = @($usings -split '\r?\n' | Where-Object { $_ -notin $existingUsings }) -join "`n"
            # The snippets omit nullable annotations; preserve their oblivious context.
            Write-GeneratedFile (Join-Path $directory "Block$index.cs") "#nullable disable`n$additionalUsings`n$code`n"
        }
        $factories.Add("(`"$locale`", `"$($example.Chapter)`", $($example.Page)) => new $namespace.MainPage(),")
    }
}

$registry = @"
using System;
using System.Windows.Controls;
namespace ReactiveProperty.Tests.Documentation;
internal static class DocumentationExamples
{
    public static Page Create(string locale, string chapter, int page)
    {
        return (locale, chapter, page) switch
        {
            $($factories -join "`n            ")
            _ => throw new ArgumentOutOfRangeException(nameof(page))
        };
    }
}
"@
Write-GeneratedFile (Join-Path $OutputDirectory 'DocumentationExamples.cs') $registry
