using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace PDFLibrary
{
    public sealed class Pdf
    {
        public Pdf(IEnumerable<byte> data) =>
            _data = Guard.Argument(data, nameof(data))
                         .NotNull()
                         .NotEmpty()
                         .Value.ToArray();

        public byte[] GetBytes() => _data.ToArray();

        public uint DataSize => (uint) _data.Length;

        private readonly byte[] _data;
    }
}