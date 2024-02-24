using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text.RegularExpressions;

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
            if (String.IsNullOrEmpty(text)) return Array.Empty<string>();
            if (font == null) throw new ArgumentNullException("font", "The 'font' parameter must not be null");
            if (maxWidth <= 0) throw new ArgumentOutOfRangeException("maxWidth", "Maximum width must be greater than zero");

            // See https://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation

            //treat English words as individual units, and everything else as a single chunk until a whitespace is encountered.
            var Pattern = @"\r?\n|[\u0250-\uFFFF]+|\b[\u0021-\u024F]+[ \f\t\v]*|\b[ \f\t\v]+|[\u0021-\u024F]";
            var reg = new Regex(Pattern, RegexOptions.Multiline | RegexOptions.CultureInvariant);
            var m = reg.Matches(text);
            var words = new List<string>(m.Select(m => m.Value).AsEnumerable());

            //Exclude empty elements or spaces at the end of the words.
            while (words.Count > 0 && string.IsNullOrWhiteSpace(words[words.Count - 1]))
            {
                words.RemoveAt(words.Count - 1);
            }

            if (words.Count == 0) return Array.Empty<string>();

            List<string> lines = new List<string>();

            float currentWidth = 0;
            string currentLine = "";
            using (StringFormat sf = new(StringFormat.GenericTypographic))
            {
                sf.FormatFlags |= StringFormatFlags.LineLimit | StringFormatFlags.FitBlackBox;
                for (int i = 0; i < words.Count; i++)
                {
                    //If the word is a line break, add the current line to the list and start a new line.
                    if (Regex.IsMatch(words[i], @"^\r?\n$"))
                    {
                        lines.Add(currentLine == "" ? "\uE007F" : currentLine);
                        currentLine = "";
                        currentWidth = 0;
                        continue;
                    }
                    float currentWordWidth = that.MeasureString(words[i], font,
                        int.MaxValue, sf).Width;
                    if (currentWidth != 0)
                    {
                        float potentialWordWidth = currentWordWidth;
                        if (currentWidth + potentialWordWidth < maxWidth)
                        {
                            currentWidth += potentialWordWidth;
                            currentLine += words[i];
                        }
                        else
                        {
                            // If words[i] starts with [\u0250-\uFFFF], treat it as CJK and try to fill the remaining without line break
                            // Otherwise, line break here
                            if (!Regex.IsMatch(words[i], @"^[\u0250-\uFFFF]"))
                            {
                                lines.Add(currentLine);
                                currentLine = words[i];
                                if (currentWordWidth <= maxWidth)
                                {
                                    currentWidth = currentWordWidth;
                                }
                                else
                                {
                                    //If the currentWordWidth is already longer than maxWidth, the characters in the current line should be split
                                    //and the remainder should be carried over to the next line.
                                    do
                                    {
                                        var pos = currentLine.Length - 1;
                                        while (that.MeasureString(currentLine.Substring(0, pos),
                                                   font, int.MaxValue, sf).Width > maxWidth && pos > 1)
                                        {
                                            pos--;
                                        }

                                        lines.Add(currentLine[..pos]);
                                        currentLine = currentLine[pos..];
                                        //Add a condition to handle the case where there is only one character
                                        //but it already exceeds the maximum width (when maxWidth is smaller than one character).
                                    } while (that.MeasureString(currentLine, font, int.MaxValue, sf).Width > maxWidth &&
                                             currentLine.Length > 1);

                                    currentWidth = that.MeasureString(currentLine, font, int.MaxValue, sf).Width;
                                }
                            }
                            else
                            {
                                currentLine += words[i];
                                if (potentialWordWidth + currentWidth <= maxWidth)
                                {
                                    currentWidth += potentialWordWidth;
                                }
                                else
                                {
                                    //If the currentWordWidth is already longer than maxWidth, the characters in the current line should be split
                                    //and the remainder should be carried over to the next line.
                                    do
                                    {
                                        var pos = currentLine.Length - 1;
                                        while (that.MeasureString(currentLine.Substring(0, pos),
                                                   font, int.MaxValue, sf).Width > maxWidth && pos > 1)
                                        {
                                            pos--;
                                        }

                                        lines.Add(currentLine[..pos]);
                                        currentLine = currentLine[pos..];
                                        //Add a condition to handle the case where there is only one character
                                        //but it already exceeds the maximum width (when maxWidth is smaller than one character).
                                    } while (that.MeasureString(currentLine, font, int.MaxValue, sf).Width > maxWidth &&
                                             currentLine.Length > 1);

                                    currentWidth = that.MeasureString(currentLine, font, int.MaxValue, sf).Width;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (currentWordWidth <= maxWidth)
                        {
                            currentWidth += currentWordWidth;
                            currentLine = words[i];
                        }
                        else
                        {
                            currentLine = words[i];
                            //If the currentWordWidth is already longer than maxWidth, the characters in the current line should be split
                            //and the remainder should be carried over to the next line.
                            //そもそも詰めようがないので、そのまま次の行に移動する。
                            if (currentLine.Length <= 1)
                            {
                                lines.Add(currentLine);
                                currentLine = "";
                                currentWidth = 0;
                            }
                            else
                            {

                                do
                                {
                                    var pos = currentLine.Length - 1;
                                    while (that.MeasureString(currentLine.Substring(0, pos),
                                               font, int.MaxValue, sf).Width > maxWidth && pos > 1)
                                    {
                                        pos--;
                                    }

                                    lines.Add(currentLine[..pos]);
                                    currentLine = currentLine[pos..];
                                    //Add a condition to handle the case where there is only one character
                                    //but it already exceeds the maximum width (when maxWidth is smaller than one character).
                                } while (that.MeasureString(currentLine, font, int.MaxValue, sf).Width > maxWidth &&
                                         currentLine.Length > 1);

                                currentWidth = that.MeasureString(currentLine, font, int.MaxValue, sf).Width;
                                //if currentLine remains only whitespaces, add it to last line.
                                if (string.IsNullOrWhiteSpace(currentLine))
                                {
                                    lines[^1] += currentLine;
                                    currentLine = "";
                                }
                            }
                        }
                    }

                    //write down the remaining content of currentLine and finish.
                    if (i == words.Count - 1 && !string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                    }
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
        /// <param name="format">Specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.</param>
        public static void DrawString(this Graphics that, string text, Font font, Brush brush, int maxWidth,
                                            int lineHeight, RectangleF layoutRectangle, StringFormat format)
        {
            string[] lines = that.GetWrappedLines(text, font, maxWidth).ToArray();
            Rectangle lastDrawn = new Rectangle(Convert.ToInt32(layoutRectangle.X), Convert.ToInt32(layoutRectangle.Y), 0, 0);
            format.FormatFlags |= StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
            format.Trimming = StringTrimming.None;
            foreach (string line in lines)
            {
                SizeF lineSize = that.MeasureString(line, font);
                int increment = lastDrawn.Height == 0 ? 0 : lineHeight;
                Point lineOrigin = new Point(lastDrawn.X, lastDrawn.Y + increment);
                RectangleF lineRect = new RectangleF(lineOrigin, lineSize);
                lineRect.Width = maxWidth;
                // Empty line markers are treated as non-drawing objects
                that.DrawString(line == "\uE007F" ? "" : line, font, brush, lineRect, format);
                lastDrawn = new Rectangle(lineOrigin, Size.Round(lineSize));
                if (line == "\uE007F") // If there is a marker for an empty line, add line height to the last drawn rectangle
                {
                    lastDrawn.Height += lineHeight;
                }
            }
        }
    }
}
