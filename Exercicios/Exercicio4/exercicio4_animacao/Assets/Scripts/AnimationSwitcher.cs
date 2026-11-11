using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationSwitcher : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Tecla 1 ativa a animação 1
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            animator.SetTrigger("PlayAnim1");
        }

        // Tecla 2 ativa a animação 2
        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            animator.SetTrigger("PlayAnim2");
        }

        // Tecla 3 ativa a animação 3
        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            animator.SetTrigger("PlayAnim3");
        }
    }
}