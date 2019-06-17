using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace NarcityMedia.DrawStringLineHeight
{
    /// <summary>
    /// Provides extension methods for the <see cref="System.Drawing.Graphics"/>
    /// </summary>
    public static class GraphicsExtension
    {
        /// <summary>
        /// Calculates how the given text will have to be wrapped in order to fit in the given width
        /// </summary>
        /// <param name="text">The full text for which to calculate the text wrap</param>
        /// <param name="font">The font tht will be used to display the text</param>
        /// <param name="maxWidth">A positive number (grater than zero) that represents the width in which the text must fit</param>
        /// <returns>
        /// A string enumerable where each string is a substring of the text param and represents a line
        /// resulting of the text wrapping
        /// </returns>
        public static IEnumerable<string> GetWrappedLines(this Graphics that, string text, Font font, double maxWidth = double.PositiveInfinity)
        {
            if (String.IsNullOrEmpty(text)) return new string[0];
            if (font == null) throw new ArgumentNullException("font", "The 'font' parameter must not be null");
            if (maxWidth <= 0) throw new ArgumentOutOfRangeException("maxWidth", "Maximum width must be greater than zero");

            // See https://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation
            string[] words = text.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0) return new string[0];

            List<string> lines = new List<string>();

            string currentLine = words[0];
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                float potentialWidth = that.MeasureString(currentLine, font).Width;
                if (potentialWidth > maxWidth)
                {
                    lines.Add(currentLine);
                    if (i + 1 < words.Length) currentLine = words[i + 1];
                    continue;
                }
                else
                {
                    currentLine = currentLine + " " + word;
                    if (i + 1 == words.Length) lines.Add(currentLine);
                }
            }

            return lines;
        }

        /// <summary>
        /// Measures the space taken up by a given text for a given font, width and line height
        /// </summary>
        /// <param name="that">The extended <see cref="System.Drawing.Graphics" /> object</param>
        /// <param name="text">The text to measure</param>
        /// <param name="font">The font used to display the text</param>
        /// <param name="maxWidth">A positive number (grater than zero) that represents the width in which the text must fit</param>        
        /// <param name="lineHeight">The custom line height used to calculate the text size</param>
        /// <returns>The size taken up by the given text with the given parameters</returns>
        /// <remarks>
        /// This extension method isn't an overload of the <see cref="System.Drawing.Graphics.MeasureString(string, Font)" /> method
        /// because the method had conflicting overloads and it ended up being confusing
        /// </remarks>
        public static SizeF MeasureStringLineHeight(this Graphics that, string text, Font font, int maxWidth, int lineHeight)
        {
            if (String.IsNullOrEmpty(text))
                return new SizeF(0, 0); 
            if (font == null)
                throw new ArgumentNullException("font");
            if (maxWidth <= 0)
                throw new ArgumentOutOfRangeException("maxWidth");

            string[] lines = that.GetWrappedLines(text, font, maxWidth).ToArray();

            if (lines.Length == 0) return new SizeF(0, 0);

            SizeF[] sizes = lines.Select(l => that.MeasureString(l, font)).ToArray();

            int lineIncrement = Math.Max(lineHeight, font.Height);
            float totalWidth = sizes.Max(s => s.Width);
            float totalHeight = lines.Length * lineIncrement;

            return new SizeF(totalWidth, totalHeight);
        }

        /// <summary>
        /// Draws the specified text string in the specified rectangle with the specified font, brush,
        /// width, line height and format
        /// </summary>
        /// <param name="that">The extended <see cref="System.Drawing.Graphics" /> object</param>        
        /// <param name="text">The text to draw</param>
        /// <param name="font">The font used to display the text</param>
        /// <param name="brush">The brush used to draw the text</param>
        /// <param name="maxWidth">A positive number (grater than zero) that represents the width in which the text must fit</param>        
        /// <param name="lineHeight">The distance in pixels that separaes the lines of the text</param>
        /// <param name="layoutRectangle"><see cref="System.Drawing.RectangleF" /> structure that specifies the location of the drawn text</param>
        /// <param name="format">that specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.</param>
        /// <returns></returns>
        public static Rectangle[] DrawString(this Graphics that, string text, Font font, Brush brush, int maxWidth,
                                            int lineHeight, RectangleF layoutRectangle, StringFormat format)
        {
            string[] lines = that.GetWrappedLines(text, font, maxWidth).ToArray();
            Rectangle[] regions = new Rectangle[lines.Length];
            Rectangle lastDrawn = new Rectangle(Convert.ToInt32(layoutRectangle.X), Convert.ToInt32(layoutRectangle.Y), 0, 0);
            foreach (string line in lines)
            {
                SizeF lineSize = that.MeasureString(line, font);
                Point lineOrigin = new Point(lastDrawn.X, lastDrawn.Y + lineHeight);
                that.DrawString(line, font, brush, lineOrigin);
                lastDrawn = new Rectangle(lineOrigin, Size.Round(lineSize));
            }

            return regions;
        }
    }
}
