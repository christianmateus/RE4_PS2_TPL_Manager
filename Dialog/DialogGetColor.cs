using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager.Dialog
{
    public partial class DialogGetColor : Form
    {
        public int ColorCount { get; set; }
        public DialogGetColor()
        {
            InitializeComponent();
        }

        private void btn16_Click(object sender, System.EventArgs e)
        {
            ColorCount = 16;
            this.Close();
            this.Dispose();
        }

        private void btn256_Click(object sender, System.EventArgs e)
        {
            ColorCount = 256;
            this.Close();
            this.Dispose();
        }
    }
}
