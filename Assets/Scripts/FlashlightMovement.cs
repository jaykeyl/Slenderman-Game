using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Animator flashlight;

    public InputActionReference flashlightWalkAction;
    public InputActionReference flashlightSprintAction;

    void Start()
    {
        flashlight = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        flashlightSprintAction.action.Enable();
        flashlightWalkAction.action.Enable();
    }

    private void OnDisable()
    {
        flashlightSprintAction.action.Disable();
        flashlightWalkAction.action.Disable();
    }
    void Update()
    {
        Vector2 moveInput = flashlightWalkAction.action.ReadValue<Vector2>(); //obtenemos el valor no es que lo modificamos, read only
        bool isMoving = moveInput.magnitude > 0.1f; // apenas tenga una magnitud mayor a 0.1, es que se esta muviendo y entonces is moving es true
        bool isSprinting = flashlightSprintAction.action.IsPressed(); //si esta presionada la tecla de sprint, entonces is sprinting es true

        if (isMoving)
        {
            if (isSprinting)
            {
                flashlight.ResetTrigger("walk");
                flashlight.SetTrigger("sprint");
            }
            else
            {
                flashlight.ResetTrigger("sprint");
                flashlight.SetTrigger("walk");
            }
        }
        else
        {
            flashlight.ResetTrigger("sprint");
            flashlight.ResetTrigger("walk");
        }
        if (isSprinting && !isMoving)
        {
            flashlight.ResetTrigger("walk");
            flashlight.SetTrigger("sprint");
        }
    }
}
