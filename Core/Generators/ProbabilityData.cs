using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    public abstract class ProbabilityData<T>
    {
        [SerializeField]
        protected T data;
        [SerializeField]
        protected float probability;
        public T Data => data;
        public float Probability
        {
            get => probability;
            set
            {
                probability = value;
                if (probability < 0)
                    probability = 0;
            }
        }

        public ProbabilityData(T data, float probability)
        {
            this.data = data;
            this.probability = probability;
        }

        public ProbabilityData(ProbabilityData<T> probData)
        {
            data = probData.data;
            probability = probData.probability;
        }
    }
}