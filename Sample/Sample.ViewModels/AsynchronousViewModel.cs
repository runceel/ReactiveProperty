using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Xml.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using Reactive.Bindings.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.IO;   // namespace for Extensions(OnErroRetry etc...)

namespace Sample.ViewModels
{
    // sample of asynchronous operation
    public class AsynchronousViewModel
    {
        public ReactiveProperty<string> SearchTerm { get; }
        public ReactiveProperty<string> SearchingStatus { get; }
        public ReactiveProperty<string> ProgressStatus { get; }
        public ReactiveProperty<WikipediaModel[]> SearchResults { get; }

        public AsynchronousViewModel()
        {
            // Notifier of network connecitng status/count
            var connect = new CountNotifier();
            // Notifier of network progress report
            var progress = new ScheduledNotifier<Tuple<long, long>>(); // current, total

            // skip initialValue on subscribe
            SearchTerm = new ReactiveProperty<string>(mode: ReactivePropertyMode.DistinctUntilChanged);

            // Search asynchronous & result direct bind
            // if network error, use OnErroRetry
            // that catch exception and do action and resubscript.
            SearchResults = SearchTerm
                .Select(async term =>
                {
                    using (connect.Increment()) // network open
                    {
                        return await WikipediaModel.SearchTermAsync(term, progress);
                    }
                })
                .Switch() // flatten
                .OnErrorRetry((HttpRequestException ex) => ProgressStatus.Value = "error occured")
                .ToReactiveProperty();

            // CountChangedStatus : Increment(network open), Decrement(network close), Empty(all complete)
            SearchingStatus = connect
                .Select(x => (x != CountChangedStatus.Empty) ? "loading..." : "complete")
                .ToReactiveProperty();

            ProgressStatus = progress
                .Select(x => string.Format("{0}/{1} {2}%", x.Item1, x.Item2, ((double)x.Item1 / x.Item2) * 100))
                .ToReactiveProperty();
        }
    }

    // Model - data & asynchronous request web api
    public class WikipediaModel
    {
        const string ApiFormat = "http://en.wikipedia.org/w/api.php?action=opensearch&search={0}&format=xml";

        public string Text { get; set; }
        public string Description { get; set; }

        public WikipediaModel(XElement item)
        {
            var ns = item.Name.Namespace;
            Text = (string)item.Element(ns + "Text");
            Description = (string)item.Element(ns + "Description");
        }

        public static async Task<WikipediaModel[]> SearchTermAsync(string term, IProgress<Tuple<long, long>> progress)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ReactivePropertySample", "1.0"));
            var r = await client.GetAsync(new Uri(string.Format(ApiFormat, term)));
            var length = r.Content.Headers.ContentLength ?? -1L;
            var buffer = new byte[1024]; // 1KB
            var stream = await r.Content.ReadAsStreamAsync();
            var memory = new MemoryStream();

            int size = 0;
            while ((size = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await memory.WriteAsync(buffer, 0, size);
                progress.Report(Tuple.Create(memory.Length, length));
                // if you want to see progress this line uncomment.
                // await Task.Delay(100); 
            }

            return Parse(Encoding.UTF8.GetString(memory.ToArray(), 0, (int)memory.Length));
        }

        static WikipediaModel[] Parse(string rawXmlText)
        {
            var xml = XElement.Parse(rawXmlText);
            var ns = xml.Name.Namespace;
            return xml.Descendants(ns + "Item")
                .Select(x => new WikipediaModel(x))
                .ToArray();
        }

        public override string ToString()
        {
            return Text + ":" + Description;
        }
    }
}