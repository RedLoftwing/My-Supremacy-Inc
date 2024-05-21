using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameState gameStateScript;
    [SerializeField] private Cheats cheatsScript;
    [SerializeField] protected float health;
    protected float defaultHealth;
    public float strength;

    [SerializeField] private bool isExplodable;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private Transform[] nodeArray;
    [SerializeField] private Transform targetNode;
    private int nodeValue = 1;

    private float distanceToBase;
    private float distanceToNextPoint;

    private bool hasEnemyRecentlySpawned = true;
    private bool valueHasRecentlyChanged = false;

    private void Start()
    {
        //Grab components and stores them as their own variables.
        gameStateScript = GameObject.Find("Manager").GetComponent<GameState>();
        cheatsScript = GameObject.Find("Manager").GetComponent<Cheats>();

        //Sets defaultHealth value.
        defaultHealth = health;


        if (!this.gameObject.CompareTag("Aerial"))
        {
            //Find the Nodes gameobject and then store all it's children as a transform array.
            nodeArray = GameObject.Find("GroundNodes").GetComponentsInChildren<Transform>();
        }
        else
        {
            //Find the Nodes gameobject and then store all it's children as a transform array.
            nodeArray = GameObject.Find("AirNodes").GetComponentsInChildren<Transform>();
        }

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        //Set the targetNode to the correct tranform in the array.
        targetNode = nodeArray[nodeValue];
        //
        transform.LookAt(targetNode);
    }

    public void WaypointReached()
    {
        nodeValue++;
        UpdateInfo();
        //Debug.Log("Node Value is: " + nodeValue + ". Target Node is: " + targetNode);
        //Debug.Log("ANTI-Ding");
    }

    //Called when health needs to be decreased.
    public void DecreaseHealth(float recievedDamage)
    {
        //IF isInvincibleEnemies is false...proceed.
        if (!cheatsScript.isInvincibleEnemies)
        {
            //IF isOneHitKill is false...proceed.
            if (!cheatsScript.isOneHitKill)
            {
                //Reduce the value of health by the value of recievedDamage.
                health -= recievedDamage;

                //IF health is less than or equal to 0...destroy this enemy unit.
                if (health <= 0)
                {
                    //IF isExplodable is true AND application is still playing...spawn an explosion on this position.
                    if (isExplodable)
                    {
                        Instantiate(explosionVFX, transform.position, Quaternion.identity);
                    }
                    Destroy(this.gameObject);
                }
            }
            //ELSE
            else
            {
                //Destroy this enemy unit.
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //On collision enter...get wave component. IF not null, start coroutine.
        var waveComp = other.gameObject.GetComponent<Wave>();
        if (waveComp)
        {
            StartCoroutine(Freeze(4));
        }

        var waterCell = other.gameObject.GetComponent<WaterCell>();
        if(waterCell)
        {
            transform.position = waterCell.gameObject.transform.position;
            StartCoroutine(Freeze(6));
        }
    }

    private IEnumerator Freeze(int duration)
    {
        //Freeze the unit.
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        //IF duration is less than or equal to 4...
        if (duration <= 4)
        {
            //Wait 4 seconds...
            yield return new WaitForSeconds(duration);
        }
        //ELSE...
        else
        {
            //Wait 2 seconds.
            yield return new WaitForSeconds(2);
            //Take a fraction of max health away from the unit.
            DecreaseHealth(defaultHealth/10);
            //Wait 2 seconds.
            yield return new WaitForSeconds(2);
            //Take a fraction of max health away from the unit.
            DecreaseHealth(defaultHealth / 10);
            //Wait 2 seconds.
            yield return new WaitForSeconds(2);
            //Take a fraction of max health away from the unit.
            DecreaseHealth(defaultHealth / 10);

        }
        //Unfreeze the unit.
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void OnDestroy()
    {
        //On destruction of this game object, remove 1 from the thisWavesEnemies value.
        gameStateScript.thisWavesEnemies--;
    }
}
