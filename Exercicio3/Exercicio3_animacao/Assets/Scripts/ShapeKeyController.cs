using UnityEngine;
using UnityEngine.InputSystem;

public class ShapeKeyController : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMesh;
    private Animator animator;

    private bool manualMode = false;

    private float weight1 = 0f;
    private float weight2 = 0f;
    private float weight3 = 0f;

    void Start()
    {
        // Busca o Render no objeto filho
        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        // Busca o Animator no objeto atual
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            manualMode = !manualMode;

            // Ativa ou desativa a animação automática com o SPACE
            if (animator != null) animator.enabled = !manualMode;

            Debug.Log("Modo Manual: " + manualMode);
        }

        if (manualMode && skinnedMesh != null)
        {
            // Controla a Shape Key 1
            if (Keyboard.current.qKey.isPressed) weight1 = Mathf.Clamp(weight1 + Time.deltaTime * 100, 0, 100);
            if (Keyboard.current.aKey.isPressed) weight1 = Mathf.Clamp(weight1 - Time.deltaTime * 100, 0, 100);

            // Controla a Shape Key 2
            if (Keyboard.current.wKey.isPressed) weight2 = Mathf.Clamp(weight2 + Time.deltaTime * 100, 0, 100);
            if (Keyboard.current.sKey.isPressed) weight2 = Mathf.Clamp(weight2 - Time.deltaTime * 100, 0, 100);

            // Controla a Shape Key 3
            if (Keyboard.current.eKey.isPressed) weight3 = Mathf.Clamp(weight3 + Time.deltaTime * 100, 0, 100);
            if (Keyboard.current.dKey.isPressed) weight3 = Mathf.Clamp(weight3 - Time.deltaTime * 100, 0, 100);

            skinnedMesh.SetBlendShapeWeight(0, weight1);
            skinnedMesh.SetBlendShapeWeight(1, weight2);
            skinnedMesh.SetBlendShapeWeight(2, weight3);
        }
    }
}