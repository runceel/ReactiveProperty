using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Reactive;
using Codeplex.Reactive;
using Codeplex.Reactive.Serialization;
using System.Runtime.Serialization;

namespace WP7.ViewModels
{
    public class SerializationViewModel
    {
        // [IgnoreDataMember] ignore serialize
        public ReactiveProperty<bool> IsChedked { get; private set; }
        [DataMember(Order = 3)] // deserialize order
        public ReactiveProperty<string> Text { get; private set; }
        public ReactiveProperty<int> SliderPosition { get; private set; }
        public ReactiveCollection<long> Items { get; private set; }
        public ReactiveCommand Serialize { get; private set; }
        public ReactiveCommand Deserialize { get; private set; }

        public SerializationViewModel()
        {
            IsChedked = new ReactiveProperty<bool>();
            Text = new ReactiveProperty<string>();
            SliderPosition = new ReactiveProperty<int>();

            var serializedString = new ReactiveProperty<string>(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            Serialize = serializedString.Select(x => x == null).ToReactiveCommand();
            Deserialize = serializedString.Select(x => x != null).ToReactiveCommand();

            // Click Serialize Button
            Serialize.Subscribe(_ =>
            {
                // Serialize ViewModel's all ReactiveProperty Values.
                // return value is string that Serialize by DataContractSerializer.
                serializedString.Value = SerializeHelper.PackReactivePropertyValue(this); // this = ViewModel
            });

            // Click Deserialize Button
            Deserialize.Subscribe(_ =>
            {
                // Deserialize to target ViewModel.
                // Deseirlization order is same as DataContract.
                // Can control DataMemberAttribute's Order Property.
                // more info see http://msdn.microsoft.com/en-us/library/ms729813.aspx
                SerializeHelper.UnpackReactivePropertyValue(this, serializedString.Value);

                serializedString.Value = null; // push to command canExecute
            });
        }
    }
}