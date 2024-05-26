using UnityEngine;

public class ExplosionVFX : MonoBehaviour
{
    public void DestroyThis()
    {
        //Checks to ensure that the game is still playing...
        if (Application.isPlaying)
        {
            //When DestroyThis is called...destroy the attached object's parent.
            Destroy(this.transform.parent.gameObject);
        }
    }
}
