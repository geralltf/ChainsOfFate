using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
    {       
        public virtual void Awake()
        {

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