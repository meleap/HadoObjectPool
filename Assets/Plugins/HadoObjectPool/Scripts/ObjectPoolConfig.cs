using System;

namespace Hado.Utils.ObjectPool
{
    public struct ObjectPoolConfig
    {
        public int NumberOfInstances { get; private set; }
        public int CreateCountPerFrame { get; private set; }

        /// <summary>
        /// if true, Call Awake, OnEnable, Start and OnDisable methods when PrepareAsync called.
        /// </summary>
        public bool NeedPreactivation { get; set; }

        public ObjectPoolConfig(int numberOfInstances, int createCountPerFrame = 1, bool needPreactivation = false)
        {
            if (numberOfInstances < 0)
                throw new ArgumentException("numberOfInstances must be >= 0: " + numberOfInstances);
            if (createCountPerFrame <= 0)
                throw new ArgumentException("createCountPerFrame must be > 0: " + createCountPerFrame);

            NumberOfInstances = numberOfInstances;
            CreateCountPerFrame = createCountPerFrame;
            NeedPreactivation = needPreactivation;
        }
    }
}