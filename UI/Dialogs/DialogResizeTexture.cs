using RE4_PS2_TPL_Manager.UI.Theming;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager
{
    public enum ResizeResampling
    {
        NearestNeighbor,
        Bilinear,
        Bicubic
    }

    public partial class DialogResizeTexture : Form
    {
        private readonly int originalWidth;
        private readonly int originalHeight;
        private readonly Bitmap sourceImage;
        private Bitmap afterPreviewImage;
        private bool syncing;

        public int TargetWidth => (int)numWidth.Value;
        public int TargetHeight => (int)numHeight.Value;
        public ResizeResampling Resampling => (ResizeResampling)cmbResampling.SelectedIndex;

        public DialogResizeTexture(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            InitializeComponent();
            sourceImage = new Bitmap(image);
            originalWidth = Math.Max(1, image.Width);
            originalHeight = Math.Max(1, image.Height);

            numWidth.Value = Math.Min(numWidth.Maximum, originalWidth);
            numHeight.Value = Math.Min(numHeight.Maximum, originalHeight);
            cmbResampling.SelectedIndex = 0;

            previewBefore.Image = sourceImage;
            DarkTheme.Apply(this);
            UpdateSummaryAndPreview();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                if (afterPreviewImage != null)
                {
                    previewAfter.Image = null;
                    afterPreviewImage.Dispose();
                    afterPreviewImage = null;
                }

                if (sourceImage != null)
                {
                    previewBefore.Image = null;
                    sourceImage.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            if (syncing || !chkKeepAspect.Checked)
            {
                UpdateSummaryAndPreview();
                return;
            }

            syncing = true;
            decimal ratio = (decimal)originalHeight / originalWidth;
            numHeight.Value = Clamp(numHeight, Math.Round(numWidth.Value * ratio));
            syncing = false;
            UpdateSummaryAndPreview();
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            if (syncing || !chkKeepAspect.Checked)
            {
                UpdateSummaryAndPreview();
                return;
            }

            syncing = true;
            decimal ratio = (decimal)originalWidth / originalHeight;
            numWidth.Value = Clamp(numWidth, Math.Round(numHeight.Value * ratio));
            syncing = false;
            UpdateSummaryAndPreview();
        }

        private void cmbResampling_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!syncing)
                UpdateSummaryAndPreview();
        }

        private static decimal Clamp(NumericUpDown numeric, decimal value)
        {
            if (value < numeric.Minimum) return numeric.Minimum;
            if (value > numeric.Maximum) return numeric.Maximum;
            return value;
        }

        private void btnHalf_Click(object sender, EventArgs e)
        {
            ApplyRelativeScale(0.5m);
        }

        private void btnDouble_Click(object sender, EventArgs e)
        {
            ApplyRelativeScale(2m);
        }

        private void btnOriginal_Click(object sender, EventArgs e)
        {
            syncing = true;
            numWidth.Value = Clamp(numWidth, originalWidth);
            numHeight.Value = Clamp(numHeight, originalHeight);
            syncing = false;
            UpdateSummaryAndPreview();
        }

        private void ApplyRelativeScale(decimal multiplier)
        {
            syncing = true;
            decimal nextWidth = Math.Max(1m, Math.Round(numWidth.Value * multiplier));
            decimal nextHeight = Math.Max(1m, Math.Round(numHeight.Value * multiplier));

            numWidth.Value = Clamp(numWidth, nextWidth);
            numHeight.Value = Clamp(numHeight, nextHeight);
            syncing = false;
            UpdateSummaryAndPreview();
        }

        private void UpdateSummaryAndPreview()
        {
            if (lblSummary == null || sourceImage == null)
                return;

            lblSummary.Text = $"{originalWidth}×{originalHeight}  →  {TargetWidth}×{TargetHeight}";

            Bitmap next = CreateResizedPreview(sourceImage, TargetWidth, TargetHeight, Resampling);
            Bitmap old = afterPreviewImage;
            afterPreviewImage = next;
            previewAfter.Image = afterPreviewImage;
            old?.Dispose();
        }

        private static Bitmap CreateResizedPreview(Bitmap source, int width, int height, ResizeResampling resampling)
        {
            Bitmap result = new Bitmap(Math.Max(1, width), Math.Max(1, height), PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.InterpolationMode = resampling == ResizeResampling.NearestNeighbor
                    ? InterpolationMode.NearestNeighbor
                    : resampling == ResizeResampling.Bilinear
                        ? InterpolationMode.HighQualityBilinear
                        : InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, new Rectangle(0, 0, result.Width, result.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel);
            }
            return result;
        }
    }
}
