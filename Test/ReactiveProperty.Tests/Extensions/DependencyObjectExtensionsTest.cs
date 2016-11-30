using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReactiveProperty.Tests.Extensions
{
    [TestClass]
    public class DependencyObjectExtensionsTest
    {
        [TestMethod]
        public void ObserveDependencyProperty()
        {
            var target = new Person();
            var count = 0;
            var token = target.ObserveDependencyProperty(Person.NameProperty)
                .Subscribe(_ => count++);

            count.Is(0);
            target.Name = "tanaka";
            count.Is(1);

            token.Dispose();
            target.Name = "kimura";
            count.Is(1);
        }

        [TestMethod]
        public void ToReadOnlyReactiveProperty()
        {
            var target = new Person();
            target.Name = "tanaka";
            var rp = target.ToReadOnlyReactiveProperty<string>(Person.NameProperty);
            rp.Value.Is("tanaka");

            target.Name = "kimura";
            rp.Value.Is("kimura");

            rp.Dispose();

            target.Name = "ohta";
            rp.Value.Is("kimura");
        }

        [TestMethod]
        public void ToReactiveProperty()
        {
            var target = new Person();
            target.Name = "tanaka";
            var rp = target.ToReactiveProperty<string>(Person.NameProperty);
            rp.Value.Is("tanaka");

            target.Name = "kimura";
            rp.Value.Is("kimura");

            rp.Value = "homuhomu";
            rp.Value.Is("homuhomu");
            target.Name.Is("kimura");

            target.Name = "xin9le";
            rp.Value.Is("xin9le");

            rp.Dispose();

            target.Name = "ohta";
            rp.Value.Is("xin9le");
        }
    }

    class Person : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Person), new PropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

    }
}
