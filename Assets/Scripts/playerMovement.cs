using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    // general:
    public GameController manager;
    public PlayerStats stats;
    Rigidbody2D body;
    private GameObject rig;
    private PlayerAnimationManager animate;

    // player properties;
    public float moveForce;
    public float jumpForce;
    public Vector3 mousePos;
    public float linkSpace; // space needed b.w. rope links
    public float ropeDeleteDistance; // distance b.w. player and rope to which the rope is then deleted
    public float maxVerticalSpeed;
    bool isOnGround = false;
    public float distToGround; // approx distance from player pos. to bottom collider edge.
    bool invincible = false;

    // camera:
    private Camera mainCamera;
    float prevY = 0;
    Plane mouseProjPlane;
    float mouseRayDistance;

    // shield vars:
    private GameObject shield;
    public bool shieldOn = false;

    // firing (gun) vars:
    public Transform gunEdgeTransform;

    // rope vars:
    public float maxRopeLength;
    List<GameObject> currRope = new List<GameObject>();
    private bool isRoping = false;
    private bool isExtraPulling = false;
    LineRenderer ropeLine;
    Vector3 ropeEnd;

    // iodine vars:
    bool isSlowingTime = false;

    // mode variables:
    private char currMode;
    public Material dashSuit;
    public Material fireSuit;
    public Material shieldSuit;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mouseProjPlane = new Plane(Vector3.back, transform.position);
        body = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        rig = transform.Find("PlayerRig").gameObject;
        animate = rig.GetComponent<PlayerAnimationManager>();
        currMode = 'r';
        shield = transform.Find("NewShield").gameObject;
        shieldOn = false;
        shield.SetActive(false);

        ropeLine = GetComponent<LineRenderer>();
        ropeLine.useWorldSpace = true;
        ropeLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }



    private void FixedUpdate()
    {
        // maintain cursor position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        mouseProjPlane.Raycast(ray, out mouseRayDistance);
        mousePos = ray.GetPoint(mouseRayDistance);

        // fall speed limit:
        if (!isExtraPulling && body.velocity.y < -maxVerticalSpeed)
        {
            body.velocity = new Vector2(body.velocity.x, -maxVerticalSpeed);
        }

        // Tracking with the camera after the character.
        float cameraOffset = Mathf.Min(0, 0.95f * prevY + 0.05f * (1.8f * body.velocity.y / (maxVerticalSpeed )));
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y + cameraOffset, mainCamera.transform.position.z);
        prevY = cameraOffset;
        //mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);

        // during rope casting:
        if (isRoping)
        {
            lineCast();
        }

        if (shieldOn)
        {
            //maintainShield();
            shield.transform.Rotate(0, 0, 40 * Time.deltaTime);
        }
    }


    public void move(char dir)
    {
        float midAirFactor = (isOnGround) ? 1 : 0.5f;
        if (dir == 'l')
        {
            body.AddForce(Vector3.left * moveForce * midAirFactor, ForceMode2D.Impulse);
        }
        else if (dir == 'r')
        {
            body.AddForce(Vector3.right * moveForce * midAirFactor, ForceMode2D.Impulse);
        }
    }

    public void jump()
    {
        if (isOnGround)
        {
            animate.jump();
            body.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            animate.setGrounded(false);
        }
    }

    public void turnLeft(bool isLeft)
    {
        if (isLeft)
        {
            rig.transform.localRotation = Quaternion.Euler(0, -90, 0);
        }

        else rig.transform.localRotation = Quaternion.Euler(0, 90, 0);
    }

    public void shoot()
    {
        if (!stats.tryUsePower(currMode))
        {
            return;
        }
        if (currMode == 'r')  // casting new rope
        {
            if (isRoping)
            {
                return;
            }
            deleteRope();
            currRope.Add(ropeLinkHandler(mousePos));
            isRoping = true;
        }
        else if (currMode == 'f')
        {
            fireShot(mousePos);
        }
        else if (currMode == 's')
        {
            startShield();
        }
    }

    public void stopShoot()
    {
        if (currMode == 's')
        {
            shieldOn = false;
            shield.SetActive(false);
        }
        if (currMode == 'r')
        {
            isRoping = false;
            generateRope(ropeEnd);
        }
    }


    public void startShield()
    {
        shield.SetActive(true);
        shieldOn = true;
        StartCoroutine(shieldPowerUse());

        IEnumerator shieldPowerUse()
        {
            while (shieldOn)
            {
                yield return new WaitForSeconds(0.3f);
                if (!stats.tryUsePower('s'))
                {
                    shieldOn = false;
                    shield.SetActive(false);
                }
            }
        }

    }

    //private void maintainShield()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
    //    float distance = transform.position.z - mainCamera.transform.position.z;
    //    Vector3 pos = ray.GetPoint(distance);
    //    Vector3 shieldDir = (pos - transform.position).normalized;
    //    shield.transform.position = transform.position + shieldDir * 1.5f;
    //    shield.transform.up = shieldDir;
    //}



    private void fireShot(Vector3 pos)
    {
        Vector3 shotDirection = (pos - gunEdgeTransform.position).normalized;
        GameObject shot = manager.getShot(true);
        shot.transform.position = gunEdgeTransform.position;
        shot.transform.up = shotDirection;
    }

    public void slowTime(bool toStart)
    {
        if (toStart)
        {
            if (!stats.setIodine(-1))
            { return; }
            stats.IodineOn();
            Time.timeScale = 0.15f;
            Time.fixedDeltaTime *= Time.timeScale;
            isSlowingTime = true;
            StartCoroutine(useIodine());
        }
        else
        {
            stats.IodineOff();
            isSlowingTime = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        IEnumerator useIodine()
        {
            while (isSlowingTime)
            {
                yield return new WaitForSecondsRealtime(1f);
                if (!stats.setIodine(-1))
                {
                    slowTime(false);
                }
            }
        }
    }

    public void changeMode(char mode)
    {
        Material modeSuit = null;
        switch (mode)
        {
            case 'r':
                modeSuit = dashSuit;
                break;
            case 'f':
                modeSuit = fireSuit;
                break;
            case 's':
                modeSuit = shieldSuit;
                break;
        }
        currMode = mode;
        rig.transform.Find("PlayerMesh").gameObject.GetComponent<SkinnedMeshRenderer>().material = modeSuit;
        stats.setMode(mode);
        if (shieldOn && mode != 's')
        {
            shieldOn = false;
            shield.SetActive(false);    
        }
        if (isRoping && mode != 'r')
        {
            isRoping = false;
            generateRope(ropeEnd);
        }
    }

    private void takeHit()
    {
        if (invincible || shieldOn) return;
        StartCoroutine(tempInvincibility(2));
        animate.takeHit();
        stats.healthDown();

        IEnumerator tempInvincibility(float time)
        {
            invincible = true;
            yield return new WaitForSeconds(time);
            invincible = false;
        }
            
    }


    void hazardKickBack(Vector2 touchPoint)
    {
        Vector2 kickBack = (body.position - touchPoint).normalized * jumpForce * 0.2f;
        body.AddForce(kickBack, ForceMode2D.Impulse);
    }


    void takeIodine()
    {
        stats.setIodine(3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyShot" || collision.tag == "Laser" || collision.tag == "Ghost")
        {
            takeHit();
        }
        if (collision.tag == "Iodine")
        {
            takeIodine();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, distToGround + 0.1f))
            {
                isOnGround = true;
                animate.setGrounded(true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Hazard")
        {
            hazardKickBack(collision.GetContact(0).point);
            takeHit();
        }

        else if (collision.collider.tag == "Rope")
        {
            if (currRope.Count > 0 && !isExtraPulling)
            {
                isExtraPulling = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
        {
            isOnGround = false;
            animate.setGrounded(false);
        }

        if (collision.collider.tag == "Rope")
        {
            StartCoroutine(waitForRopeRemove(collision.collider.transform.position));
            Vector2 dir = body.velocity.normalized * Mathf.Max(0.3f, Mathf.Sqrt(body.velocity.magnitude));

            body.AddForce(0.6f * body.velocity, ForceMode2D.Impulse);
            isExtraPulling = false;
        }
    }

    GameObject ropeLinkHandler(Vector3 pos)
    {
        GameObject newLink = manager.getLink(true);
        newLink.transform.position = pos;
        newLink.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        newLink.GetComponent<Collider2D>().enabled = false;
        return newLink;
    }

    void lineCast()
    {
        if (currRope.Count == 0)
        {
            return;
        }
        Vector3 pos1 = currRope[0].transform.position;
        ropeLine.SetPosition(0, pos1);
        if ((mousePos - pos1).magnitude <= maxRopeLength)
        {
            ropeEnd = mousePos;
        }
        else
        {
            ropeEnd = pos1 + (mousePos - pos1).normalized * maxRopeLength;
        }
        ropeLine.SetPosition(1, ropeEnd);
        if (!ropeLine.enabled)
        {
            ropeLine.enabled = true;
        }
    }

    void generateRope(Vector3 endPos)
    {
        if (currRope.Count == 0)
        {
            return;
        }
        ropeLine.enabled = false;
        GameObject p1 = currRope[0];
        GameObject p2 = ropeLinkHandler(endPos);
        Vector3 pos1 = p1.transform.position;
        Vector3 pos2 = p2.transform.position;
        Vector3 linkGap = (pos2 - pos1).normalized;
        linkGap *= linkSpace;
        Vector3 newPos = pos1 + linkGap;
        int i = 1;
        while ((pos1 - pos2).magnitude >= (pos1 - newPos).magnitude)
        {
            currRope.Add(manager.getLink(false));
            currRope[i].transform.position = newPos;
            currRope[i].transform.up = Vector2.Perpendicular(pos1 - pos2);
            currRope[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            currRope[i - 1].GetComponent<SpringJoint2D>().connectedBody = currRope[i].GetComponent<Rigidbody2D>();
            newPos += linkGap;
            i++;
        }
        currRope[i - 1].GetComponent<SpringJoint2D>().connectedBody = p2.GetComponent<Rigidbody2D>();
        currRope.Add(p2);
    }

    IEnumerator waitForRopeRemove(Vector3 ropePos)
    {
        float playerRopeDistance = (transform.position - ropePos).magnitude;
        float duration = 0;
        while (playerRopeDistance < ropeDeleteDistance && duration < 3)
        {
            yield return new WaitForSeconds(0.5f);
            duration += 0.5f;
            playerRopeDistance = (transform.position - ropePos).magnitude;
        }
        deleteRope();
    }

    private void deleteRope()
    {
        for (int i = 0; i < currRope.Count; i++)
        {
            manager.resetLink(currRope[i]);
        }
        currRope.Clear();
    }
}
