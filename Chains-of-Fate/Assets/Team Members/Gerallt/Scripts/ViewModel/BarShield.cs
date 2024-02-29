using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class BarShield : MonoBehaviour
    {
        public BlockBarUI ParentView;
        public BarHitSquare currentTarget = null;
        
        //public event Action onHitSquare;
        
        private void OnCollisionEnter(Collision collision)
        {
            GameObject go = collision.gameObject;
            BarHitSquare hit = go.GetComponent<BarHitSquare>();

            if (hit != null)
            {
                currentTarget = hit;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            GameObject go = other.gameObject;
            BarHitSquare hit = go.GetComponent<BarHitSquare>();

            if (hit != null)
            {
                currentTarget = null;
            }
        }
    }

}