using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public float speed;
    public float jumpForce;
    public Text score;
    private int scoreValue = 0;
    public Text lives;
    private int livesValue = 3;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    private bool facingRight = true;
    private bool isOnGround;
    public Transform groundcheck;
    public float checkRadius;
    public LayerMask allGround;
    public Text hozText;
    public Text jumpText;
    public AudioSource musicSource;

    public AudioClip musicClipOne;

    public AudioClip musicClipTwo;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //for this gameobject look at the components, finds the component that's a rigidbody2d and save a reference to it
        rd2d = GetComponent<Rigidbody2D>();

        SetScoreText();
        winTextObject.SetActive(false);

        SetLivesText();
        loseTextObject.SetActive(false);

        musicSource.clip = musicClipOne;
        musicSource.Play();

        anim = GetComponent<Animator>();
    }

    // FixedUpdate is called based on the physics input
    void FixedUpdate()
    {
        //uses the default input keys for horizontal and vertical movements
        float hozMovement = Input.GetAxis("Horizontal");
        float verMovement = Input.GetAxis("Vertical");
        //Setting the value of isOnGround. Makes it a circle that overlaps with other layers and checks to see if it's contacting allGround
        isOnGround = Physics2D.OverlapCircle(groundcheck.position, checkRadius, allGround);

        //for the default input, if there's any values involved with horizontal or vertical, save those values and apply the force to rd2d
        //multiplies by speed so the horizontal and vertical movement can be set in unity as a public variable
        rd2d.AddForce(new Vector2(hozMovement * speed, verMovement * speed));

        //Flipping the Player
        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }

        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }

        if (hozMovement > 0 && facingRight == true)
        {
            //Debug.log prints the message into your console
            Debug.Log ("Facing Right");
            hozText.text = "Facing Right";
        }

        if (hozMovement < 0 && facingRight == false)
        {
            Debug.Log ("Facing Left");
            hozText.text = "Facing Left";
        }

        if (verMovement > 0 && isOnGround == false)
        {
            Debug.Log ("Jumping");
            jumpText.text = "Jumping";
        }

        else if (verMovement == 0 && isOnGround == true)
        {
            Debug.Log ("Not Jumping");
            jumpText.text = "Not Jumping";
        }
    }    
        
    void Update()
    {    
        //Movement animation
        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetInteger("State", 2);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            //Sets state to 1 when W is pressed
            anim.SetInteger("State", 1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetInteger("State", 1);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetInteger("State", 0);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetInteger("State", 0);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetInteger("State", 0);
        }
    }

    void SetScoreText()
    {
        score.text = "Coins: " + scoreValue.ToString();
        if (scoreValue >= 9)
        {
            winTextObject.SetActive(true);

            musicSource.clip = musicClipOne;
            musicSource.Stop();

            musicSource.clip = musicClipTwo;
            musicSource.Play();
        }
    }

    void SetLivesText()
    {
        lives.text = "Lives: " + livesValue.ToString();
        if (livesValue <= 0)
        {
            loseTextObject.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the gameObject is tagged "coin" destroy it once you collide
        if(collision.collider.tag == "Coin")
        {
            //each time you collide increase score by 1
            scoreValue += 1;
            SetScoreText();
            //changes scoretext to the string integer value
            score.text = "Coins: " + scoreValue.ToString();
            Destroy(collision.collider.gameObject);
        }
        else if(collision.collider.tag == "Enemy")
        {
            livesValue -= 1;
            SetLivesText();
            lives.text = "Lives: " + livesValue.ToString();
            Destroy(collision.collider.gameObject);
        }
        if (scoreValue == 4)
        {
            transform.position = new Vector2(43f, 0.0f);
            livesValue = 3;
            SetLivesText();
        }
        if (livesValue == 0)
        {
            Destroy(gameObject);
        }
    }

    //OnCollisionStay2D will keep touching over and over again
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && isOnGround)
        {
            if (Input.GetKey(KeyCode.W))
            {
                //enacts an upward force and delays the input of gravity
                rd2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }

    void Flip()
    {
        facingRight =!facingRight;
        Vector2 Scaler = transform.localScale;
        //flips the sprite
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }
}
