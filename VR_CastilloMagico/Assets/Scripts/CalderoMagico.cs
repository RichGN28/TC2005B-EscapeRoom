using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CalderoMagico : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    [Tooltip("El orden exacto de los tags para resolver el puzzle")]
    [SerializeField] private List<string> ordenCorrectoReceta;

    [Tooltip("Arrastra aquí TODOS los ingredientes que participan en este puzzle")]
    [SerializeField] private List<GameObject> todosLosIngredientes;

    // Diccionarios para guardar la posición y rotación original de cada ingrediente
    private Dictionary<GameObject, Vector3> posicionesIniciales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> rotacionesIniciales = new Dictionary<GameObject, Quaternion>();

    private int pasoActual = 0;

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

        if (objetoInteractuado.CompareTag(ordenCorrectoReceta[pasoActual]))
        {
            IngredienteCorrecto(objetoInteractuado);
        }
        else
        {
            IngredienteIncorrecto(objetoInteractuado);
        }
    }

    private void IngredienteCorrecto(GameObject ingrediente)
    {
        pasoActual++;
        Debug.Log("¡Ingrediente correcto! Paso: " + pasoActual);

        // Desconectamos el objeto del socket
        socket.interactionManager.SelectExit(socket, (IXRSelectInteractable)ingrediente.GetComponent<XRGrabInteractable>());

        // En lugar de Destroy(), lo ocultamos. Así existe en memoria para poder recuperarlo.
        ingrediente.SetActive(false);

        if (pasoActual >= ordenCorrectoReceta.Count)
        {
            AbrirPuertaCastillo();
        }
    }

    private void IngredienteIncorrecto(GameObject ingrediente)
    {
        Debug.Log("Ingrediente incorrecto. Poción arruinada. Reiniciando el puzle...");

        // Forzar al socket a soltar el objeto incorrecto
        socket.interactionManager.SelectExit(socket, (IXRSelectInteractable)ingrediente.GetComponent<XRGrabInteractable>());

        // Reiniciar la lógica del puzle
        pasoActual = 0;

        // Llamamos a la función que restaura la habitación
        RestaurarIngredientes();
    }

    private void RestaurarIngredientes()
    {
        // Recorremos la lista de todos los ingredientes involucrados
        foreach (GameObject ingrediente in todosLosIngredientes)
        {
            if (ingrediente != null)
            {
                // 1. Los volvemos a hacer visibles (por si estaban "destruidos/ocultos" en el caldero)
                ingrediente.SetActive(true);

                // 2. Detenemos sus físicas. Si caen de golpe conservan inercia y podrían salir volando.
                Rigidbody rb = ingrediente.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // 3. Los teletransportamos a las posiciones y rotaciones que guardamos en el Start()
                ingrediente.transform.position = posicionesIniciales[ingrediente];
                ingrediente.transform.rotation = rotacionesIniciales[ingrediente];
            }
        }
    }

    private void AbrirPuertaCastillo()
    {
        Debug.Log("¡Poción completada! La puerta se abre.");
    }
}