using System.Collections.Generic;
using System.Linq;
using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Represents an aggregation of text nodes that are considered as a single unit.
    /// </summary>
    public sealed class PdfTextRegion
    {
        public PdfTextRegion()
            : this(Point.Zero) {}

        public PdfTextRegion(Point origin)
        {
            Guard.Argument(origin.X, nameof(origin))
                 .NotNegative();

            Guard.Argument(origin.Y, nameof(origin))
                 .NotNegative();

            Origin = origin;
        }

        public PdfTextRegion AddBlankRow()
        {
            _rows.Add(new PdfTextRow()
                         .AddText(string.Empty));

            return this;
        }

        public PdfTextRegion AddBlankRow(PdfFont font)
        {
            Guard.Argument(font, nameof(font))
                 .NotNull();

            _rows.Add(new PdfTextRow()
                         .AddText(new PdfStaticTextNode(font)));

            return this;
        }

        public PdfTextRegion AddRow(string text, PdfFont font, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            Guard.Argument(font, nameof(font))
                 .NotNull();

            _rows.Add(new PdfTextRow()
                         .AddText(new PdfStaticTextNode(text, font, alignment, placement)));

            return this;
        }

        public PdfTextRegion AddRow(PdfTextNodeBase textNode)
        {
            Guard.Argument(textNode, nameof(textNode))
                 .NotNull();

            _rows.Add(new PdfTextRow().AddText(textNode));

            return this;
        }

        public PdfTextRegion AddRows(IEnumerable<string> rows, PdfFont font, TextAlignment textAlignment = TextAlignment.Start)
        {
            var rowArray = Guard.Argument(rows, nameof(rows))
                                .NotNull()
                                .Value.ToArray();

            foreach (var row in rowArray)
            {
                AddRow(row, font, textAlignment);
            }

            return this;
        }

        public PdfTextRegion AddRow(PdfTextRow row)
        {
            Guard.Argument(row, nameof(row))
                 .NotNull();

            _rows.Add(row);

            return this;
        }

        public Point Origin { get; }

        public IEnumerable<PdfTextRow> Rows => _rows.AsReadOnly();

        private readonly List<PdfTextRow> _rows = new List<PdfTextRow>();
    }
}