using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] float JumpForce;
    [SerializeField] Transform FireTransform;
    [SerializeField] GameObject GranadePrefab;
    [SerializeField]public float AttackRate;
    [SerializeField]public float AttackSecond;
    [SerializeField]public GameObject RestartLevel;
    [SerializeField] int CandyCount = 0;
    private bool _IsDead=false;
    private float SpeedLR = 3f;
    public bool _isGround= true;
    private float Horizontal;
    private float NextAttackTime;
    public AudioSource walking;
    public ParticleSystem particlewalk;
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachineTransposer _transposer;
    public Text candytext;
    SplineFollower _splineFollower;
    Rigidbody rb;
    Animator anim;
    Vector3 distance;

    
    // Getting Components
    void Start()
    {
        walking = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        _splineFollower = GetComponentInParent<SplineFollower>();
        _cinemachineCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        _transposer = _cinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
    }
    // Moving and Throwing Grenade
    void Update()
    {
        //Throw grenade
         if (Input.GetKeyDown(KeyCode.Q) && Time.time > NextAttackTime)
        {
            Instantiate(GranadePrefab, FireTransform.position, FireTransform.rotation);
            NextAttackTime = Time.time + AttackSecond / AttackRate;
        }
        //Moving Jumping
        if (Input.GetKeyDown(KeyCode.Space) && _isGround && _IsDead == false)
        {
           // anim.SetTrigger("JumpT");
            rb.AddForce(Vector3.up * JumpForce,ForceMode.Impulse);
            _isGround=false;
            walking.enabled = false;
            particlewalk.Stop(true);
        }
         _splineFollower.followSpeed = Speed;
        Horizontal = Input.GetAxis("Horizontal");         
        transform.localPosition -= new Vector3(Horizontal, 0, 0) *Time.deltaTime * SpeedLR;
        transform.localPosition = new Vector3((Mathf.Clamp(transform.localPosition.x, -3.5f, 3.5f)), transform.localPosition.y, transform.localPosition.z);
    }
             // Finish Line
         public void OnTriggerStay(Collider other)
         {
            if (other.transform.CompareTag("Finish"))
            {
                //Rotation on Finish
                 transform.DOLocalRotate(new Vector3(0,-180,0),0.3f);    
                _transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
                _transposer.m_FollowOffset = new Vector3(10f,7f,1.5f);
                //anim.SetTrigger("Win");
                Speed = 0f;
                SpeedLR = 0f;
                walking.enabled = false;
                particlewalk.Stop(true);
                RestartLevel.SetActive(true);
        }
         }
        // Checking enemy
    public void OnTriggerEnter(Collider other) 
    {
        if (other.transform.CompareTag("Enemy"))
        {
            //anim.SetTrigger("Death");
            Speed = 0f;
            SpeedLR = 0f;
            RestartLevel.SetActive(true);  
            _IsDead = true;
            walking.enabled = false;
            particlewalk.Stop(true);
        }
        // Collecting Candys
        if (other.transform.CompareTag("Candy"))
        {
            Debug.Log("Collected Candy");
            Destroy(other.gameObject);
            CandyCount++;
            candytext.text = "It's Your Candy :" + CandyCount.ToString();
        }         
    }
        // Checking ground
     private void OnCollisionEnter(Collision other) 
     {
         if(other.transform.CompareTag("Ground"))
        {
           // anim.SetBool("GroundT",true);
            _isGround=true;
            walking.enabled = true;
            particlewalk.Play(true);
        }

        // Grenade Type Enemys
        if (other.transform.CompareTag("Enemy"))
        {
            //anim.SetTrigger("Death");
            Speed = 0f;
            RestartLevel.SetActive(true);  
            _IsDead = true;
            walking.enabled = false;
            particlewalk.Stop(true);
        }
    }
        // Restarting
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
