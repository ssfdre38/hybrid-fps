using System;
using System.Collections.Generic;

namespace HybridFPS.Systems
{
    /// <summary>
    /// Generic object pooling system for performance optimization.
    /// Reuses objects instead of constantly creating and destroying them.
    /// </summary>
    public class ObjectPool<T> where T : class
    {
        private readonly Queue<T> pool;
        private readonly Func<T> createFunc;
        private readonly Action<T> resetFunc;
        private readonly int maxSize;

        public ObjectPool(Func<T> createFunc, Action<T> resetFunc, int initialSize = 10, int maxSize = 100)
        {
            this.createFunc = createFunc;
            this.resetFunc = resetFunc;
            this.maxSize = maxSize;
            pool = new Queue<T>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                pool.Enqueue(createFunc());
            }
        }

        public T Get()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return createFunc();
        }

        public void Return(T obj)
        {
            if (pool.Count < maxSize)
            {
                resetFunc?.Invoke(obj);
                pool.Enqueue(obj);
            }
        }

        public int PoolSize => pool.Count;
    }
}
