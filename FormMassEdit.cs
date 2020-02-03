using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormMassEdit : Form
    {
        public FormMassEdit(DataGridView dgvRows)
        {
            InitializeComponent();
        }
        private void FormMassEdit_Load(object sender, EventArgs e)
        {

        }

        private void fieldSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void multiplyCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void valueBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
