using System.Collections;
using UnityEngine;

public class WaterCell : MonoBehaviour
{
    private void Start()
    {
        //Start coroutine.
        StartCoroutine(Expire());
    }

    private IEnumerator Expire()
    {
        //Wait 10 seconds, then destroy this gameobject.
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }
}
