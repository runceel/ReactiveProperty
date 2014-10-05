ReactiveProperty
================

![ReactiveProperty overview](Images/rpsummary.png)

ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions.
Target Framework is .NET 4.0 Client Profile, .NET 4.5, Windows Phone 8.1/8.0, Windows store app 8.0/8.1.

## Features

- ReactiveProperty - Two-way bindable IObservable, from V to VM and VM to V.
- ReactiveCommand - Convert observable condition sequence to ICommand
- Typesafe convert INotifyPropertyChanged to ReactiveProperty
- Event to ReactiveProperty Blend trigger
- There means V -> VM -> M -> VM -> V completely connected in reactive, everything is asynchronous
- NuGet installation support.
- PM > Install-Package ReactiveProperty
- <b>ReactiveProperty makes viewmodel extermely clean</b>

> Note:
> ReactiveProperty is not replace existing MVVM Framework.
> ReactiveProperty no provides ViewModelBase, Messenger, etc.
> I recommend that use MVVM Framework together.

## ReactiveProperty & ReactiveCommand Basics

![Basics](Images/basics.png)

```xml
<StackPanel>
    <TextBlock>Appears chracter after 1 second.</TextBlock>
    <!-- ReactiveProperty binding ".Value" -->
    <TextBox Text="{Binding InputText.Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    <TextBlock Text="{Binding DisplayText.Value}" />
    <Button Command="{Binding ReplaceTextCommand}">Is TextBox empty or not?</Button>
</StackPanel>
```

```cs
// ReactiveProperty and ReactiveCommand simple example.
public class ReactivePropertyBasicsViewModel
{
    public ReactiveProperty<string> InputText { get; private set; }
    public ReactiveProperty<string> DisplayText { get; private set; }
    public ReactiveCommand ReplaceTextCommand { get; private set; }

    public ReactivePropertyBasicsViewModel()
    {
        // mode is Flags. (default is all)
        // DistinctUntilChanged is no push value if next value is same as current
        // RaiseLatestValueOnSubscribe is push value when subscribed
        var allMode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe;

        // binding value from UI Control
        // if no set initialValue then initialValue is default(T). int:0, string:null...
        InputText = new ReactiveProperty<string>(initialValue: "", mode: allMode);

        // send value to UI Control
        DisplayText = InputText
            .Select(s => s.ToUpper())       // rx query1
            .Delay(TimeSpan.FromSeconds(1)) // rx query2
            .ToReactiveProperty();          // convert to ReactiveProperty

        ReplaceTextCommand = InputText
            .Select(s => !string.IsNullOrEmpty(s))   // condition sequence of CanExecute
            .ToReactiveCommand(); // convert to ReactiveCommand

        // ReactiveCommand's Subscribe is set ICommand's Execute
        // ReactiveProperty.Value set is push(& set) value
        ReplaceTextCommand.Subscribe(_ => InputText.Value = "Hello, ReactiveProperty!");
    }
}
```

ReactiveProperty's direct binding is very simple and clear syntax.

## Event to ReactiveProperty

![Basics](Images/eventtoreactive.PNG)

```xml
<Window.Resources>
    <vm:Converters x:Key="Converters" />
</Window.Resources>
<Grid>
	<!-- Use Blend SDK's Interaction Trigger -->
	<!-- Event binding to ReactiveProperty -->
	<i:Interaction.Triggers>
		<i:EventTrigger EventName="MouseMove">
			<!-- ConvertBack function is Func<object, object>-->
			<r:EventToReactive ReactiveProperty="{Binding MouseMove}" 
				Converter="{Binding MouseEventToPoint, Source={StaticResource Converters}}" />
		</i:EventTrigger>
		<i:EventTrigger EventName="MouseDown">
			<!-- direct event bind -->
			<r:EventToReactive ReactiveProperty="{Binding MouseDown}" IgnoreEventArgs="True" />
		</i:EventTrigger>
		<i:EventTrigger EventName="MouseEnter">
			<!-- IgnoreEventArgs = true send Unit -->
			<r:EventToReactive ReactiveProperty="{Binding MouseEnter}" IgnoreEventArgs="true" />
		</i:EventTrigger>
	</i:Interaction.Triggers>
	<TextBlock Text="{Binding CurrentPoint.Value}" />
	<TextBlock Text="{Binding Entered.Value}" Margin="0,100,0,0" />
</Grid>
```

```cs
public class EventToReactiveViewModel
{
    // binding from UI, event direct bind
    public ReactiveProperty<Unit> MouseDown { get; private set; }
    // binding from UI, event with converter
    public ReactiveProperty<Tuple<int, int>> MouseMove { get; private set; }
    // binding from UI, IgnoreEventArgs = true
    public ReactiveProperty<Unit> MouseEnter { get; private set; }

    public ReactiveProperty<string> CurrentPoint { get; private set; }
    public ReactiveProperty<string> Entered { get; private set; }

    public ReactiveProperty<string> AlertMessage { get; private set; }

    public EventToReactiveViewModel()
    {
        // mode off RaiseLatestValueOnSubscribe, because initialValue is null.
        // mode off DistinctUntilChanged, because if Unit no send any values.
        var none = ReactivePropertyMode.None;

        MouseMove = new ReactiveProperty<Tuple<int, int>>(mode: none);
        MouseDown = new ReactiveProperty<Unit>(mode: none);
        MouseEnter = new ReactiveProperty<Unit>(mode: none);

        CurrentPoint = MouseMove
            .Select(p => string.Format("X:{0} Y:{1}", p.Item1, p.Item2))
            .ToReactiveProperty();

        Entered = MouseEnter
            .Select(_ => Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)))
            .Switch()
            .Select(x => "entered:" + x + "sec")
            .ToReactiveProperty();

        this.AlertMessage = MouseDown.Select(_ => "MouseDown!").ToReactiveProperty(mode: none);
    }
}

// EventToReactive convert functions
// Converter/IgnoreEventArgs is useful for unit testings
// for example, MouseMove.Value = new Point(10, 10) is simulate MouseMove
// MouseEnter.Value = new Unit() is simulate raise MouseEnter event
public class Converters
{
    public Func<object, object> MouseEventToPoint { get; private set; }

    public Converters()
    {
        MouseEventToPoint = ev =>
            {
                var position = ((dynamic)ev).GetPosition(null);
                return Tuple.Create((int)position.X, (int)position.Y);
            };
    }
}
```

## Validation

![Basics](Images/validation.PNG)

```cs
public class ValidationViewModel
{
    [Required(ErrorMessage = "Required")]
    [Range(0, 100, ErrorMessage = "Range 0...100")]
    public ReactiveProperty<string> ValidationAttr { get; private set; }
    public ReactiveProperty<string> ValidationData { get; private set; }
    [StringLength(5, ErrorMessage = "Length < 5")]
    public ReactiveProperty<string> ValidationBoth { get; private set; }
    public ReactiveProperty<string> ErrorInfo { get; private set; }
    public ReactiveCommand NextCommand { get; private set; }
    public ReactiveProperty<string> AlertMessage { get; private set; }

    public ValidationViewModel()
    {
        // DataAnnotation Attribute, call SetValidateAttribute and select self property
        // Note:error result dispatch to IDataErrorInfo, not exception.
        //      therefore, XAML is ValidatesOnDataErrors=True
        ValidationAttr = new ReactiveProperty<string>()
            .SetValidateAttribute(() => ValidationAttr);

        // IDataErrorInfo, call SetValidateError and set validate condition
        // null is success(have no error), string is error message
        ValidationData = new ReactiveProperty<string>()
            .SetValidateNotifyError((string s) => 
                string.IsNullOrEmpty(s) ? 
                    "required" :
                    s.Cast<char>().All(Char.IsUpper) ? 
                        null : 
                        "not all uppercase");

        // Can set both validation
        ValidationBoth = new ReactiveProperty<string>()
            .SetValidateAttribute(() => ValidationBoth)
            .SetValidateNotifyError(s => string.IsNullOrEmpty(s) ?
                "required" :
                s.Cast<char>().All(Char.IsLower) ?
                    null :
                    "not all lowercase")
            .SetValidateNotifyError(async x =>
            {
                await Task.Delay(2000);
                if (x == null)          return null;
                if (x.Contains("a"))    return "'a' shouldn't be contained";
                return null;
            })
            .SetValidateNotifyError(xs =>
            {
                return  xs
						.Throttle(TimeSpan.FromMilliseconds(500))
                        .Select(x =>
                        {
                            if (x == null)          return null;
                            if (x.Contains("b"))    return "'b' shouldn't be contained";
                            return null;
                        });
            });


        // Validation result is pushed to ObserveErrorChanged
        var errors = Observable.Merge(
            ValidationAttr.ObserveErrorChanged,
            ValidationData.ObserveErrorChanged,
            ValidationBoth.ObserveErrorChanged);

        // Use OfType, choose error source
        ErrorInfo   = errors
                    .Select(x => x == null ? null : x.OfType<string>().FirstOrDefault())
                    .ToReactiveProperty();

        // Validation is view initialized not run in default.
        // If want to validate on view initialize,
        // use ReactivePropertyMode.RaiseLatestValueOnSubscribe to ReactiveProperty
        // that mode is validate values on initialize.
        NextCommand = ValidationAttr.ObserveErrorChanged
            .CombineLatest(
                ValidationData.ObserveErrorChanged,
                ValidationBoth.ObserveErrorChanged, 
                (a, b, c) => new[] { a, b, c }.All(x => x == null))
            .ToReactiveCommand(initialValue: false);
        this.AlertMessage = this.NextCommand.Select(_ => "Can go to next!")
            .ToReactiveProperty(mode: ReactivePropertyMode.None);
    }
}
```

```xml
<Grid>
	<StackPanel>
		<Grid>
			<Grid.ColumnDefinitions>
				<ColumnDefinition />
				<ColumnDefinition />
			</Grid.ColumnDefinitions>
			<Grid.RowDefinitions>
				<RowDefinition />
				<RowDefinition />
				<RowDefinition />
			</Grid.RowDefinitions>
			<!-- all validation set ValidatesOnDataErrors=True -->
			<TextBlock Grid.Column="0" Grid.Row="0"><Run Text="Range 0..100"/></TextBlock>
			<TextBox  Grid.Column="1" Grid.Row="0"
				Text="{Binding ValidationAttr.Value, ValidatesOnDataErrors=True}"/>
			<TextBlock Grid.Column="0" Grid.Row="1"><Run Text="All Upper"/></TextBlock>
			<TextBox  Grid.Column="1" Grid.Row="1"
				Text="{Binding ValidationData.Value, ValidatesOnDataErrors=True}"/>
			<TextBlock Grid.Column="0" Grid.Row="2"><Run Text="All Lower, Length 5"/></TextBlock>
			<TextBox  Grid.Column="1" Grid.Row="2"
				Text="{Binding ValidationBoth.Value, ValidatesOnDataErrors=True, UpdateSourceTrigger=PropertyChanged}"/>
		</Grid>
		<TextBlock Text="{Binding ErrorInfo.Value}" Foreground="Red" />
		<Button Command="{Binding NextCommand}" Content="Next"/>
	</StackPanel>
	<Border x:Name="border" BorderBrush="Black" BorderThickness="1" Background="#FFF9A4A4" Visibility="Hidden">
		<TextBlock Margin="0" TextWrapping="Wrap" Text="{Binding AlertMessage.Value}" HorizontalAlignment="Center" VerticalAlignment="Center"/>
	</Border>
</Grid>
```

Supporting three types validation. more details, see Sample/Validation.

## Synchronize existing models

models.
```cs
public class ObservableObject : INotifyPropertyChanged
{
    private string name;
    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Name"));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged = (_, __) => { };
}

public class PlainObject
{
    public string Name { get; set; }
}
```

Synchronize existing models.
```cs
// Synchroinize exsiting models.
public class SynchronizeObjectViewModel
{
    public ReactiveProperty<string> TwoWay { get; private set; }
    public ReactiveProperty<string> OneWay { get; private set; }
    public ReactiveProperty<string> OneWayToSource { get; private set; }
    public ReactiveCommand CheckCommand { get; private set; }
    public ReactiveProperty<string> AlertMessage { get; private set; }

    public SynchronizeObjectViewModel()
    {
        var inpc = new ObservableObject { Name = "Bill" };
        var poco = new PlainObject { Name = "Steve" };

        // TwoWay synchronize
        TwoWay = inpc.ToReactivePropertyAsSynchronized(x => x.Name);

        // OneWay synchronize
        OneWay = inpc.ObserveProperty(x => x.Name).ToReactiveProperty();

        // OneWayToSource synchronize
        OneWayToSource = ReactiveProperty.FromObject(poco, x => x.Name);

        // synchronization check
        CheckCommand = new ReactiveCommand();
        this.AlertMessage = CheckCommand.Select(_ => 
            "INPC Name:" + inpc.Name + Environment.NewLine
          + "POCO Name:" + poco.Name)
          .ToReactiveProperty(mode: ReactivePropertyMode.None);
    }
}
```

using with existing MVVM Framework, auto generated models, etc.
 more details see sample/SynchronizeObject.

## Serialization

```cs
// a ViewModel
public class SerializationViewModel
{
    // no attribute, simply serialize/deserialize
    public ReactiveProperty<bool> IsChecked { get; private set; }
    [IgnoreDataMember] // ignore serialize target
    public ReactiveProperty<int> SelectedIndex { get; private set; }
    [DataMember(Order = 3)] // deserialize order
    public ReactiveProperty<int> SliderPosition { get; private set; }
}

// case Windows Phone 7 TombStone
private SerializationViewModel viewmodel = new SerializationViewModel();
private string viewmodelData = null;

protected override void OnNavigatingFrom(System.Windows.Navigation.NavigationEventArgs e)
{
    viewmodelData = SerializeHelper.PackReactivePropertyValue(viewmodel);
}

protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
{
    SerializeHelper.UnpackReactivePropertyValue(viewmodel, viewmodelData);
}
```

Supporting serialize ignore and deserialize order by DataAnnotations.
 more details, see Sample/Serialization.

## Author info

Yoshifumi Kawai a.k.a. neuecc is software developer in Tokyo, Japan.
Awarded Microsoft MVP for Visual C# since April, 2011.

Takaaki Suzuki a.k.a. xin9le software devleoper in Fukui, Japan.
Awarded Microsoft MVP for Visual C# since 2011.

Kazuki Ota a.k.a. okazuki software developer in Tokyo, Japan.
Awarded Microsoft MVP for Windows Platform Developer since July, 2011.



