using System.Drawing;
using System.Windows.Forms;

namespace Utility.WindowsForms.CustomControls
{
    public class ColoredProgressBar : ProgressBar
    {

        public ColoredProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        public Color FillColor { get; set; }

        private Brush Brush => new SolidBrush(FillColor);

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            rec.Width = (int) (rec.Width * ((double) Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
            {
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            }

            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(Brush, 2, 2, rec.Width, rec.Height);
        }

    }
}