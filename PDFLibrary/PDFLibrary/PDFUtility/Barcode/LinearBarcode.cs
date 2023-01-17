using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace PDFLibrary.PDFUtility.Barcode
{
    public sealed class LinearBarcode : IReadOnlyList<bool>
    {
        public LinearBarcode(IEnumerable<bool> barFlags) =>
            _barFlags = Guard.Argument(barFlags, nameof(barFlags))
                             .NotNull()
                             .NotEmpty()
                             .Value
                             .ToList();

        public int Count => _barFlags.Count;

        public bool this[int index]
        {
            get
            {
                Guard.Argument(index, nameof(index))
                     .NotNegative()
                     .LessThan(_barFlags.Count);

                return _barFlags[index];
            }
        }

        public IEnumerator<bool> GetEnumerator() => _barFlags.GetEnumerator();

        // Note: only used for debugging
        public override string ToString()
        {
            var text = _barFlags.Aggregate(string.Empty, (current, flag) => current + (flag ? "|" : "."));

            return text;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly List<bool> _barFlags;
    }
}