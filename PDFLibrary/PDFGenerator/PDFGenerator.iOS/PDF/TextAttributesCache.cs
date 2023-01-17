using System.Collections.Generic;
using Dawn;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Pdfgenerator.iOS.Pdf
{
    public sealed class TextAttributesCache
    {
        public NSDictionary GetTextAttributes(UIFont font, TextAlignment alignment)
        {
            Guard.Argument(font, nameof(font))
                 .NotNull();

            Guard.Argument(alignment, nameof(alignment))
                 .Defined();

            if (!_textAttributesMap.TryGetValue((font, alignment), out var textAttributes))
            {
                var nativeAlignment = GetNativeAlignment(alignment);

                var style = new NSMutableParagraphStyle
                                {
                                    Alignment = nativeAlignment,

                                    // TODO, for text that is intended to be single-line, this is probably the best choice
                                    // for text that is intended to be multi-line, it should probably be WordWrap
                                    LineBreakMode = UILineBreakMode.CharacterWrap
                                };

                textAttributes = new NSMutableDictionary
                                     {
                                         {UIStringAttributeKey.Font, font},
                                         {UIStringAttributeKey.ParagraphStyle, style}
                                     };

                _textAttributesMap.Add((font, alignment), textAttributes);
            }

            return textAttributes;
        }

        private static UITextAlignment GetNativeAlignment(TextAlignment alignment)
        {
            UITextAlignment nativeAlignment;

            switch (alignment)
            {
                case TextAlignment.Center:
                    nativeAlignment = UITextAlignment.Center;

                    break;

                case TextAlignment.End:
                    nativeAlignment = UITextAlignment.Right;

                    break;

                default:
                    nativeAlignment = UITextAlignment.Left;

                    break;
            }

            return nativeAlignment;
        }

        private readonly Dictionary<(UIFont, TextAlignment), NSDictionary> _textAttributesMap = new Dictionary<(UIFont, TextAlignment), NSDictionary>();
    }
}