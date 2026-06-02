using System.Collections;
using UnityEngine;

public class MagicChest : MonoBehaviour
{
    [Header("Animacion del cofre")]
    public Animator chestAnimator;
    public string openAnimationName = "Open";

    [Header("Llave")]
    public GameObject keyObject;
    public float keyRevealDelay = 0.8f;

    private bool isOpen;

    private void Start()
    {
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

        if (chestAnimator != null)
        {
            chestAnimator.Play(openAnimationName);
        }

        StartCoroutine(ShowKeyAfterDelay());
    }

    private IEnumerator ShowKeyAfterDelay()
    {
        yield return new WaitForSeconds(keyRevealDelay);

        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
    }
}