using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.Events;

public class CalderoMagico : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    [Tooltip("El orden exacto de los tags para resolver el puzzle")]
    [SerializeField] private List<string> ordenCorrectoReceta;

    [Tooltip("Arrastra aquí TODOS los ingredientes que participan en este puzzle")]
    [SerializeField] private List<GameObject> todosLosIngredientes;

    [Tooltip("Arrastra aquí lo que deba pasar cuando se complete la poción")]
    public UnityEvent alCompletarPocion;

    // Diccionarios para guardar la posición y rotación original de cada ingrediente
    private Dictionary<GameObject, Vector3> posicionesIniciales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> rotacionesIniciales = new Dictionary<GameObject, Quaternion>();

    // Lista para guardar lo que el jugador va metiendo al caldero
    private List<string> tagsIngresados = new List<string>();

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    void Start()
    {
        // Al iniciar el juego, guardamos las coordenadas exactas de cada ingrediente
        foreach (GameObject ingrediente in todosLosIngredientes)
        {
            if (ingrediente != null)
            {
                posicionesIniciales.Add(ingrediente, ingrediente.transform.position);
                rotacionesIniciales.Add(ingrediente, ingrediente.transform.rotation);
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

        // 2. Desconectar el objeto del socket para dejarlo libre
        socket.interactionManager.SelectExit(socket, (IXRSelectInteractable)objetoInteractuado.GetComponent<XRGrabInteractable>());

        // 3. Ocultar el ingrediente (simulando que se hundió en la mezcla)
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

        // Comparamos lo que ingresó el jugador contra la receta original
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
            Debug.Log("Poción arruinada. El orden fue incorrecto. Reiniciando el puzle...");

            // Aquí puedes agregar un sonido de explosión o fallo

            // Limpiamos la lista temporal para volver a empezar
            tagsIngresados.Clear();

            // Restauramos los ingredientes a su lugar
            RestaurarIngredientes();
        }
    }

    private void RestaurarIngredientes()
    {
        // Recorremos la lista de todos los ingredientes involucrados
        foreach (GameObject ingrediente in todosLosIngredientes)
        {
            if (ingrediente != null)
            {
                // 1. Los volvemos a hacer visibles
                ingrediente.SetActive(true);

                // 2. Detenemos sus físicas. 
                Rigidbody rb = ingrediente.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // 3. Los teletransportamos a las posiciones iniciales
                ingrediente.transform.position = posicionesIniciales[ingrediente];
                ingrediente.transform.rotation = rotacionesIniciales[ingrediente];
            }
        }
    }

    private void AbrirPuertaCastillo()
    {
        Debug.Log("¡Poción completada! Lanzando evento...");
        alCompletarPocion.Invoke();
    }
}