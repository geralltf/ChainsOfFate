using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
    {
        public static T Instance;
        
        public virtual void Awake()
        {
            Instance = (T) this;
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }
    }
}