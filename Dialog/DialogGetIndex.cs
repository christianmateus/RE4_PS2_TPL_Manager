using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager.Dialog
{
    public partial class DialogGetIndex : Form
    {
        public string tplFile;
        public DialogGetIndex()
        {
            InitializeComponent();
        }
        public int GetIndex()
        {
            return (int)spinIndexForReplace.Value;
        }
        public void btnConfirmIndex_Click(object sender, System.EventArgs e)
        {
            GetIndex();
            this.Close();
        }

        private void btnOpenNewInstance_Click(object sender, System.EventArgs e)
        {
            FrmMain frmMain = new FrmMain();
            frmMain.Show();
        }
    }
}
