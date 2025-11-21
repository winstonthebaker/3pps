using System;
using System.Collections.Generic;
using Godot;

namespace Utilities
{

    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _buffer;
        private int _start;
        private int _size;
        public int Capacity { get; }
        public int Count => _size;
        public bool IsEmpty => _size == 0;
        public bool IsFull => _size == Capacity;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be positive", nameof(capacity));
            _buffer = new T[capacity];
            Capacity = capacity;
            _start = 0;
            _size = 0;
        }

        public void Enqueue(T item)
        {
            if (_size == Capacity)
            {
                // Overwrite oldest element
                _buffer[_start] = item;
                _start = (_start + 1) % Capacity;
            }
            else
            {
                // Add to end
                _buffer[(_start + _size) % Capacity] = item;
                _size++;
            }
        }

        public T Dequeue()
        {
            if (_size == 0)
                throw new InvalidOperationException("Buffer is empty");
            T result = _buffer[_start];
            _buffer[_start] = default(T); // Clear the reference
            _start = (_start + 1) % Capacity;
            _size--;
            return result;
        }

        public bool TryDequeue(out T item)
        {
            if (_size == 0)
            {
                item = default(T);
                return false;
            }
            item = Dequeue();
            return true;
        }

        public T Peek()
        {
            if (_size == 0)
                throw new InvalidOperationException("Buffer is empty");
            return _buffer[_start];
        }

        public bool TryPeek(out T item)
        {
            if (_size == 0)
            {
                item = default(T);
                return false;
            }
            item = _buffer[_start];
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < Capacity; i++)
                _buffer[i] = default(T);
            _start = 0;
            _size = 0;
        }

        public T[] GetAllItems()
        {
            T[] result = new T[_size];
            for (int i = 0; i < _size; i++)
            {
                result[i] = _buffer[(_start + i) % Capacity];
            }
            return result;
        }

        // Indexer for direct access
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _buffer[(_start + index) % Capacity];
            }
        }

        // Iterator implementation
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                yield return _buffer[(_start + i) % Capacity];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
