using System;
using System.Drawing;
using System.Windows.Forms;
using RE4_PS2_TPL_Manager.UI.Theming;

namespace RE4_PS2_TPL_Manager.Dialog
{
    public enum BatchColorDepthMode
    {
        Preserve,
        Force4Bit,
        Force8Bit
    }

    public sealed class DialogBatchReplaceMode : Form
    {
        private readonly RadioButton radioPreserve;
        private readonly RadioButton radio4Bit;
        private readonly RadioButton radio8Bit;
        private readonly Label description;
        private readonly Button btnStart;
        private readonly Button btnCancel;

        public BatchColorDepthMode SelectedMode
        {
            get
            {
                if (radio4Bit.Checked) return BatchColorDepthMode.Force4Bit;
                if (radio8Bit.Checked) return BatchColorDepthMode.Force8Bit;
                return BatchColorDepthMode.Preserve;
            }
        }

        public DialogBatchReplaceMode()
        {
            Text = "Batch Replace - Color Depth";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            ClientSize = new Size(540, 355);

            var title = new Label
            {
                AutoSize = false,
                Location = new Point(20, 17),
                Size = new Size(500, 42),
                Text = "Choose how indexed PNG files should be converted during Batch Replace.",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            var inputHint = new Label
            {
                AutoSize = false,
                Location = new Point(24, 61),
                Size = new Size(492, 42),
                Text = "Input: PNG only. Name each image with the texture index, for example: 0.png, 1.png, 25.png.",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            radioPreserve = new RadioButton
            {
                AutoSize = true,
                Location = new Point(24, 114),
                Text = "Preserve TPL color depth (recommended)",
                Checked = true
            };
            radio4Bit = new RadioButton
            {
                AutoSize = true,
                Location = new Point(24, 145),
                Text = "Force 4-bit (16 colors)"
            };
            radio8Bit = new RadioButton
            {
                AutoSize = true,
                Location = new Point(24, 176),
                Text = "Force 8-bit (256 colors)"
            };

            description = new Label
            {
                AutoSize = false,
                Location = new Point(24, 214),
                Size = new Size(492, 58),
                Text = GetDescription(BatchColorDepthMode.Preserve)
            };

            btnStart = new Button
            {
                Location = new Point(318, 300),
                Size = new Size(106, 34),
                Text = "Start Batch",
                DialogResult = DialogResult.OK
            };
            btnCancel = new Button
            {
                Location = new Point(430, 300),
                Size = new Size(86, 34),
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };

            radioPreserve.CheckedChanged += ModeChanged;
            radio4Bit.CheckedChanged += ModeChanged;
            radio8Bit.CheckedChanged += ModeChanged;

            Controls.Add(title);
            Controls.Add(inputHint);
            Controls.Add(radioPreserve);
            Controls.Add(radio4Bit);
            Controls.Add(radio8Bit);
            Controls.Add(description);
            Controls.Add(btnStart);
            Controls.Add(btnCancel);

            AcceptButton = btnStart;
            CancelButton = btnCancel;

            DarkTheme.ApplyDialog(this);
        }

        private void ModeChanged(object sender, EventArgs e)
        {
            description.Text = GetDescription(SelectedMode);
        }

        private static string GetDescription(BatchColorDepthMode mode)
        {
            switch (mode)
            {
                case BatchColorDepthMode.Force4Bit:
                    return "Every imported image is quantized to 16 colors / 4-bit, regardless of the destination texture's current color depth.";
                case BatchColorDepthMode.Force8Bit:
                    return "Every imported image is quantized to 256 colors / 8-bit, regardless of the destination texture's current color depth.";
                default:
                    return "Each image follows the destination texture: 4-bit targets remain 4-bit and 8-bit targets remain 8-bit.";
            }
        }
    }
}
