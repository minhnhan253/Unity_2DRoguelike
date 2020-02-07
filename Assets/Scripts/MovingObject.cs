using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float movingTime = 0.1f;
    public LayerMask blockingPlayer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / movingTime;
    }
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;
        while(sqrRemainDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCollider.enabled = false;
        hit =Physics2D.Linecast(start, end, blockingPlayer);
        boxCollider.enabled = true;
        if (hit.transform ==null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }
    protected virtual void AttemptMove<T> (int xDir, int yDir) where T: Component
    {
        RaycastHit2D hit2D;
        bool canMove = Move(xDir, yDir, out hit2D);
        if (hit2D.transform == null)
            return;
        T hitComponent = hit2D.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
            onCantMove(hitComponent);

    }
    protected abstract void onCantMove <T> (T component) where T: Component;

    // Update is called once per frame
    void Update()
    {
        
    }
}
