using UnityEngine;

public class RectangleMover : MonoBehaviour
{
    public Vector2 bottomLeft = new Vector2(-5f, -3f);
    public Vector2 topRight = new Vector2(5f, 3f);
    public float speed = 1f;

    public Color color1 = Color.green;
    public Color color2 = Color.red;
    public float blinkInterval = 0.5f;

    private Vector3[] corners;
    private int currentTarget = 0;

    private Renderer rend;
    private float blinkTimer;
    private bool isColor1 = true;
    private bool is_Active = false;

    public void Init()
    {
        corners = new Vector3[4];
        corners[0] = new Vector3(bottomLeft.x, 0, bottomLeft.y);
        corners[1] = new Vector3(topRight.x, 0, bottomLeft.y);
        corners[2] = new Vector3(topRight.x, 0, topRight.y);
        corners[3] = new Vector3(bottomLeft.x, 0, topRight.y);

        transform.localPosition = corners[0];

        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color1;
        }

        blinkTimer = blinkInterval;
        is_Active = true;
    }

    void Update()
    {
        if (is_Active) 
        {
            MoveAlongRectangle();
            BlinkColor();
        }
    }

    void MoveAlongRectangle()
    {
        Vector3 target = corners[currentTarget];
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, target) < 0.01f)
        {
            currentTarget = (currentTarget + 1) % corners.Length;
        }
    }

    void BlinkColor()
    {
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            isColor1 = !isColor1;
            rend.material.color = isColor1 ? color1 : color2;
            blinkTimer = blinkInterval;
        }
    }
}
