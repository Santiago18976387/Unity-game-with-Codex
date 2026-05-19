using UnityEngine;

public class OstiasMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) horizontalInput += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) verticalInput -= 1f;
        if (Input.GetKey(KeyCode.UpArrow)) verticalInput += 1f;

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
