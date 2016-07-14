using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class DisjointForest<T>
    {
        private readonly IDictionary<T, int> _Sets;
        private readonly DisjointTuple<T>[] _Array;

        public DisjointForest(IEnumerable<T> items) : this(items, EqualityComparer<T>.Default)
        {

        }

        public DisjointForest(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            if (items == null)
            {
                throw new ArgumentNullException();
            }

            if (equalityComparer == null)
            {
                throw new ArgumentNullException();
            }

            _Sets = new Dictionary<T, int>(equalityComparer);
            IList<DisjointTuple<T>> tuples = new List<DisjointTuple<T>>();
            int i = 0;

            foreach (T item in items)
            {
                if (!_Sets.ContainsKey(item))
                {
                    _Sets.Add(item, i);
                    tuples.Add(new DisjointTuple<T>(item, i, 0));
                    i++;
                }
            }

            _Array = tuples.ToArray();
        }

        public bool IsInTheSameSet(T x, T y)
        {
            if (!_Sets.ContainsKey(x) || !_Sets.ContainsKey(y))
            {
                throw new InvalidOperationException("Item is not a part of original set.");
            }

            int xIndex = Find(_Sets[x]);
            int yIndex = Find(_Sets[y]);

            return xIndex == yIndex;
        }

        public void Union(T x, T y)
        {
            if (!_Sets.ContainsKey(x) || !_Sets.ContainsKey(y))
            {
                throw new InvalidOperationException("Item is not a part of original set.");
            }

            int xIndex = Find(_Sets[x]);
            int yIndex = Find(_Sets[y]);

            if (_Array[xIndex].Rank > _Array[yIndex].Rank)
            {
                _Array[yIndex].Index = xIndex;
            }
            else
            {
                _Array[xIndex].Index = yIndex;

                if (_Array[xIndex].Rank == _Array[yIndex].Rank)
                {
                    _Array[yIndex].Rank++;
                }
            }
        }

        private int Find(int index)
        {
            bool flag;

            do
            {
                int newIndex = _Array[index].Index;
                flag = newIndex != index;

                if (flag)
                {
                    index = newIndex;
                }

            } while (flag);

            return index;
        }

        public T Find(T item)
        {
            if (!_Sets.ContainsKey(item))
            {
                throw new InvalidOperationException("Item is not a part of original set.");
            }

            return _Array[Find(_Sets[item])].Value;
        }
    }

    class DisjointTuple<T>
    {
        public T Value
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            set;
        }

        public int Rank
        {
            get;
            set;
        }

        public DisjointTuple(T value, int index, int rank)
        {
            Value = value;
            Rank = rank;
            Index = index;
        }
    }
}
