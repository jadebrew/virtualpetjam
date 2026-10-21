using System;
using System.Collections.Generic;

namespace Notification
{
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private struct Node : IComparable<Node>
        {
            public TElement Element;
            public TPriority Priority;
            public long Sequence;

            public int CompareTo(Node other)
            {
                int comp = Priority.CompareTo(other.Priority);
                if (comp != 0) return comp;
                return Sequence.CompareTo(other.Sequence); // FIFO tie-breaker
            }
        }

        private readonly List<Node> _heap = new();
        private long _sequenceCounter = 0;

        public int Count => _heap.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            Node node = new Node { Element = element, Priority = priority, Sequence = _sequenceCounter++ };
            _heap.Add(node);
            HeapifyUp(_heap.Count - 1);
        }

        public TElement Dequeue()
        {
            if (_heap.Count == 0)
                return default;
        
            TElement front = _heap[0].Element;
            Node last = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                _heap[0] = last;
                HeapifyDown(0);
            }

            return front;
        }

        public TElement Peek()
        {
            if (_heap.Count == 0)
                return default;
            return _heap[0].Element;
        }
    
        /// <summary>
        /// Returns both the element and its priority at the front of the queue without removing it.
        /// </summary>
        public (TElement element, TPriority priority) PeekWithPriority()
        {
            if (_heap.Count == 0)
                return (default, default);
    
            Node front = _heap[0];
            return (front.Element, front.Priority);
        }

        public void Clear()
        {
            _heap.Clear();
            _sequenceCounter = 0;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_heap[index].CompareTo(_heap[parent]) < 0)
                {
                    Node temp = _heap[index];
                    _heap[index] = _heap[parent];
                    _heap[parent] = temp;
                    index = parent;
                }
                else break;
            }
        }
    
        /// <summary>
        /// Removes and returns both the element and its priority at the front of the queue.
        /// </summary>
        public (TElement element, TPriority priority) DequeueWithPriority()
        {
            if (_heap.Count == 0)
                return (default, default);
    
            Node frontNode = _heap[0];
            Node last = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                _heap[0] = last;
                HeapifyDown(0);
            }

            return (frontNode.Element, frontNode.Priority);
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = _heap.Count - 1;
            while (index * 2 + 1 <= lastIndex)
            {
                int leftChild = index * 2 + 1;
                int rightChild = index * 2 + 2;
                int smallest = index;

                if (_heap[leftChild].CompareTo(_heap[smallest]) < 0)
                    smallest = leftChild;

                if (rightChild <= lastIndex && _heap[rightChild].CompareTo(_heap[smallest]) < 0)
                    smallest = rightChild;

                if (smallest != index)
                {
                    Node temp = _heap[index];
                    _heap[index] = _heap[smallest];
                    _heap[smallest] = temp;
                    index = smallest;
                }
                else break;
            }
        }
    }
}