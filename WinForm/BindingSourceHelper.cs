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
    /// Base class for classes which assist in various data binding,
    /// persistence and validation tasks related to use of BindingSource
    /// and PersistedBindingList objects on forms. This class handles persistence
    /// operations related to selecting or navigating to different objects
    /// in the PersistedBindingList, and validation and persistence performed on
    /// form closing. Subclasses or the caller must handle all else, including
    /// validation upon navigation or selection of different objects in the
    /// PersistedBindingList and binding particular list member properties to
    /// controls in the UI.
    /// </summary>
    /// <typeparam name="TPersistable"></typeparam>
    public abstract class BindingSourceHelper<TPersistable>
        where TPersistable : class, IPersistable
    {
        private event BindingSourceEntityEventHandler<TPersistable> mCurrentChanged;
        private bool mCurrentChangedRegistered;
        private TPersistable mCurrentEntity;
        private BindingSource mBindingSource;
        private PersistedBindingList<TPersistable> mEntityList;
        private Form mForm;

        /// <summary>
        /// Construct an object which uses the specified BindingSource
        /// on the specified Form.
        /// </summary>
        /// <param name="bindingSource">The BindingSource used to bind
        /// controls on the form.</param>
        /// <param name="form">The form being managed.</param>
        public BindingSourceHelper(BindingSource bindingSource, Form form)
        {
            mCurrentChanged = null;
            mCurrentEntity = null;
            mBindingSource = bindingSource;
            mCurrentChangedRegistered = false;
            mForm = form;
            mForm.FormClosing += FormClosingHandler;
        }

        /// <summary>
        /// The BindingSource passed to the constructor.
        /// </summary>
        protected BindingSource BindingSource
        {
            get { return mBindingSource; }
        }

        /// <summary>
        /// The data source for the form. The caller must NOT directly
        /// assign the DataSource of the BindingSource passed to the constructor,
        /// instead they must assign to this property. This imposes the further
        /// restriction that only an EntityBindingList may be used as a data source.
        /// </summary>
        public PersistedBindingList<TPersistable> DataSource
        {
            get
            {
                return mEntityList;
            }
            set
            {
                if (mCurrentChangedRegistered)
                    mBindingSource.CurrentChanged -= CurrentChangedHandler;
                mEntityList = value;
                mBindingSource.DataSource = mEntityList;
                mCurrentEntity = (TPersistable)mBindingSource.Current;
                mBindingSource.CurrentChanged += CurrentChangedHandler;
                mCurrentChangedRegistered = true;
                ConfigureUI();
                OnCurrentChanged(mCurrentEntity);
            }
        }

        /// <summary>
        /// Get or set the currently selected/viewed entity in the UI.
        /// </summary>
        public TPersistable CurrentEntity
        {
            get
            {
                return mCurrentEntity;
            }
            set
            {
                mBindingSource.MoveFirst();
                for (; ; )
                {
                    if (mCurrentEntity == value)
                        break;
                    mBindingSource.MoveNext();
                }
            }
        }

        /// <summary>
        /// Set mCurrentEntity to the current entity after a navigation event.
        /// Also commits the formerly current entity back to the database.
        /// Subclasses are responsible for validating the current Entity
        /// before navigating to a different one and preventing that navigation
        /// if the current Entity is invalid. I.e., this method assumes that
        /// if mCurrentEntity is not null it is valid.
        /// Deletes are handled in mEntityList.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void CurrentChangedHandler(object source, EventArgs args)
        {
            // This event can fire when loading a form even if the BindingSource
            // has not been bound to anything.
            if (mBindingSource.DataSource == null)
                return;
            TPersistable entity = (TPersistable)mBindingSource.Current;
            OnCurrentChanged(entity);
            if (entity == null)
                return;
            if (mCurrentEntity != null)
            {
                if (mCurrentEntity.IsDirty)
                {
                    mEntityList.Save(mCurrentEntity);
                    mCurrentEntity.IsDirty = false;
                }
            }
            mCurrentEntity = entity;
        }

        public event BindingSourceEntityEventHandler<TPersistable> CurrentChanged
        {
            add { mCurrentChanged += value; }
            remove { mCurrentChanged -= value; }
        }

        protected void OnCurrentChanged(TPersistable entity)
        {
            if (mCurrentChanged != null)
                mCurrentChanged(this, new BindingSourceEntityEventArgs<TPersistable>(entity));
        }

        /// <summary>
        /// Validate and persist the current entity when the form closes.
        /// Cannot persist an invalid entity, so asks the user whether they
        /// want to close form if the current entity is invalid. This is the
        /// only place this class performs validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (mCurrentEntity != null)
            {
                if (mCurrentEntity.IsDirty)
                {
                    ErrorList errors = new ErrorList();
                    SetCurrentFixedValuesAndValidate(errors);
                    if (errors.MaxSeverity > ErrorSeverity.Warning)
                    {
                        // Validation errors may be displayed before this event fires
                        // in some scenarios (e.g. DataGridView.RowValidating event).
                        if (ShowErrorsInFormClosing)
                            ErrorDisplayForm.Show(errors);
                        // Ask if want to discard changes.
                        string msg = string.Format("If you close the form you will lose " +
                            "edits to the current {0}. Do you wish to close the form?",
                            DataSource.EntityDisplayName);
                        DialogResult result = MessageBox.Show(msg, "Close Form", MessageBoxButtons.YesNo);
                        // NOTE: Empirically, on entry c.Cancel==true if there was a validation
                        // error in RowValidating, and c.Cancel==false if not. This suggests that
                        // DataGridView adds its own FormClosing event handler and prevents the
                        // form from closing if there is a validation error.
                        e.Cancel = (result != DialogResult.Yes);
                    }
                    else
                    {
                        // Save without asking.
                        DataSource.Save(CurrentEntity);
                    }
                }
            }
        }

        /// <summary>
        /// Do validation errors need to be displayed in the FormClosing event?
        /// Needed because some subclasses display validation errors on the current
        /// entity before the FormClosing event fires.
        /// </summary>
        protected abstract bool ShowErrorsInFormClosing
        { get; }

        /// <summary>
        /// Determine if it is okay to change this.DataSource, and if it is okay
        /// and the current record has unsaved changed then save that record.
        /// Is not okay if the current record is invalid.
        /// </summary>
        /// <returns></returns>
        public bool IsOkayToChangeDataSource(string operation)
        {
            if (mCurrentEntity != null)
            {
                if (mCurrentEntity.IsDirty)
                {
                    ErrorList errors = new ErrorList();
                    SetCurrentFixedValuesAndValidate(errors);
                    if (errors.MaxSeverity > ErrorSeverity.Warning)
                    {
                        ErrorDisplayForm.Show(errors);
                        // Ask if want to discard changes.
                        string msg = string.Format("If you {1} you will lose " +
                            "edits to the current {0}. Do you wish to {1}?",
                            DataSource.EntityDisplayName, operation);
                        DialogResult result = MessageBox.Show(msg, "Close Form", MessageBoxButtons.YesNo);
                        if (result != DialogResult.Yes)
                            return false;
                    }
                    else
                    {
                        // Save without asking.
                        DataSource.Save(CurrentEntity);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Let subclasses configure their UI according to position in the
        /// EntityBindingList, e.i. availability of next/prev entity.
        /// </summary>
        protected abstract void ConfigureUI();

        protected void ShowException(Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        /// <summary>
        /// Used by subclasses to validate the current entity if there is one,
        /// and show errors if any found.
        /// </summary>
        /// <returns></returns>
        public bool CurrentEntityIsValid()
        {
            EndEdit();
            ErrorList errors = new ErrorList();
            if (mCurrentEntity != null)
            {
                SetCurrentFixedValuesAndValidate(errors);
                if (errors.MaxSeverity > ErrorSeverity.Warning)
                {
                    ErrorDisplayForm.Show(errors);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Empirically, this flushes the current control back to the entity.
        /// </summary>
        public void EndEdit()
        {
            mBindingSource.EndEdit();
        }

        /// <summary>
        /// Validate mCurrentEntity, after setting any properties of it
        /// whose values are not set by the user in the UI.
        /// </summary>
        /// <param name="errors"></param>
        private void SetCurrentFixedValuesAndValidate(ErrorList errors)
        {
            SetCurrentFixedValues();
            mCurrentEntity.Validate(errors);
            AdditionalRowValidation(errors);
        }

        /// <summary>
        /// Subclasses can override this to do additional validation.
        /// </summary>
        /// <param name="errors"></param>
        protected virtual void AdditionalRowValidation(ErrorList errors)
        {
        }

        /// <summary>
        /// Set any fixed values in mCurrentEntity, for example properties
        /// whose value is known because they were used as filtering criteria
        /// when loading the PersistedBindingList and the UI does not allow
        /// those properties to be changed.
        /// </summary>
        /// <param name="errors"></param>
        protected virtual void SetCurrentFixedValues()
        {
        }
    }

    /// <summary>
    /// Delegate type for BindingSourceHelper events relating to a single entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void BindingSourceEntityEventHandler<T>(object sender,
        BindingSourceEntityEventArgs<T> e);

    /// <summary>
    /// EventArgs subclass for BindingSourceEntityEventHandler<>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingSourceEntityEventArgs<T> : EventArgs
    {
        private T mEntity;

        public BindingSourceEntityEventArgs(T entity)
        {
            mEntity = entity;
        }

        public T Entity
        {
            get { return mEntity; }
        }
    }
}
