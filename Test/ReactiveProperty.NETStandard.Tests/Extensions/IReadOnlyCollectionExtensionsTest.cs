using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Reactive.Bindings.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests.Extensions;

[TestClass]
public class IReadOnlyCollectionExtensionsTest : ReactiveTest
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
        IReadOnlyCollectionExtensions.ObserveElementProperty(people, x => x.FirstName);
        var disposable = people.ObserveElementProperty(x => x.FirstName).Subscribe(observer);

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
}


