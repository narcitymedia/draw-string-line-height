using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
        /// <param name="maxWidth">The width in which the text must fit</param>
        /// <returns>
        /// A string enumerable where each string is a substring of the text param and represents a line
        /// resulting of the text wrapping
        /// </returns>
        public static IEnumerable<string> GetWrappedLines(this Graphics that, string text, Font font, int maxWidth)
        {
            // See https://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation
            string[] words = text.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return new string[0];

            List<string> lines = new List<string>();
            List<string> l = new List<string>();

            for (int i = 0; i < words.Length; i++)
            {
                SizeF subStrSize = that.MeasureString(words[i], font);
                if (subStrSize.Width < maxWidth)
                {
                    l.Add(words[i]);
                }
                else
                {
                    // Add the current line to the lines list
                    lines.Add(String.Join(' ', l));

                    // Clear the list used to store the words of the current line
                    l.Clear();

                    // Since the current word did not fit in the current line, add it to the new one
                    l.Add(words[i]);
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
        /// <param name="maxWidth">The width in which the text must fit</param>
        /// <param name="lineHeight">The custom line height used to calculate the text size</param>
        /// <returns>The size taken up by the given text with the given parameters</returns>
        public static SizeF MeasureString(this Graphics that, string text, Font font, int maxWidth, int lineHeight)
        {
            if (text == null || text.Length == 0)
                return new SizeF(0, 0); 
            if (font == null)
                throw new ArgumentNullException("font");

            string[] lines = that.GetWrappedLines(text, font, maxWidth).ToArray();
            foreach (string l in lines)
                Console.WriteLine(l);

            return new SizeF(1,1);
        }
    }
}
