using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FileCopyLib
{
   /// <summary>
   /// Interface for application-specific collections.
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public interface IBaseCollection<T> : ICollection<T>
   {
      /// <summary>
      /// Gets or sets the object at the specified index.
      /// </summary>
      /// <param name="index">Index of the object to get or set.</param>
      /// <returns>Object at the specified index.</returns>
      T this[int index]
      {
         get;
         set;
      }

      /// <summary>
      /// Inserts an object at the specified index.
      /// </summary>
      /// <param name="item">Item to insert.</param>
      /// <param name="index">Index to insert the object at.</param>
      void InsertAt( T item, int index );

      /// <summary>
      /// Removes the object at the specified index.
      /// </summary>
      /// <param name="index">Index of the object to remove.</param>
      void RemoveAt( int index );

      /// <summary>
      /// Adds a collection of objects.
      /// </summary>
      /// <param name="collection">Collection to add.</param>
      void AddRange( IEnumerable<T> collection );
   }

   /// <summary>
   /// Provides a default implementation of IBaseCollection&lt;T&gt;.
   /// </summary>
   /// <typeparam name="T">Arbitrary type.</typeparam>
   public abstract class BaseCollection<T> : IBaseCollection<T>
   {
      /// <summary>
      /// Event raised when items are added.
      /// </summary>
      public event ItemsAddedHandler<T> ItemAdded;

      /// <summary>
      /// Event raised when items are removed.
      /// </summary>
      public event ItemsRemovedHandler<T> ItemRemoved;

      /// <summary>
      /// List that is wrapped by the collection class.
      /// </summary>
      private List<T> list = new List<T>();

      /// <summary>
      /// Creates an object of type BaseCollection.
      /// </summary>
      public BaseCollection()
      {
      }

      /// <summary>
      /// Creates an object of type BaseCollection.
      /// </summary>
      /// <param name="capacity">Initial capacity.</param>
      public BaseCollection( int capacity )
      {
         this.list = new List<T>( capacity );
      }

      /// <summary>
      /// Creates an object of type BaseCollection.
      /// </summary>
      /// <param name="collection">Collection of elements to copy into the collection.</param>
      public BaseCollection( IEnumerable<T> collection )
      {
         this.list = new List<T>( collection );
      }

      /// <summary>
      /// Copies the elements of the JmwSoftware.Collections.BaseCollection&lt;T&gt; to a new array.
      /// </summary>
      /// <returns></returns>
      public T[] ToArray()
      {
         return this.list.ToArray();
      }

      public void ForEach( Action<T> action )
      {
         this.list.ForEach( action );
      }

      #region IBaseCollection<T> Members

      /// <summary>
      /// Gets or sets the object at the specified index.
      /// </summary>
      /// <param name="index">Index of the object to get or set.</param>
      /// <returns>Object at the specified index.</returns>
      public T this[int index]
      {
         get { return this.list[index]; }
         set { this.list[index] = value; }
      }

      /// <summary>
      /// Inserts an object at the specified index.
      /// </summary>
      /// <param name="item">Item to insert.</param>
      /// <param name="index">Index to insert the object at.</param>
      public void InsertAt( T item, int index )
      {
         this.list.Insert( index, item );

         if ( this.ItemAdded != null )
         {
            this.ItemAdded( new KeyValuePair<int, T>[] { new KeyValuePair<int, T>( index, item ) } );
         }
      }

      /// <summary>
      /// Removes the object at the specified index.
      /// </summary>
      /// <param name="index">Index of the object to remove.</param>
      public void RemoveAt( int index )
      {
         var item = this.list[index];

         this.list.RemoveAt( index );

         if ( this.ItemRemoved != null )
         {
            this.ItemRemoved( new T[] { item } );
         }
      }

      /// <summary>
      /// Adds a collection of objects.
      /// </summary>
      /// <param name="collection">Collection to add.</param>
      public void AddRange( IEnumerable<T> collection )
      {
         int count = this.list.Count;

         this.list.AddRange( collection );

         if ( this.ItemAdded != null )
         {
            var addedItems = new List<KeyValuePair<int, T>>();


            foreach ( T item in collection )
            {
               addedItems.Add( new KeyValuePair<int, T>( count, item ) );
               count++;
            }

            this.ItemAdded( addedItems.ToArray() );
         }
      }

      #endregion

      #region ICollection<T> Members

      /// <summary>
      /// Adds an item to the ICollection&lt;(Of &lt;(T&gt;)&gt;).
      /// </summary>
      /// <param name="item">The object to add to the ICollection&lt;(Of &lt;(T&gt;)&gt;).</param>
      public void Add( T item )
      {
         this.list.Add( item );

         if ( this.ItemAdded != null )
         {
            this.ItemAdded( new KeyValuePair<int, T>[] { new KeyValuePair<int, T>( this.list.Count - 1, item ) } );
         }
      }

      /// <summary>
      /// Removes all items from the ICollection&lt;(Of &lt;(T&gt;)&gt;).
      /// </summary>
      public void Clear()
      {
         var array = this.list.ToArray();

         this.list.Clear();

         if ( this.ItemRemoved != null )
         {
            this.ItemRemoved( array );
         }
      }

      /// <summary>
      /// Determines whether the ICollection&lt;(Of &lt;(T&gt;)&gt;) contains a specific value.
      /// </summary>
      /// <param name="item">The object to locate in the ICollection&lt;(Of &lt;(T&gt;)&gt;).</param>
      /// <returns>true if item is found in the ICollection&lt;(Of &lt;(T&gt;)&gt;); otherwise, false.</returns>
      public bool Contains( T item )
      {
         return this.list.Contains( item );
      }

      /// <summary>
      /// Copies the elements of the ICollection&lt;(Of &lt;(T&gt;)&gt;) to an Array, starting at a particular Array index.
      /// </summary>
      /// <param name="array">The one-dimensional Array  that is the destination of the elements copied from ICollection&lt;(Of &lt;(T&gt;)&gt;). The Array  must have zero-based indexing.</param>
      /// <param name="arrayIndex">The zero-based index in array  at which copying begins.</param>
      public void CopyTo( T[] array, int arrayIndex )
      {
         this.list.CopyTo( array, arrayIndex );
      }

      /// <summary>
      /// Gets the number of elements contained in the ICollection&lt;(Of &lt;(T&gt;)&gt;).
      /// </summary>
      public int Count
      {
         get { return this.list.Count; }
      }

      /// <summary>
      /// Gets a value indicating whether the ICollection&lt;(Of &lt;(T&gt;)&gt;) is read-only.
      /// </summary>
      public bool IsReadOnly
      {
         get { return false; }
      }

      /// <summary>
      /// Removes the first occurrence of a specific object from the ICollection&lt;(Of &lt;(T&gt;)&gt;).
      /// </summary>
      /// <param name="item">The object to remove from the ICollection&lt;(Of &lt;(T&gt;)&gt;).</param>
      /// <returns> true if item was successfully removed from the ICollection&lt;(Of &lt;(T&gt;)&gt;); otherwise, false. This method also returns false if item is not found in the original ICollection&lt;(Of &lt;(T&gt;)&gt;).</returns>
      public bool Remove( T item )
      {
         var result = this.list.Remove( item );

         if ( this.ItemRemoved != null )
         {
            this.ItemRemoved( new T[] { item } );
         }

         return result;
      }

      #endregion

      #region IEnumerable<T> Members

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>An IEnumerator  object that can be used to iterate through the collection.</returns>
      public IEnumerator<T> GetEnumerator()
      {
         return this.list.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>An IEnumerator  object that can be used to iterate through the collection.</returns>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return ( (System.Collections.IEnumerable) this.list ).GetEnumerator();
      }

      #endregion
   }

   /// <summary>
   /// Handler called when items are added to the collection.
   /// </summary>
   /// <typeparam name="T">Type of the items added.</typeparam>
   /// <param name="items">Array of items added and their indexes.</param>
   public delegate void ItemsAddedHandler<T>( KeyValuePair<int, T>[] items );

   /// <summary>
   /// Handler called when items are removed from the collection.
   /// </summary>
   /// <typeparam name="T">Type of the items removed.</typeparam>
   /// <param name="items">Array of items removed.</param>
   public delegate void ItemsRemovedHandler<T>( T[] items );
}
