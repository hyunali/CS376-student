using UnityEngine;

public class TargetBox : MonoBehaviour
{
    /// <summary>
    /// Targets that move past this point score automatically.
    /// </summary>
    public static float OffScreen;

    private SpriteRenderer targetSpriteRenderer;
    private Rigidbody2D targetRigidBody;
    private Color targetColor;

    internal void Start() {
        OffScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-100, 0, 0)).x;
        targetRigidBody = GetComponent<Rigidbody2D>();
        targetSpriteRenderer = GetComponent<SpriteRenderer>();
        targetColor = Color.green;
    }

    internal void Update()
    {
        if (transform.position.x > OffScreen)
            Scored();
    }

    private void Scored()
    {
        // FILL ME IN
        targetSpriteRenderer.color = targetColor;
        ScoreKeeper.AddToScore(targetRigidBody.mass);
        
        // make sure this only calls the first time the targetbox hits the ground
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO
        if (collision.collider.tag == "Ground") // get component return the component or null
        {   
            Scored(); // call scored if you hit the ground
        }
    }
}
