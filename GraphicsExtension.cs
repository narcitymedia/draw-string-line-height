using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace draw_string_line_height
{
    public static class GraphicsExtension
    {
        // private static IEnumerable<string> WrapTextLines(string text, int maxWidth)
        // {
        //     if (text == null) { throw new ArgumentNullException("text", "The text paramater cannot be null"); }

        //     if (maxWidth <= 0) { throw new ArgumentException("The specified maximum width must be greater than 0", "maxWidth"); }


        // }

        /// <summary>
        /// Calculates how the given text will have to be wrapped nin order to fit in the given width
        /// </summary>
        /// <param name="text">The full text for which to calculate the text wrap</param>
        /// <param name="font">he font tht will be used to display the text</param>
        /// <param name="maxWidth">The width in which the text must fit</param>
        /// <returns>
        /// A string enumerable where each string is a substring of the text param and represents a line
        /// resulting of the text wrapping
        /// </returns>
        public static IEnumerable<string> GetLinesWrap(this Graphics that, string text, Font font, int maxWidth)
        {
            // See https://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation
            string[] words = text.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return new string[0];

            List<List<string>> lines = new List<List<string>>();
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
                    lines.Add(new List<string>(l));

                    // Clear the lit used to store the words of the current line
                    l.Clear();

                    // Since the current word did not fit in the current line, add it to the new one
                    l.Add(words[i]);
                }
            }

            return new string[0];
        }

        public static SizeF MeasureString(this Graphics that, string text, Font font, int maxWidth, int lineHeight)
        {
            if (text == null || text.Length == 0)
                return new SizeF(0, 0); 
            if (font == null)
                throw new ArgumentNullException("font");

            string[] lines = that.GetLinesWrap(text, font, maxWidth).ToArray();

            return new SizeF(1,1);
        }
    }
}
