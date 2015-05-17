using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.WinForm
{
    /// <summary>
    /// Wrapper around a ListView control to make it convenient to
    /// display lists of arbitrary objects. I'd prefer to subclass
    /// ListView, but the IDE and designer don't play nice with
    /// generic control classes.
    /// 
    /// To use on a form do the following:
    /// 1) Subclass for your object type, e.g. MyEntityViewer. The
    ///    MyEntityViewer constructor must take a ListView as an arg
    ///    and pass it to the base class constructor.
    /// 2) Create a private field in your form class of type MyEntityViewer.
    /// 3) Assign this field to a new instance immediately after calling
    ///    InitializeComponent() in the form constructor, passing the
    ///    ListView control on the form to the MyEntityViewer constructor.
    /// 4) Use ListViewExtender properties to access objects displayed or
    ///    seleced in the ListView, or use the ListView directly to do
    ///    other things.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListViewExtender<T>
        where T : class
    {
        private ListView mLvw;

        public ListViewExtender(ListView lvw)
        {
            mLvw = lvw;
            lvw.View = View.Details;
            lvw.MultiSelect = true;
            SetupListView(mLvw);
        }

        public ListView Lvw
        {
            get { return mLvw; }
        }

        public void SetItems(IEnumerable<T> entities)
        {
            mLvw.Items.Clear();
            foreach (T entity in entities)
            {
                ListViewItem item = new EntityListViewItem(entity, GetItemValues(entity));
                mLvw.Items.Add(item);
            }
        }

        public IList<T> SelectedItems
        {
            get
            {
                IList<T> results = new List<T>();
                foreach (ListViewItem item in mLvw.SelectedItems)
                {
                    T entity = ((EntityListViewItem)item).Entity;
                    results.Add(entity);
                }
                return results;
            }
        }

        public T SelectedItem
        {
            get
            {
                IList<T> results = SelectedItems;
                if (results.Count > 0)
                    return results[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Configure the ListView for type T. Generally all that will
        /// be necessary is to add columns corresponding to the data returned
        /// by GetItemValues(). The ListView will already be set to "details"
        /// view and "multi-select" before reaching here. You can lvw.MultiSelect
        /// if you like, but you should not change lvw.View.
        /// </summary>
        /// <param name="lvw"></param>
        protected abstract void SetupListView(ListView lvw);

        /// <summary>
        /// Return an array of strings to display in one row in "details" view
        /// for the specified T. SetupListView() must be implemented
        /// in the subclass to add corresponding columns.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract string[] GetItemValues(T entity);

        private class EntityListViewItem : ListViewItem
        {
            private T mEntity;

            public EntityListViewItem(T entity, string[] columnValues)
                : base(columnValues)
            {
                mEntity = entity;
            }

            public T Entity
            {
                get { return mEntity; }
            }
        }
    }
}
