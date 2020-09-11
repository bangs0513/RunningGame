using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Range(0,5)]
    public float WalkSpeed = 2f;

    [Range(0,5)]
    public float BigJumpspeed = 5f;

    [Range(0, 5)]
    public float SmallJumpspeed = 2.5f;

    [Header("점프횟수")]
    public int Jumpcount = 1;

    //player 가 땅에 있는지 없는지 check
    public bool isGround = false;

    public GameObject player;

    public Rigidbody rb;

    public Animator animator;

    //무적시간 
    [Header("무적시간")]
    public float invincibilityTimeset = 0.2f;


    //jump check
    private  bool Jumpkey = false;

    //점프 강약 key check
    private float keyTime = 0f;

    //현재 player 상태 저장 
    public string playerst = playerstatus.GROUND.ToString();

    //무적 switching
    private bool invincibility = false;

    //player에 rendering 정보 저장 
    private Renderer playerRenderer;

    //초기에 color값 저장
    private Color preplayerColor;

    

    // Start is called before the first frame update
    void Start()
    {
        Jumpcount = 0;

        Transform test = transform.Find("Toon ChickMat");
        GameObject test1 = test.gameObject;
        playerRenderer = test.GetComponent<Renderer>();

        preplayerColor = playerRenderer.material.color;
    }   

    // Update is called once per frame
    void Update()
    {

        //position 이동 
        player.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * WalkSpeed;

        //camera 이동 
        GameManager.Instance.cameraManager.gameObject.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * WalkSpeed;


        if (Jumpkey)
        {
            keyTime += Time.deltaTime;
        }

        if (isGround)
        {
            if(Jumpcount > 0)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Jumpkey = true;
                    keyTime = 0;
       
                }
                if(Input.GetKeyUp(KeyCode.Space))
                {
                    Jumpkey = false;
                    if(keyTime > 0.3f)
                    {
                        BigJump();
                    }
                    else
                    {
                        smallJump();
                    }
                }
            }
        }
        
    }

    private void BigJump()
    {

        animator.SetTrigger("Jump");
        rb.AddForce(new Vector3(0, 1, 0) * BigJumpspeed, ForceMode.Impulse);
        Jumpcount--;
    }

    private void smallJump()
    {

        animator.SetTrigger("Jump");
        rb.AddForce(new Vector3(0, 1, 0) * SmallJumpspeed, ForceMode.Impulse);
        Jumpcount--;
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
            Jumpcount = 2;
            animator.SetBool("Walk", true);
        }


       if(collision.gameObject.tag == "obstacle")
        {
            invincibility = true;
            StartCoroutine(invincibilityTime());
        }
    }



    private enum playerstatus
    {
        GROUND,
        JUMP,
        Coll,
        DASH,

    }


    //피격당했을시에 무적시간  
    IEnumerator invincibilityTime()
    {
        int count = 0;

        while(count < 10)
        {
            if(count%2 == 0)
            {
                playerRenderer.material.color = new Color32(255, 255, 255, 90);
            }
            else
            {
                playerRenderer.material.color = new Color32(255, 255, 255, 180);
            }

            yield return new WaitForSeconds(invincibilityTimeset);

            count++;
        }
        playerRenderer.material.color = preplayerColor;

        invincibility = false;

        yield return null;
    }




}
