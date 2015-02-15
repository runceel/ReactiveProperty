using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.Globalization;
using System.Reactive;
using System.Diagnostics;

namespace ReactiveProperty.Tests.Binding
{
    [TestClass]
    public class RxBindingExtensionsTest
    {
        [TestMethod]
        public void BindToOneWayTest()
        {
            var target = new ReactiveProperty<string>();
            var obj = new Poco();

            target.BindTo(obj, o => o.Name);

            obj.Name.IsNull();

            target.Value = "Hello world";
            obj.Name.Is("Hello world");
        }

        [TestMethod]
        public void BindToOnwWayConvertTest()
        {
            var target = new ReactiveProperty<int>();
            var obj = new Poco();

            target.BindTo(
                obj, 
                o => o.Name,
                convert: i => "value is " + i);

            obj.Name.Is("value is 0");

            target.Value = 1;
            obj.Name.Is("value is 1");
        }

        [TestMethod]
        public void BindToTwoWayTest()
        {
            var target = new ReactiveProperty<string>();
            var obj = new Poco();

            target.BindTo(obj,
                o => o.Name,
                mode: BindingMode.TwoWay,
                targetUpdateTrigger: obj.ObserveProperty(o => o.Name).ToUnit());
            obj.Name.IsNull();

            target.Value = "Hello world";
            obj.Name.Is("Hello world");

            obj.Name = "tanaka";
            target.Value.Is("tanaka");
        }

        [TestMethod]
        public void BindToTwoWayConvertTest()
        {
            var target = new ReactiveProperty<int>();
            var obj = new Poco();

            target.BindTo(obj,
                o => o.Name,
                mode: BindingMode.TwoWay,
                convert: i => "value is " + i,
                convertBack: s =>
                {
                    Debug.WriteLine(s);
                    return int.Parse(s, NumberStyles.Integer);
                },
                targetUpdateTrigger: obj.ObserveProperty(o => o.Name).ToUnit());

            obj.Name.Is("value is 0");

            target.Value = 1;
            obj.Name.Is("value is 1");

            obj.Name = "3";
            target.Value.Is(3);
        }

        [TestMethod]
        public void BindToOneWayToSourceTest()
        {
            var target = new ReactiveProperty<string>();
            var obj = new Poco();

            target.BindTo(obj,
                o => o.Name,
                mode: BindingMode.OneWayToSource,
                convertBack: s =>
                {
                    Debug.WriteLine(s);
                    return s + "!";
                },
                targetUpdateTrigger: obj.ObserveProperty(o => o.Name).ToUnit());

            obj.Name.IsNull();

            obj.Name = "Hello";
            target.Value.Is("Hello!");
        }


    }

    public class Poco : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string name;

        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

    }

}
