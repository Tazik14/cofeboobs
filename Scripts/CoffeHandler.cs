using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeHandler : MonoBehaviour
{
    private float handleAngle;
    private Coroutine progressCor;
    private Transform coffeHandler;
    public Transform coffeStream;

    [System.Obsolete]
    void Update()
    {
        //крутить рычаг
        if (Input.GetMouseButton(0) && CoffeManager.instance.coffeProgress < 100 && CoffeManager.instance.GetIngredient())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Handler")))
            {
                if (hit.collider.tag == "CoffeMachine")
                {
                    coffeHandler = hit.transform;

                    float rotX = Input.GetAxis("Mouse X") * Mathf.Deg2Rad;
                    coffeHandler.localEulerAngles = 
                        new Vector3(0, Mathf.Clamp(coffeHandler.localEulerAngles.y + (rotX * 240), 0, 35), 0);

                    handleAngle = coffeHandler.localEulerAngles.y;
                }
            }
        }

        if (handleAngle > 5)
        {
            coffeStream.transform.localPosition = 
                Vector3.Lerp(coffeStream.transform.localPosition, new Vector3(0.8467561f, 0.85f, 2.093815f), Time.deltaTime * 20);

            if (progressCor == null)
            {
                progressCor = StartCoroutine(ProgressCoffe());
            }

            if (!CoffeManager.instance.stinkyParticle.loop)
            {
                CoffeManager.instance.stinkyParticle.Play();
                CoffeManager.instance.cloudParticle.Play();
                CoffeManager.instance.stinkyParticle.loop = true;
                CoffeManager.instance.cloudParticle.loop = true;
            }
        }
        else
        {
            coffeStream.transform.localPosition = 
                Vector3.Lerp(coffeStream.transform.localPosition, new Vector3(0.8467561f, 1.1f, 2.093815f), Time.deltaTime * 20);

            if (progressCor != null)
            {
                StopCoroutine(progressCor);
                progressCor = null;
            }

            if (CoffeManager.instance.stinkyParticle.loop)
            {
                CoffeManager.instance.stinkyParticle.loop = false;
                CoffeManager.instance.cloudParticle.loop = false;
            }
        }
    }

    //стру€ кофе(мочи) и прогресс бар
    public IEnumerator ProgressCoffe()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.03f);

            if (CoffeManager.instance.coffeProgress < 100)
            {
                CoffeManager.instance.coffeProgress++;
                CoffeManager.instance.progressText.text = CoffeManager.instance.coffeProgress.ToString() + "%";
            }
            else
            {
                StopCoroutine(progressCor);
                progressCor = null;

                handleAngle = 0;
                coffeHandler.localEulerAngles = Vector3.zero;

                CoffeManager.instance.CompleteCoffe();
            }
        }
    }
}
