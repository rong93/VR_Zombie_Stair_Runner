using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ZombieCharacterControl : MonoBehaviour
{
    GameObject current_floor;

    [SerializeField] int HP;
    [SerializeField] GameObject HP_Bar;
    [SerializeField] Text ScoreText;
    int score;
    float score_time;

    AudioSource death_sound;
    [SerializeField] GameObject Replay_Button;

    private enum ControlMode
    {
        /// <summary>
        /// Up moves the character forward, left and right turn the character gradually and down moves the character backwards
        /// </summary>
        Tank,
        /// <summary>
        /// Character freely moves in the chosen direction from the perspective of the camera
        /// </summary>
        Direct
    }

    [SerializeField] private float m_moveSpeed = 7;
    [SerializeField] private float m_turnSpeed = 200;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Tank;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10000;

    private Vector3 m_currentDirection = Vector3.zero;

    private Vector3 now_character_position;
    private Vector3 past_character_position;


    void Start()
    {
        HP = 10;
        score = 0;
        score_time = 0f;
        death_sound = GetComponent<AudioSource>();
    }


    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }
    }

    private void FixedUpdate()
    {
        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }
    }

    private void TankUpdate()
    {
        GameObject wallObject = GameObject.FindWithTag("Wall_front");
        GameObject character = GameObject.FindWithTag("Character");
        Transform camera = Camera.main.transform;
        RaycastHit hit;
        Ray ray = new Ray(camera.position, camera.rotation * Vector3.forward);
        
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject == wallObject) //如果指標在範圍內(wall_front)
            {
                past_character_position = now_character_position; //紀錄腳色上一點位置
                now_character_position = character.transform.position; //紀錄目前位置
                // now_character_position = hit.point; //紀錄目前位置

                float step = m_moveSpeed * Time.deltaTime;
                Vector3 target_point = new Vector3(hit.point.x, 0, 0);

                if(Mathf.Abs(hit.point.x - now_character_position.x) < 0.2) //如果腳色沒有動，就留在原地
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,180,0), 0.5f); //轉向正面
                    m_currentV = Mathf.Lerp(m_currentV, 0, Time.deltaTime * m_interpolation); //達成滑順的轉化效果
                }
                else if(now_character_position.x < target_point.x) //要向右轉
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,90,0), 1f); //轉向右邊
                    m_currentV = Mathf.Lerp(m_currentV, 3, Time.deltaTime * m_interpolation); //達成滑順的轉化效果
                }
                else if(now_character_position.x > target_point.x) //要向左轉
                { 
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,-90,0), 1f); //轉向左邊
                    m_currentV = Mathf.Lerp(m_currentV, -3, Time.deltaTime * m_interpolation); //達成滑順的轉化效果
                }

                // transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime; //移動位置
                // transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0); //向左向右轉? 好像也可以不用了

                


                transform.position = Vector3.MoveTowards(transform.position, target_point, step);
                    //m_animator.SetFloat("MoveSpeed", m_currentV); //移動的動畫
            }
            
        }
        else //如果指標不在範圍內(wall_front)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,180,0), 0.5f); //轉向正面
            m_currentV = Mathf.Lerp(m_currentV, 0, Time.deltaTime * m_interpolation); //達成滑順的轉化效果

            // m_animator.SetFloat("MoveSpeed", m_currentV); //移動的動畫
        }

    Update_Score();

        // 使用A、D鍵控制--start
        // float v = Input.GetAxis("Vertical");
        // float h = Input.GetAxis("Horizontal");

        // if(v == 0 || h ==0){
        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,180,0), 0.5f);
        //     m_currentV = Mathf.Lerp(m_currentV, h, Time.deltaTime * m_interpolation); //達成滑順的轉化效果

        // }
        // if(Input.GetKey(KeyCode.D) || v < 0)
        // {
        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,90,0), 1f);
        //     m_currentV = Mathf.Lerp(m_currentV, h, Time.deltaTime * m_interpolation); //達成滑順的轉化效果

        // }
        // else if(Input.GetKey(KeyCode.A) || v > 0)
        // {
        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,-90,0), 1f);
        //     m_currentV = Mathf.Lerp(m_currentV, -h, Time.deltaTime * m_interpolation); //達成滑順的轉化效果

        // }

    
        // transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime; //移動位置
        // transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0); //向左向右轉? 好像也可以不用了

        // m_animator.SetFloat("MoveSpeed", m_currentV); //移動的動畫

        // 使用A、D鍵控制--end
       
    }

    void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Normal_floor")
        {
            if(other.contacts[0].normal == new Vector3(0f, 1f))
            {
                Debug.Log("Normal_floor");
                current_floor = other.gameObject;
                ModifyHP(1);
                other.gameObject.GetComponent<AudioSource>().Play();
            }        
        }
        else if(other.gameObject.tag == "Trap_floor")
        {
            if(other.contacts[0].normal == new Vector3(0f, 1f))
            {
                Debug.Log("Trap_floor");
                current_floor = other.gameObject;
                ModifyHP(-1);
                other.gameObject.GetComponent<AudioSource>().Play();
            }        

        }
        else if(other.gameObject.tag == "Top")
        {
            Debug.Log("撞到天花板");
            current_floor.GetComponent<BoxCollider>().enabled = false;
            ModifyHP(-1);
            other.gameObject.GetComponent<AudioSource>().Play();
        }
        
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "End_Line")
        {
            Debug.Log("你輸了");
            Die();
        }
    }


    void ModifyHP(int num)
    {
        HP += num;
        if(HP > 10)
        {
            HP = 10;
        }
        else if(HP <= 0)
        {
            HP = 0;
            Die();
        }
        Update_HP_Bar();
    }

    void Update_HP_Bar()
    {
        for(int i=0; i<HP_Bar.transform.childCount; i++)
        {
            if(HP>i)
            {
                HP_Bar.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                HP_Bar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void Update_Score()
    {
        score_time += Time.deltaTime;
        if(score_time >2f)
        {
            score++;
            score_time = 0f;
            ScoreText.text = "地下" + score.ToString() + "層";
        }
    }

    void Die()
    {
        death_sound.Play();
        Time.timeScale = 0;
        Replay_Button.SetActive(true); 
    }

    public void Relpay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scene_1");
    }









    private void DirectUpdate()
    {
        // float h = Input.GetAxis("Mouse X");
        // float v = Input.GetAxis("Mouse Y");
        //transform.Translate(h* move_speed * Time.deltaTime, v* move_speed * Time.deltaTime, 0);
        

        
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }
}
