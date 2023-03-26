using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcScript : MonoBehaviour
{
    public GameObject PC;
    public Sprite OFF;
    public SpriteRenderer ON;

    [SerializeField] private AudioSource powerOFF;

    bool collide = false;

    private void Start()
    {
        ON = gameObject.GetComponent<SpriteRenderer>();
    }

    // Enable trigger so it could be used in the update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collide = true;
        }
    }

    // Swap sprites when a player presses E
    private void Update()
    {
        if(collide)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                powerOFF.Play();
                SwapSprite();
            }
        }
    }

    // Make trigger false after player leaves trigger box
    private void OnTriggerExit2D(Collider2D collision)
    {
        collide = false;
    }

    // Swap computer sprites
    void SwapSprite()
    {
        ON.sprite = OFF;
    }
}