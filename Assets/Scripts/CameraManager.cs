using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    bool canPan;
    public float panSpeed;
    Vector2 panVec;
    int minX = -5;
    int maxX = 4;
    int minY = -3;
    int maxY = 2;

    private void Awake()
    {
        instance = this;
        canPan = false;
    }
    private void Update()
    {
        if (panVec != Vector2.zero)
        {
            PanCamera();
        }
    }
    public void PanCamera()
    {
        if (canPan)
        {
            transform.position += new Vector3(panVec.x * panSpeed, panVec.y * panSpeed);
            if (transform.position.x > maxX || transform.position.x < minX || transform.position.y > maxY || transform.position.y < minY)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), -10);
                panVec = Vector2.zero;
            }
        }
    }

    public void SetPan(Vector2 vector)
    {
        panVec = vector;
    }

    public void Zoom(float value)
    {
        if(value < 0)
        {
            Camera.main.orthographicSize = 10;
            transform.position = new Vector3(-.5f, -.5f, -10);
            canPan = false;
        }
        if(value > 0)
        {
            Camera.main.orthographicSize = 7.5f;
            transform.position = new Vector3(0, 0, -10);
            canPan = true;
        }
    }
}
