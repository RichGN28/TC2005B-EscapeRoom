using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;

public class CalderoMagico : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    [Tooltip("El orden exacto de los tags para resolver el puzzle (Ej: Ingrediente1, Ingrediente2, Ingrediente3)")]
    [SerializeField] private List<string> ordenCorrectoReceta;

    [Tooltip("Arrastra aquí TODOS los ingredientes que participan en este puzzle, incluyendo los falsos")]
    [SerializeField] private List<GameObject> todosLosIngredientes;

    [Tooltip("Arrastra aquí la puerta o los eventos que deben ocurrir al ganar")]
    public UnityEvent alCompletarPocion;

    // Diccionarios para guardar el estado original de cada ingrediente (Posición, Rotación y Tamaño)
    private Dictionary<GameObject, Vector3> posicionesIniciales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> rotacionesIniciales = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, Vector3> escalasIniciales = new Dictionary<GameObject, Vector3>();

    // Lista temporal para guardar lo que el jugador va metiendo al caldero
    private List<string> tagsIngresados = new List<string>();

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    void Start()
    {
        // Al iniciar el juego, guardamos las coordenadas, rotación y escala exacta de cada ingrediente
        foreach (GameObject ingrediente in todosLosIngredientes)
        {
            if (ingrediente != null)
            {
                posicionesIniciales.Add(ingrediente, ingrediente.transform.position);
                rotacionesIniciales.Add(ingrediente, ingrediente.transform.rotation);
                escalasIniciales.Add(ingrediente, ingrediente.transform.localScale); // Previene el bug de la poción gigante
            }
        }
    }

    void OnEnable()
    {
        socket.selectEntered.AddListener(OnIngredienteEnter);
    }

    void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnIngredienteEnter);
    }

    private void OnIngredienteEnter(SelectEnterEventArgs args)
    {
        GameObject objetoInteractuado = args.interactableObject.transform.gameObject;

        // 1. Guardar el tag del objeto que acaba de entrar en nuestra lista temporal
        tagsIngresados.Add(objetoInteractuado.tag);
        Debug.Log("Ingrediente echado: " + objetoInteractuado.tag + ". Total en caldero: " + tagsIngresados.Count);

        // 2. Desconectar el objeto del socket para dejarlo libre y listo para el siguiente
        socket.interactionManager.SelectExit(socket, (IXRSelectInteractable)objetoInteractuado.GetComponent<XRGrabInteractable>());

        // 3. Ocultar el ingrediente (simulando que se disolvió en el líquido)
        objetoInteractuado.SetActive(false);

        // 4. Checar si ya echamos la cantidad total que requiere la receta
        if (tagsIngresados.Count >= ordenCorrectoReceta.Count)
        {
            VerificarReceta();
        }
    }

    private void VerificarReceta()
    {
        bool esCorrecta = true;

        // Comparamos el historial de lo que ingresó el jugador contra la receta original
        for (int i = 0; i < ordenCorrectoReceta.Count; i++)
        {
            if (tagsIngresados[i] != ordenCorrectoReceta[i])
            {
                esCorrecta = false;
                break; // Rompemos el ciclo al primer error encontrado
            }
        }

        if (esCorrecta)
        {
            AbrirPuertaCastillo();
        }
        else
        {
            Debug.Log("Poción arruinada. El orden o los ingredientes fueron incorrectos. Reiniciando...");

            // Limpiamos la lista temporal para que el jugador vuelva a intentarlo desde cero
            tagsIngresados.Clear();

            // Devolvemos todos los objetos a su estado inicial
            RestaurarIngredientes();
        }
    }

    private void RestaurarIngredientes()
    {
        // Recorremos la lista de todos los ingredientes involucrados en la escena
        foreach (GameObject ingrediente in todosLosIngredientes)
        {
            if (ingrediente != null)
            {
                // 1. Los volvemos a hacer visibles
                ingrediente.SetActive(true);

                // 2. Detenemos sus físicas para que no salgan volando por la inercia acumulada
                Rigidbody rb = ingrediente.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // 3. Restauramos las posiciones, rotaciones y la escala original
                ingrediente.transform.position = posicionesIniciales[ingrediente];
                ingrediente.transform.rotation = rotacionesIniciales[ingrediente];
                ingrediente.transform.localScale = escalasIniciales[ingrediente];
            }
        }
    }

    private void AbrirPuertaCastillo()
    {
        Debug.Log("¡Poción completada con éxito! Lanzando eventos de victoria...");
        alCompletarPocion.Invoke();
    }
}