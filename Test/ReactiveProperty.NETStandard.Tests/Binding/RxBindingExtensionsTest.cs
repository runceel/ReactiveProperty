using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Extensions;

namespace ReactiveProperty.Tests.Binding;

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
    public void BindToOneWayTestForReadOnlyRP()
    {
        var source = new ReactiveProperty<string>();
        var target = source.ToReadOnlyReactiveProperty();
        var obj = new Poco();

        target.BindTo(obj, o => o.Name);

        obj.Name.IsNull();

        source.Value = "Hello world";
        obj.Name.Is("Hello world");
    }

    [TestMethod]
    public void BindToOneWayTestForRPSlim()
    {
        var target = new ReactivePropertySlim<string>();
        var obj = new Poco();

        target.BindTo(obj, o => o.Name);

        obj.Name.IsNull();

        target.Value = "Hello world";
        obj.Name.Is("Hello world");
    }

    [TestMethod]
    public void BindToOneWayTestForReadOnlyRPSlim()
    {
        var source = new ReactivePropertySlim<string>();
        var target = source.ToReadOnlyReactivePropertySlim();
        var obj = new Poco();

        target.BindTo(obj, o => o.Name);

        obj.Name.IsNull();

        source.Value = "Hello world";
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
    public void BindToOnwWayConvertTestForReadOnlyRP()
    {
        var source = new ReactiveProperty<int>();
        var target = source.ToReadOnlyReactiveProperty();
        var obj = new Poco();

        target.BindTo(
            obj,
            o => o.Name,
            convert: i => "value is " + i);

        obj.Name.Is("value is 0");

        source.Value = 1;
        obj.Name.Is("value is 1");
    }

    [TestMethod]
    public void BindToOnwWayConvertTestForRPSlim()
    {
        var target = new ReactivePropertySlim<int>();
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
    public void BindToOnwWayConvertTestForReadOnlyRPSlim()
    {
        var source = new ReactivePropertySlim<int>();
        var target = source.ToReadOnlyReactivePropertySlim();
        var obj = new Poco();

        target.BindTo(
            obj,
            o => o.Name,
            convert: i => "value is " + i);

        obj.Name.Is("value is 0");

        source.Value = 1;
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
    public void BindToTwoWayTestForRPSlim()
    {
        var target = new ReactivePropertySlim<string>();
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
    public void BindToTwoWayConvertTestForRPSlim()
    {
        var target = new ReactivePropertySlim<int>();
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

    [TestMethod]
    public void BindToOneWayToSourceTestForRPSlim()
    {
        var target = new ReactivePropertySlim<string>();
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
        get { return name; }
        set { SetProperty(ref name, value); }
    }
}
