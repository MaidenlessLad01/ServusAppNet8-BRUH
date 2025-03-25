using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace ServusApp.Drawables
{
    internal class WaveShape : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Set up the wave shape colors and strokes
            canvas.FillColor = Colors.White;

            var path = new PathF();

            // Create a simple wave pattern path
            path.MoveTo(0, dirtyRect.Height * 0.7f);
            path.CurveTo(dirtyRect.Width * 0.25f, dirtyRect.Height * 0.6f,
                         dirtyRect.Width * 0.75f, dirtyRect.Height * 0.8f,
                         dirtyRect.Width, dirtyRect.Height * 0.7f);

            path.LineTo(dirtyRect.Width, dirtyRect.Height);
            path.LineTo(0, dirtyRect.Height);

            path.Close();

            // Fill the path with white to create the wave effect
            canvas.FillPath(path);
        }
    }
}
