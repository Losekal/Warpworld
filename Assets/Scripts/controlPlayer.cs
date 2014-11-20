using UnityEngine;
using System.Collections;
using System;

public class controlPlayer : SingletonGeneric<controlPlayer>
{

    private Vector2 inputDir; // direction ZQSD et fleches données par l'utilisateur
    private Vector2 directionVisee; //vecteur unitaire de direction de visée
    public bool isOnGround; // booleen definissant si le personnage est sur le terrain ou non.
    private float distanceToGround; //la distance entre le centre de masse du personnage et le sol

    private bool isRunning; //Si le personnage court

    public float _maxSpeedXonGroundForWalking; //Vitesse max du personnage sur le terrain MARCHE
    public float _accelerationXonGroundForWalking; //Acceleration du personnage pendant la phase de prise de vitesse MARCHE
    public float _brakeForceXonWalking; //Force de freinage quand changement de direction et relachement des touches directionnelles MARCHE

    public float _maxSpeedXonGroundForRunning; //Vitesse max du personnage sur le terrain COURT
    public float _accelerationXonGroundForRunning; //Acceleration du personnage pendant la phase de prise de vitesse COURT
    public float _brakeForceXonRunning; //Force de freinage quand changement de direction et relachement des touches directionnelles COURT

    public bool jumpToNextFixedUpdate;
    public float jumpForce;//Force du saut
    public float percentageOnXJumping;//facteur qui reduit la composante du vecteur vitesse suivant x au moment d'un saut
    public float _brakeForceXonAir;//resistance de l'air en l'air 
    public float jumpControl; //capacite à controler sa vitesse selon X pendant un saut
 
    

	// Use this for initialization
    void Start() {
        directionVisee = Vector2.right;
        inputDir = new Vector2(0.0f, 0.0f);
        setDistanceBetweenCharAndGround();
        jumpToNextFixedUpdate = false;
        this.InfoPlayer += ChangePlatformes.Instance.rotatePlateFormsIfPlayerJumping; 
    }

	
	// Update is called once per frame
	void Update () {
        inputDir = getInputDir();
        if (Input.GetKeyDown("space"))
        {
            jumpToNextFixedUpdate=true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

	}


    void FixedUpdate()
    {
        checkIsGrounded();
        if (isOnGround && inputDir.x != 0.0f)
        {
            
            if (!isRunning)
            {
                 Debug.Log("lol0");
                moveCharOnXAxis(inputDir.x, _maxSpeedXonGroundForWalking, _accelerationXonGroundForWalking);
                
            }
            else
            {
                moveCharOnXAxis(inputDir.x, _maxSpeedXonGroundForRunning, _accelerationXonGroundForRunning);
            }
            if (jumpToNextFixedUpdate)
            {
                jump();
                this.onEventPlayerJumping(new InfoFromPlayerToPlateformManagerArgs()); //Previent le script de changement de plateformes du saut!
                jumpToNextFixedUpdate = false;
            }
        }
        else
        {
            controlTheJumpOnAir(inputDir.x);
            jumpToNextFixedUpdate = false;
        }

    }


    /////Fonctions utilisées pour le personnage jouable////
    Vector2 getInputDir()
    {
        if (Input.GetAxis("Horizontal")!=0.0f)
        {
            directionVisee = Vector2.right * Input.GetAxis("Horizontal");
        }
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }  //recupere les inputs des touches directionnelles

    void moveCharOnXAxis(float inputX,float speedMax,float acceleration)
    {
        //Si le personnage est à l'arret, on lui octroie une certaine accel.
        if (Mathf.Abs(rigidbody2D.velocity.x) < speedMax && inputX != 0.0f)
        {
            rigidbody2D.AddForce(directionVisee * acceleration);
            Debug.Log("bigup1");
        }
        //Si le personnage a atteint sa vitesse max, on la bride à v = vmax
        if (Mathf.Abs(rigidbody2D.velocity.x) >= speedMax && inputX != 0.0f)
        {
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * speedMax, rigidbody2D.velocity.y);
            Debug.Log("bigup2");
        }
 
    } //deplace le personnage lorsqu'il est sur une plateforme

    void controlTheJumpOnAir(float inputX)
    {
            rigidbody2D.AddForce(inputX * jumpControl * Vector2.right);
    } //permet de controler légerement la composante x du vecteur position du personnage lorsque le personnage saute

    void jump()
    {
        rigidbody2D.velocity = new Vector2(percentageOnXJumping * rigidbody2D.velocity.x, rigidbody2D.velocity.y) ;
        rigidbody2D.AddForce(new Vector2(0.0f, jumpForce));
    } //effectue un saut seulement si isOnGround

    public delegate void InfoFromPlayerToPlateformManagerHandler(object sender, EventArgs e);
    public event InfoFromPlayerToPlateformManagerHandler InfoPlayer; //Permet de récupérer l'évenement : le joueur à sauté ! au script qui effectue le cycle des plates formes
    public void onEventPlayerJumping(InfoFromPlayerToPlateformManagerArgs info)
    {
        if (InfoPlayer != null)
        {
            InfoPlayer(this, info);
        }
    }


    void checkIsGrounded()
    {
        collider2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        collider2D.enabled = true;
        if (hit.collider != null)
        {
            if (Mathf.Abs(distanceToGround - Mathf.Abs(hit.point.y - transform.position.y)) < 0.01f)
            {
                isOnGround = true;
                rigidbody2D.drag = _brakeForceXonWalking;
            }
            else
            {
                isOnGround = false;
                rigidbody2D.drag = _brakeForceXonAir;
            }
        }
    } //Verifie si le personnage est sur une plateforme ou non

    void setDistanceBetweenCharAndGround()
    {
        collider2D.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        collider2D.enabled = true;
        if (hit.collider != null)
        {
            distanceToGround = Mathf.Abs(hit.point.y - transform.position.y);
        }
    }//Calibre la distance entre le centre de masse du personnage et le sol
    
}

public class InfoFromPlayerToPlateformManagerArgs : EventArgs
{
}
