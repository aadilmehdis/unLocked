using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    Light thunder;
    public float delayBetweenFlicker = 0.15f;
    // Start is called before the first frame update
    void Start()
    {
        thunder = GetComponent<Light>();
        thunder.enabled = false;
        StartCoroutine(Flashing());
        
    }

    IEnumerator Flashing(){
        while(true){
            yield return new WaitForSeconds(3.0f);
            thunder.enabled = !thunder.enabled;
            yield return new WaitForSeconds(delayBetweenFlicker);
            thunder.enabled = !thunder.enabled;
            yield return new WaitForSeconds(delayBetweenFlicker);
            thunder.enabled = !thunder.enabled;
            yield return new WaitForSeconds(delayBetweenFlicker);
            thunder.enabled = !thunder.enabled;
            yield return new WaitForSeconds(delayBetweenFlicker);
            thunder.enabled = !thunder.enabled;
            yield return new WaitForSeconds(delayBetweenFlicker);
            thunder.enabled = !thunder.enabled;
            
        }
    }
}
