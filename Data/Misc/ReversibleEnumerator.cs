using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Willowsoft.WillowLib.Data.Misc
{
    // An Enumerator<T> plus a MovePrevious() method with the
    // same semantics as MoveNext() but in the opposite direction.
    // WARNING: THIS CLASS IS ENTIRELY UNTESTED. BUYER BEWARE.

    public class ReversibleEnumerator<T> : IEnumerator<T>
        where T : class
    {
        private IEnumerator<T> mEnumer;
        private LinkedList<T> mPrevious;    // .First is the nearest T in the reverse direction.
        private LinkedList<T> mNext;        // .First is the nearest T in the forward direction.
        private T mCurrent;                 // Not null if there is a current element.
        private bool mDisposed;             // True iff Dispose(bool) has already been called.

        public ReversibleEnumerator(IEnumerator<T> enumer)
        {
            mEnumer = enumer;
            mPrevious = new LinkedList<T>();
            mNext = new LinkedList<T>();
            mCurrent = null;
            throw new NotImplementedException("This class has never been used or tested");
        }

        public T Current
        {
            get
            {
                if (mCurrent!=null)
                    return mCurrent;
                else
                    throw new InvalidOperationException();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    mPrevious = null;
                    mNext = null;
                }
                mEnumer.Dispose();
                mDisposed = true;
            }
        }

        ~ReversibleEnumerator()
        {
            Dispose(false);
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (mCurrent != null)
                    return mCurrent;
                else
                    throw new InvalidOperationException();
            }
        }

        public bool MoveNext()
        {
            if (mNext.Count > 0)
            {
                if (mCurrent != null)
                    mPrevious.AddFirst(mCurrent);
                LinkedListNode<T> first = mNext.First;
                mNext.Remove(first);
                mCurrent = first.Value;
                return true;
            }
            if (mEnumer.MoveNext())
            {
                if (mCurrent != null)
                    mPrevious.AddFirst(mCurrent);
                mCurrent = mEnumer.Current;
                return true;
            }
            mCurrent = null;
            return false;
        }

        public bool MovePrevious()
        {
            if (mPrevious.Count > 0)
            {
                if (mCurrent != null)
                    mNext.AddFirst(mCurrent);
                LinkedListNode<T> first = mPrevious.First;
                mPrevious.Remove(first);
                mCurrent = first.Value;
                return true;
            }
            mCurrent = null;
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
