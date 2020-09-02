﻿using Semver;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CellType = SoulsFormats.PARAM.CellType;
using GameType = Yapped.GameMode.GameType;

namespace Yapped
{
    public partial class FormMain : Form
    {
        private const string UPDATE_URL = "https://www.nexusmods.com/sekiro/mods/121?tab=files";
        private static Properties.Settings settings = Properties.Settings.Default;

        private string regulationPath;
        private IBinder regulation;
        private bool encrypted;
        private BindingSource rowSource;
        private Dictionary<string, (int Row, int Cell)> dgvIndices;
        private string lastFindRowPattern, lastFindFieldPattern;

        public FormMain()
        {
            InitializeComponent();
            regulation = null;
            rowSource = new BindingSource();
            dgvIndices = new Dictionary<string, (int Row, int Cell)>();
            dgvRows.DataSource = rowSource;
            dgvParams.AutoGenerateColumns = false;
            dgvRows.AutoGenerateColumns = false;
            dgvCells.AutoGenerateColumns = false;
            lastFindRowPattern = "";
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            Text = "Yapped - Honey Bear Edition";

            Location = settings.WindowLocation;
            if (settings.WindowSize.Width >= MinimumSize.Width && settings.WindowSize.Height >= MinimumSize.Height)
                Size = settings.WindowSize;
            if (settings.WindowMaximized)
                WindowState = FormWindowState.Maximized;

            toolStripComboBoxGame.ComboBox.DisplayMember = "Name";
            toolStripComboBoxGame.Items.AddRange(GameMode.Modes);
            var game = (GameType)Enum.Parse(typeof(GameType), settings.GameType);
            toolStripComboBoxGame.SelectedIndex = Array.FindIndex(GameMode.Modes, m => m.Game == game);
            if (toolStripComboBoxGame.SelectedIndex == -1)
                toolStripComboBoxGame.SelectedIndex = 0;

            regulationPath = settings.RegulationPath;
            hideUnusedParamsToolStripMenuItem.Checked = settings.HideUnusedParams;
            verifyDeletionsToolStripMenuItem.Checked = settings.VerifyRowDeletion;
            splitContainer2.SplitterDistance = settings.SplitterDistance2;
            splitContainer1.SplitterDistance = settings.SplitterDistance1;

            LoadParams();

            foreach (Match match in Regex.Matches(settings.DGVIndices, @"[^,]+"))
            {
                string[] components = match.Value.Split(':');
                dgvIndices[components[0]] = (int.Parse(components[1]), int.Parse(components[2]));
            }

            if (settings.SelectedParam >= dgvParams.Rows.Count)
                settings.SelectedParam = 0;

            if (dgvParams.Rows.Count > 0)
            {
                dgvParams.ClearSelection();
                dgvParams.Rows[settings.SelectedParam].Selected = true;
                dgvParams.CurrentCell = dgvParams.SelectedCells[0];
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.WindowMaximized = WindowState == FormWindowState.Maximized;
            if (WindowState == FormWindowState.Normal)
            {
                settings.WindowLocation = Location;
                settings.WindowSize = Size;
            }
            else
            {
                settings.WindowLocation = RestoreBounds.Location;
                settings.WindowSize = RestoreBounds.Size;
            }

            settings.GameType = ((GameMode)toolStripComboBoxGame.SelectedItem).Game.ToString();
            settings.RegulationPath = regulationPath;
            settings.HideUnusedParams = hideUnusedParamsToolStripMenuItem.Checked;
            settings.VerifyRowDeletion = verifyDeletionsToolStripMenuItem.Checked;
            settings.SplitterDistance2 = splitContainer2.SplitterDistance;
            settings.SplitterDistance1 = splitContainer1.SplitterDistance;

            if (dgvParams.SelectedCells.Count > 0)
                settings.SelectedParam = dgvParams.SelectedCells[0].RowIndex;

            // Force saving the dgv indices
            dgvParams.ClearSelection();

            var components = new List<string>();
            foreach (string key in dgvIndices.Keys)
                components.Add($"{key}:{dgvIndices[key].Row}:{dgvIndices[key].Cell}");
            settings.DGVIndices = string.Join(",", components);
        }

        private void LoadParams()
        {
            string resDir = GetResRoot();
            Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
            Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
            
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, hideUnusedParamsToolStripMenuItem.Checked);
        
            if (result == null)
            {
                exportToolStripMenuItem.Enabled = false;
            }
            else
            {
                encrypted = result.Encrypted;
                regulation = result.ParamBND;
                exportToolStripMenuItem.Enabled = encrypted;
                foreach (ParamWrapper wrapper in result.ParamWrappers)
                {
                    if (!dgvIndices.ContainsKey(wrapper.Name))
                        dgvIndices[wrapper.Name] = (0, 0);
                }
                dgvParams.DataSource = result.ParamWrappers;
                toolStripStatusLabel1.Text = regulationPath;

                foreach (DataGridViewRow row in dgvParams.Rows)
                {
                    var wrapper = (ParamWrapper)row.DataBoundItem;
                    if (wrapper.Error)
                        row.Cells[0].Style.BackColor = Color.Pink;
                }
            }
        }

        #region File Menu
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdRegulation.FileName = regulationPath;
            if (ofdRegulation.ShowDialog() == DialogResult.OK)
            {
                regulationPath = ofdRegulation.FileName;
                LoadParams();
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (BinderFile file in regulation.Files)
            {
                foreach (DataGridViewRow paramRow in dgvParams.Rows)
                {
                    ParamWrapper paramFile = (ParamWrapper)paramRow.DataBoundItem;
                    if (Path.GetFileNameWithoutExtension(file.Name) == paramFile.Name)
                        file.Bytes = paramFile.Param.Write();
                }
            }

            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            if (!File.Exists(regulationPath + ".bak"))
                File.Copy(regulationPath, regulationPath + ".bak");

            if (encrypted)
            {
                if (gameMode.Game == GameType.DarkSouls2)
                    Util.EncryptDS2Regulation(regulationPath, regulation as BND4);
                else if (gameMode.Game == GameType.DarkSouls3)
                    SFUtil.EncryptDS3Regulation(regulationPath, regulation as BND4);
                else
                    Util.ShowError("Encryption is only valid for DS2 and DS3.");
            }
            else
            {
                if (regulation is BND3 bnd3)
                    bnd3.Write(regulationPath);
                else if (regulation is BND4 bnd4)
                    bnd4.Write(regulationPath);
            }
            SystemSounds.Asterisk.Play();
        }

        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(regulationPath + ".bak"))
            {
                DialogResult choice = MessageBox.Show("Are you sure you want to restore the backup?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (choice == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy(regulationPath + ".bak", regulationPath, true);
                        LoadParams();
                        SystemSounds.Asterisk.Play();
                    }
                    catch (Exception ex)
                    {
                        Util.ShowError($"Failed to restore backup\r\n\r\n{regulationPath}.bak\r\n\r\n{ex}");
                    }
                }
            }
            else
            {
                Util.ShowError($"There is no backup to restore at:\r\n\r\n{regulationPath}.bak");
            }
        }

        private void ExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(regulationPath));
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }
        #endregion

        #region Edit Menu
        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateRow("Add a new row...");
        }

        private void DuplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count == 0)
            {
                Util.ShowError("You can't duplicate a row without one selected!");
                return;
            }

            int index = dgvRows.SelectedCells[0].RowIndex;
            ParamWrapper wrapper = (ParamWrapper)rowSource.DataSource;
            PARAM.Row oldRow = wrapper.Rows[index];
            PARAM.Row newRow;
            if ((newRow = CreateRow("Duplicate a row...")) != null)
            {
                for (int i = 0; i < oldRow.Cells.Count; i++)
                {
                    newRow.Cells[i].Value = oldRow.Cells[i].Value;
                }
            }
        }

        private PARAM.Row CreateRow(string prompt)
        {
            if (rowSource.DataSource == null)
            {
                Util.ShowError("You can't create a row with no param selected!");
                return null;
            }

            PARAM.Row result = null;
            var newRowForm = new FormNewRow(prompt);
            if (newRowForm.ShowDialog() == DialogResult.OK)
            {
                long id = newRowForm.ResultID;
                string name = newRowForm.ResultName;
                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;
                if (paramWrapper.Rows.Any(row => row.ID == id))
                {
                    Util.ShowError($"A row with this ID already exists: {id}");
                }
                else
                {
                    result = new PARAM.Row(id, name, paramWrapper.Layout);
                    rowSource.Add(result);
                    paramWrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                    int index = paramWrapper.Rows.FindIndex(row => ReferenceEquals(row, result));
                    int displayedRows = dgvRows.DisplayedRowCount(false);
                    dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                    dgvRows.ClearSelection();
                    dgvRows.Rows[index].Selected = true;
                    dgvRows.Refresh();
                }
            }
            return result;
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count > 0)
            {
                DialogResult choice = DialogResult.Yes;
                if (verifyDeletionsToolStripMenuItem.Checked)
                    choice = MessageBox.Show("Are you sure you want to delete this row?",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (choice == DialogResult.Yes)
                {
                    int rowIndex = dgvRows.SelectedCells[0].RowIndex;
                    rowSource.RemoveAt(rowIndex);

                    // If you remove a row it automatically selects the next one, but if you remove the last row
                    // it doesn't automatically select the previous one
                    if (rowIndex == dgvRows.RowCount)
                    {
                        if (dgvRows.RowCount > 0)
                            dgvRows.Rows[dgvRows.RowCount - 1].Selected = true;
                        else
                            dgvCells.DataSource = null;
                    }
                }
            }
        }

        private void ImportNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool replace = MessageBox.Show("If a row already has a name, would you like to skip it?\r\n" +
                "Click Yes to skip existing names.\r\nClick No to replace existing names.",
                "Importing Names", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            string namesDir = $@"{GetResRoot()}\Names";
            List<ParamWrapper> paramFiles = (List<ParamWrapper>)dgvParams.DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                if (File.Exists($@"{namesDir}\{paramFile.Name}.txt"))
                {
                    var names = new Dictionary<long, string>();
                    string nameStr = File.ReadAllText($@"{namesDir}\{paramFile.Name}.txt");
                    foreach (string line in Regex.Split(nameStr, @"\s*[\r\n]+\s*"))
                    {
                        if (line.Length > 0)
                        {
                            Match match = Regex.Match(line, @"^(\d+) (.+)$");
                            long id = long.Parse(match.Groups[1].Value);
                            string name = match.Groups[2].Value;
                            names[id] = name;
                        }
                    }

                    foreach (PARAM.Row row in paramFile.Param.Rows)
                    {
                        if (names.ContainsKey(row.ID))
                        {
                            if (replace || row.Name == null || row.Name == "")
                                row.Name = names[row.ID];
                        }
                    }
                }
            }

            dgvRows.Refresh();
        }

        private void ExportNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string namesDir = $@"{GetResRoot()}\Names";
            List<ParamWrapper> paramFiles = (List<ParamWrapper>)dgvParams.DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                StringBuilder sb = new StringBuilder();
                foreach (PARAM.Row row in paramFile.Param.Rows)
                {
                    string name = (row.Name ?? "").Trim();
                    if (name != "")
                    {
                        sb.AppendLine($"{row.ID} {name}");
                    }
                }

                try
                {
                    File.WriteAllText($@"{namesDir}\{paramFile.Name}.txt", sb.ToString());
                }
                catch (Exception ex)
                {
                    Util.ShowError($"Failed to write name file: {paramFile.Name}.txt\r\n\r\n{ex}");
                    break;
                }
            }
        }

        private void FindRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var findForm = new FormFind("Find row with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindRow(findForm.ResultPattern);
            }
        }

        private void FindNextRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindRow(lastFindRowPattern);
        }

        private void FindRow(string pattern)
        {
            if (rowSource.DataSource == null)
            {
                Util.ShowError("You can't search for a row when there are no rows!");
                return;
            }

            int startIndex = dgvRows.SelectedCells.Count > 0 ? dgvRows.SelectedCells[0].RowIndex + 1 : 0;
            List<PARAM.Row> rows = ((ParamWrapper)rowSource.DataSource).Rows;
            int index = -1;

            for (int i = 0; i < rows.Count; i++)
            {
                if ((rows[(startIndex + i) % rows.Count].Name ?? "").ToLower().Contains(pattern.ToLower()))
                {
                    index = (startIndex + i) % rows.Count;
                    break;
                }
            }

            if (index != -1)
            {
                int displayedRows = dgvRows.DisplayedRowCount(false);
                dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                dgvRows.ClearSelection();
                dgvRows.Rows[index].Selected = true;
                lastFindRowPattern = pattern;
            }
            else
            {
                Util.ShowError($"No row found matching: {pattern}");
            }
        }

        private void GotoRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var gotoForm = new FormGoto();
            if (gotoForm.ShowDialog() == DialogResult.OK)
            {
                if (rowSource.DataSource == null)
                {
                    Util.ShowError("You can't goto a row when there are no rows!");
                    return;
                }

                long id = gotoForm.ResultID;
                List<PARAM.Row> rows = ((ParamWrapper)rowSource.DataSource).Rows;
                int index = rows.FindIndex(row => row.ID == id);

                if (index != -1)
                {
                    int displayedRows = dgvRows.DisplayedRowCount(false);
                    dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                    dgvRows.ClearSelection();
                    dgvRows.Rows[index].Selected = true;
                }
                else
                {
                    Util.ShowError($"Row ID not found: {id}");
                }
            }
        }

        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var findForm = new FormFind("Find field with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindField(findForm.ResultPattern);
            }
        }

        private void FindNextFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindField(lastFindFieldPattern);
        }

        private void FindField(string pattern)
        {
            if (dgvCells.DataSource == null)
            {
                Util.ShowError("You can't search for a field when there are no fields!");
                return;
            }

            int startIndex = dgvCells.SelectedCells.Count > 0 ? dgvCells.SelectedCells[0].RowIndex + 1 : 0;
            var cells = (PARAM.Cell[])dgvCells.DataSource;
            int index = -1;

            for (int i = 0; i < cells.Length; i++)
            {
                if ((cells[(startIndex + i) % cells.Length].Name ?? "").ToLower().Contains(pattern.ToLower()))
                {
                    index = (startIndex + i) % cells.Length;
                    break;
                }
            }

            if (index != -1)
            {
                int displayedRows = dgvCells.DisplayedRowCount(false);
                dgvCells.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                dgvCells.ClearSelection();
                dgvCells.Rows[index].Selected = true;
                lastFindFieldPattern = pattern;
            }
            else
            {
                Util.ShowError($"No field found matching: {pattern}");
            }
        }
        #endregion

        private void UpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(UPDATE_URL);
        }

        #region dgvParams
        private void DgvParams_SelectionChanged(object sender, EventArgs e)
        {
            if (rowSource.DataSource != null)
            {
                ParamWrapper paramFile = (ParamWrapper)rowSource.DataSource;
                (int Row, int Cell) indices = (0, 0);

                if (dgvRows.SelectedCells.Count > 0)
                    indices.Row = dgvRows.SelectedCells[0].RowIndex;
                else if (dgvRows.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Row = dgvRows.FirstDisplayedScrollingRowIndex;

                if (dgvCells.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Cell = dgvCells.FirstDisplayedScrollingRowIndex;

                dgvIndices[paramFile.Name] = indices;
            }

            rowSource.DataSource = null;
            dgvCells.DataSource = null;
            if (dgvParams.SelectedCells.Count > 0)
            {
                ParamWrapper paramFile = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                // Yes, I need to set this again every time because it gets cleared out when you null the DataSource for some stupid reason
                rowSource.DataMember = "Rows";
                rowSource.DataSource = paramFile;
                (int Row, int Cell) indices = dgvIndices[paramFile.Name];

                if (indices.Row >= dgvRows.RowCount)
                    indices.Row = dgvRows.RowCount - 1;

                if (indices.Row < 0)
                    indices.Row = 0;

                dgvIndices[paramFile.Name] = indices;
                dgvRows.ClearSelection();
                if (dgvRows.RowCount > 0)
                {
                    dgvRows.FirstDisplayedScrollingRowIndex = indices.Row;
                    dgvRows.Rows[indices.Row].Selected = true;
                }
            }
        }

        private void DgvParams_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var paramWrapper = (ParamWrapper)dgvParams.Rows[e.RowIndex].DataBoundItem;
                e.ToolTipText = paramWrapper.Description;
            }
        }
        #endregion

        #region dgvRows
        private void DgvRows_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count > 0)
            {
                ParamWrapper paramFile = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                (int Row, int Cell) indices = dgvIndices[paramFile.Name];
                if (dgvCells.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Cell = dgvCells.FirstDisplayedScrollingRowIndex;

                PARAM.Row row = (PARAM.Row)dgvRows.SelectedCells[0].OwningRow.DataBoundItem;
                dgvCells.DataSource = row.Cells.Where(cell => cell.Type != CellType.dummy8).ToArray();

                if (indices.Cell >= dgvCells.RowCount)
                    indices.Cell = dgvCells.RowCount - 1;

                if (indices.Cell < 0)
                    indices.Cell = 0;

                dgvIndices[paramFile.Name] = indices;
                if (dgvCells.RowCount > 0)
                    dgvCells.FirstDisplayedScrollingRowIndex = indices.Cell;
            }
        }

        private void DgvRows_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // ID
            if (e.ColumnIndex == 0)
            {
                bool parsed = int.TryParse((string)e.FormattedValue, out int value);
                if (!parsed || value < 0)
                {
                    Util.ShowError("Row ID must be a positive integer.\r\nEnter a valid number or press Esc to cancel.");
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region dgvCells
        private void DgvCells_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvCells.Rows)
            {
                var cell = (PARAM.Cell)row.DataBoundItem;
                if (cell.Enum != null)
                {
                    var paramWrapper = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                    PARAM.Layout layout = paramWrapper.Layout;
                    PARAM.Enum pnum = layout.Enums[cell.Enum];
                    if (pnum.Any(v => v.Value.Equals(cell.Value)))
                    {
                        row.Cells[2] = new DataGridViewComboBoxCell
                        {
                            DataSource = pnum,
                            DisplayMember = "Name",
                            ValueMember = "Value",
                            ValueType = cell.Value.GetType()
                        };
                    }
                }
                else if (cell.Type == CellType.b8 || cell.Type == CellType.b16 || cell.Type == CellType.b32)
                {
                    row.Cells[2] = new DataGridViewCheckBoxCell();
                }
                else
                {
                    row.Cells[2].ValueType = cell.Value.GetType();
                }
            }
        }

        private void DgvCells_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            DataGridViewRow row = dgvCells.Rows[e.RowIndex];
            if (!(row.Cells[2] is DataGridViewComboBoxCell))
            {
                var cell = (PARAM.Cell)row.DataBoundItem;
                if (cell.Type == CellType.x8)
                {
                    e.Value = $"0x{e.Value:X2}";
                    e.FormattingApplied = true;
                }
                else if (cell.Type == CellType.x16)
                {
                    e.Value = $"0x{e.Value:X4}";
                    e.FormattingApplied = true;
                }
                else if (cell.Type == CellType.x32)
                {
                    e.Value = $"0x{e.Value:X8}";
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvCells_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            DataGridViewRow row = dgvCells.Rows[e.RowIndex];
            try
            {
                if (!(row.Cells[2] is DataGridViewComboBoxCell))
                {
                    var cell = (PARAM.Cell)row.DataBoundItem;
                    if (cell.Type == CellType.x8)
                        Convert.ToByte((string)e.FormattedValue, 16);
                    else if (cell.Type == CellType.x16)
                        Convert.ToUInt16((string)e.FormattedValue, 16);
                    else if (cell.Type == CellType.x32)
                        Convert.ToUInt32((string)e.FormattedValue, 16);
                }
            }
            catch
            {
                e.Cancel = true;
                dgvCells.EditingPanel.BackColor = Color.Pink;
                dgvCells.EditingControl.BackColor = Color.Pink;
                SystemSounds.Hand.Play();
            }
        }

        private void DgvCells_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            DataGridViewRow row = dgvCells.Rows[e.RowIndex];
            if (!(row.Cells[2] is DataGridViewComboBoxCell))
            {
                var cell = (PARAM.Cell)row.DataBoundItem;
                if (cell.Type == CellType.x8)
                {
                    e.Value = Convert.ToByte((string)e.Value, 16);
                    e.ParsingApplied = true;
                }
                else if (cell.Type == CellType.x16)
                {
                    e.Value = Convert.ToUInt16((string)e.Value, 16);
                    e.ParsingApplied = true;
                }
                else if (cell.Type == CellType.x32)
                {
                    e.Value = Convert.ToUInt32((string)e.Value, 16);
                    e.ParsingApplied = true;
                }
            }
        }

        private void DgvCells_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            if (dgvCells.EditingPanel != null)
            {
                dgvCells.EditingPanel.BackColor = Color.Pink;
                dgvCells.EditingControl.BackColor = Color.Pink;
            }
            SystemSounds.Hand.Play();
        }

        private void DgvCells_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 1)
            {
                var cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;
                e.ToolTipText = cell.Description;
            }
        }
        #endregion

        private void MassDuplicateRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var massDuplicateForm = new FormMassDuplicate(0);

            if (massDuplicateForm.ShowDialog() == DialogResult.OK)
            {
                if (dgvRows.SelectedCells.Count == 0)
                {
                    Util.ShowError("You can't duplicate a row without one selected!");
                    return;
                }

                int startID = massDuplicateForm.startRowID;
                int endID = massDuplicateForm.endRowID;
                int stepValue = massDuplicateForm.stepValue;

                int index = dgvRows.SelectedCells[0].RowIndex;
                ParamWrapper wrapper = (ParamWrapper)rowSource.DataSource;
                PARAM.Row oldRow = wrapper.Rows[index];

                for (int rowID = startID; rowID <= endID; rowID += stepValue)
                {
                    PARAM.Row newRow;
                    if ((newRow = CreateRow_MassDuplicate(rowID)) != null)
                    {
                        for (int j = 0; j < oldRow.Cells.Count; j++)
                        {
                            newRow.Cells[j].Value = oldRow.Cells[j].Value;
                        }
                    }
                }
            }
        }

        private PARAM.Row CreateRow_MassDuplicate(int rowID)
        {
            if (rowSource.DataSource == null)
            {
                Util.ShowError("You can't create a row with no param selected!");
                return null;
            }

            PARAM.Row result = null;
            ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;
            if (paramWrapper.Rows.Any(row => row.ID == rowID))
            {
                Util.ShowError($"A row with this ID already exists: {rowID}");
            }
            else
            {
                result = new PARAM.Row(rowID, "Blank", paramWrapper.Layout);
                rowSource.Add(result);
                paramWrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                int index = paramWrapper.Rows.FindIndex(row => ReferenceEquals(row, result));
                int displayedRows = dgvRows.DisplayedRowCount(false);
                dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                dgvRows.ClearSelection();
                dgvRows.Rows[index].Selected = true;
                dgvRows.Refresh();
            }
            return result;
        }

        private void massEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var massEditForm = new FormMassEdit(dgvRows);
        }

        private string GetResRoot()
        {
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            return $@"res\{gameMode.Directory}";
        }
    }
}
