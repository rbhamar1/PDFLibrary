using System.Collections.Generic;
using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    public sealed class PdfTextRow
    {
        public PdfTextRow AddText(PdfTextNodeBase textNode)
        {
            Guard.Argument(textNode, nameof(textNode))
                 .NotNull();

            _textNodes.Add(textNode);

            return this;
        }

        public PdfTextRow AddText(string text, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            var node = new PdfStaticTextNode(text, null, alignment, placement);
            _textNodes.Add(node);

            return this;
        }

        public PdfTextRow AddText(string text, PdfFont font, uint charOffset, uint charWidth,
                                  TextAlignment alignment = TextAlignment.Start)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            Guard.Argument(font, nameof(font))
                 .NotNull();

            var node = new PdfStaticTextNode(text,
                                             font,
                                             alignment,
                                             CreateSimplePlacement(font, charOffset, charWidth));

            _textNodes.Add(node);

            return this;
        }

        public PdfTextRow AddUnderline(PdfFont font, uint charOffset, uint charWidth)
        {
            var node = new PdfStaticTextNode(new string('=', (int) charWidth),
                                             font,
                                             TextAlignment.Start,
                                             CreateSimplePlacement(font, charOffset, charWidth));

            _textNodes.Add(node);

            return this;
        }

        public PdfTextRow AddSingleDottedUnderline(PdfFont font, uint charOffset, uint charWidth)
        {
            var node = new PdfStaticTextNode(new string('-', (int) charWidth),
                                             font,
                                             TextAlignment.Start,
                                             CreateSimplePlacement(font, charOffset, charWidth));

            _textNodes.Add(node);

            return this;
        }

        public PdfTextRow AddAsterikLine(PdfFont font, uint charOffset, uint charWidth)
        {
            var node = new PdfStaticTextNode(new string('*', (int) charWidth),
                                             font,
                                             TextAlignment.Start,
                                             CreateSimplePlacement(font, charOffset, charWidth));

            _textNodes.Add(node);

            return this;
        }

        public PdfTextRow AddPromptText(string text, PdfTextPlacement placement = null)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            var promptText = text + ": ";
            AddText(promptText, TextAlignment.Start, placement);

            return this;
        }

        public PdfTextRow AddNumber(uint number, PdfFont font, uint charOffset, uint charWidth) => AddText(number.ToString(), font, font, TextAlignment.End, charOffset, charWidth);

        public PdfTextRow AddNumber(int? number, PdfFont font, uint charOffset, uint charWidth) => AddText(number.ToString(), font, font, TextAlignment.End, charOffset, charWidth);

        public PdfTextRow AddCurrency(decimal currency, bool printAsNegative, uint decimalPlaces, bool useCurrencySymbol,
                                      PdfFont font, uint charOffset, uint charWidth, TextAlignment textAlignment = TextAlignment.End) =>
            AddCurrency(currency, printAsNegative, decimalPlaces, useCurrencySymbol, font, font, charOffset,
                        charWidth, textAlignment);

        public PdfTextRow AddCurrency(decimal currency, bool printAsNegative, uint decimalPlaces, bool useCurrencySymbol,
                                      PdfFont printFont, PdfFont measureFont, uint charOffset, uint charWidth,
                                      TextAlignment textAlignment = TextAlignment.End)
        {
            var text = FormatCurrency(currency, decimalPlaces, useCurrencySymbol, charWidth);

            if (printAsNegative)
            {
                // Note: we manually add a minus sign because we want zero to print with a minus sign
                text = '-' + text;
            }

            return AddText(text, printFont, measureFont, textAlignment, charOffset, charWidth);
        }

        public PdfTextRow AddText(string text, PdfFont font, TextAlignment textAlignment,
                                  uint charOffset, uint charWidth) =>
            AddText(text, font, font, textAlignment, charOffset, charWidth);

        public PdfTextRow AddText(string text, PdfFont printFont, PdfFont measureFont, TextAlignment textAlignment,
                                  uint charOffset, uint charWidth)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            Guard.Argument(printFont, nameof(printFont))
                 .NotNull();

            Guard.Argument(measureFont, nameof(measureFont))
                 .NotNull();

            var node = new PdfStaticTextNode(text, printFont, textAlignment, CreateSimplePlacement(measureFont, charOffset, charWidth));

            _textNodes.Add(node);

            return this;
        }

        public IEnumerable<PdfTextNodeBase> TextNodes => _textNodes;

        private static string FormatCurrency(decimal input, uint decimalPlaces, bool useCurrencySymbol, uint maxCharWidth)
        {
            if (useCurrencySymbol)
            {
                return input.ToString($"C{decimalPlaces}");
            }

            var text = input.ToString($"N{decimalPlaces}");

            if (text.Length > maxCharWidth)
            {
                // Note: spacing is very tight on some printed output. If the formatted text for a currency value
                // is too large with the thousands separator included, then format it without the thousands separator.
                text = input.ToString($"F{decimalPlaces}");
            }

            return text;
        }

        private static PdfTextPlacement CreateSimplePlacement(PdfFont font, uint charOffset, uint charWidth) => new PdfTextPlacement(charOffset * font.CharWidthMm, 0, charWidth * font.CharWidthMm);

        private readonly List<PdfTextNodeBase> _textNodes = new List<PdfTextNodeBase>();
    }
}