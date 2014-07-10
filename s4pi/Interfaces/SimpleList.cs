using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace s4pi.Interfaces
{
    /// <summary>
    /// A flexible generic list that implements <see cref="DependentList{T}"/> for
    /// a simple data type (such as <see cref="UInt32"/>).
    /// </summary>
    /// <typeparam name="T">A simple data type (such as <see cref="UInt32"/>).</typeparam>
    /// <example>
    /// The following method shows a way to create a list of UInt32 values, with an Int32 entry count
    /// stored in the stream immediately before the list.
    /// <code>
    /// <![CDATA[
    /// SimpleList<UInt32> ReadUInt32List(EventHandler e, Stream s)
    /// {
    ///     return new SimpleList<UInt32>(e, s,
    ///         s => new BinaryReader(s).ReadUInt32(),
    ///         (s, value) => new BinaryWriter(s).Write(value));
    /// }
    /// ]]>
    /// </code>
    /// For more complex cases, or where repeated use of the same kind of <see cref="SimpleList{T}"/> is needed in different contexts,
    /// it can be worthwhile extending the class, as shown below.  This example is for a list of byte values prefixed
    /// by a one byte count.  It shows that the list length can also be specified (here using <c>Byte.MaxValue</c>
    /// <code>
    /// <![CDATA[
    /// public class ByteList : SimpleList<Byte>
    /// {
    ///     public ByteList(EventHandler handler) : base(handler, ReadByte, WriteByte, Byte.MaxValue, ReadListCount, WriteListCount) { }
    ///     public ByteList(EventHandler handler, Stream s) : base(handler, s, ReadByte, WriteByte, Byte.MaxValue, ReadListCount, WriteListCount) { }
    ///     public ByteList(EventHandler handler, IList<Byte> le) : base(handler, le, ReadByte, WriteByte Byte.MaxValue, ReadListCount, WriteListCount) { }
    ///     
    ///     static uint ReadListCount(Stream s) { return new BinaryReader(s).ReadByte(); }
    ///     static void WriteListCount(Stream s, uint count) { new BinaryWriter(s).Write((byte)count); }
    ///     static byte ReadByte(Stream s) { return new BinaryReader(s).ReadByte(); }
    ///     static void WriteByte(Stream s, byte value) { new BinaryWriter(s).Write(value); }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="HandlerElement{T}"/>
    /// <seealso cref="IndexList{T}"/>
    public class SimpleList<T> : DependentList<HandlerElement<T>>, IEnumerable<T>
        where T : struct, IComparable, IConvertible, IEquatable<T>, IComparable<T>
    {
        #region Attributes
        CreateElementMethod createElement;
        WriteElementMethod writeElement;
        ReadCountMethod readCount;
        WriteCountMethod writeCount;
        #endregion

        #region Enumerator<U>
        /// <summary>
        /// Supports a simple iteration over a generic collection.
        /// </summary>
        /// <typeparam name="U">The type of objects to enumerate.</typeparam>
        public class Enumerator<U> : IEnumerator<U>
            where U : struct, IComparable, IConvertible, IEquatable<U>, IComparable<U>
        {
            DependentList<HandlerElement<U>> list;
            DependentList<HandlerElement<U>>.Enumerator enumerator;

            internal Enumerator(DependentList<HandlerElement<U>> list) { this.list = list; Reset(); }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public U Current { get { return enumerator.Current; } }

            /// <summary>
            /// Releases all resources used by the enumerator.
            /// </summary>
            public void Dispose()
            {
                enumerator.Dispose();
                list = null;
            }

            object System.Collections.IEnumerator.Current { get { return enumerator.Current; } }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element;
            /// false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext() { return enumerator.MoveNext(); }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public void Reset() { enumerator = list.GetEnumerator(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleList{T}"/> class
        /// that is empty.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> to call on changes to the list or its elements.</param>
        /// <param name="createElement">Optional; the method to create a new element in the list from a stream.  If null, return default{T}.</param>
        /// <param name="writeElement">Optional; the method to create a new element in the list from a stream.  No operation if null.</param>
        /// <param name="size">Optional maximum number of elements in the list.</param>
        /// <param name="readCount">Optional; default is to read a <see cref="Int32"/> from the <see cref="Stream"/>.</param>
        /// <param name="writeCount">Optional; default is to write a <see cref="Int32"/> to the <see cref="Stream"/>.</param>
        public SimpleList(EventHandler handler, CreateElementMethod createElement = null, WriteElementMethod writeElement = null, long size = -1, ReadCountMethod readCount = null, WriteCountMethod writeCount = null) : base(handler, size) { this.createElement = createElement; this.writeElement = writeElement; this.readCount = readCount; this.writeCount = writeCount; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleList{T}"/> class
        /// from <paramref name="s"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> to call on changes to the list or its elements.</param>
        /// <param name="s">The <see cref="Stream"/> to read for the initial content of the list.</param>
        /// <param name="createElement">Required; the method to create a new element in the list from a stream.</param>
        /// <param name="writeElement">Required; the method to create a new element in the list from a stream.</param>
        /// <param name="size">Optional maximum number of elements in the list.</param>
        /// <param name="readCount">Optional; default is to read a <see cref="Int32"/> from the <see cref="Stream"/>.</param>
        /// <param name="writeCount">Optional; default is to write a <see cref="Int32"/> to the <see cref="Stream"/>.</param>
        public SimpleList(EventHandler handler, Stream s, CreateElementMethod createElement, WriteElementMethod writeElement, long size = -1, ReadCountMethod readCount = null, WriteCountMethod writeCount = null) : this(null, createElement, writeElement, size, readCount, writeCount) { elementHandler = handler; Parse(s); this.handler = handler; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleList{T}"/> class
        /// from <paramref name="collection"/>, wrapping each entry in a <see cref="HandlerElement{T}"/> instance.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> to call on changes to the list or its elements.</param>
        /// <param name="collection">The source to use as the initial content of the list.</param>
        /// <param name="createElement">Optional; the method to create a new element in the list from a stream.  If null, return default{T}.</param>
        /// <param name="writeElement">Optional; the method to create a new element in the list from a stream.  No operation if null.</param>
        /// <param name="size">Optional maximum number of elements in the list.</param>
        /// <param name="readCount">Optional; default is to read a <see cref="Int32"/> from the <see cref="Stream"/>.</param>
        /// <param name="writeCount">Optional; default is to write a <see cref="Int32"/> to the <see cref="Stream"/>.</param>
        public SimpleList(EventHandler handler, IEnumerable<T> collection, CreateElementMethod createElement = null, WriteElementMethod writeElement = null, long size = -1, ReadCountMethod readCount = null, WriteCountMethod writeCount = null) : this(null, createElement, writeElement, size, readCount, writeCount) { elementHandler = handler; this.AddRange(collection); this.handler = handler; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleList{T}"/> class from the existing <paramref name="collection"/>.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> to call on changes to the list or its elements.</param>
        /// <param name="collection">The source to use as the initial content of the list.</param>
        /// <param name="createElement">Optional; the method to create a new element in the list from a stream.  If null, return default{T}.</param>
        /// <param name="writeElement">Optional; the method to create a new element in the list from a stream.  No operation if null.</param>
        /// <param name="size">Optional maximum number of elements in the list.</param>
        /// <param name="readCount">Optional; default is to read a <see cref="Int32"/> from the <see cref="Stream"/>.</param>
        /// <param name="writeCount">Optional; default is to write a <see cref="Int32"/> to the <see cref="Stream"/>.</param>
        private SimpleList(EventHandler handler, IEnumerable<HandlerElement<T>> collection, CreateElementMethod createElement = null, WriteElementMethod writeElement = null, long size = -1, ReadCountMethod readCount = null, WriteCountMethod writeCount = null) : this(null, createElement, writeElement, size, readCount, writeCount) { elementHandler = handler; base.AddRange(collection); this.handler = handler; }
        #endregion

        #region Data I/O
        /// <summary>
        /// Return the number of elements to be created.
        /// </summary>
        /// <param name="s"><see cref="System.IO.Stream"/> being processed.</param>
        /// <returns>The number of elements to be created.</returns>
        protected override int ReadCount(Stream s) { return readCount == null ? base.ReadCount(s) : readCount(s); }
        /// <summary>
        /// Write the count of list elements to the stream.
        /// </summary>
        /// <param name="s"><see cref="System.IO.Stream"/> to write <paramref name="count"/> to.</param>
        /// <param name="count">Value to write to <see cref="System.IO.Stream"/> <paramref name="s"/>.</param>
        protected override void WriteCount(Stream s, int count) { if (writeCount == null) base.WriteCount(s, count); else writeCount(s, count); }

        /// <summary>
        /// Creates an new list element of type <typeparamref name="T"/> by reading <paramref name="s"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> containing data.</param>
        /// <returns>New list element.</returns>
        protected override HandlerElement<T> CreateElement(Stream s) { return new HandlerElement<T>(0, elementHandler, createElement == null ? default(T) : createElement(s)); }
        /// <summary>
        /// Writes the value of a list element to <paramref name="s"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> containing data.</param>
        /// <param name="element">List element for which to write the value to the <see cref="Stream"/>.</param>
        protected override void WriteElement(Stream s, HandlerElement<T> element) { if (writeElement != null) writeElement(s, element.Val); }
        #endregion

        #region Sub-classes
        /// <summary>
        /// Create a new element of type <typeparamref name="T"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> from which to read the element data.</param>
        /// <returns>A new element of type <typeparamref name="T"/>.</returns>
        public delegate T CreateElementMethod(Stream s);
        /// <summary>
        /// Write an element of type <typeparamref name="T"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> to which to write the value.</param>
        /// <param name="value">The value of type <typeparamref name="T"/> to write.</param>
        public delegate void WriteElementMethod(Stream s, T value);
        /// <summary>
        /// Return the number of list elements to read.
        /// </summary>
        /// <param name="s">A <see cref="Stream"/> that may contain the number of elements.</param>
        /// <returns>The number of list elements to read.</returns>
        public delegate int ReadCountMethod(Stream s);
        /// <summary>
        /// Store the number of elements in the list.
        /// </summary>
        /// <param name="s">A <see cref="Stream"/> to which list elements will be written after the count.</param>
        /// <param name="count">The number of list elements.</param>
        public delegate void WriteCountMethod(Stream s, int count);
        #endregion

        #region DependentList<HandlerElement<T>>
        /// <summary>
        /// Add a default element to a <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <exception cref="NotImplementedException">Lists of abstract classes will fail
        /// with a NotImplementedException.</exception>
        /// <exception cref="InvalidOperationException">Thrown when list size exceeded.</exception>
        /// <exception cref="NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public override void Add() { this.Add(default(T)); }
        #endregion

        #region List<T>
        /// <summary>
        /// Adds an entry to a <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="item">The object to add.</param>
        /// <returns>True on success</returns>
        /// <exception cref="InvalidOperationException">Thrown when list size exceeded.</exception>
        /// <exception cref="NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public virtual void Add(T item) { base.Add(new HandlerElement<T>(0, elementHandler, item)); }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="SimpleList{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when list size would be exceeded.</exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        /// <remarks>Calls <see cref="Add(T)"/> for each item in <paramref name="collection"/>.</remarks>
        public virtual void AddRange(IEnumerable<T> collection)
        {
            int newElements = new List<T>(collection).Count;
            if (maxSize >= 0 && Count >= maxSize - newElements) throw new InvalidOperationException();

            //Note that the following is required to allow for implementation specific processing on items added to the list:
            EventHandler h = handler;
            handler = null;
            foreach (T item in collection) this.Add(item);
            handler = h;

            OnListChanged();
        }

        /// <summary>
        ///     Searches a the entire sorted <see cref="SimpleList{T}"/>
        ///     for an element using the default comparer and returns the zero-based index
        ///     of the element.
        /// </summary>
        /// <param name="item">
        ///     The object to locate.
        /// </param>
        /// <returns>
        ///     The zero-based index of item in the sorted <see cref="SimpleList{T}"/>,
        ///     if item is found; otherwise, a negative number that is the bitwise complement
        ///     of the index of the next element that is larger than item or, if there is
        ///     no larger element, the bitwise complement of <see cref="SimpleList{T}"/>.Count.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     The default comparer <see cref="Comparer{T}.Default"/>
        ///     cannot find an implementation of the <see cref="System.IComparable{T}"/> generic interface
        ///     or the <see cref="System.IComparable"/> interface for type T.
        /// </exception>
        public int BinarySearch(T item) { return this.BinarySearch(item, null); }
        /// <summary>
        ///     Searches a the entire sorted <see cref="SimpleList{T}"/>
        ///     for an element using the specified comparer and returns the zero-based index
        ///     of the element.
        /// </summary>
        /// <param name="item">
        ///     The object to locate.
        /// </param>
        /// <param name="comparer">
        ///     The <see cref="IComparer{T}"/> implementation to use when comparing
        ///     elements.
        ///     <br/>-or-<br/>
        ///     null to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <returns>
        ///     The zero-based index of item in the sorted <see cref="SimpleList{T}"/>,
        ///     if item is found; otherwise, a negative number that is the bitwise complement
        ///     of the index of the next element that is larger than item or, if there is
        ///     no larger element, the bitwise complement of <see cref="SimpleList{T}"/>.Count.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     <paramref name="comparer"/> is null, and the default comparer <see cref="Comparer{T}.Default"/>
        ///     cannot find an implementation of the <see cref="System.IComparable{T}"/> generic interface
        ///     or the <see cref="System.IComparable"/> interface for type T.
        /// </exception>
        public int BinarySearch(T item, IComparer<T> comparer) { return this.BinarySearch(0, this.Count, item, comparer); }
        /// <summary>
        ///     Searches a range of elements in the sorted <see cref="SimpleList{T}"/>
        ///     for an element using the specified comparer and returns the zero-based index
        ///     of the element.
        /// </summary>
        /// <param name="index">
        ///     The zero-based starting index of the range to search.
        /// </param>
        /// <param name="count">
        ///     The length of the range to search.
        /// </param>
        /// <param name="item">
        ///     The object to locate.
        /// </param>
        /// <param name="comparer">
        ///     The <see cref="IComparer{T}"/> implementation to use when comparing
        ///     elements, or null to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <returns>
        ///     The zero-based index of item in the sorted <see cref="SimpleList{T}"/>,
        ///     if item is found; otherwise, a negative number that is the bitwise complement
        ///     of the index of the next element that is larger than item or, if there is
        ///     no larger element, the bitwise complement of <see cref="SimpleList{T}"/>.Count.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="index"/> is less than 0.
        ///     <br/>-or-<br/>
        ///     <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the <see cref="SimpleList{T}"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     <paramref name="comparer"/> is null, and the default comparer <see cref="Comparer{T}.Default"/>
        ///     cannot find an implementation of the <see cref="System.IComparable{T}"/> generic interface
        ///     or the <see cref="System.IComparable"/> interface for type T.
        /// </exception>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) { return base.BinarySearch(index, count, new HandlerElement<T>(0, null, item), new SimpleComparer(comparer)); }

        /// <summary>
        ///     Converts the elements in the current <see cref="SimpleList{T}"/> to
        ///     another type, and returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">
        ///     The target type of the elements of the <see cref="System.Collections.Generic.List{T}"/>.
        /// </typeparam>
        /// <param name="converter">
        ///     A <see cref="System.Converter{TInput,TOutput}"/> delegate that converts each element from
        ///     one type to another type.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Collections.Generic.List{TOutput}"/> of the target type containing the converted
        ///     elements from the current <see cref="SimpleList{T}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="converter"/> is null.
        /// </exception>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) { return base.ConvertAll<TOutput>(x => converter(x.Val)); }

        /// <summary>
        ///     Copies the entire <see cref="SimpleList{T}"/> to
        ///     a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional System.Array that is the destination of the elements
        ///     copied from <see cref="SimpleList{T}"/>. The <see cref="System.Array"/> must have
        ///     zero-based indexing.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The number of elements in the source <see cref="SimpleList{T}"/> is
        ///     greater than the number of elements that the destination <paramref name="array"/> can contain.
        /// </exception>
        public void CopyTo(T[] array) { this.CopyTo(0, array, 0, this.Count); }
        /// <summary>
        ///     Copies the entire <see cref="SimpleList{T}"/> to
        ///     a compatible one-dimensional array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional System.Array that is the destination of the elements
        ///     copied from <see cref="SimpleList{T}"/>. The <see cref="System.Array"/> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="arrayIndex"/> is equal to or greater than the length of array.
        /// <br/>-or-<br/>
        ///     The number of elements in the source <see cref="SimpleList{T}"/> is greater
        ///     than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) { this.CopyTo(0, array, arrayIndex, this.Count); }
        /// <summary>
        ///     Copies a range of elements from the <see cref="SimpleList{T}"/> to
        ///     a compatible one-dimensional array, starting at the specified <paramref name="index"/> of the
        ///     target <paramref name="array"/>.
        /// </summary>
        /// <param name="index">
        ///     The zero-based index in the source <see cref="SimpleList{T}"/> at
        ///     which copying begins.
        /// </param>
        /// <param name="array">
        ///     The one-dimensional System.Array that is the destination of the elements
        ///     copied from <see cref="SimpleList{T}"/>. The <see cref="System.Array"/> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <param name="count">
        ///     The number of elements to copy.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="index"/> is less than 0.
        /// <br/>-or-<br/>
        ///     <paramref name="arrayIndex"/> is less than 0.
        /// <br/>-or-<br/>
        ///     <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="index"/> is equal to or greater than the <see cref="SimpleList{T}"/>.Count
        ///     of the source <see cref="SimpleList{T}"/>.
        /// <br/>-or-<br/>
        ///     <paramref name="arrayIndex"/> is equal to or greater than the length of array.
        /// <br/>-or-<br/>
        ///     The number of elements from
        ///     <paramref name="index"/> to the end of the source <see cref="SimpleList{T}"/> is greater
        ///     than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count) { base.ConvertAll<T>(e => e.Val).CopyTo(index, array, arrayIndex, count); }


        /// <summary>
        /// Determines whether an element is in the <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SimpleList{T}"/>.</param>
        /// <returns>true if item is found in the <see cref="SimpleList{T}"/>; otherwise, false.</returns>
        public bool Contains(T item) { return base.Exists(e => e.Val.Equals(item)); }
        /// <summary>
        ///     Determines whether the <see cref="SimpleList{T}"/> contains elements
        ///     that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements
        ///     to search for.
        /// </param>
        /// <returns>
        ///     true if the <see cref="SimpleList{T}"/> contains one or more elements
        ///     that match the conditions defined by the specified predicate; otherwise,
        ///     false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     match is null.
        /// </exception>
        public bool Exists(Predicate<T> match) { return base.Exists(e => match(e.Val)); }

        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the first occurrence within the entire <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The first element that matches the conditions defined by the specified predicate,
        ///     if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        public T Find(Predicate<T> match) { return base.Find(e => match(e.Val)).Val; }
        /// <summary>
        ///     Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements
        ///     to search for.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Collections.Generic.List{T}"/> containing all the elements that match
        ///     the conditions defined by the specified predicate, if found; otherwise, an
        ///     empty <see cref="System.Collections.Generic.List{T}"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        public List<T> FindAll(Predicate<T> match) { return base.FindAll(e => match(e.Val)).ConvertAll<T>(e => e.Val); }
        /// <summary>
        ///     Retrieves all the elements that match the conditions defined by the specified predicate.
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the first occurrence within
        ///     the entire <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of an element that matches the
        ///     conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        public int FindIndex(Predicate<T> match) { return this.FindIndex(0, this.Count, match); }
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the first occurrence within
        ///     the range of elements in the <see cref="SimpleList{T}"/> that extends
        ///     from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">
        ///     The zero-based starting index of the search.
        /// </param>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of an element that matches the
        ///     conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int FindIndex(int startIndex, Predicate<T> match) { return this.FindIndex(startIndex, this.Count - startIndex, match); }
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the first occurrence within
        ///     the range of elements in the <see cref="SimpleList{T}"/> that starts
        ///     at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">
        ///     The zero-based starting index of the search.
        /// </param>
        /// <param name="count">
        ///     The number of elements in the section to search.
        /// </param>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of an element that matches the
        ///     conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        /// <br/>-or-<br/>
        ///     <paramref name="count"/> is less than 0.
        /// <br/>-or-<br/>
        ///     <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int FindIndex(int startIndex, int count, Predicate<T> match) { return this.IndexOf(this.Find(match), startIndex, count); }
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the last occurrence within the entire <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The last element that matches the conditions defined by the specified predicate,
        ///     if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        public T FindLast(Predicate<T> match) { return base.FindLast(e => match(e.Val)).Val; }
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the last occurrence within
        ///     the entire <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The zero-based index of the last occurrence of an element that matches the
        ///     conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        public int FindLastIndex(Predicate<T> match) { return this.FindLastIndex(0, this.Count, match); }
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the last occurrence within
        ///     the range of elements in the <see cref="SimpleList{T}"/> that
        ///     extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">
        ///     The zero-based starting index of the backward search.
        /// </param>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The zero-based index of the last occurrence of an element that matches the
        ///     conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int FindLastIndex(int startIndex, Predicate<T> match) { return this.FindLastIndex(startIndex, this.Count - startIndex, match); }
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the last occurrence within
        ///     the range of elements in the <see cref="SimpleList{T}"/> that
        ///     contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">
        ///     The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        ///     The number of elements in the section to search.
        /// </param>
        /// <param name="match">
        ///     The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element
        ///     to search for.
        /// </param>
        /// <returns>
        ///     The zero-based index of the last occurrence of an element that matches the
        ///     conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="match"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        /// <br/>-or-<br/>
        ///     <paramref name="count"/> is less than 0.
        /// <br/>-or-<br/>
        ///     <paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) { return this.LastIndexOf(this.FindLast(match), startIndex, count); }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="action">
        /// The <see cref="System.Action{T}"/> delegate to perform on each element of the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="action"/> is null.
        /// </exception>
        public void ForEach(Action<T> action) { base.ForEach(e => action(e.Val)); }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="SimpleList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="SimpleList{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <br/>-or-<br/>
        /// <paramref name="index"/> is greater than <see cref="SimpleList{T}"/>.Count.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Thrown when list size would be exceeded.</exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        /// <remarks>Calls <see cref="Insert(int, T)"/> for each item in <paramref name="collection"/>.</remarks>
        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            int newElements = new List<T>(collection).Count;
            if (maxSize >= 0 && Count >= maxSize - newElements) throw new InvalidOperationException();

            //Note that the following is required to allow for implementation specific processing on items inserted into the list:
            EventHandler h = handler;
            handler = null;
            foreach (T item in collection) this.Insert(index++, item);
            handler = h;

            OnListChanged();
        }

        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the range of elements in the <see cref="SimpleList{T}"/>
        ///     that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <param name="index">
        ///     The zero-based starting index of the search.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of item within the range of
        ///     elements in the <see cref="SimpleList{T}"/> that extends from index
        ///     to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int IndexOf(T item, int index) { return this.IndexOf(item, index, this.Count - index); }
        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the range of elements in the <see cref="SimpleList{T}"/>
        ///     that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <param name="index">
        ///     The zero-based starting index of the search.
        /// </param>
        /// <param name="count">
        ///     The number of elements in the section to search.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of item within the range of
        ///     elements in the <see cref="SimpleList{T}"/> that
        ///     starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements,
        ///     if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        ///     <br/>-or-<br/>
        ///     count is less than 0.
        ///     <br/>-or-<br/>
        ///     index and count do not specify a valid section in the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int IndexOf(T item, int index, int count) { return base.IndexOf(base.Find(e => e.Val.Equals(item)), index, count); }
        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     last occurrence within the entire <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <returns>
        ///     The zero-based index of the last occurrence of item within the entire <see cref="SimpleList{T}"/>,
        ///     if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf(T item) { return this.LastIndexOf(item, this.Count - 1, this.Count); }
        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     last occurrence within the range of elements in the <see cref="SimpleList{T}"/>
        ///     that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <param name="index">
        ///     The zero-based starting index of the backward search.
        /// </param>
        /// <returns>
        ///     The zero-based index of the last occurrence of item within the range of
        ///     elements in the <see cref="SimpleList{T}"/> that
        ///     extends from the first element and ends at <paramref name="index"/>,
        ///     if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int LastIndexOf(T item, int index) { return this.LastIndexOf(item, index, index + 1); }
        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     last occurrence within the range of elements in the <see cref="SimpleList{T}"/>
        ///     that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <param name="index">
        ///     The zero-based starting index of the backward search.
        /// </param>
        /// <param name="count">
        ///     The number of elements in the section to search.
        /// </param>
        /// <returns>
        ///     The zero-based index of the last occurrence of item within the range of
        ///     elements in the <see cref="SimpleList{T}"/> that
        ///     contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>,
        ///     if found; otherwise, –1.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is outside the range of valid indexes for the <see cref="SimpleList{T}"/>.
        ///     <br/>-or-<br/>
        ///     count is less than 0.
        ///     <br/>-or-<br/>
        ///     index and count do not specify a valid section in the <see cref="SimpleList{T}"/>.
        /// </exception>
        public int LastIndexOf(T item, int index, int count) { return base.LastIndexOf(base.FindLast(e => e.Val.Equals(item)), index, count); }

        /// <summary>
        /// Removes the first occurrence of an entry from the <see cref="SimpleList{T}"/> with the value given.
        /// </summary>
        /// <param name="item">The value to remove from the <see cref="SimpleList{T}"/>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="SimpleList{T}"/>
        /// otherwise, false. This method also returns false if item is not found in
        /// the original <see cref="SimpleList{T}"/>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">The <see cref="AHandlerList{T}"/> is read-only.</exception>
        public virtual bool Remove(T item) { int index = this.IndexOf(item); if (index < 0) return false; this.RemoveAt(index); return true; }
        /// <summary>
        /// Removes the all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="SimpleList{T}"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public virtual int RemoveAll(Predicate<T> match) { return base.RemoveAll(e => match(e.Val)); }

        /// <summary>
        /// Sorts the elements in the entire <see cref="SimpleList{T}"/> using the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
        /// <exception cref="System.ArgumentException">The implementation of <paramref name="comparison"/> caused an error during the sort.
        /// For example, <paramref name="comparison"/> might not return 0 when comparing an item with itself.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="comparison"/> is null.</exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public virtual void Sort(Comparison<T> comparison) { base.Sort((x, y) => comparison(x.Val, y.Val)); }
        /// <summary>
        /// Sorts the elements in the entire <see cref="SimpleList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements,
        /// or null to use the default comparer <see cref="Comparer{T}.Default"/>.</param>
        /// <exception cref="System.ArgumentException">
        /// The implementation of <paramref name="comparer"/> caused an error during the sort.
        /// For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="comparer"/> is null, and the default comparer <see cref="Comparer{T}.Default"/>
        /// cannot find implementation of the <see cref="IComparable{T}"/> generic interface
        /// or the System.IComparable interface for type <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public virtual void Sort(IComparer<T> comparer) { if (comparer != null) this.Sort(comparer.Compare); else this.Sort(); }
        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="SimpleList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The number of elements in the range to sort.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing elements,
        /// or null to use the default comparer <see cref="Comparer{T}.Default"/>.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="SimpleList{T}"/>.
        /// <br/>-or-<br/>
        /// The implementation of <paramref name="comparer"/> caused an error during the sort.
        /// For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.
        /// <br/>-or-<br/>
        /// <paramref name="comparer"/> is null, and an implementation of the <see cref="IComparable{T}"/> generic interface
        /// or the <see cref="IComparable"/> interface for type <typeparamref name="T"/> cannot be found.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// <br/>-or-<br/>
        /// <paramref name="count"/> is less than 0.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public virtual void Sort(int index, int count, IComparer<T> comparer) { base.Sort(index, count, new SimpleComparer(comparer)); }
        #region SimpleComparer
        class SimpleComparer : IComparer<HandlerElement<T>>
        {
            IComparer<T> comparer; public SimpleComparer(IComparer<T> comparer) { this.comparer = comparer != null ? comparer : System.Collections.Generic.Comparer<T>.Default; }
            public int Compare(HandlerElement<T> x, HandlerElement<T> y) { return comparer.Compare(x.Val, y.Val); }
        }
        #endregion

        /// <summary>
        ///     Copies the elements of the <see cref="SimpleList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        ///     An array containing copies of the elements of the <see cref="SimpleList{T}"/>.
        /// </returns>
        public new T[] ToArray() { return base.ConvertAll<T>(e => e.Val).ToArray(); }

        /// <summary>
        ///     Determines whether every element in the <see cref="SimpleList{T}"/>
        ///     matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        ///     true if every element in the <see cref="SimpleList{T}"/> matches the
        ///     conditions defined by the specified predicate; otherwise, false. If the list
        ///     has no elements, the return value is true.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool TrueForAll(Predicate<T> match) { return base.TrueForAll(e => match(e.Val)); }
        #endregion

        #region IList<T>
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="SimpleList{T}"/>.</exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public new virtual T this[int index] { get { return base[index].Val; } set { if (!base[index].Val.Equals(value)) { base[index].Val = value; OnListChanged(); } } }

        /// <summary>
        ///     Searches for the specified object and returns the zero-based index of the
        ///     first occurrence within the entire <see cref="SimpleList{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="SimpleList{T}"/>.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of item within the entire <see cref="SimpleList{T}"/>,
        ///     if found; otherwise, –1.
        /// </returns>
        public int IndexOf(T item) { return this.IndexOf(item, 0, this.Count); }
        /// <summary>
        /// Inserts an item to the <see cref="SimpleList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="SimpleList{T}"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="SimpleList{T}"/>.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when list size exceeded.</exception>
        /// <exception cref="System.NotSupportedException">The <see cref="SimpleList{T}"/> is read-only.</exception>
        public virtual void Insert(int index, T item) { base.Insert(index, new HandlerElement<T>(0, elementHandler, item)); }
        #endregion

        #region IEnumerable<T>
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public new IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this);
        }
        #endregion
    }

    /// <summary>
    /// Commonly used simple list.  The list count is an integer stored immediately before the list.  The elements are UInt32.
    /// </summary>
    public class UIntList : SimpleList<uint>
    {
        #region Constructors
        /// <summary>
        /// Create an empty UIntList.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="size">Optional list size.</param>
        public UIntList(EventHandler handler, long size = -1) : base(handler, ReadUInt32, WriteUInt32, size) { }
        /// <summary>
        /// Create a UIntList populated from an existing set of values.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="basis">Basis on which to populate the list.</param>
        /// <param name="size">Optional list size.</param>
        public UIntList(EventHandler handler, IEnumerable<uint> basis, long size = -1) : base(handler, basis, ReadUInt32, WriteUInt32, size) { }
        /// <summary>
        /// Create a UIntList populated from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="s"><see cref="Stream"/> from which to read elements.</param>
        /// <param name="size">Optional list size.</param>
        public UIntList(EventHandler handler, Stream s, long size = -1) : base(handler, s, ReadUInt32, WriteUInt32, size) { }
        #endregion

        #region Data I/O
        static UInt32 ReadUInt32(Stream s) { return new BinaryReader(s).ReadUInt32(); }
        static void WriteUInt32(Stream s, UInt32 value) { new BinaryWriter(s).Write(value); }
        #endregion
    }

    /// <summary>
    /// Commonly used simple list.  The list count is an integer stored immediately before the list.  The elements are Int32.
    /// </summary>
    /// <seealso cref="Int32IndexList"/>
    public class IntList : SimpleList<int>
    {
        #region Constructors
        /// <summary>
        /// Create an empty IntList.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="size">Optional list size.</param>
        public IntList(EventHandler handler, long size = -1) : base(handler, ReadInt32, WriteInt32, size) { }
        /// <summary>
        /// Create an IntList populated from an existing set of values.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="basis">Basis on which to populate the list.</param>
        /// <param name="size">Optional list size.</param>
        public IntList(EventHandler handler, IEnumerable<int> basis, long size = -1) : base(handler, basis, ReadInt32, WriteInt32, size) { }
        /// <summary>
        /// Create an IntList populated from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="s"><see cref="Stream"/> from which to read elements.</param>
        /// <param name="size">Optional list size.</param>
        public IntList(EventHandler handler, Stream s, long size = -1) : base(handler, s, ReadInt32, WriteInt32, size) { }
        #endregion

        #region Data I/O
        static Int32 ReadInt32(Stream s) { return new BinaryReader(s).ReadInt32(); }
        static void WriteInt32(Stream s, Int32 value) { new BinaryWriter(s).Write(value); }
        #endregion
    }
}