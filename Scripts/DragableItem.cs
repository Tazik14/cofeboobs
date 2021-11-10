using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int objectIndex;
    public GameObject Object;

    private bool isDrag;
    private Transform objectTransform;
    private Vector3 mousePos;
    private Animator animator;
    private ParticleSystem dropParticle;

    void Update()
    {
        if (isDrag)
        {
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

            objectTransform.position = mousePos;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;

        objectTransform = Instantiate(Object).transform;

        dropParticle = objectTransform.GetChild(0).GetComponent<ParticleSystem>();
        animator = objectTransform.GetComponent<Animator>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Machine")))
        {
            if (hit.collider.tag == "Machine")
            {
                CoffeManager.instance.AddIngredient(objectIndex);
                animator.Play("DropItem");
                dropParticle.Play();
            }
        }
        else
        {
            Destroy(objectTransform.gameObject);
        }
    }
}
