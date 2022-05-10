using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using BulletSharp.Math;

namespace BulletSharp
{
	internal class ListDebugView
	{
		private System.Collections.IEnumerable _list;

		public ListDebugView(System.Collections.IEnumerable list)
		{
			_list = list;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public System.Collections.ArrayList Items
		{
			get
			{
				var list = new System.Collections.ArrayList();
				foreach (object o in _list)
				{
					list.Add(o);
				}

				return list;
			}
		}
	};

	internal class Vector3ListDebugView
	{
		private IList<Vector3> _list;

		public Vector3ListDebugView(IList<Vector3> list)
		{
			_list = list;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public Vector3[] Items
		{
			get
			{
				var arr = new Vector3[_list.Count];
				_list.CopyTo(arr, 0);
				return arr;
			}
		}
	};

	public class CompoundShapeChildArrayEnumerator : IEnumerator<CompoundShapeChild>
	{
		private int _i;
		private int _count;
		private CompoundShapeChild[] _array;

		public CompoundShapeChildArrayEnumerator(CompoundShapeChildArray array)
		{
			_array = array._backingArray;
			_count = array.Count;
			_i = -1;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			_i++;
			return _i != _count;
		}

		public void Reset()
		{
			_i = 0;
		}

		public CompoundShapeChild Current => _array[_i];

		object System.Collections.IEnumerator.Current => _array[_i];
	}

	public class UIntArrayEnumerator : IEnumerator<uint>
	{
		private int _i;
		private int _count;
		private IList<uint> _array;

		public UIntArrayEnumerator(IList<uint> array)
		{
			_array = array;
			_count = array.Count;
			_i = -1;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			_i++;
			return _i != _count;
		}

		public void Reset()
		{
			_i = 0;
		}

		public uint Current => _array[_i];

		object System.Collections.IEnumerator.Current => _array[_i];
	}

	public class Vector3ArrayEnumerator : IEnumerator<Vector3>
	{
		private int _i;
		private int _count;
		private IList<Vector3> _array;

		public Vector3ArrayEnumerator(IList<Vector3> array)
		{
			_array = array;
			_count = array.Count;
			_i = -1;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			_i++;
			return _i != _count;
		}

		public void Reset()
		{
			_i = 0;
		}

		public Vector3 Current => _array[_i];

		object System.Collections.IEnumerator.Current => _array[_i];
	}

	public class FixedSizeArray
	{
		internal IntPtr _native;

		protected int _count;

		public FixedSizeArray(IntPtr native, int count)
		{
			_native = native;
			_count = count;
		}

		public void Clear()
		{
			throw new InvalidOperationException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public int Count => _count;

		public bool IsReadOnly => false;
	}

	public class CompoundShapeChildArray : FixedSizeArray, IList<CompoundShapeChild>
	{
		internal CompoundShapeChild[] _backingArray = new CompoundShapeChild[0];

		internal CompoundShapeChildArray(IntPtr compoundShape)
			: base(compoundShape, 0)
		{
		}

		public void Add(CompoundShapeChild item)
		{
			throw new NotSupportedException();
		}

		public void AddChildShape(ref Matrix localTransform, CollisionShape shape)
		{
			var childListOld = (_count != 0) ? btCompoundShape_getChildList(_native) : IntPtr.Zero;
			btCompoundShape_addChildShape(_native, ref localTransform, shape._native);
			var childList = btCompoundShape_getChildList(_native);

			// Adjust the native pointer of existing children if the array was reallocated.
			if (childListOld != childList)
			{
				for (int i = 0; i < _count; i++)
				{
					_backingArray[i]._native = btCompoundShapeChild_array_at(childList, i);
				}
			}

			// Add the child to the backing store.
			int childIndex = _count;
			_count++;
			Array.Resize<CompoundShapeChild>(ref _backingArray, _count);
			_backingArray[childIndex] = new CompoundShapeChild(btCompoundShapeChild_array_at(childList, childIndex), shape);
		}

		public int IndexOf(CompoundShapeChild item)
		{
			throw new NotImplementedException();
		}

		public CompoundShapeChild this[int index]
		{
			get => _backingArray[index];
			set => throw new NotImplementedException();
		}

		public bool Contains(CompoundShapeChild item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(CompoundShapeChild[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<CompoundShapeChild> GetEnumerator()
		{
			return new CompoundShapeChildArrayEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new CompoundShapeChildArrayEnumerator(this);
		}

		public void Insert(int index, CompoundShapeChild item)
		{
			throw new InvalidOperationException();
		}

		public bool Remove(CompoundShapeChild item)
		{
			throw new NotSupportedException();
		}

		public void RemoveChildShape(CollisionShape shape)
		{
			var shapePtr = shape._native;
			for (int i = 0; i < _count; i++)
			{
				if (_backingArray[i].ChildShape._native == shapePtr)
				{
					RemoveChildShapeByIndex(i);
				}
			}
		}

		internal void RemoveChildShapeByIndex(int childShapeIndex)
		{
			btCompoundShape_removeChildShapeByIndex(_native, childShapeIndex);
			_count--;

			// Swap the last item with the item to be removed like Bullet does.
			if (childShapeIndex != _count)
			{
				var lastItem = _backingArray[_count];
				lastItem._native = _backingArray[childShapeIndex]._native;
				_backingArray[childShapeIndex] = lastItem;
			}
			_backingArray[_count] = null;
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCompoundShape_addChildShape(IntPtr obj, [In] ref Matrix localTransform, IntPtr shape);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCompoundShape_getChildList(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern int btCompoundShape_getNumChildShapes(IntPtr obj);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCompoundShape_removeChildShape(IntPtr obj, IntPtr shape);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern void btCompoundShape_removeChildShapeByIndex(IntPtr obj, int childShapeindex);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btCompoundShapeChild_array_at(IntPtr obj, int n);
	}

	[DebuggerTypeProxy(typeof(ListDebugView))]
	public class UIntArray : FixedSizeArray, IList<uint>
	{
		internal UIntArray(IntPtr native, int count)
			: base(native, count)
		{
		}

		public void Add(uint item)
		{
			throw new NotSupportedException();
		}

		public int IndexOf(uint item)
		{
			throw new NotImplementedException();
		}

		public uint this[int index]
		{
			get
			{
				if ((uint)index >= (uint)Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (uint)Marshal.ReadInt32(_native.Add(index * sizeof(uint)));
			}
			set => Marshal.WriteInt32(_native, index * sizeof(uint), (int)value);
		}

		public bool Contains(uint item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(uint[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<uint> GetEnumerator()
		{
			return new UIntArrayEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new UIntArrayEnumerator(this);
		}

		public void Insert(int index, uint item)
		{
			throw new InvalidOperationException();
		}

		public bool Remove(uint item)
		{
			throw new NotSupportedException();
		}
	}

	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(Vector3ListDebugView))]
	public class Vector3Array : FixedSizeArray, IList<Vector3>
	{
		internal Vector3Array(IntPtr native, int count)
			: base(native, count)
		{
		}

		public int IndexOf(Vector3 item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Vector3 item)
		{
			throw new NotSupportedException();
		}

		public Vector3 this[int index]
		{
			get
			{
				if ((uint)index >= (uint)Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				btVector3_array_at(_native, index, out var value);
				return value;
			}
			set => btVector3_array_set(_native, index, ref value);
		}

		public void Add(Vector3 item)
		{
			throw new NotSupportedException();
		}

		public bool Contains(Vector3 item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Vector3[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("array");
			}

			int count = Count;
			if (arrayIndex + count > array.Length)
			{
				throw new ArgumentException("Array too small.", "array");
			}

			for (int i = 0; i < count; i++)
			{
				array[arrayIndex + i] = this[i];
			}
		}

		public bool Remove(Vector3 item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<Vector3> GetEnumerator()
		{
			return new Vector3ArrayEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new Vector3ArrayEnumerator(this);
		}

		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btVector3_array_at(IntPtr obj, int n, [Out] out Vector3 value);
		[DllImport(Native.Dll, CallingConvention = Native.Conv), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr btVector3_array_set(IntPtr obj, int n, [In] ref Vector3 value);
	}
}
