using System.Collections.Generic;

namespace ui
{
	internal class Pool<T> where T : new()
	{
		static Pool<T> instance_;
		public static Pool<T> instance
		{
			get
			{
				if (instance_ == null)
				{
					instance_ = new Pool<T>();
				}
				return instance_;
			}

		}

		List<T> freeList_ = new List<T>();

		public T Alloc()
		{
			if (freeList_.Count == 0)
			{
				var item = new T();
				return item;
			}
			else
			{
				lock (freeList_)
				{
					var item = freeList_[freeList_.Count - 1];
					freeList_.RemoveAt(freeList_.Count - 1);
					return item;
				}
			}

		}

		public void Free(T item)
		{
			lock (freeList_)
			{
				freeList_.Add(item);
			}
		}

	}
}
