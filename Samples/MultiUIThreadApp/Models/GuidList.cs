using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MultiUIThreadApp.Models
{
    public static class GuidList
    {
        public static ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();
        public static void AddNewGuid() => Items.Add(new Item());
    }
}
