using System.Drawing;
using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager.UI.Theming
{
    /// <summary>
    /// Centralized visual theme for the application. Keeping colors and control
    /// styling here avoids scattering UI constants through Designer files.
    /// </summary>
    internal static class DarkTheme
    {
        public static readonly Color Window = Color.FromArgb(18, 18, 18);
        public static readonly Color Surface = Color.FromArgb(27, 27, 27);
        public static readonly Color SurfaceRaised = Color.FromArgb(36, 36, 36);
        public static readonly Color SurfaceHover = Color.FromArgb(48, 48, 48);
        public static readonly Color Border = Color.FromArgb(62, 62, 62);
        public static readonly Color Foreground = Color.FromArgb(232, 232, 232);
        public static readonly Color Muted = Color.FromArgb(170, 170, 170);
        public static readonly Color Selection = Color.FromArgb(70, 70, 70);

        private static readonly Font DefaultFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        public static void Apply(Form form, params ToolStrip[] toolStrips)
        {
            form.BackColor = Window;
            form.ForeColor = Foreground;
            form.Font = DefaultFont;
            form.MinimumSize = new Size(900, 600);

            ApplyToChildren(form.Controls);

            if (toolStrips != null)
            {
                foreach (ToolStrip strip in toolStrips)
                {
                    if (strip != null)
                    {
                        StyleToolStrip(strip);
                    }
                }
            }
        }

        public static void ApplyDialog(Form form)
        {
            form.BackColor = Window;
            form.ForeColor = Foreground;
            form.Font = DefaultFont;
            ApplyToChildren(form.Controls);
        }

        private static void ApplyToChildren(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                StyleControl(control);

                if (control.HasChildren)
                {
                    ApplyToChildren(control.Controls);
                }
            }
        }

        private static void StyleControl(Control control)
        {
            control.Font = DefaultFont;

            if (control is DataGridView)
            {
                StyleGrid((DataGridView)control);
                return;
            }

            if (control is Button)
            {
                Button button = (Button)control;
                button.BackColor = SurfaceRaised;
                button.ForeColor = Foreground;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.MouseOverBackColor = SurfaceHover;
                button.FlatAppearance.MouseDownBackColor = Selection;
                button.UseVisualStyleBackColor = false;
                return;
            }

            if (control is NumericUpDown)
            {
                NumericUpDown numeric = (NumericUpDown)control;
                numeric.BackColor = SurfaceRaised;
                numeric.ForeColor = Foreground;
                numeric.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            if (control is TextBoxBase)
            {
                TextBoxBase textBox = (TextBoxBase)control;
                textBox.BackColor = SurfaceRaised;
                textBox.ForeColor = Foreground;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                return;
            }

            if (control is ComboBox)
            {
                ComboBox combo = (ComboBox)control;
                combo.BackColor = SurfaceRaised;
                combo.ForeColor = Foreground;
                combo.FlatStyle = FlatStyle.Flat;
                return;
            }

            if (control is GroupBox)
            {
                control.BackColor = Surface;
                control.ForeColor = Foreground;
                return;
            }

            if (control is PictureBox)
            {
                control.BackColor = Color.FromArgb(12, 12, 12);
                return;
            }

            if (control is Panel || control is SplitContainer || control is TabPage)
            {
                control.BackColor = Surface;
                control.ForeColor = Foreground;
                return;
            }

            if (control is Label || control is CheckBox || control is RadioButton || control is LinkLabel)
            {
                control.ForeColor = Foreground;
                if (!(control is LinkLabel))
                {
                    control.BackColor = Color.Transparent;
                }
                else
                {
                    LinkLabel link = (LinkLabel)control;
                    link.LinkColor = Color.FromArgb(190, 190, 190);
                    link.ActiveLinkColor = Color.White;
                    link.VisitedLinkColor = Color.FromArgb(150, 150, 150);
                }
                return;
            }

            if (control is ListBox || control is CheckedListBox)
            {
                control.BackColor = SurfaceRaised;
                control.ForeColor = Foreground;
            }
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Window;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = true;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 32;
            grid.RowTemplate.Height = 28;

            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Foreground;
            grid.DefaultCellStyle.SelectionBackColor = Selection;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(31, 31, 31);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Foreground;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Selection;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceRaised;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Foreground;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceRaised;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Foreground;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(DefaultFont, FontStyle.Bold);
        }

        private static void StyleToolStrip(ToolStrip strip)
        {
            strip.BackColor = SurfaceRaised;
            strip.ForeColor = Foreground;
            strip.Font = DefaultFont;
            strip.RenderMode = ToolStripRenderMode.Professional;
            strip.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable());

            foreach (ToolStripItem item in strip.Items)
            {
                StyleToolStripItem(item);
            }
        }

        public static void ApplyToToolStripItem(ToolStripItem item)
        {
            if (item != null)
            {
                StyleToolStripItem(item);
            }
        }

        private static void StyleToolStripItem(ToolStripItem item)
        {
            item.BackColor = SurfaceRaised;
            item.ForeColor = Foreground;

            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem != null)
            {
                dropDownItem.DropDown.BackColor = SurfaceRaised;
                dropDownItem.DropDown.ForeColor = Foreground;
                dropDownItem.DropDown.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable());

                foreach (ToolStripItem child in dropDownItem.DropDownItems)
                {
                    StyleToolStripItem(child);
                }
            }
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground { get { return SurfaceRaised; } }
            public override Color ImageMarginGradientBegin { get { return SurfaceRaised; } }
            public override Color ImageMarginGradientMiddle { get { return SurfaceRaised; } }
            public override Color ImageMarginGradientEnd { get { return SurfaceRaised; } }
            public override Color MenuBorder { get { return Border; } }
            public override Color MenuItemBorder { get { return Border; } }
            public override Color MenuItemSelected { get { return SurfaceHover; } }
            public override Color MenuItemSelectedGradientBegin { get { return SurfaceHover; } }
            public override Color MenuItemSelectedGradientEnd { get { return SurfaceHover; } }
            public override Color MenuItemPressedGradientBegin { get { return Selection; } }
            public override Color MenuItemPressedGradientMiddle { get { return Selection; } }
            public override Color MenuItemPressedGradientEnd { get { return Selection; } }
            public override Color ToolStripBorder { get { return Border; } }
            public override Color ToolStripGradientBegin { get { return SurfaceRaised; } }
            public override Color ToolStripGradientMiddle { get { return SurfaceRaised; } }
            public override Color ToolStripGradientEnd { get { return SurfaceRaised; } }
            public override Color StatusStripGradientBegin { get { return SurfaceRaised; } }
            public override Color StatusStripGradientEnd { get { return SurfaceRaised; } }
            public override Color SeparatorDark { get { return Border; } }
            public override Color SeparatorLight { get { return SurfaceHover; } }
        }
    }
}
