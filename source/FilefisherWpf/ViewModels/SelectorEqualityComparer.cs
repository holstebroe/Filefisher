using System;
using System.Collections.Generic;

namespace FilefisherWpf.ViewModels
{
    public class SelectorEqualityComparer<TA, TB> : IEqualityComparer<TA>
    {
        private readonly Func<TA, TB> selector;

        public SelectorEqualityComparer(Func<TA, TB> selector)
        {
            this.selector = selector;
        }

        public bool Equals(TA x, TA y)
        {
            if (object.Equals(x, default(TA)) && object.Equals(y, default(TA))) return true;
            if (object.Equals(x, default(TA))) return false;
            if (object.Equals(y, default(TA))) return false;

            return EqualityComparer<TB>.Default.Equals(selector(x), selector(y));
        }

        public int GetHashCode(TA obj)
        {
            if (Equals(obj, default)) return 0;
            var value = selector(obj);
            if (Equals(value, default(TB))) return 0;
            return value.GetHashCode();
        }
    }
}