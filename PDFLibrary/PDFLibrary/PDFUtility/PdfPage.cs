using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a page of PDF output.
    /// </summary>
    public sealed class PdfPage
    {
        public void AddBlock(IPdfBlock block)
        {
            Guard.Argument(block, nameof(block))
                 .NotNull();

            Blocks.Add(block);
        }

        public double ContentHeight
        {
            get
            {
                Guard.Operation(Blocks.Any(), "Page contains no blocks");

                return Blocks.Max(b => b.Bounds.Bottom);
            }
        }

        public List<IPdfBlock> Blocks { get; } = new List<IPdfBlock>();
    }
}