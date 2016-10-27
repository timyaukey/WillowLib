using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Willowsoft.WillowLib.Data.Misc;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.WinForm
{
    /// <summary>
    /// Subclass BindingSourceHelper to be used with an EntityBindingList displayed
    /// on a DataGridView. Handles validation upon selection or navigation between
    /// grid rows. Subclasses or the caller are responsible for adding grid columns
    /// and binding them to the correct entity properties. There are helper methods
    /// subclasses or the caller can use to create and bind columns.
    /// </summary>
    /// <typeparam name="TPersistable"></typeparam>
    public class GridBindingHelper<TPersistable> : BindingSourceHelper<TPersistable>
        where TPersistable : class, IPersistable
    {
        private DataGridView mGrid;
        private int mCharWidth;

        public GridBindingHelper(BindingSource bindingSource, DataGridView grid, Form form)
            : base(bindingSource, form)
        {
            mGrid = grid;
            Size charSize = TextRenderer.MeasureText("a", mGrid.DefaultCellStyle.Font);
            mCharWidth = charSize.Width;
            mGrid.DataSource = bindingSource;
            EnableValidation();
            mGrid.AutoGenerateColumns = false;
        }

        protected override void ConfigureUI()
        {
        }

        private void CellValidatingHandler(object sender,
            DataGridViewCellValidatingEventArgs e)
        {
            string errorMsg = ValidateCell(mGrid.Columns[e.ColumnIndex], e.FormattedValue);
            if (errorMsg != null)
            {
                mGrid.Rows[e.RowIndex].ErrorText = errorMsg;
                e.Cancel = true;
            }
        }

        protected bool ValidInt32Cell(DataGridViewColumn currentColumn, DataGridViewColumn targetColumn, object value)
        {
            if (currentColumn == targetColumn)
            {
                int intValue;
                return Int32.TryParse(value.ToString(), out intValue);
            }
            return true;
        }

        protected bool ValidDecimalCell(DataGridViewColumn currentColumn, DataGridViewColumn targetColumn, object value)
        {
            if (currentColumn == targetColumn)
            {
                decimal decimalValue;
                return Decimal.TryParse(value.ToString(), System.Globalization.NumberStyles.Currency, null, out decimalValue);
            }
            return true;
        }

        protected bool ValidDateCell(DataGridViewColumn currentColumn, DataGridViewColumn targetColumn, object value)
        {
            if (currentColumn == targetColumn)
            {
                DateTime datetimeValue;
                bool parsable =
                    DateTime.TryParseExact(value.ToString(), "M/d/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out datetimeValue) ||
                    DateTime.TryParseExact(value.ToString(), "MM/dd/yyyy", null,
                        System.Globalization.DateTimeStyles.None, out datetimeValue);
                return parsable;
            }
            return true;
        }

        protected virtual string ValidateCell(DataGridViewColumn column, object value)
        {
            return null;
        }

        void CellEndEditHandler(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.   
            mGrid.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        private void RowValidatingHandler(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (CurrentEntity != null)
            {
                if (CurrentEntity.IsDirty)
                    e.Cancel = !CurrentEntityIsValid();
            }
        }

        private void RowDeletingHandler(object sender, DataGridViewRowCancelEventArgs e)
        {
            EndEdit();
            ErrorList errors = new ErrorList();
            if (CurrentEntity != null && CurrentEntity.IsPersisted)
            {
                ValidateDeleting(errors);
                if (errors.MaxSeverity > ErrorSeverity.Warning)
                {
                    ErrorDisplayForm.Show(errors);
                    e.Cancel = true;
                    return;
                }
                DialogResult dlgRes = MessageBox.Show("Are you sure you want to delete this row?",
                    "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dlgRes != DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        protected virtual void ValidateDeleting(ErrorList errors)
        {
        }

        private void RowAddedHandler(Object sender,
            DataGridViewRowEventArgs e)
        {
            RowAdded();
        }

        protected virtual void RowAdded()
        {
        }

        public void EnableValidation()
        {
            mGrid.CellValidating += CellValidatingHandler;
            mGrid.CellEndEdit += CellEndEditHandler;
            mGrid.RowValidating += RowValidatingHandler;
            mGrid.UserDeletingRow += RowDeletingHandler;
            mGrid.UserAddedRow += RowAddedHandler;
        }

        public void DisableValidation()
        {
            mGrid.CellValidating -= CellValidatingHandler;
            mGrid.CellEndEdit -= CellEndEditHandler;
            mGrid.RowValidating -= RowValidatingHandler;
            mGrid.UserDeletingRow -= RowDeletingHandler;
            mGrid.UserAddedRow -= RowAddedHandler;
        }

        protected override bool ShowErrorsInFormClosing
        {
            get { return false; }
        }

        protected DataGridViewTextBoxColumn AddTextBoxColumn(string propertyName, string columnTitle,
            int widthInChars, bool readOnly)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = propertyName;
            col.HeaderText = columnTitle;
            col.Width = widthInChars * mCharWidth;
            col.ReadOnly = readOnly;
            mGrid.Columns.Add(col);
            return col;
        }

        protected DataGridViewTextBoxColumn AddCurrencyColumn(string propertyName, string columnTitle,
            int widthInChars, bool readOnly)
        {
            DataGridViewTextBoxColumn col = AddTextBoxColumn(propertyName, columnTitle, widthInChars, readOnly);
            col.DefaultCellStyle.Format = "c";
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            return col;
        }

        protected DataGridViewTextBoxColumn AddDateColumn(string propertyName, string columnTitle,
            int widthInChars, bool readOnly)
        {
            DataGridViewTextBoxColumn col = AddTextBoxColumn(propertyName, columnTitle, widthInChars, readOnly);
            col.DefaultCellStyle.Format = "d";
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            return col;
        }

        protected DataGridViewTextBoxColumn AddIntegerColumn(string propertyName, string columnTitle,
            int widthInChars, bool readOnly)
        {
            DataGridViewTextBoxColumn col = AddTextBoxColumn(propertyName, columnTitle, widthInChars, readOnly);
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            return col;
        }

        protected DataGridViewCheckBoxColumn AddCheckBoxColumn(string propertyName, string columnTitle,
            int widthInChars, bool readOnly)
        {
            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
            col.DataPropertyName = propertyName;
            col.HeaderText = columnTitle;
            col.Width = widthInChars * mCharWidth;
            col.ReadOnly = readOnly;
            mGrid.Columns.Add(col);
            return col;
        }

        protected DataGridViewComboBoxColumn AddComboBoxColumn(string propertyName,
            string columnTitle, int widthInChars, bool readOnly,
            object dataSource, string displayMember, string valueMember)
        {
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.DataPropertyName = propertyName;
            col.HeaderText = columnTitle;
            col.Width = widthInChars * mCharWidth;
            col.ReadOnly = readOnly;
            col.AutoComplete = true;
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            col.DisplayMember = displayMember;
            col.ValueMember = valueMember;
            col.DataSource = dataSource;
            mGrid.Columns.Add(col);
            return col;
        }
    }
}
