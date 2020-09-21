using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // 플레이어 상태
    public enum PlayerStatus
    {
        GROUND,
        JUMP,
        Coll,
        DASH,
        INVINCIBILITY, // 무적
        DEAD, // 사망
        CLEAR
    }

    private GameObject chickmat;

    [Range(0, 50)]
    public float WalkSpeed = 10f;
    // clamp 조절용
    private int minSpeed = 4;
    private int maxSpeed = 9;

    //jump 1단 
    [Range(0, 50)]
    public float OneJumpSpeed = 10f; //


    [Range(0, 30)]
    public float TwoJumpSpeed = 10f; //

    [Header("점프횟수")]
    public int Jumpcount = 1;

    private int startJumpcount = 0;

    //player 가 땅에 있는지 없는지 check
    public bool isGround = false;

    public GameObject player;

    public Rigidbody rb;

    public Animator animator;

    //무적시간 
    [Header("무적시간")]
    public float invincibilityTimeset = 0.2f;

    [SerializeField]
    private UiController uiController;

    private bool missonCheck = false;
    [SerializeField, Tooltip("상호작용 UI의 이동에 관련된 트랜스폼")]
    private RectTransform missonResultPos;

    [SerializeField, Header("데미지 나눌 퍼센트")]
    private float damagePercnt = 2f;

    [SerializeField, Header("피버타임 시간")]
    private float fever = 2f;

    [SerializeField, Header("피버이펙트")]
    private GameObject feverEffect;

    [SerializeField, Header("점프이펙트")]
    private GameObject JumpEffect;

    //현재 player 상태 저장 
    public PlayerStatus playerStatus = PlayerStatus.GROUND;

    ////현재 player 상태 저장 
    //public string playerst = playerstatus.GROUND.ToString();

    //jump check
    private bool Jumpkey = false;

    //점프 강약 key check
    private float keyTime = 0f;

    //무적 switching
    private bool invincibility = false;

    //player에 rendering 정보 저장 
    private Renderer playerRenderer;

    //초기에 color값 저장
    private Color preplayerColor;

    private RaycastHit hit;

    //점프가 최고점에서 떨어질때를 체크 
    private bool checkJumpHigh = false;


    //점프 ray 사거리
    private float maxDistance = 0.2f;

    private bool isjump = false;


    void Start()
    {
        //chickmat = transform.Find("Toon ChickMat").gameObject;

        JumpEffect.SetActive(false);

        startJumpcount = Jumpcount;

       // Transform test = transform.Find("Toon ChickMat");
        //GameObject test1 = test.gameObject;

        //playerRenderer = test.GetComponent<Renderer>();

        //preplayerColor = playerRenderer.material.color;
    }

    void Update()
    {

        //점프가 내려올때 체크 
        if (rb.velocity.y <= 0f && !checkJumpHigh)
        {
            checkJumpHigh = true;
        }
        //camera 이동 
        //   GameManager.Instance.cameraManager.gameObject.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * WalkSpeed;

        
      
        //jump를 위한 raycast 
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.4f))
        {

            if (checkJumpHigh)
            {
                checkJumpHigh = false;

                if (hit.transform.gameObject.tag == "Ground")
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    if(rb.velocity.y == 0)
                    {
                        animator.SetBool("Jump", false);
                    }

                    Jumpcount = startJumpcount;
                    playerStatus = PlayerStatus.GROUND;
                    JumpEffect.SetActive(false);
                }
            }

        }


        if (Input.GetKeyDown(KeyCode.Space) && !isjump && Jumpcount != 0)
        {
            isjump = true;
        }



            updateMissonResultPos();
    }

    // 물리 이동,점프 처리
    private void FixedUpdate()
    {
        if (playerStatus != PlayerStatus.CLEAR)
        {
            var rig = rb.velocity;
            rig.x = 50 * Time.deltaTime * WalkSpeed;
            rb.velocity = rig;
        }

            if (isjump)
            {
                if (playerStatus == PlayerStatus.DASH) return;

                Jump();

            }
        

    }

    private void Jump()
    {
        animator.SetBool("Jump", true);

        playerStatus = PlayerStatus.JUMP;
        if(Jumpcount == 2)
        {
            rb.AddForce(Vector3.up * OneJumpSpeed, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(Vector3.up * TwoJumpSpeed / 2, ForceMode.Impulse);
        }
        JumpEffect.gameObject.transform.position = gameObject.transform.position;
       
        JumpEffect.SetActive(true);
        isjump = false;
        Jumpcount--;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            invincibility = true;
            StartCoroutine(invincibilityTime());
        }

        // 도착지점
        if (collision.gameObject.CompareTag("End"))
        {
            print("끝");
            playerStatus = PlayerStatus.CLEAR;
            animator.StopPlayback();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 우편물
        if (other.CompareTag("Mail"))
        {
            print("우편물");
            // ui 카운트 증가(최대 소지개수 15)
            uiController.mailCount++;
            uiController.mailCount = Mathf.Clamp(uiController.mailCount, 0, 15);
            // 캐릭터 이동속도 증가(우편물이 5개 이상일 때 부터)
            if (uiController.mailCount >= 5)
            {
                // 피버상태에서 원래 스피드로 되돌아 오는 현상 방지
                if (playerStatus == PlayerStatus.DASH) return;
                WalkSpeed++;
                WalkSpeed = Mathf.Clamp(WalkSpeed, minSpeed, maxSpeed);
                //float culWalkSpeed = WalkSpeed++;
                //WalkSpeed = Mathf.Lerp(WalkSpeed, culWalkSpeed, Time.deltaTime * 2);
            }
            // 우편물 삭제
            Destroy(other.gameObject);
        }

        // 장애물
        if (other.CompareTag("obstacle"))
        {
            print("장애물");

            // 무적, 대쉬 상태일 경우 피격 판정 처리를 하지 않음
            if (playerStatus == PlayerStatus.INVINCIBILITY || playerStatus == PlayerStatus.DASH) return;

            // ui 카운트 감소
            float Damage = Mathf.Ceil((uiController.mailCount / damagePercnt)); // 퍼센트로 나눈 값의 소수점 올림 처리하여 감소
            WalkSpeed--;
            WalkSpeed = Mathf.Clamp(WalkSpeed, minSpeed, maxSpeed);
            uiController.mailCount -= Damage;
            uiController.mailCount = Mathf.Clamp(uiController.mailCount, 0, 15);
            // 나중에 죽는 거 처리
            if (uiController.mailCount <= 0)
            {
                SceneManager.LoadScene("Bang"); // 리로드
            }

            // 장애물 삭제
            Destroy(other.gameObject);

            invincibility = true;
            StartCoroutine(invincibilityTime());
        }

        // 아이템
        if (other.CompareTag("Item"))
        {
            // 상태 변경
            playerStatus = PlayerStatus.DASH;
            // 속도 증가
            StartCoroutine(feverTime());
            // 이펙트 활성화
            feverEffect.SetActive(true);
            // 아이템 삭제
            Destroy(other.gameObject);
        }

        //// 임시 사망처리
        //if (other.CompareTag("Finish"))
        //{
        //    SceneManager.LoadScene("Bang"); // 리로드
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        // 우편물 미션존
        if (other.CompareTag("Misson"))
        {
            // 타일 색 변경
            var tilecolor = other.gameObject.GetComponent<MeshRenderer>().material.color;
            //other.gameObject.GetComponent<MeshRenderer>().material.color = new Color(tilecolor.r - 0.01f, tilecolor.g - 0.01f, tilecolor.b, tilecolor.a);
            other.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(200, 200, 200, 0);
        
            print("미션존");
            // 우편물을 소지하고 있지 않으면
            if (uiController.mailCount <= 0) return;
            // 액션 키 입력
            if (Input.GetKeyDown(KeyCode.LeftShift) || playerStatus == PlayerStatus.DASH)
            {
                print("미션성공");
                missonCheck = true;
                // 우편물 카운트 감소
                uiController.mailCount--;
                uiController.mailCount = Mathf.Clamp(uiController.mailCount, 0, 15);
                if (uiController.mailCount >= 5 && WalkSpeed != 1)
                {
                    WalkSpeed--;
                }

                //미션 결과 활성화
                uiController.missionResultTx.text = "성공";
                uiController.missionResultTx.color = new Color32(0, 0, 255, 180);
                StartCoroutine(MissonResultAtive());

                // 미션 카운트 증가
                uiController.missionCount++;

                // 미션존 삭제
                Destroy(other.gameObject);
            }
        }
    }

    // 미션 결과 활성화
    IEnumerator MissonResultAtive()
    {
        missonResultPos.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        missonResultPos.gameObject.SetActive(false);
        missonCheck = false;
    }

    // 피버타임 활성화
    IEnumerator feverTime()
    {
        float culSpeed = WalkSpeed; // 피버 전 스피드

        WalkSpeed = 15f; // 최고 스피드
        // 중력 무시
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        yield return new WaitForSeconds(fever);

        WalkSpeed = culSpeed; // 원래 스피드로 돌아옴
                              // 상태 변경
        //playerStatus = PlayerStatus.GROUND; // 상태 정의 픽스되면 변경되어야 함
        GetComponent<Rigidbody>().useGravity = true; // 중력 활성화
        feverEffect.SetActive(false); // 이펙트 비활성화
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Misson"))
        {
            if (missonCheck) return;
            //미션 결과 활성화
            uiController.missionResultTx.text = "실패";
            uiController.missionResultTx.color = new Color32(255, 0, 0, 180);
            StartCoroutine(MissonResultAtive());
        }
    }

    // 미션 결과 ui 처리
    private void updateMissonResultPos()
    {
        //체력 UI가 할당되어 있지 않다면, 아래 코드 구문 실행 X
        if (missonResultPos == null) return;

        var UIPos = transform.position;
        UIPos = new Vector3(UIPos.x + 0.2f, UIPos.y += 1.2f, UIPos.z);

        missonResultPos.position = UIPos;
    }

    private void updateSpeed(float val)
    {

    }

    //피격당했을시에 무적시간  
    IEnumerator invincibilityTime()
    {
        int count = 0;

        while (count < 10)
        {
            if (count % 2 == 0)
            {
                
             //  playerRenderer.material.SetColor("col", Color.red);
            }
            else
            {
              //  playerRenderer.enabled = false;
            }

            yield return new WaitForSeconds(invincibilityTimeset);

            count++;
        }
        //playerRenderer.material.color = preplayerColor;

        //invincibility = false;

        yield return null;
    }


    void OnDrawGizmos()
    {

        float maxDistance = 0.3f;
        RaycastHit hit;
        // Physics.Raycast (레이저를 발사할 위치, 발사 방향, 충돌 결과, 최대 거리)
        bool isHit = Physics.Raycast(transform.position, -transform.up, out hit, maxDistance);

        Gizmos.color = Color.red;
     
            Gizmos.DrawRay(transform.position, -transform.up * hit.distance);
      
    }

}
