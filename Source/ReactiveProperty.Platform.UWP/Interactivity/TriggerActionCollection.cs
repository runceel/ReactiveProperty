using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xaml.Interactivity;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Represents a collection of TriggerActions with a shared <see cref="TriggerActionCollection.AssociatedObject"/>
    /// </summary>
    public class TriggerActionCollection : DependencyObjectCollection
    {
        private readonly List<TriggerAction> _oldCollection = new List<TriggerAction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerActionCollection"/> class.
        /// </summary>
        public TriggerActionCollection()
        {
            VectorChanged += TriggerActionCollection_VectorChanged;
        }

        /// <summary>
        /// Gets the <see cref="Windows.UI.Xaml.DependencyObject"/> to which the <see cref="TriggerAction"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        /// <summary>
        /// Attaches the collection of TriggerActions to the specified <see cref="Windows.UI.Xaml.DependencyObject"/>
        /// </summary>
        /// <param name="associatedObject"></param>
        public void Attach(DependencyObject associatedObject)
        {
            if (AssociatedObject == associatedObject)
            {
                return;
            }

            if (associatedObject == null)
            {
                throw new ArgumentNullException("associatedObject");
            }

            AssociatedObject = associatedObject;

            foreach (TriggerAction triggerAction in this)
            {
                triggerAction.Attach(AssociatedObject);
            }
        }

        /// <summary>
        /// Detaches the collection of behaviors from the <see cref="TriggerActionCollection.AssociatedObject"/>.
        /// </summary>
        public void Detach()
        {
            foreach (DependencyObject item in this)
            {
                IBehavior behaviorItem = item as IBehavior;
                if (behaviorItem.AssociatedObject != null)
                {
                    behaviorItem.Detach();
                }
            }

            AssociatedObject = null;
            _oldCollection.Clear();
        }

        private void TriggerActionCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
        {
            if (eventArgs.CollectionChange == CollectionChange.Reset)
            {
                foreach (IBehavior behavior in _oldCollection)
                {
                    if (behavior.AssociatedObject != null)
                    {
                        behavior.Detach();
                    }
                }

                _oldCollection.Clear();

                foreach (DependencyObject newItem in this)
                {
                    _oldCollection.Add(VerifiedAttach(newItem));
                }

                return;
            }

            int eventIndex = (int)eventArgs.Index;
            DependencyObject changedItem = this[eventIndex];

            switch (eventArgs.CollectionChange)
            {
                case CollectionChange.ItemInserted:
                    _oldCollection.Insert(eventIndex, VerifiedAttach(changedItem));

                    break;

                case CollectionChange.ItemChanged:
                    IBehavior oldItem = _oldCollection[eventIndex];
                    if (oldItem.AssociatedObject != null)
                    {
                        oldItem.Detach();
                    }

                    _oldCollection[eventIndex] = VerifiedAttach(changedItem);

                    break;

                case CollectionChange.ItemRemoved:
                    oldItem = _oldCollection[eventIndex];
                    if (oldItem.AssociatedObject != null)
                    {
                        oldItem.Detach();
                    }

                    _oldCollection.RemoveAt(eventIndex);
                    break;

                default:
                    Debug.Assert(false, "Unsupported collection operation attempted.");
                    break;
            }
        }

        private TriggerAction VerifiedAttach(DependencyObject item)
        {
            if (!(item is TriggerAction triggerAction))
            {
                throw new InvalidOperationException("Only TriggerAction types are supported in a TriggerActionCollection.");
            }

            if (_oldCollection.Contains(triggerAction))
            {
                throw new InvalidOperationException("Cannot add an instance of a TriggerAction to a TriggerActionCollection more than once.");
            }

            if (AssociatedObject != null)
            {
                triggerAction.Attach(AssociatedObject);
            }

            return triggerAction;
        }
    }
}
