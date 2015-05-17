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
    /// Subclass BindingSourceHelper for forms which bind individual
    /// controls to a BindingSource, use a BindingNavigator,
    /// and do not use a grid.
    /// </summary>
    /// <typeparam name="TPersistable"></typeparam>
    public class BindingNavigatorHelper<TPersistable> : BindingSourceHelper<TPersistable>
        where TPersistable : class, IPersistable
    {
        #region BindingNavigator child controls.

        ToolStripItem mMoveNext;
        ToolStripItem mMovePrevious;
        ToolStripItem mMoveFirst;
        ToolStripItem mMoveLast;
        ToolStripItem mAddNew;

        #endregion

        public BindingNavigatorHelper(BindingSource bindingSource, Form form)
            : base(bindingSource, form)
        {
        }

        protected override bool ShowErrorsInFormClosing
        {
            get { return true; }
        }

        /// <summary>
        /// Bypass normal processing for the navigation buttons on the
        /// BindingNavigator by providing our own button click event handlers
        /// for all the buttons. Those event handlers validate the current
        /// entity, and only navigate off that entity if it is valid.
        /// </summary>
        /// <param name="navigator"></param>
        public void AddValidation(BindingNavigator navigator)
        {
            mMoveNext = navigator.MoveNextItem;
            navigator.MoveNextItem = null;
            mMoveNext.Click += ClickMoveNext;

            mMovePrevious = navigator.MovePreviousItem;
            navigator.MovePreviousItem = null;
            mMovePrevious.Click += ClickMovePrevious;

            mMoveFirst = navigator.MoveFirstItem;
            navigator.MoveFirstItem = null;
            mMoveFirst.Click += ClickMoveFirst;

            mMoveLast = navigator.MoveLastItem;
            navigator.MoveLastItem = null;
            mMoveLast.Click += ClickMoveLast;

            mAddNew = navigator.AddNewItem;
            navigator.AddNewItem = null;
            mAddNew.Click += ClickAddNew;
        }

        private void ClickMoveNext(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEntityIsValid())
                {
                    BindingSource.MoveNext();
                    ConfigureUI();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ClickMovePrevious(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEntityIsValid())
                {
                    BindingSource.MovePrevious();
                    ConfigureUI();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ClickMoveFirst(object sender, EventArgs e)
        {
            try
            {

                if (CurrentEntityIsValid())
                {
                    BindingSource.MoveFirst();
                    ConfigureUI();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ClickMoveLast(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEntityIsValid())
                {
                    BindingSource.MoveLast();
                    ConfigureUI();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ClickAddNew(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEntityIsValid())
                {
                    BindingSource.AddNew();
                    ConfigureUI();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        override protected void ConfigureUI()
        {
            mMoveFirst.Enabled = (BindingSource.Position > 0);
            mMovePrevious.Enabled = mMoveFirst.Enabled;
            mMoveNext.Enabled = (BindingSource.Position < (DataSource.Count - 1));
            mMoveLast.Enabled = mMoveNext.Enabled;
        }

    }
}
