using System.Collections;
using UnityEngine;

public class MagicChest : MonoBehaviour
{
    [Header("Animacion del cofre")]
    public Animator chestAnimator;
    public string openAnimationName = "Fantasy_Polygon_Chest_Animation";

    [Header("Tiempo")]
    public float stopAnimatorDelay = 0.8f;
    public float keyRevealDelay = 0.1f;

    [Header("Llave")]
    public GameObject keyObject;

    private bool isOpen;

    [Tooltip("Arrastra aquí tu archivo de sonido")]
    [SerializeField] private AudioClip sonidoApertura;
    [SerializeField] private AudioClip puzzleCorrecto;

    private AudioSource audioLocal;

    private void Start()
    {
        audioLocal = GetComponent<AudioSource>();
        if (keyObject != null)
        {
            keyObject.SetActive(false);
        }

        if (chestAnimator != null)
        {
            chestAnimator.enabled = false;
            chestAnimator.speed = 1f;
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
            chestAnimator.enabled = true;
            chestAnimator.speed = 1f;
            chestAnimator.Play(openAnimationName, 0, 0f);

            if (audioLocal != null && sonidoApertura != null)
            {
                audioLocal.PlayOneShot(sonidoApertura);
                audioLocal.PlayOneShot(puzzleCorrecto);
            }



        }

        StartCoroutine(StopChestWhenOpen());
    }

    private IEnumerator StopChestWhenOpen()
    {
        yield return new WaitForSeconds(stopAnimatorDelay);

        if (chestAnimator != null)
        {
            chestAnimator.speed = 0f;
        }

        yield return new WaitForSeconds(keyRevealDelay);

        if (keyObject != null)
        {
            keyObject.SetActive(true);
        }
    }
}