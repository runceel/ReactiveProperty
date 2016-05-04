# ReadOnlyReactiveCollection.ToCollectionChanged(*T*) Method (ReadOnlyObservableCollection(*T*))
 

convert ReadOnlyObservableCollection to IO<T>

**Namespace:**&nbsp;<a href="c3971206-685a-088e-bb60-d89f59135b99">Reactive.Bindings</a><br />**Assembly:**&nbsp;ReactiveProperty (in ReactiveProperty.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static IObservable<CollectionChanged<T>> ToCollectionChanged<T>(
	this ReadOnlyObservableCollection<T> self
)

```


#### Parameters
&nbsp;<dl><dt>self</dt><dd>Type: <a href="http://msdn2.microsoft.com/en-us/library/ms668620" target="_blank">System.Collections.ObjectModel.ReadOnlyObservableCollection</a>(*T*)<br />source</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>source type</dd></dl>

#### Return Value
Type: <a href="http://msdn2.microsoft.com/en-us/library/dd990377" target="_blank">IObservable</a>(<a href="24c66563-ab8b-9a2a-e823-ec1fe1f272b9">CollectionChanged</a>(*T*))<br />dest

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type <a href="http://msdn2.microsoft.com/en-us/library/ms668620" target="_blank">ReadOnlyObservableCollection</a>(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="20665008-c291-afc1-b027-ec7b0cf8b44d">ReadOnlyReactiveCollection Class</a><br /><a href="d19d1e08-6ee3-7c66-4acb-b2716b2f6f68">ToCollectionChanged Overload</a><br /><a href="c3971206-685a-088e-bb60-d89f59135b99">Reactive.Bindings Namespace</a><br />