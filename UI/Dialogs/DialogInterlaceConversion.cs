using System.Drawing;
using System.Windows.Forms;
using RE4_PS2_TPL_Manager.UI.Theming;

namespace RE4_PS2_TPL_Manager.Dialog
{
    public sealed class DialogInterlaceConversion : Form
    {
        private readonly RadioButton radioBgra;
        private readonly RadioButton radioPs2;

        public bool TargetPs2 => radioPs2.Checked;

        public DialogInterlaceConversion(string currentInterlace)
        {
            Text = "Convert Interlace";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            ClientSize = new Size(525, 285);

            var title = new Label
            {
                Location = new Point(20, 16), Size = new Size(485, 44),
                Text = "Lossless pixel-layout conversion",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            var current = new Label
            {
                Location = new Point(20, 58), Size = new Size(485, 25),
                Text = "Current: " + currentInterlace
            };
            var note = new Label
            {
                Location = new Point(20, 84), Size = new Size(485, 65),
                Text = "Changes the physical pixel-index layout without re-quantizing the image or rebuilding the CLUT. The Normal/Inverted flag is preserved automatically (0↔2 or 1↔3)."
            };

            radioBgra = new RadioButton { Location = new Point(24, 158), AutoSize = true, Text = "Convert to BGRA / linear family" };
            radioPs2 = new RadioButton { Location = new Point(24, 188), AutoSize = true, Text = "Convert to PS2 / swizzled family" };

            var btnConvert = new Button { Location = new Point(320, 234), Size = new Size(100, 34), Text = "Convert", DialogResult = DialogResult.OK };
            var btnCancel = new Button { Location = new Point(426, 234), Size = new Size(79, 34), Text = "Cancel", DialogResult = DialogResult.Cancel };

            Controls.Add(title); Controls.Add(current); Controls.Add(note); Controls.Add(radioBgra); Controls.Add(radioPs2); Controls.Add(btnConvert); Controls.Add(btnCancel);
            AcceptButton = btnConvert;
            CancelButton = btnCancel;
            bool currentlyPs2 = currentInterlace.StartsWith("PS2");
            radioBgra.Checked = currentlyPs2;
            radioPs2.Checked = !currentlyPs2;
            DarkTheme.ApplyDialog(this);
        }
    }
}
