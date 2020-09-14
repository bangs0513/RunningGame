using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [Range(0, 15)]
    public float WalkSpeed = 2f;

    [Range(0, 10)]
    public float BigJumpspeed = 5f; //

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
    private bool Jumpkey = false;

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
            if (Jumpcount > 0)
            {
                if (status == playerstatus.DASH) return;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jumpkey = true;
                    keyTime = 0;

                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Jumpkey = false;
                    if (keyTime > 0.3f)
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

        updateMissonResultPos();
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
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
            Jumpcount = 2;
            animator.SetBool("Walk", true);
        }


        if (collision.gameObject.tag == "obstacle")
        {
            invincibility = true;
            StartCoroutine(invincibilityTime());
        }
    }

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
    playerstatus status;

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
                if (status == playerstatus.DASH) return;
                WalkSpeed++;
                WalkSpeed = Mathf.Clamp(WalkSpeed, 1, 5);
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
            if (status == playerstatus.INVINCIBILITY || status == playerstatus.DASH) return;

            // ui 카운트 감소
            float Damage = Mathf.Ceil((uiController.mailCount / damagePercnt)); // 퍼센트로 나눈 값의 소수점 올림 처리하여 감소
            WalkSpeed--;
            WalkSpeed = Mathf.Clamp(WalkSpeed, 1, 5);
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
            status = playerstatus.DASH;
            // 속도 증가
            StartCoroutine(feverTime());
            // 이펙트 활성화
            feverEffect.SetActive(true);
            // 아이템 삭제
            Destroy(other.gameObject);
        }

        // 임시 사망처리
        if (other.CompareTag("Finish"))
        {
            SceneManager.LoadScene("Bang"); // 리로드
        }
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
            if (Input.GetKeyDown(KeyCode.LeftShift) || status == playerstatus.DASH)
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
        yield return new WaitForSeconds(1f);
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
        status = playerstatus.GROUND; // 상태 정의 픽스되면 변경되어야 함
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
        UIPos = new Vector3(UIPos.x+0.2f, UIPos.y += 1.2f, UIPos.z);

        missonResultPos.position = UIPos;
    }

    private void updateSpeed(float val)
    {

    }

    private enum playerstatus
    {
        GROUND,
        JUMP,
        Coll,
        DASH,
        INVINCIBILITY // 무적
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
