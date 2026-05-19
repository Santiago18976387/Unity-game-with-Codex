using UnityEngine;

public class OstiasMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            verticalInput = -1f;
        }

        Vector3 movement = Vector3.up * (verticalInput * speed * Time.deltaTime);
        transform.Translate(movement);
    }
}
