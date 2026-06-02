using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Configuracion")]
    public string requiredTag = "PiedraMagica";
    public float activationDelay = 0.4f;
    public bool activateOnlyOnce = true;

    [Header("Visual de la placa")]
    public Transform plateVisual;
    public float pressDistance = 0.08f;
    public float pressSpeed = 4f;

    [Header("Eventos")]
    public UnityEvent onPlateActivated;

    private bool isActivated;
    private bool isChecking;
    private Vector3 originalPlatePosition;
    private Vector3 pressedPlatePosition;
    private Coroutine checkCoroutine;

    private void Start()
    {
        if (plateVisual != null)
        {
            originalPlatePosition = plateVisual.localPosition;
            pressedPlatePosition = originalPlatePosition + Vector3.down * pressDistance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated && activateOnlyOnce)
        {
            return;
        }

        if (!other.CompareTag(requiredTag))
        {
            return;
        }

        if (!isChecking)
        {
            checkCoroutine = StartCoroutine(CheckActivation(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(requiredTag))
        {
            return;
        }

        if (isChecking && checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            isChecking = false;
        }
    }

    private IEnumerator CheckActivation(Collider objectDetected)
    {
        isChecking = true;

        yield return new WaitForSeconds(activationDelay);

        if (objectDetected != null)
        {
            ActivatePlate();
        }

        isChecking = false;
    }

    private void ActivatePlate()
    {
        if (isActivated && activateOnlyOnce)
        {
            return;
        }

        isActivated = true;
        onPlateActivated.Invoke();

        if (plateVisual != null)
        {
            StopAllCoroutines();
            StartCoroutine(MovePlateDown());
        }
    }

    private IEnumerator MovePlateDown()
    {
        while (Vector3.Distance(plateVisual.localPosition, pressedPlatePosition) > 0.01f)
        {
            plateVisual.localPosition = Vector3.Lerp(
                plateVisual.localPosition,
                pressedPlatePosition,
                Time.deltaTime * pressSpeed
            );

            yield return null;
        }

        plateVisual.localPosition = pressedPlatePosition;
    }
}