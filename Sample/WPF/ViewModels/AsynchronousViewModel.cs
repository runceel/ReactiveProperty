using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Xml.Linq;
using Codeplex.Reactive;
using Codeplex.Reactive.Asynchronous; // namespace for Asynchronous Extension Methods

namespace WPF.ViewModels
{
    // sample of asynchronous operation
    public class AsynchronousViewModel
    {
        public ReactiveProperty<string> SearchTerm { get; private set; }
        public ReactiveProperty<WikipediaModel[]> SearchResults { get; private set; }

        public AsynchronousViewModel()
        {
            SearchTerm = new ReactiveProperty<string>();

            SearchResults = SearchTerm
                .Select(WikipediaModel.SearchTermAsync)
                .Switch() // flatten
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

        // ***ObservableAsync Extensions Methods for WebClient
        // others, WebRequest/WebResponse/Stream Extensio Methods exists.
        public static IObservable<WikipediaModel[]> SearchTermAsync(string term)
        {
            //var clinet = new WebClient();
            //clinet.Headers[HttpRequestHeader.UserAgent] = "ReactiveProperty Sample";
            //return clinet.DownloadStringObservableAsync(string.Format(ApiFormat, term))
            //    .Select(Parse);
            var req = (HttpWebRequest)WebRequest.Create(string.Format(ApiFormat, term));
            req.UserAgent = "Test";
            return req.DownloadStringAsync().Select(Parse);
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
