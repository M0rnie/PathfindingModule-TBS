using System.Collections.Generic;

namespace PathfindingModule
{
    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> elements = new List<(T, float)>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add((item, priority));
            int ci = elements.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (elements[ci].priority >= elements[pi].priority)
                    break;
                (elements[ci], elements[pi]) = (elements[pi], elements[ci]);
                ci = pi;
            }
        }

        public T Dequeue()
        {
            int last = elements.Count - 1;
            var frontItem = elements[0].item;
            elements[0] = elements[last];
            elements.RemoveAt(last);

            last--;
            int pi = 0;
            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > last) break;
                int rc = ci + 1;
                if (rc <= last && elements[rc].priority < elements[ci].priority)
                    ci = rc;
                if (elements[pi].priority <= elements[ci].priority)
                    break;
                (elements[pi], elements[ci]) = (elements[ci], elements[pi]);
                pi = ci;
            }
            return frontItem;
        }
    }
}
