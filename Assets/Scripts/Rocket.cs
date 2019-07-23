using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

   

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    //enum State { Alive, Dying, Transcending };
    //State state = State.Alive;                // overengineering

    bool isTransitioning = false;

    //bool collisionsDisabled = false;


	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
    
        if (/*state == State.Alive*/ !isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
	}

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        /*else if (Input.GetKeyDown(KeyCode.C))
        {
            //toggle collision
            collisionsDisabled = !collisionsDisabled;
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("Collided");

        if (/*state != State.Alive*/isTransitioning /*|| !collisionsDisabled*/) { return; } //ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":

                print("OK");

                break;
            case "Finish":

                print("Finish");
                SuccessSequence();

                break;
            default:

                print("Dead");
                DeathSequence();

                break;
        }
    }

    private void SuccessSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(success);

        successParticles.Play();

        //state = State.Transcending;
        isTransitioning = true;
        Invoke("LoadNextLevel", 3f); //'f' stands for seconds
    }

    private void DeathSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(death);

        deathParticles.Play();

        //state = State.Dying;
        isTransitioning = true;
        Invoke("LoadNextLevel", 2f);
        
    }


    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        print("currentSceneIndex");
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; //loop back to start from the last scene to the first
        }
        //SceneManager.LoadScene(nextSceneIndex);
        SceneManager.LoadScene(0); // Could be like that, but we want to disable control system
    }

    /*private void ReloadFirstLevel()
    {
        //SceneManager.LoadScene(0);
        SceneManager.GetActiveScene();

    }*/

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //print("Thrusting");    //can trust while rotating
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        //rigidBody.freezeRotation = true; // take manual control of rotation
        rigidBody.angularVelocity = Vector3.zero; //better way to remove rotation
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            //print("Rotating left");

            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //print("Rotating right");

            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        //rigidBody.freezeRotation = false; // resume rotation
    }

    
}
