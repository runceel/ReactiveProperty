using System;
using System.Collections.Generic;
using System.Text;

namespace Reactive.Todo.Main.Events
{
    public class TargetViewChangedEvent
    {
        public TargetViewChangedEvent(TargetViewType targetViewType)
        {
            TargetViewType = targetViewType;
        }

        public TargetViewType TargetViewType { get; }
    }

    public enum TargetViewType
    {
        All,
        Completed,
        Active,
    }
}
