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
    public bool _isGround= true;
    public GameObject endingmenu;
    private float Horizontal;
    private float Vertical;
    private float NextAttackTime;
    public AudioSource walking;
    public ParticleSystem particlewalk;
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachineTransposer _transposer;
    public Text candytext;
    public Text Elxyrtext;
    public float TurnSpeed;
    Vector3 movement;
    SplineFollower _splineFollower;
    Rigidbody rb;
    Animator anim;
    Vector3 distance;
    public int Elxyr = 0;
    Vector3 direction;

    
    // Getting Components
    void Start()
    {
        particlewalk = GetComponentInChildren<ParticleSystem>();
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
         if (Input.GetKeyDown(KeyCode.Q) && Time.time > NextAttackTime && Elxyr >= 1)
        {
            Instantiate(GranadePrefab, FireTransform.position, FireTransform.rotation);
            NextAttackTime = Time.time + AttackSecond / AttackRate;
            Elxyr --;
            Elxyrtext.text = "Elxyr :" + Elxyr.ToString();
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

         if (movement.magnitude >= 0.01f && _isGround == true)
         {
            walking.enabled = true;
            particlewalk.Play(true);
         }
         else if (movement.magnitude <= 0f && _isGround == true)
         {
            walking.enabled = false;
         }

    }
    void FixedUpdate()
    {
        MovePlayer();
        TurnPlayer();
    }
        public void MovePlayer()
        {
          movement = transform.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime;
          rb.MovePosition(rb.position + movement); 
        }
        public void TurnPlayer()
         {
            float turn = Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f,turn,0f);
            rb.MoveRotation(rb.rotation * turnRotation);
         }


         
             // Finish Line
         public void OnTriggerStay(Collider other)
         {
            if (other.transform.CompareTag("Finish"))
            {
                //Rotation on Finish
                //  transform.DOLocalRotate(new Vector3(0,-180,0),0.3f);    
                // _transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
                // _transposer.m_FollowOffset = new Vector3(10f,7f,1.5f);
                //anim.SetTrigger("Win");
                Speed = 0f;
                TurnSpeed = 0f;
                walking.enabled = false;
                particlewalk.Stop(true);
                endingmenu.SetActive(true);
        }
         }
        // Checking enemy
    public void OnTriggerEnter(Collider other) 
    {
        if (other.transform.CompareTag("Enemy"))
        {
            //anim.SetTrigger("Death");
            Speed = 0f;
            TurnSpeed = 0f;
            RestartLevel.SetActive(true);  
            _IsDead = true;
            walking.enabled = false;
            particlewalk.Stop(true);
            rb.isKinematic = true;
        }
        // Collecting Candys
        if (other.transform.CompareTag("Candy"))
        {
            Debug.Log("Collected Candy");
            Destroy(other.gameObject);
            CandyCount++;
            candytext.text = "It's Your Candy :" + CandyCount.ToString();
        } 
        if (other.transform.CompareTag("Elxyr"))
        {
            Debug.Log("Collected Elxyr");
            Destroy(other.gameObject);
            Elxyr++;
            Elxyrtext.text = "Elxyr :" + Elxyr.ToString();
        }        
    }
        // Checking ground
     private void OnCollisionEnter(Collision other) 
     {
         if(other.transform.CompareTag("Ground"))
        {
           // anim.SetBool("GroundT",true);
            _isGround=true;   
        }

        // Grenade Type Enemys
        if (other.transform.CompareTag("Enemy"))
        {
            //anim.SetTrigger("Death");
            Speed = 0f;
            TurnSpeed = 0f;
            RestartLevel.SetActive(true);  
            _IsDead = true;
            walking.enabled = false;
            particlewalk.Stop(true);
        }
    }
        // Restarting
    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
}
