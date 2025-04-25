using UnityEngine;

namespace Player
{
    public class SuperVillain : MonoBehaviour
    {
        public static SuperVillain Instance { get; private set; }
        [SerializeField] private Animator superVillainAnimator;
        
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(gameObject); }
        }
        
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
