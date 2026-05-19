using UnityEngine;

public class OstiasMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }

        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            verticalInput = -1f;
        }

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * (speed * Time.deltaTime);
        Vector3 movement = Vector3.up * (verticalInput * speed * Time.deltaTime);
        transform.Translate(movement);
    }
}
