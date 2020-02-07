﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
    public int wallDamge = 1;
    public int pointPerFood = 10;
    public int pointPerSoda = 20;
    public float restartLevelDelay = 1f;
    private Animator animator;
    private int food;
    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        base.Start();        
    }
    private void OnDisable() {
        GameManager.instance.playerFoodPoints = food;
    }
    private void CheckIfGameOver()
    {
        if (food <= 0)
            GameManager.instance.GameOver();
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food --;
        base.AttemptMove <T>(xDir, yDir);
        RaycastHit2D hit2D;
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;
        horizontal =(int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");
        if (horizontal != 0)
            vertical = 0;
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }
    protected override void onCantMove<T>(T component){
        Wall hitWall = component as Wall;
        hitWall.DamgeWall(wallDamge);
        animator.SetTrigger("playerChop");
    }
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
        }
        else if (other.tag == "Food")
        {
            food += pointPerFood;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointPerSoda;
            other.gameObject.SetActive(false);
        }
    }
}
