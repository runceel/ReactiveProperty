using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeplex.Reactive.Asynchronous
{
    public class ProgressStatus
    {
        public const int Unknown = -1;

        public long CurrentLength { get; private set; }
        public long TotalLength { get; private set; }

        public int Percentage
        {
            get
            {
                return (TotalLength <= 0 || CurrentLength <= 0)
                    ? 0
                    : (int)(((double)CurrentLength / (double)TotalLength) * 100);
            }
        }

        public ProgressStatus(long currentLength, long totalLength)
        {
            CurrentLength = currentLength;
            TotalLength = totalLength;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1} - {2}%", CurrentLength, TotalLength, Percentage);
        }
    }
}