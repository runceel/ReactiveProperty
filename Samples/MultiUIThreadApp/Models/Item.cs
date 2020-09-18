using System;
using System.Collections.Generic;
using System.Text;

namespace MultiUIThreadApp.Models
{
    public class Item
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public override string ToString() => Guid.ToString();
    }
}
