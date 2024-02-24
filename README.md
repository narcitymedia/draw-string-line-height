This package contains extension methods to the `System.Drawing.Graphics` class to allow for measuring and drawing text with a custom line height and font.

The project source code is located in the `src` folder and the unit tests project for the library is located in the `tests` directory.

# PLEASE NOTE:
**This is a forked version.** Original repo is located at https://github.com/narcitymedia/draw-string-line-height 

# Requirements
This package targets `.net8.0`. 

Additionally, you'll probably need to install tools like a [dotnet sdk and runtime](https://dotnet.microsoft.com/download) on your computer

# Running tests

In order to run the unit tests for yourself:
 1. Change directory to the root of this repository (where the .sln file is located)
 2. Run `dotnet test`

# Installation

Clone repo, and build by your own.

# Usage

Firstly, you'll need to use the `NarcityMedia.DrawStringLineHeight` namespace to have access to the extensions methods to the `System.Drawing.Graphics` class.

## Code Example

```Csharp
public static Bitmap GenerateImage()
{
    string text = "Some pretty cooooooool text";
    // PAY ATTENTION to the GraphicsUnit you use, it should match that of the PageUnit property of your System.Drawing.Graphics instance
    Font arialFont = new Font("Arial", 32, GraphicsUnit.Pixel);

    SolidBrush brush = new SolidBrush(Color.Black);

    int maxWidth = 300;
    int customLineHeight = 20; // px
    // The variable g here represents a globaly available 'dummy' static instance of System.Drawing.Graphics (it's not important)
    SizeF textSize = g.MeasureStringLineHeight(text, arialFont, maxWidth, customLineHeight)
    Bitmap newImage = new Bitmap(1080, 1080);
    using (Graphics gr = Graphics.FromImage(newImage))
    {
        gr.Clear(Color.White);

        Point textOrigin = new Point(10, 10);
        gr.DrawString(text, arialFont, brush, maxWidth, customLineHeight,
                                        new Rectangle(textOrigin, new Size(maxWidth, 1080)), StringFormat.GenericDefault);

        return newImage;
    }
}
```

# API

## MeasureStringLineHeight

```Csharp
public static SizeF MeasureStringLineHeight(this Graphics that, string text, Font font, int maxWidth, int lineHeight)
```

### Summary
Measures the space taken up by a given text for a given font, width and line height

### Parameters
`String text` : The text to measure

`System.Drawing.Font font` : Instance of `System.Drawing.Font` to use

`int maxWidth` : A positive number (grater than zero) that represents the width in which the text must fit (will affect text wrapping)

`int lineHeight` : The custom line height used to calculate the text size


### Returns `System.Drawing.SizeF`
The size taken up by the given text with the given parameters

### Remarks
This extension method isn't an overload of the `System.Drawing.Graphics.MeasureString(string, Font)` method
because the method had conflicting overloads and it ended up being confusing

## DrawString

```Csharp
public static void DrawString(this Graphics that, string text, Font font, Brush brush, int maxWidth,
                                            int lineHeight, RectangleF layoutRectangle, StringFormat format)
```

### Summary
Draws the specified text string in the specified rectangle with the specified font, brush,
width, line height and format

### Parameters
`String text` : The text to measure

`System.Drawing.Font font` : Instance of `System.Drawing.Font` to use

`System.Drawing.Brush brush` : The brush to use to paint the text

`int lineHeight` : The custom line height used to calculate the text size

`System.Drawing.RectangleF layoutRectangle` : structure that specifies the location of the drawn text

`System.Drawing.StringFormat format` : Specifies formatting attributes, such as line spacing and alignment, that are applied to the drawn text.

## GetWrappedLines

```Csharp
public static IEnumerable<string> GetWrappedLines(this Graphics that, string text, Font font, double maxWidth = double.PositiveInfinity)
```

### Summary
Calculates how the given text will have to be wrapped in order to fit in the given width.

This method is used by the library internally to perform text wrapping but you can use it if you need it.

### Parameters

`String text` : The text to measure

`System.Drawing.Font font` : Instance of `System.Drawing.Font` to use

`int maxWidth` : A positive number (grater than zero) that represents the width in which the text must fit (will affect text wrapping)

### Returns `IEnumerable<string>`
A string enumerable where each string is a substring of the text param and represents a line
resulting of the text wrapping
