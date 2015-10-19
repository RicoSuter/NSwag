//-----------------------------------------------------------------------
// <copyright file="ObservableDictionary.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace NSwag.Collections
{
    /// <summary>An implementation of an observable dictionary.</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>, INotifyCollectionChanged,
        INotifyPropertyChanged, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _dictionary;

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        public ObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="dictionary">The dictionary to initialize this dictionary. </param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="comparer">The comparer. </param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="capacity">The capacity. </param>
        public ObservableDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="dictionary">The dictionary to initialize this dictionary. </param>
        /// <param name="comparer">The comparer. </param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class. </summary>
        /// <param name="capacity">The capacity. </param>
        /// <param name="comparer">The comparer. </param>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        /// <summary>Gets the underlying dictonary. </summary>
        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
        }

        /// <summary>Adds multiple key-value pairs the the dictionary. </summary>
        /// <param name="items">The key-value pairs. </param>
        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (items.Count > 0)
            {
                if (Dictionary.Count > 0)
                {
                    if (items.Keys.Any(k => Dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");

                    foreach (var item in items)
                        Dictionary.Add(item);
                }
                else
                    _dictionary = new Dictionary<TKey, TValue>(items);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }

        /// <summary>Inserts a key-value pair into the dictionary. </summary>
        /// <param name="key">The key. </param>
        /// <param name="value">The value. </param>
        /// <param name="add">If true and key already exists then an exception is thrown. </param>
        protected virtual void Insert(TKey key, TValue value, bool add)
        {
            TValue item;
            if (Dictionary.TryGetValue(key, out item))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.");

                if (Equals(item, value))
                    return;

                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        /// <summary>Called when the property has changed.</summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
                copy(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Called when the collection has changed.</summary>
        protected void OnCollectionChanged()
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>Called when the collection has changed.</summary>
        /// <param name="action">The action.</param>
        /// <param name="changedItem">The changed item.</param>
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(action, changedItem, 0));
        }

        /// <summary>Called when the collection has changed.</summary>
        /// <param name="action">The action.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, 0));
        }

        /// <summary>Called when the collection has changed.</summary>
        /// <param name="action">The action.</param>
        /// <param name="newItems">The new items.</param>
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            var copy = CollectionChanged;
            if (copy != null)
                copy(this, new NotifyCollectionChangedEventArgs(action, newItems, 0));
        }

        private void OnPropertyChanged()
        {
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
        }

        #region IDictionary<TKey,TValue> interface

        /// <summary>Adds the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <summary>Determines whether the specified key contains key.</summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        /// <summary>Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        public ICollection<TKey> Keys
        {
            get { return Dictionary.Keys; }
        }

        ICollection IDictionary.Values { get { return ((IDictionary)Dictionary).Values; } }

        ICollection IDictionary.Keys { get { return ((IDictionary)Dictionary).Keys; } }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        /// <summary>Removes the specified key.</summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">key</exception>
        public virtual bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            TValue value;
            Dictionary.TryGetValue(key, out value);

            var removed = Dictionary.Remove(key);
            if (removed)
                OnCollectionChanged();
            //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
            return removed;
        }

        /// <summary>Tries the get value.</summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        /// <summary>Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</summary>
        public ICollection<TValue> Values
        {
            get { return Dictionary.Values; }
        }

        /// <summary>Gets or sets the TValue with the specified key.</summary>
        /// <value>The TValue.</value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get { return Dictionary[key]; }
            set { Insert(key, value, false); }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> interface

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        void IDictionary.Add(object key, object value)
        {
            Insert((TKey)key, (TValue)value, true);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnCollectionChanged();
            }
        }

        /// <summary>Initializes the specified key value pairs.</summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        public void Initialize(IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            var pairs = keyValuePairs.ToList();
            foreach (var pair in pairs)
                Dictionary[pair.Key] = pair.Value;

            foreach (var key in Dictionary.Keys.Where(k => !pairs.Any(p => Equals(p.Key, k))).ToArray())
                Dictionary.Remove(key);

            OnCollectionChanged();
        }

        /// <summary>Initializes the specified key value pairs.</summary>
        /// <param name="keyValuePairs">The key value pairs.</param>
        public void Initialize(IEnumerable keyValuePairs)
        {
            Initialize(keyValuePairs.Cast<KeyValuePair<TKey, TValue>>());
        }

        /// <summary>Determines whether [contains] [the specified key].</summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        /// <summary>Removes the specified key.</summary>
        /// <param name="key">The key.</param>
        public void Remove(object key)
        {
            Remove((TKey)key);
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.</summary>
        public bool IsFixedSize { get { return false; } }

        /// <summary>Determines whether [contains] [the specified item].</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        /// <summary>Copies to.</summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>Copies to.</summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Array array, int index)
        {
            ((IDictionary)Dictionary).CopyTo(array, index);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        public int Count
        {
            get { return Dictionary.Count; }
        }

        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
        public bool IsSynchronized { get; private set; }

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
        public object SyncRoot { get; private set; }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        public bool IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }

        object IDictionary.this[object key]
        {
            get { return this[(TKey)key]; }
            set { this[(TKey)key] = (TValue)value; }
        }

        /// <summary>Removes the specified item.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> interface

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable interface

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged interface

        /// <summary>Occurs when the collection has changed.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged interface

        /// <summary>Occurs when a property has changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

