namespace Codeplex.Reactive.Extensions
{
    /// <summary>Value pair of OldItem and NewItem.</summary>
    public class OldNewPair<T>
    {
        public T OldItem { get; private set; }
        public T NewItem { get; private set; }

        public OldNewPair(T oldItem, T newItem)
        {
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }

        public override string ToString()
        {
            return "{ Old = " + OldItem + ", New = " + NewItem + " }";
        }
    }
}