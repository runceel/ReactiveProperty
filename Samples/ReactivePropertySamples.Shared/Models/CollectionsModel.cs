using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ReactivePropertySamples.Models
{
    public class CollectionsModel
    {
        public ObservableCollection<Guid> Guids { get; } = new ObservableCollection<Guid>();
    }
}
