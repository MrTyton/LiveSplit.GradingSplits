using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace LiveSplit.GradingSplits.Model
{
    /// <summary>
    /// Generates grade icons programmatically using GDI+.
    /// </summary>
    public static class GradeIconGenerator
    {
        /// <summary>
        /// The default icon size (24x24 pixels).
        /// </summary>
        public const int IconSize = 24;

        /// <summary>
        /// Generates a grade icon with the specified label and colors.
        /// </summary>
        /// <param name="label">The grade label (e.g., "S", "A", "★").</param>
        /// <param name="foregroundColor">The color for the label text and border.</param>
        /// <param name="backgroundColor">Optional background color. If null, uses a darker version of foreground.</param>
        /// <returns>A 24x24 bitmap of the grade icon.</returns>
        public static Image GenerateIcon(string label, Color foregroundColor, Color? backgroundColor = null)
        {
            var bitmap = new Bitmap(IconSize, IconSize);
            
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.Transparent);

                // Calculate background color if not provided
                Color bgColor = backgroundColor ?? GetBackgroundColor(foregroundColor);

                // Draw circular background
                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillEllipse(bgBrush, 1, 1, IconSize - 2, IconSize - 2);
                }

                // Draw border
                using (var borderPen = new Pen(foregroundColor, 1.5f))
                {
                    g.DrawEllipse(borderPen, 1, 1, IconSize - 3, IconSize - 3);
                }

                // Draw label text
                DrawCenteredText(g, label, foregroundColor);
            }

            return bitmap;
        }

        /// <summary>
        /// Draws centered text on the icon.
        /// </summary>
        private static void DrawCenteredText(Graphics g, string label, Color textColor)
        {
            if (string.IsNullOrEmpty(label)) return;

            // Adjust font size based on label length
            float fontSize = label.Length <= 1 ? 12f : (label.Length == 2 ? 9f : 7f);
            
            using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var textBrush = new SolidBrush(textColor))
            {
                var size = g.MeasureString(label, font);
                float x = (IconSize - size.Width) / 2f;
                float y = (IconSize - size.Height) / 2f;
                g.DrawString(label, font, textBrush, x, y);
            }
        }

        /// <summary>
        /// Gets a suitable background color (darker version of the foreground).
        /// </summary>
        private static Color GetBackgroundColor(Color foreground)
        {
            // Create a darker version for the background
            int r = (int)(foreground.R * 0.25);
            int g = (int)(foreground.G * 0.25);
            int b = (int)(foreground.B * 0.25);
            return Color.FromArgb(Math.Max(10, r), Math.Max(10, g), Math.Max(10, b));
        }
    }
}
