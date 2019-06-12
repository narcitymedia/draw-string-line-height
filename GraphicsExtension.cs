using System;
using System.Collections;
using System.Collections.Generic;
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

        public static SizeF MeasureString(this Graphics that, string text, Font font, int width, int lineHeight)
        {
            if (text == null || text.Length == 0)
                return new SizeF(0, 0); 
            if (font == null)
                throw new ArgumentNullException("font");

            // See https://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation
            string[] words = text.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return new SizeF(0, 0);

            List<List<string>> lines = new List<List<string>>();
            
            // This list is used to store each new line's words and cleared when text needs to wrap,
            // hopefully reducing memory allocation
            List<string> lineWords = new List<string>();

            for (int i = 0; i < words.Length; i++)
            {
                SizeF subStrSize = that.MeasureString(words[i], font);
                if (subStrSize.Width > width)
                {
                    
                }
            }

            return new SizeF(1,1);
        }
    }
}
