/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace s4pi.Interfaces
{
	/// <summary>
	///     Abstract extension to <see cref="AHandlerList{T}" /> adding support for <see cref="System.IO.Stream" /> IO
	///     and partially implementing <see cref="IGenericAdd" />.
	/// </summary>
	/// <typeparam name="T"><see cref="Type" /> of list element</typeparam>
	/// <seealso cref="AHandlerList{T}" />
	/// <seealso cref="IGenericAdd" />
	public abstract class DependentList<T> : AHandlerList<T>, IGenericAdd
		where T : AHandlerElement, IEquatable<T>
	{
		/// <summary>
		///     Holds the <see cref="EventHandler" /> delegate to invoke if an element in the <see cref="DependentList{T}" />
		///     changes.
		/// </summary>
		/// <remarks>Work around for list event handler triggering during stream constructor and other places.</remarks>
		protected EventHandler elementHandler;

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="DependentList{T}" /> class
		///     that is empty.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="maxSize">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		protected DependentList(EventHandler handler, long maxSize = -1)
			: base(handler, maxSize)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="DependentList{T}" /> class
		///     filled with the content of <paramref name="ilt" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="ilt">The initial content of the list.</param>
		/// <param name="maxSize">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		/// <exception cref="System.InvalidOperationException">Thrown when list size exceeded.</exception>
		/// <remarks>
		///     Calls <c>this.Add(...)</c> to ensure a fresh instance is created, rather than passing <paramref name="ilt" />
		///     to the base constructor.
		/// </remarks>
		protected DependentList(EventHandler handler, IEnumerable<T> ilt, long maxSize = -1)
			: base(null, maxSize)
		{
			this.elementHandler = handler;
			foreach (var t in ilt)
			{
				this.Add((T)t.Clone(null));
			}
			this.handler = handler;
		}

		// Add stream-based constructors and support
		/// <summary>
		///     Initializes a new instance of the <see cref="DependentList{T}" /> class
		///     filled from <see cref="System.IO.Stream" /> <paramref name="s" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="s">The <see cref="System.IO.Stream" /> to read for the initial content of the list.</param>
		/// <param name="maxSize">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		/// <exception cref="System.InvalidOperationException">Thrown when list size exceeded.</exception>
		protected DependentList(EventHandler handler, Stream s, long maxSize = -1)
			: base(null, maxSize)
		{
			this.elementHandler = handler;
			this.Parse(s);
			this.handler = handler;
		}

		#endregion

		#region Data I/O

		/// <summary>
		///     Read list entries from a stream
		/// </summary>
		/// <param name="s">Stream containing list entries</param>
		//// <remarks>This method bypasses <see cref="DependentList{T}.Add(object[])"/>
		//// because <see cref="CreateElement(Stream, out bool)"/> must take care of the same issues.</remarks>
		protected virtual void Parse(Stream s)
		{
			this.Clear();
			var inc = true;
			int count = this.ReadCount(s);
			for (var i = count; i > 0; i = i - (inc ? 1 : 0))
			{
				base.Add(this.CreateElement(s, out inc));
			}
		}

		/// <summary>
		///     Return the number of elements to be created.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> being processed.</param>
		/// <returns>The number of elements to be created.</returns>
		protected virtual int ReadCount(Stream s)
		{
			return (new BinaryReader(s)).ReadInt32();
		}

		/// <summary>
		///     Create a new element from the <see cref="System.IO.Stream" />.
		/// </summary>
		/// <param name="s">Stream containing element data.</param>
		/// <returns>A new element.</returns>
		protected abstract T CreateElement(Stream s);

		/// <summary>
		///     Create a new element from the <see cref="System.IO.Stream" /> and indicates whether it counts towards the number of
		///     elements to be created.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> containing element data.</param>
		/// <param name="inc">Whether this call towards the number of elements to be created.</param>
		/// <returns>A new element.</returns>
		protected virtual T CreateElement(Stream s, out bool inc)
		{
			inc = true;
			return this.CreateElement(s);
		}

		/// <summary>
		///     Write list entries to a stream
		/// </summary>
		/// <param name="s">Stream to receive list entries</param>
		public virtual void UnParse(Stream s)
		{
			this.WriteCount(s, this.Count);
			foreach (var element in this)
			{
				this.WriteElement(s, element);
			}
		}

		/// <summary>
		///     Write the count of list elements to the stream.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> to write <paramref name="count" /> to.</param>
		/// <param name="count">Value to write to <see cref="System.IO.Stream" /> <paramref name="s" />.</param>
		protected virtual void WriteCount(Stream s, int count)
		{
			(new BinaryWriter(s)).Write(count);
		}

		/// <summary>
		///     Write an element to the stream.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> to write <paramref name="element" /> to.</param>
		/// <param name="element">Value to write to <see cref="System.IO.Stream" /> <paramref name="s" />.</param>
		protected abstract void WriteElement(Stream s, T element);

		#endregion

		#region IGenericAdd

		/// <summary>
		///     Add a default element to a <see cref="DependentList{T}" />.
		/// </summary>
		/// <exception cref="NotImplementedException"><typeparamref name="T" /> is abstract.</exception>
		/// <exception cref="InvalidOperationException">Thrown when list size exceeded.</exception>
		/// <exception cref="NotSupportedException">The <see cref="DependentList{T}" /> is read-only.</exception>
		public virtual void Add()
		{
			if (typeof(T).IsAbstract)
			{
				throw new NotImplementedException();
			}

			this.Add(typeof(T));
		}

#if OLDVERSION
	/// <summary>
	/// Adds an entry to an <see cref="DependentList{T}"/>.
	/// </summary>
	/// <param name="fields">
	/// The object to add: either an instance or, for abstract generic lists,
	/// a concrete type or (to be deprecated) the concrete type constructor arguments.  See 'remarks'.
	/// </param>
	/// <returns>True on success</returns>
	/// <exception cref="InvalidOperationException">Thrown when list size exceeded.</exception>
	/// <exception cref="NotSupportedException">The <see cref="DependentList{T}"/> is read-only.</exception>
	/// <remarks>
	/// <para>As at 27 April 2012, changes are afoot in how this works.
	/// Using <see cref="ConstructorParametersAttribute"/> will soon be deprecated.
	/// Concrete implementations of abstract types will be expected to have a default constructor taking only <c>APIversion</c> and <c>elementHandler</c>.</para>
	/// <para>Currently, this method supports the following invocations:</para>
	/// <list type="bullet">
	/// <item><term><c>Add(T newElement)</c></term><description>Create a new instance from the one supplied.</description></item>
	/// <item><term><c>Add(Type concreteOfT)</c></term>
	/// <description>Create a default instance of the given concrete implementation of an abstract class.
	/// Where <c>concreteOfT</c> has a <see cref="ConstructorParametersAttribute"/>, these will additionally be passed to the constructor (to be deprecated).</description>
	/// </item>
	/// <item><term><c>Add(param object[] parameters)</c></term>
	/// <description>(to be deprecated)
	/// For abstract types, the concrete type will be looked up from the supplied parameters.
	/// A new instance will be created, passing the supplied parameters.</description>
	/// </item>
	/// </list>
	/// <para>The new instance will be created passing a zero APIversion and the list's change handler.</para>
	/// </remarks>
	/// <seealso cref="Activator.CreateInstance(Type, object[])"/>
        public virtual bool Add(params object[] fields)
        {
            if (fields == null) return false;

            Type elementType = typeof(T);

            if (fields.Length == 1)
            {
                if (fields[0] is T) // Add(new ConcreteType(0, null, foo, bar))
                {
                    AHandlerElement element = fields[0] as AHandlerElement;
                    base.Add(element.Clone(elementHandler) as T);
                    return true;
                }
            }

            if (elementType.IsAbstract)
            {
                if (fields.Length == 1 && fields[0] is Type && elementType.IsAssignableFrom(fields[0] as Type)) // Add(typeof(ConcreteType))
                {
                    elementType = fields[0] as Type;
                    ConstructorParametersAttribute[] constructorParametersArray = elementType.GetCustomAttributes(typeof(ConstructorParametersAttribute), true) as ConstructorParametersAttribute[];
                    if (constructorParametersArray.Length == 1) // ConstructorParametersAttribute present -- deprecated
                        fields = constructorParametersArray[0].parameters;
                    else // ConstructorParametersAttribute absent: this will become the only way
                        fields = new object[] { };
                }
                else // Add(foo, bar) -- deprecated
                {
                    elementType = GetElementType(fields);
                }
            }

            try
            {
                object[] args = new object[fields.Length + 2];
                args[0] = (int)0;
                args[1] = elementHandler;
                Array.Copy(fields, 0, args, 2, fields.Length);
                T newElement = Activator.CreateInstance(elementType, args) as T;
                //T newElement = Activator.CreateInstance(elementType, new object[] { 0, elementHandler, }) as T; // eventually...
                base.Add(newElement);
                return true;
            }
            catch (MissingMethodException) { } // That's allowed... for now
            return false;
        }
#endif

		/// <summary>
		///     Adds an entry to a <see cref="DependentList{T}" />, setting the element change handler.
		/// </summary>
		/// <param name="newElement">An instance of type <c>{T}</c> to add to the list.</param>
		/// <exception cref="InvalidOperationException">Thrown when list size exceeded.</exception>
		/// <exception cref="NotSupportedException">The <see cref="DependentList{T}" /> is read-only.</exception>
		public override void Add(T newElement)
		{
			newElement.SetHandler(this.elementHandler);
			base.Add(newElement);
		}

		/// <summary>
		///     Add a default element to a <see cref="DependentList{T}" /> of the specified type, <paramref name="elementType" />.
		/// </summary>
		/// <param name="elementType">A concrete type assignable to the list generic type, <typeparamref name="T" />.</param>
		/// <exception cref="ArgumentException"><paramref name="elementType" /> is abstract.</exception>
		/// <exception cref="ArgumentException">
		///     Thrown when <paramref name="elementType" /> is not an implementation of the list type, <typeparamref name="T" />.
		/// </exception>
		/// <exception cref="InvalidOperationException">Thrown when list size exceeded.</exception>
		/// <exception cref="NotSupportedException">The <see cref="DependentList{T}" /> is read-only.</exception>
		/// <seealso cref="Activator.CreateInstance(Type, object[])" />
		public virtual void Add(Type elementType)
		{
			if (elementType.IsAbstract)
			{
				throw new ArgumentException("Must pass a concrete element type.", "elementType");
			}

			if (!typeof(T).IsAssignableFrom(elementType))
			{
				throw new ArgumentException("The element type must belong to the generic type of the list.", "elementType");
			}

			var newElement = Activator.CreateInstance(elementType, 0, this.elementHandler) as T;
			base.Add(newElement);
		}

		/// <summary>
		///     Return the type to get the constructor from, for the given set of fields.
		/// </summary>
		/// <param name="fields">Constructor parameters</param>
		/// <returns>Class on which to invoke constructor</returns>
		/// <remarks>
		///     <paramref name="fields" />[0] could be an instance of the abstract class: it should provide a constructor that
		///     accepts a "template"
		///     object and creates a new instance on that basis.
		/// </remarks>
		[Obsolete]
		protected virtual Type GetElementType(params object[] fields)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	/// <summary>
	///     A TGIBlock list class where the count of elements is separate from the stored list
	/// </summary>
	public class CountedTGIBlockList : DependentList<TGIBlock>
	{
		private readonly int origCount; // count at the time the list was constructed, used to Parse() list from stream
		private readonly string order = "TGI";

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     that is empty
		///     with <see cref="TGIBlock.Order" /> of "TGI".
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, long size = -1) : this(handler, "TGI", size)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     filled with the content of <paramref name="ilt" />
		///     with <see cref="TGIBlock.Order" /> of "TGI".
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="ilt">The <see cref="IEnumerable{TGIBlock}" /> to use as the initial content of the list.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, IEnumerable<TGIBlock> ilt, long size = -1)
			: this(handler, "TGI", ilt, size)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     filled with <paramref name="count" /> elements from <see cref="System.IO.Stream" /> <paramref name="s" />
		///     with <see cref="TGIBlock.Order" /> of "TGI".
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="count">The number of list elements to read.</param>
		/// <param name="s">The <see cref="System.IO.Stream" /> to read for the initial content of the list.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, int count, Stream s, long size = -1)
			: this(handler, "TGI", count, s, size)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     that is empty
		///     with the specified <see cref="TGIBlock.Order" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="order">The <see cref="TGIBlock.Order" /> of the <see cref="TGIBlock" /> values.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, TGIBlock.Order order, long size = -1)
			: this(handler, "" + order, size)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     filled with the content of <paramref name="ilt" />
		///     with the specified <see cref="TGIBlock.Order" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="order">The <see cref="TGIBlock.Order" /> of the <see cref="TGIBlock" /> values.</param>
		/// <param name="ilt">The <see cref="IEnumerable{TGIBlock}" /> to use as the initial content of the list.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, TGIBlock.Order order, IEnumerable<TGIBlock> ilt, long size = -1)
			: this(handler, "" + order, ilt, size)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     filled with <paramref name="count" /> elements from <see cref="System.IO.Stream" /> <paramref name="s" />
		///     with the specified <see cref="TGIBlock.Order" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="order">The <see cref="TGIBlock.Order" /> of the <see cref="TGIBlock" /> values.</param>
		/// <param name="count">The number of list elements to read.</param>
		/// <param name="s">The <see cref="System.IO.Stream" /> to read for the initial content of the list.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, TGIBlock.Order order, int count, Stream s, long size = -1)
			: this(handler, "" + order, count, s, size)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     that is empty
		///     with the specified <see cref="TGIBlock.Order" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="order">A string representing the <see cref="TGIBlock.Order" /> of the <see cref="TGIBlock" /> values.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, string order, long size = -1) : base(handler, size)
		{
			this.order = order;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     filled with the content of <paramref name="ilt" />
		///     with the specified <see cref="TGIBlock.Order" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="order">A string representing the <see cref="TGIBlock.Order" /> of the <see cref="TGIBlock" /> values.</param>
		/// <param name="ilt">The <see cref="IEnumerable{TGIBlock}" /> to use as the initial content of the list.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, string order, IEnumerable<TGIBlock> ilt, long size = -1)
			: base(handler, ilt, size)
		{
			this.order = order;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CountedTGIBlockList" /> class
		///     filled with <paramref name="count" /> elements from <see cref="System.IO.Stream" /> <paramref name="s" />
		///     with the specified <see cref="TGIBlock.Order" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="order">A string representing the <see cref="TGIBlock.Order" /> of the <see cref="TGIBlock" /> values.</param>
		/// <param name="count">The number of list elements to read.</param>
		/// <param name="s">The <see cref="System.IO.Stream" /> to read for the initial content of the list.</param>
		/// <param name="size">Optional; -1 for unlimited size, otherwise the maximum number of elements in the list.</param>
		public CountedTGIBlockList(EventHandler handler, string order, int count, Stream s, long size = -1)
			: base(null, size)
		{
			this.origCount = count;
			this.order = order;
			this.elementHandler = handler;
			this.Parse(s);
			this.handler = handler;
		}

		#endregion

		#region Data I/O

		/// <summary>
		///     Create a new element from the <see cref="System.IO.Stream" />.
		/// </summary>
		/// <param name="s">Stream containing element data.</param>
		/// <returns>A new element.</returns>
		protected override TGIBlock CreateElement(Stream s)
		{
			return new TGIBlock(0, this.elementHandler, this.order, s);
		}

		/// <summary>
		///     Write an element to the stream.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> to write <paramref name="element" /> to.</param>
		/// <param name="element">Value to write to <see cref="System.IO.Stream" /> <paramref name="s" />.</param>
		protected override void WriteElement(Stream s, TGIBlock element)
		{
			element.UnParse(s);
		}

		/// <summary>
		///     Return the number of elements to be created.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> being processed -- ignored.</param>
		/// <returns>The number of elements to be created, as provided to the <see cref="CountedTGIBlockList" /> constructor.</returns>
		protected override int ReadCount(Stream s)
		{
			return this.origCount;
		}

		/// <summary>
		///     This list does not manage a count within the <see cref="System.IO.Stream" />.
		/// </summary>
		/// <param name="s">Ignored.</param>
		/// <param name="count">Ignored.</param>
		protected override void WriteCount(Stream s, int count)
		{
		}

		#endregion

		/// <summary>
		///     Add a new default element to the list.
		/// </summary>
		public override void Add()
		{
			base.Add(new TGIBlock(0, this.elementHandler, this.order));
		} // Need to pass "order"

		/// <summary>
		///     Adds a new <see cref="TGIBlock" /> to the list using the values of the specified <see cref="TGIBlock" />.
		/// </summary>
		/// <param name="item">The <see cref="TGIBlock" /> to use as a basis for the new <see cref="TGIBlock" />.</param>
		/// <exception cref="System.InvalidOperationException">Thrown when list size exceeded.</exception>
		/// <exception cref="System.NotSupportedException">The <see cref="AHandlerList{T}" /> is read-only.</exception>
		/// <remarks>
		///     A new element is created rather than using the element passed
		///     as the order (TGI/ITG/etc) may be different.
		/// </remarks>
		public override void Add(TGIBlock item)
		{
			base.Add(new TGIBlock(0, this.elementHandler, this.order, item));
		} // Need to pass "order" and there's no property

		/// <summary>
		///     Adds a new TGIBlock to the list using the values of the IResourceKey.
		/// </summary>
		/// <param name="rk">The ResourceKey values to use for the TGIBlock.</param>
		/// <remarks>
		///     A new element is created rather than using the element passed
		///     as the order (TGI/ITG/etc) may be different.
		/// </remarks>
		public void Add(IResourceKey rk)
		{
			base.Add(new TGIBlock(0, this.elementHandler, this.order, rk));
		} // Need to pass "order"

		/// <summary>
		///     Inserts a new <see cref="TGIBlock" /> to the list at the specified index using the values of the specified
		///     <see cref="TGIBlock" />.
		/// </summary>
		/// <param name="index">The zero-based index at which the new <see cref="TGIBlock" /> should be inserted.</param>
		/// <param name="item">The <see cref="TGIBlock" /> to use as a basis for the new <see cref="TGIBlock" />.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		///     <paramref name="index" /> is not a valid index
		///     in the <see cref="CountedTGIBlockList" />.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">Thrown when list size exceeded.</exception>
		/// <exception cref="System.NotSupportedException">The <see cref="CountedTGIBlockList" /> is read-only.</exception>
		/// <remarks>
		///     A new element is created rather than using the element passed
		///     as the order (TGI/ITG/etc) may be different.
		/// </remarks>
		public override void Insert(int index, TGIBlock item)
		{
			base.Insert(index, new TGIBlock(0, this.elementHandler, this.order, item));
		} // Need to pass "order" and there's no property
	}

	/// <summary>
	///     A TGIBlock list class where the count and size of the list are stored separately (but managed by this class)
	/// </summary>
	public class TGIBlockList : DependentList<TGIBlock>
	{
		private readonly bool addEight;
		private readonly bool ignoreTgiSize;

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="TGIBlockList" /> class
		///     that is empty.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="addEight">When true, invoke fudge factor in parse/unparse</param>
		/// <param name="ignoreTgiSize">When true, the size of TGIBlockList will be ignored (only the entry count will be used).</param>
		public TGIBlockList(EventHandler handler, [Optional] bool addEight, [Optional] bool ignoreTgiSize) : base(handler)
		{
			this.addEight = addEight;
			this.ignoreTgiSize = ignoreTgiSize;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="TGIBlockList" /> class
		///     filled with the content of <paramref name="ilt" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="ilt">The <see cref="IEnumerable{TGIBlock}" /> to use as the initial content of the list.</param>
		/// <param name="addEight">When true, invoke fudge factor in parse/unparse</param>
		/// <param name="ignoreTgiSize">When true, the size of TGIBlockList will be ignored (only the entry count will be used).</param>
		public TGIBlockList(EventHandler handler,
							IEnumerable<TGIBlock> ilt,
							[Optional] bool addEight,
							[Optional] bool ignoreTgiSize) : base(handler, ilt)
		{
			this.addEight = addEight;
			this.ignoreTgiSize = ignoreTgiSize;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="TGIBlockList" /> class
		///     filled with elements from <see cref="System.IO.Stream" /> <paramref name="s" />.
		/// </summary>
		/// <param name="handler">The <see cref="EventHandler" /> to call on changes to the list or its elements.</param>
		/// <param name="s">The <see cref="System.IO.Stream" /> to read for the initial content of the list.</param>
		/// <param name="tgiPosn">
		///     Position in the <see cref="System.IO.Stream" /> where the list of <see cref="TGIBlock" />s
		///     starts.
		/// </param>
		/// <param name="tgiSize">Size (in bytes) of the stored list.</param>
		/// <param name="addEight">When true, invoke fudge factor in parse/unparse</param>
		/// <param name="ignoreTgiSize">When true, the size of TGIBlockList will be ignored (only the entry count will be used).</param>
		public TGIBlockList(EventHandler handler,
							Stream s,
							long tgiPosn,
							long tgiSize,
							[Optional] bool addEight,
							[Optional] bool ignoreTgiSize)
			: base(null)
		{
			this.elementHandler = handler;
			this.addEight = addEight;
			this.ignoreTgiSize = ignoreTgiSize;
			this.Parse(s, tgiPosn, tgiSize);
			this.handler = handler;
		}

		#endregion

		#region Data I/O

		/// <summary>
		///     Create a new element from the <see cref="System.IO.Stream" />.
		/// </summary>
		/// <param name="s">Stream containing element data.</param>
		/// <returns>A new element.</returns>
		protected override TGIBlock CreateElement(Stream s)
		{
			return new TGIBlock(0, this.elementHandler, s);
		}

		/// <summary>
		///     Write an element to the stream.
		/// </summary>
		/// <param name="s"><see cref="System.IO.Stream" /> to write <paramref name="element" /> to.</param>
		/// <param name="element">Value to write to <see cref="System.IO.Stream" /> <paramref name="s" />.</param>
		protected override void WriteElement(Stream s, TGIBlock element)
		{
			element.UnParse(s);
		}

		/// <summary>
		///     Read list entries from a stream
		/// </summary>
		/// <param name="s">Stream containing list entries</param>
		/// <param name="tgiPosn">
		///     Position in the <see cref="System.IO.Stream" /> where the list of <see cref="TGIBlock" />s
		///     starts.
		/// </param>
		/// <param name="tgiSize">Size (in bytes) of the stored list.</param>
		protected void Parse(Stream s, long tgiPosn, long tgiSize)
		{
			var checking = true;
			if (checking)
			{
				if (tgiPosn != s.Position)
				{
					throw new InvalidDataException(string.Format("Position of TGIBlock read: 0x{0:X8}, actual: 0x{1:X8}",
																 tgiPosn,
																 s.Position));
				}
			}

			if ((this.ignoreTgiSize && s.Position < s.Length) || tgiSize > 0)
			{
				this.Parse(s);
			}

			if (checking && !this.ignoreTgiSize)
			{
				if (tgiSize != s.Position - tgiPosn + (this.addEight ? 8 : 0))
				{
					throw new InvalidDataException(string.Format("Size of TGIBlock read: 0x{0:X8}, actual: 0x{1:X8}; at 0x{2:X8}",
																 tgiSize,
																 s.Position - tgiPosn,
																 s.Position));
				}
			}
		}

		/// <summary>
		///     Write list entries to a stream
		/// </summary>
		/// <param name="s">Stream to receive list entries</param>
		/// <param name="ptgiO">Position in <see cref="System.IO.Stream" /> to write list position and size values.</param>
		public void UnParse(Stream s, long ptgiO)
		{
			var w = new BinaryWriter(s);

			var tgiPosn = s.Position;
			this.UnParse(s);
			var pos = s.Position;

			s.Position = ptgiO;
			w.Write((uint)(tgiPosn - ptgiO - sizeof(uint)));
			w.Write((uint)(pos - tgiPosn + (this.addEight ? 8 : 0)));

			s.Position = pos;
		}

		#endregion
	}
}
