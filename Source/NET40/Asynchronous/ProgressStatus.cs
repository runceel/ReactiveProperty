using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeplex.Reactive.Asynchronous
{
    public class ProgressStatus
    {
        /// <summary>Represents unknown length. This equals -1.</summary>
        public const int Unknown = -1;

        /// <summary>Current length of status.</summary>
        public long CurrentLength { get; private set; }
        /// <summary>Total(Max) length of status.</summary>
        public long TotalLength { get; private set; }

        /// <summary>Current/Total</summary>
        public int Percentage
        {
            get
            {
                return (TotalLength <= 0 || CurrentLength <= 0)
                    ? 0
                    : (int)(((double)CurrentLength / (double)TotalLength) * 100);
            }
        }

        /// <summary>
        /// Represents progress status.
        /// </summary>
        /// <param name="currentLength">Current length of status.</param>
        /// <param name="totalLength">Total length of status.</param>
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