using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using NUnit.Framework;
using NarcityMedia.DrawStringLineHeight;

namespace Tests
{
    public class GraphicsExtensionTests_GetWrappedLines
    {
        protected Bitmap SampleBmp;
        protected Graphics GraphicsInstance;
        protected Font MonoSpaceFont;
        protected float CharacterWidth;

        [OneTimeSetUp]
        public void Setup()
        {
            this.SampleBmp = new Bitmap(200, 200);
            this.GraphicsInstance = Graphics.FromImage(this.SampleBmp);
            this.GraphicsInstance.Clear(Color.White);
            this.MonoSpaceFont = new Font(FontFamily.GenericMonospace, 20);

            this.CharacterWidth = GraphicsInstance.MeasureString("D", this.MonoSpaceFont).Width;
        }

        [Test]
        /// <summary>
        /// Passing an empty string to get wrapper lines should return an empty
        /// string array
        /// </summary>
        [TestCase("")]
        [TestCase(" ")] // One space
        [TestCase("            ")] // A lot of spaces
        [TestCase("     \t     ")] // A lot of spaces and a tab character
        public void GetWrappedLines_Empty_String(string s)
        {
            string[] lines = this.GraphicsInstance.GetWrappedLines(s, MonoSpaceFont, 20).ToArray();
            Assert.AreEqual(0, lines.Length, "Must return an empty lines array for an empty string");            
        }

        [Test]
        /// <summary>
        /// A single character must result in a single line
        /// </summary>
        public void GetWrappedLines_One_Char()
        {
            string[] lines = this.GraphicsInstance.GetWrappedLines("a", this.MonoSpaceFont).ToArray();
            Assert.AreEqual(1, lines.Length, "Must return a line array of length 1 for a single chracter");
        }

        [Test]
        /// <summary>
        /// When passed a max width that is smaller than the size of a single character, the returned string enumerable
        /// is expected to have a length equal to the number of words in the given string becase the split occurs at every word
        /// (NOT every character)
        /// </summary>
        [TestCase("a")]
        [TestCase("Test 123")]
        [TestCase("I love good coffee")]
        public void GetWrappedLines_MaxWidth_Smaller_Than_CharWidth(string s)
        {
            int wordCount = s.Split((char[]) null).Length;
            string[] lines = this.GraphicsInstance.GetWrappedLines(s, this.MonoSpaceFont, this.CharacterWidth - 1).ToArray();
            Assert.AreEqual(wordCount, lines.Length);
        }

        [Test]
        /// <summary>
        /// Make sure that words contained in the returned array are in the same order
        /// as they appear in the original string
        /// </summary>
        [TestCase("Coding is fun!")]
        [TestCase("I love when tests pass")]
        public void GetWrappedLines_MaxWidth_Smaller_Than_CharWidth_Correct_Order(string s)
        {
            string[] words = s.Split((char[]) null);
            string[] lines = this.GraphicsInstance.GetWrappedLines(s, this.MonoSpaceFont, this.CharacterWidth - 1).ToArray();
            
            // Each line contains a single word since the maxwidth is smaller than a single character,
            // see this.GetWrappedLines_MaxWidth_Smaller_Than_CharWidth() 
            for (int i = 0; i < words.Length; i++)
            {
                Assert.AreEqual(words[i], lines[i]);
            }
        }

        [Test]
        /// <summary>
        /// When using the default maxWidth (Infinity), an array of length 1 should be returned for any non empty or blank string
        /// </summary>
        public void GetWrappedLines_Default_Width()
        {
            // Haiku inspired by https://ubuntuforums.org/showthread.php?t=1101510&p=6929375#post6929375 :)
            string s = "php how i hate thee, your objects and libs are messy, now hope it doesn't fail if you pass String.Empty";
            string[] lines = this.GraphicsInstance.GetWrappedLines(s, this.MonoSpaceFont).ToArray();
            Assert.AreEqual(1, lines.Length);
        }

    }

    public class GraphicsExtensionTests_MeasureString
    {
        protected Bitmap Canvas;
        protected Graphics GraphicsInstance;
        protected Font MonoSpaceFont;
        protected SolidBrush TextBrush;

        /// <summary>
        /// Declard as a static constant so it's accessible to attributes
        /// </summary>
        protected const int FONT_SIZE = 16;
        protected Random RndGenerator;

        [OneTimeSetUp]
        public void Setup()
        {
            this.Canvas = new Bitmap(1080, 1080);
            this.GraphicsInstance = Graphics.FromImage(this.Canvas);
            this.GraphicsInstance.Clear(Color.White);
            this.MonoSpaceFont = new Font(FontFamily.GenericMonospace, FONT_SIZE);
            this.TextBrush = new SolidBrush(Color.Black);
            this.RndGenerator = new Random();
        }

        [Test]
        public void MeasureString_Empty_String()
        {
            int randomMaxWidth = this.RndGenerator.Next(10, 100);
            int randomLineHeight = this.RndGenerator.Next(10, 100);
            SizeF size = this.GraphicsInstance.MeasureStringLineHeight(String.Empty, this.MonoSpaceFont, randomMaxWidth, randomLineHeight);
            Assert.AreEqual(0, size.Width);
            Assert.AreEqual(0, size.Height);
        }

        [Test]
        /// <summary>
        /// When a line height smaller than the character height is passed to MeasureString, the height
        /// of the character itself should be returned to make sure the 'real' text painting region
        /// is returned to the user, otherwise, the line height should be returned
        /// </summary>
        /// <param name="lineHeight">Tested lineheight</param>
        [TestCase(FONT_SIZE)]
        [TestCase(FONT_SIZE - 1)]
        [TestCase(FONT_SIZE + 1)]
        [TestCase(int.MaxValue)]
        public void MeasureString_One_Line_Height(int lineHeight)
        {
            string s = "a";
            SizeF size = this.GraphicsInstance.MeasureStringLineHeight(s, this.MonoSpaceFont, int.MaxValue, lineHeight);

            if (lineHeight <= this.MonoSpaceFont.Height)
            {
                Assert.AreEqual(this.MonoSpaceFont.Height, size.Height, "Expected returned size's height to correspond to the font's height");
            }
            else if (lineHeight > this.MonoSpaceFont.Height)
            {
                Assert.AreEqual(lineHeight, size.Height, "Expected returned size's height to correspond to the custom line height");
            }
        }
    }
    
    public class GraphicsExtensionTests_DrawString
    {
        protected Bitmap canvas;
        protected Graphics GraphicsInstance;
        protected Font MonoSpaceFont;
        protected SolidBrush TextBrush;
        protected const string ImagesOutputDirName = "testimgoutput";
        protected string AbsImageOutputDir;

        [OneTimeSetUp]
        public void Setup()
        {
            this.canvas = new Bitmap(1080, 1080);
            this.GraphicsInstance = Graphics.FromImage(this.canvas);
            this.GraphicsInstance.Clear(Color.White);
            this.MonoSpaceFont = new Font(FontFamily.GenericMonospace, 16);
            this.TextBrush = new SolidBrush(Color.Black);
            this.AbsImageOutputDir = Path.Join(TestContext.CurrentContext.WorkDirectory, ImagesOutputDirName);
            if (!Directory.Exists(this.AbsImageOutputDir))
            {
                Directory.CreateDirectory(this.AbsImageOutputDir);
            }
        }

        [Test]
        public void DrawString_Empty_String()
        {
            Point origin = new Point(10,10);
            this.GraphicsInstance.DrawString("Coding is pretty fun!", this.MonoSpaceFont, this.TextBrush,
                    20, 80, new Rectangle(origin, new Size(1000, 1000)), StringFormat.GenericDefault);

            string imgPath = Path.Join(
                this.AbsImageOutputDir,
                "test_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() + ".png"
            );

            TestContext.Progress.WriteLine("Saving test generated image to " + imgPath);
            this.canvas.Save(imgPath, ImageFormat.Png);
            Assert.AreEqual(true, true);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // this.canvas.Save(Path.Join("testresults", "test_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()));        }
        }
    }
}
