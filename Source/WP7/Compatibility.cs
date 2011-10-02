using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Codeplex.Reactive
{
    // Compatible with .NET - WP7 & Rx-Main - Phone.Reactive
    internal static class Compatibility
    {
        public static bool HasFlag(this Enum self, Enum flags)
        {
            if (self.GetType() != flags.GetType()) throw new ArgumentException("not sampe type");

            var sval = Convert.ToUInt64(self);
            var fval = Convert.ToUInt64(flags);

            return (sval & fval) == fval;
        }
    }
}
