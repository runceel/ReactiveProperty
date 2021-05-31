using System;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// A behavior that listens for a specified event on its source and executes its actions when that event is fired.
    /// </summary>
    public sealed class EventTriggerBehavior : Trigger<FrameworkElement>
    {
        /// <summary>
        /// Identifies the <seealso cref="EventName"/> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(
            nameof(EventName),
            typeof(string),
            typeof(EventTriggerBehavior),
            new PropertyMetadata("Loaded", new PropertyChangedCallback(EventTriggerBehavior.OnEventNameChanged)));

        /// <summary>
        /// Identifies the <seealso cref="SourceObject"/> dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register(
            nameof(SourceObject),
            typeof(object),
            typeof(EventTriggerBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(EventTriggerBehavior.OnSourceObjectChanged)));

        private object _resolvedSource;
        private Delegate _eventHandler;
        private bool _isLoadedEventRegistered;
        private bool _isWindowsRuntimeEvent;
        private Func<Delegate, EventRegistrationToken> _addEventHandlerMethod;
        private Action<EventRegistrationToken> _removeEventHandlerMethod;

        /// <summary>
        /// Gets or sets the name of the event to listen for. This is a dependency property.
        /// </summary>
        public string EventName
        {
            get
            {
                return (string)GetValue(EventTriggerBehavior.EventNameProperty);
            }

            set
            {
                SetValue(EventTriggerBehavior.EventNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source object from which this behavior listens for events.
        /// If <seealso cref="SourceObject"/> is not set, the source will default to <seealso cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>. This is a dependency property.
        /// </summary>
        public object SourceObject
        {
            get
            {
                return (object)GetValue(EventTriggerBehavior.SourceObjectProperty);
            }

            set
            {
                SetValue(EventTriggerBehavior.SourceObjectProperty, value);
            }
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            Actions.Attach(AssociatedObject);
            SetResolvedSource(ComputeResolvedSource());
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="Microsoft.Xaml.Interactivity.Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            Actions.Detach();
            SetResolvedSource(null);
        }

        private void SetResolvedSource(object newSource)
        {
            if (AssociatedObject == null || _resolvedSource == newSource)
            {
                return;
            }

            if (_resolvedSource != null)
            {
                UnregisterEvent(EventName);
            }

            _resolvedSource = newSource;

            if (_resolvedSource != null)
            {
                RegisterEvent(EventName);
            }
        }

        private object ComputeResolvedSource()
        {
            // If the SourceObject property is set at all, we want to use it. It is possible that it is data
            // bound and bindings haven't been evaluated yet. Plus, this makes the API more predictable.
            if (ReadLocalValue(EventTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue)
            {
                return SourceObject;
            }

            return AssociatedObject;
        }

        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (eventName != "Loaded")
            {
                Type sourceObjectType = _resolvedSource.GetType();
                EventInfo info = sourceObjectType.GetRuntimeEvent(eventName);
                if (info == null)
                {
                    return;
                }

                MethodInfo methodInfo = typeof(EventTriggerBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");
                _eventHandler = methodInfo.CreateDelegate(info.EventHandlerType, this);

                _isWindowsRuntimeEvent = EventTriggerBehavior.IsWindowsRuntimeEvent(info);
                if (_isWindowsRuntimeEvent)
                {
                    _addEventHandlerMethod = add => (EventRegistrationToken)info.AddMethod.Invoke(_resolvedSource, new object[] { add });
                    _removeEventHandlerMethod = token => info.RemoveMethod.Invoke(_resolvedSource, new object[] { token });

                    WindowsRuntimeMarshal.AddEventHandler(_addEventHandlerMethod, _removeEventHandlerMethod, _eventHandler);
                }
                else
                {
                    info.AddEventHandler(_resolvedSource, _eventHandler);
                }
            }
            else if (!_isLoadedEventRegistered)
            {
                FrameworkElement element = _resolvedSource as FrameworkElement;
                if (element != null && !EventTriggerBehavior.IsElementLoaded(element))
                {
                    _isLoadedEventRegistered = true;
                    element.Loaded += OnEvent;
                }
            }
        }

        private void UnregisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (eventName != "Loaded")
            {
                if (_eventHandler == null)
                {
                    return;
                }

                EventInfo info = _resolvedSource.GetType().GetRuntimeEvent(eventName);
                if (_isWindowsRuntimeEvent)
                {
                    WindowsRuntimeMarshal.RemoveEventHandler(_removeEventHandlerMethod, _eventHandler);
                }
                else
                {
                    info.RemoveEventHandler(_resolvedSource, _eventHandler);
                }

                _eventHandler = null;
            }
            else if (_isLoadedEventRegistered)
            {
                _isLoadedEventRegistered = false;
                FrameworkElement element = (FrameworkElement)_resolvedSource;
                element.Loaded -= OnEvent;
            }
        }

        private void OnEvent(object sender, object eventArgs)
        {
            if (Actions == null)
            {
                return;
            }

            foreach (DependencyObject item in Actions)
            {
                IAction action = item as IAction;
                action.Execute(sender, eventArgs);
            }
        }

        private static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            EventTriggerBehavior behavior = (EventTriggerBehavior)dependencyObject;
            behavior.SetResolvedSource(behavior.ComputeResolvedSource());
        }

        private static void OnEventNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            EventTriggerBehavior behavior = (EventTriggerBehavior)dependencyObject;
            if (behavior.AssociatedObject == null || behavior._resolvedSource == null)
            {
                return;
            }

            string oldEventName = (string)args.OldValue;
            string newEventName = (string)args.NewValue;

            behavior.UnregisterEvent(oldEventName);
            behavior.RegisterEvent(newEventName);
        }

        internal static bool IsElementLoaded(FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }

            UIElement rootVisual = Window.Current.Content;
            DependencyObject parent = element.Parent;
            if (parent == null)
            {
                // If the element is the child of a ControlTemplate it will have a null parent even when it is loaded.
                // To catch that scenario, also check it's parent in the visual tree.
                parent = VisualTreeHelper.GetParent(element);
            }

            return (parent != null || (rootVisual != null && element == rootVisual));
        }

        private static bool IsWindowsRuntimeEvent(EventInfo eventInfo)
        {
            return eventInfo != null &&
                EventTriggerBehavior.IsWindowsRuntimeType(eventInfo.EventHandlerType) &&
                EventTriggerBehavior.IsWindowsRuntimeType(eventInfo.DeclaringType);
        }

        private static bool IsWindowsRuntimeType(Type type)
        {
            if (type != null)
            {
                return type.AssemblyQualifiedName.EndsWith("ContentType=WindowsRuntime", StringComparison.Ordinal);
            }

            return false;
        }
    }
}
