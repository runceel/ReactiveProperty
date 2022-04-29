module.exports = {
    base: '/ReactiveProperty/',
    title: 'ReactiveProperty documentation',
    description: 'Official document for ReactiveProperty(https://github.com/runceel/ReactiveProperty)',
    ga: 'UA-113459349-1',
    themeConfig: {
        nav: [
            { text: 'Home', link: '/' },
            {
                text: 'Getting started',
                items: [
                    { text: 'Windows Presentation Foundation', link: '/getting-started/wpf.html' },
                    { text: 'Universal Windows Platform', link: '/getting-started/uwp.html' },
                    { text: 'Xamarin.Forms', link: '/getting-started/xf.html' },
                    { text: 'Avalonia', link: '/getting-started/avalonia.html' },
                    { text: 'Uno Platform', link: '/getting-started/uno-platform.html' },
                    { text: 'Blazor', link: '/getting-started/blazor.html' },
                    { text: 'Add code snippets', link: '/getting-started/add-snippets.html' },
                ]
            },
            {
                text: 'Features',
                items: [
                    { text: 'ReactiveProperty', link: '/features/ReactiveProperty.html' },
                    { text: 'ReactivePropertySlim', link: '/features/ReactivePropertySlim.html' },
                    { text: 'Commanding', link: '/features/Commanding.html' },
                    { text: 'Collections', link: '/features/Collections.html' },
                    { text: 'Work together with plane model layer objects', link: '/features/Work-together-with-plane-model-layer-objects.html' },
                    { text: 'Useful classes which implement IObservable', link: '/features/Notifiers.html' },
                    { text: 'Extension methods', link: '/features/Extension-methods.html' },
                    { text: 'Transfer event to ViewModel from View', link: '/features/Event-transfer-to-ViewModel-from-View.html' },
                ]
            },
            {
                text: 'Advanced topics',
                items: [
                    { text: 'Thread control', link: '/advanced/thread.html' },
                    { text: 'Work with await operator', link: '/advanced/awaitable.html' },
                    { text: 'Work with other MVVM Frameworks', link: '/advanced/work-with-other-mvvm-framwork.html' },
                ]
            },
            { text: 'Samples', link: '/samples.html' },
        ],
        lastUpdated: true,
        repo: 'runceel/ReactiveProperty',
        docsRepo: 'runceel/ReactiveProperty',
        docsDir: 'docs/docs',
        editLinks: true,
        editLinkText: 'Help us improve this document!',
    }
}
