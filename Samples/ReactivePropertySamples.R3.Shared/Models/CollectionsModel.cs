using System;
using System.Collections.ObjectModel;

namespace ReactivePropertySamples.Migrated.Models
{
    public class CollectionsModel
    {
        public ObservableCollection<Guid> Guids { get; } = new ObservableCollection<Guid>();
    }
}
