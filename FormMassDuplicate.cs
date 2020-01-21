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
    public partial class FormMassDuplicate : Form
    {
        public int startRowID;
        public int endRowID;
        public int initialID;

        public FormMassDuplicate(int initialID)
        {
            InitializeComponent();
            startRowID = initialID;
            endRowID = initialID + 10;

            startIDInput.Text = startRowID.ToString();
            endIDInput.Text = endRowID.ToString();

            DialogResult = DialogResult.Cancel;
        }

        private void duplicateRowsBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            bool isValid_startRowID = Int32.TryParse(startIDInput.Text, out startRowID);
            bool isValid_endRowID = Int32.TryParse(endIDInput.Text, out endRowID);

            if (!isValid_startRowID)
                startRowID = initialID;

            if (!isValid_endRowID)
                endRowID = initialID + 10;

            Close();
        }

        private void FormMassDuplicate_Load(object sender, EventArgs e)
        {

        }
    }
}
