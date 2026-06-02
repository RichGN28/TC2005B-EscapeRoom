using System.Collections;
using UnityEngine;

public class MagicChest : MonoBehaviour
{
    [Header("Partes del cofre")]
    public Transform lidPivot;
    public GameObject keyObject;

    [Header("Animacion")]
    public Vector3 openRotationOffset = new Vector3(-80f, 0f, 0f);
    public float openDuration = 1f;

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        if (lidPivot != null)
        {
            closedRotation = lidPivot.localRotation;
            openRotation = Quaternion.Euler(lidPivot.localEulerAngles + openRotationOffset);
        }

        if (keyObject != null)
        {
            keyObject.SetActive(false);
        }
    }

    public void OpenChest()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;

        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }

        if (lidPivot != null)
        {
            StartCoroutine(OpenLid());
        }
    }

    private IEnumerator OpenLid()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / openDuration;

            lidPivot.localRotation = Quaternion.Slerp(closedRotation, openRotation, t);

            yield return null;
        }

        lidPivot.localRotation = openRotation;
    }
}