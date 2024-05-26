using UnityEngine;

namespace Player
{
    public class SuperVillain : MonoBehaviour
    {
        [SerializeField] private Animator superVillainAnimator;
        
        public void AngerState()
        {
            superVillainAnimator.SetTrigger("trAnger");
        }

        public void CheerState()
        {
            superVillainAnimator.SetTrigger("trCheer");
        }
    }
}
