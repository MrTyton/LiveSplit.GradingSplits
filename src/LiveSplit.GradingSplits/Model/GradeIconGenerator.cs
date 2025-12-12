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
        /// <param name="label">The grade label (e.g., "S", "A+", "★").</param>
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

                // Draw rounded rectangle background
                int cornerRadius = 4;
                using (var bgBrush = new SolidBrush(bgColor))
                using (var path = CreateRoundedRectangle(1, 1, IconSize - 2, IconSize - 2, cornerRadius))
                {
                    g.FillPath(bgBrush, path);
                }

                // Draw border
                using (var borderPen = new Pen(foregroundColor, 1.5f))
                using (var path = CreateRoundedRectangle(1, 1, IconSize - 3, IconSize - 3, cornerRadius))
                {
                    g.DrawPath(borderPen, path);
                }

                // Draw label text
                DrawCenteredText(g, label, foregroundColor);
            }

            return bitmap;
        }

        /// <summary>
        /// Creates a rounded rectangle graphics path.
        /// </summary>
        private static GraphicsPath CreateRoundedRectangle(int x, int y, int width, int height, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Top left arc
            path.AddArc(x, y, diameter, diameter, 180, 90);
            // Top right arc
            path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
            // Bottom right arc
            path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
            // Bottom left arc
            path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draws centered text on the icon.
        /// </summary>
        private static void DrawCenteredText(Graphics g, string label, Color textColor)
        {
            if (string.IsNullOrEmpty(label)) return;

            // Larger font sizes now that we have more space with square shape
            float fontSize = label.Length <= 1 ? 14f : (label.Length == 2 ? 11f : 9f);
            
            using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
            using (var textBrush = new SolidBrush(textColor))
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                
                var rect = new RectangleF(0, 0, IconSize, IconSize);
                g.DrawString(label, font, textBrush, rect, format);
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
