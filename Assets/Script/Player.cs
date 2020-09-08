using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Range(0,5)]
    public float WalkSpeed = 2f;

    [Range(0,5)]
    public float Jumpspeed = 5f;

    public int Jumpcount = 1;


    public bool isGround = false;

    public GameObject player;

    public Rigidbody rb;

    public Animator animator;


    public float playerHP;

    // Start is called before the first frame update
    void Start()
    {
        Jumpcount = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //position 이동 
        player.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * WalkSpeed;

        //camera 이동 
        GameManager.Instance.cameraManager.gameObject.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * WalkSpeed;


        if(isGround)
        {
            if(Jumpcount > 0)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    animator.SetTrigger("Jump");
                    rb.AddForce(new Vector3(0, 1, 0) * Jumpspeed, ForceMode.Impulse);
                    Jumpcount--;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ground")
        {
            isGround = true;
            Jumpcount = 2;
            animator.SetBool("Walk", true);
        }
    }


}
