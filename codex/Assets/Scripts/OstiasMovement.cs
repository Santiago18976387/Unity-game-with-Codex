using UnityEngine;

public class OstiasMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Virtual Joystick")]
    [SerializeField] private bool showJoystick = true;
    [SerializeField] private float joystickRadius = 75f;
    [SerializeField] private float joystickMargin = 25f;
    [SerializeField] private float joystickDeadZone = 0.15f;
    [SerializeField] private Color joystickBaseColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color joystickKnobColor = new Color(1f, 1f, 1f, 0.65f);

    private Vector2 joystickInput;
    private bool joystickPressed;
    private int joystickPointerId = -1;
    private Vector2 joystickCenter;

    private Texture2D baseTexture;
    private Texture2D knobTexture;

    private void Awake()
    {
        baseTexture = CreateCircleTexture((int)(joystickRadius * 2f), joystickBaseColor);
        knobTexture = CreateCircleTexture((int)joystickRadius, joystickKnobColor);
    }

    private void Update()
    {
        HandleVirtualJoystick();

        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) verticalInput -= 1f;
        if (Input.GetKey(KeyCode.UpArrow)) verticalInput += 1f;

        Vector2 keyboardInput = new Vector2(horizontalInput, verticalInput);
        Vector2 combinedInput = Vector2.ClampMagnitude(keyboardInput + joystickInput, 1f);

        Vector3 movement = new Vector3(combinedInput.x, combinedInput.y, 0f);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void HandleVirtualJoystick()
    {
        joystickCenter = new Vector2(joystickMargin + joystickRadius, Screen.height - joystickMargin - joystickRadius);

        if (Input.touchCount > 0)
        {
            HandleTouchJoystick();
            return;
        }

        HandleMouseJoystick();
    }

    private void HandleTouchJoystick()
    {
        bool hasActiveTouch = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (joystickPointerId == -1 && touch.phase == TouchPhase.Began && IsInsideJoystickArea(touch.position))
            {
                joystickPointerId = touch.fingerId;
                joystickPressed = true;
            }

            if (touch.fingerId != joystickPointerId)
            {
                continue;
            }

            hasActiveTouch = true;

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                ReleaseJoystick();
                return;
            }

            UpdateJoystickInput(touch.position);
            return;
        }

        if (joystickPressed && !hasActiveTouch)
        {
            ReleaseJoystick();
        }
    }

    private void HandleMouseJoystick()
    {
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && IsInsideJoystickArea(mousePos))
        {
            joystickPressed = true;
            joystickPointerId = 0;
        }

        if (joystickPressed && Input.GetMouseButton(0))
        {
            UpdateJoystickInput(mousePos);
            return;
        }

        if (joystickPressed && Input.GetMouseButtonUp(0))
        {
            ReleaseJoystick();
        }
    }

    private void UpdateJoystickInput(Vector2 pointerPosition)
    {
        Vector2 offset = pointerPosition - joystickCenter;
        Vector2 normalized = Vector2.ClampMagnitude(offset / joystickRadius, 1f);

        joystickInput = normalized.magnitude < joystickDeadZone ? Vector2.zero : normalized;
    }

    private bool IsInsideJoystickArea(Vector2 pointerPosition)
    {
        return Vector2.Distance(pointerPosition, joystickCenter) <= joystickRadius * 1.6f;
    }

    private void ReleaseJoystick()
    {
        joystickPressed = false;
        joystickPointerId = -1;
        joystickInput = Vector2.zero;
    }

    private void OnGUI()
    {
        if (!showJoystick)
        {
            return;
        }

        float baseSize = joystickRadius * 2f;
        Rect baseRect = new Rect(
            joystickCenter.x - joystickRadius,
            Screen.height - joystickCenter.y - joystickRadius,
            baseSize,
            baseSize);

        GUI.DrawTexture(baseRect, baseTexture);

        Vector2 knobOffset = joystickInput * (joystickRadius * 0.55f);
        float knobSize = joystickRadius;
        Rect knobRect = new Rect(
            baseRect.center.x - (knobSize / 2f) + knobOffset.x,
            baseRect.center.y - (knobSize / 2f) - knobOffset.y,
            knobSize,
            knobSize);

        GUI.DrawTexture(knobRect, knobTexture);
    }

    private Texture2D CreateCircleTexture(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color clear = new Color(0f, 0f, 0f, 0f);
        float radius = size / 2f;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                texture.SetPixel(x, y, distance <= radius ? color : clear);
            }
        }

        texture.Apply();
        return texture;
    }

    private void OnDestroy()
    {
        if (baseTexture != null)
        {
            Destroy(baseTexture);
        }

        if (knobTexture != null)
        {
            Destroy(knobTexture);
        }
    }
}
