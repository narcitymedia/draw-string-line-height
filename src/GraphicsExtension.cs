using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace draw_string_line_height
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
            string currentLine = String.Empty;

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                string potentialLine = currentLine + " " + word;
                float potentialWidth = that.MeasureString(potentialLine, font).Width;
                if (potentialWidth > maxWidth)
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = potentialLine;
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
        public static SizeF MeasureString(this Graphics that, string text, Font font, int maxWidth, int lineHeight)
        {
            if (text == null || text.Length == 0)
                return new SizeF(0, 0); 
            if (font == null)
                throw new ArgumentNullException("font");

            string[] lines = that.GetWrappedLines(text, font, maxWidth).ToArray();
            
            if (lines.Length == 0) return new SizeF(0, 0);

            return new SizeF(maxWidth, lineHeight * lines.Length);
        }


        public static Region[] DrawString(this Graphics that, string text, Font font, Brush brush, int maxWidth,
                                            int lineHeight, RectangleF layoutRectangle, StringFormat format)
        {
            string[] lines = that.GetWrappedLines(text, font, maxWidth).ToArray();
            Region[] regions = new Region[lines.Length];

            foreach (string line in lines)
            {
                
                that.DrawString(line, font, brush,layoutRectangle);
            }

            return regions;
        }
    }
}
