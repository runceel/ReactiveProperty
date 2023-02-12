using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Reactive.Bindings.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using System.Linq;
using Reactive.Bindings.Helpers;
using Reactive.Bindings;

namespace ReactiveProperty.Tests.Helpers;

[TestClass]
public class CollectionUtilitiesTest : ReactiveTest
{
    [TestMethod]
    public void ObserveElementPropertyTest()
    {
        var people = new[]
        {
            new Person { FirstName = "Kazuki", LastName = "Ota" },
            new Person { FirstName = "Taro", LastName = "Tanaka" },
            new Person { FirstName = "Nobunaga", LastName = "Oda" },
        };

        var testScheduler = new TestScheduler();
        var observer = testScheduler.CreateObserver<PropertyPack<Person, string>>();
        var disposable = CollectionUtilities.ObserveElementProperty(people, x => x.FirstName).Subscribe(observer);

        testScheduler.AdvanceTo(1);
        people[1].FirstName = "Jiro";

        disposable.Dispose();
        people[2].FirstName = "Saburo";

        observer.Messages
            .Is(
                // Subscribe
                OnNext<PropertyPack<Person, string>>(0, x => x.Instance == people[0] && x.Value == "Kazuki"),
                OnNext<PropertyPack<Person, string>>(0, x => x.Instance == people[1] && x.Value == "Taro"),
                OnNext<PropertyPack<Person, string>>(0, x => x.Instance == people[2] && x.Value == "Nobunaga"),
                OnNext<PropertyPack<Person, string>>(1, x => x.Instance == people[1] && x.Value == "Jiro")
            );
    }

    [TestMethod]
    public void ObserveElementProperty_ChangeElementInstanceTest()
    {
        var people = new[]
        {
            new Person { FirstName = "Kazuki", LastName = "Ota" },
            new Person { FirstName = "Taro", LastName = "Tanaka" },
            new Person { FirstName = "Nobunaga", LastName = "Oda" },
        };

        var original = people.ToArray();

        var testScheduler = new TestScheduler();
        var observer = testScheduler.CreateObserver<PropertyPack<Person, string>>();
        var disposable = CollectionUtilities.ObserveElementProperty(people, x => x.FirstName).Subscribe(observer);

        testScheduler.AdvanceTo(1);
        people[1].FirstName = "Jiro";

        people[1] = new Person { FirstName = "Test", LastName = "User" };
        people[1].FirstName = "Another instance";

        disposable.Dispose();
        people[2].FirstName = "Saburo";

        observer.Messages
            .Is(
                // Subscribe
                OnNext<PropertyPack<Person, string>>(0, x => x.Instance == original[0] && x.Value == "Kazuki"),
                OnNext<PropertyPack<Person, string>>(0, x => x.Instance == original[1] && x.Value == "Taro"),
                OnNext<PropertyPack<Person, string>>(0, x => x.Instance == original[2] && x.Value == "Nobunaga"),
                OnNext<PropertyPack<Person, string>>(1, x => x.Instance == original[1] && x.Value == "Jiro")
            );
    }

    class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }

    class PersonRp
    {
        public ReactivePropertySlim<string> FirstName { get; } = new("");
        public ReactivePropertySlim<string> LastName { get; } = new ("");
    }
}


