using UnityEngine;

public class PuertaMagica : MonoBehaviour
{
    [Tooltip("Los grados en el eje Y que rotará la puerta al abrirse (ej. 90 o -90)")]
    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadApertura = 2f;

    private bool abriendo = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionFinal;

    void Start()
    {
        rotacionInicial = transform.rotation;
        // Calculamos cuál será la rotación objetivo sumando el ángulo de apertura
        rotacionFinal = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + anguloApertura, transform.eulerAngles.z);
    }

    void Update()
    {
        // Si recibimos la orden, rotamos la puerta suavemente hacia su destino
        if (abriendo)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionFinal, Time.deltaTime * velocidadApertura);
        }
    }

    // ¡ESTE ES EL MÉTODO PÚBLICO QUE LLAMAREMOS DESDE AFUERA!
    public void AbrirPuerta()
    {
        abriendo = true;
        // Aquí podrías agregar: audioSource.PlayOneShot(sonidoRechinido);
    }

}
