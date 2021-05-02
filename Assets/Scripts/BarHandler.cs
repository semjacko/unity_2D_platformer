using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarHandler : MonoBehaviour
{

    private Transform healthBar;
    private Transform defenseBar;
    private bool loading;
    private float loadingStarted;
    private float loadingTime;


    void Awake()
    {
        this.healthBar = this.gameObject.transform.Find("HealthBar").transform.Find("HealthStatus");
        this.defenseBar = this.gameObject.transform.Find("DefenseBar").transform.Find("DefenseStatus");
        this.loading = false;
    }


    void Update()
    {
        if (this.loading)
        {
            float size = (Time.time - this.loadingStarted) / loadingTime;
            this.loading = !(size >= 1);
            this.SetDefense(size);
        }

    }


    public void SetHealth(float currentHealth, float maxHealth)
    {
        float size = currentHealth / maxHealth;
        this.healthBar.localScale = new Vector2(size, 1f);
    }


    public void SetDefense(float currentDefense, float maxDefense)
    {
        float size = currentDefense / maxDefense;
        this.defenseBar.localScale = new Vector2(size, 1f);
    }


    public void SetDefense(float size)
    {
        if (size > 1)
            size = 1;
        if (size < 0)
            size = 0;

        this.defenseBar.localScale = new Vector2(size, 1f);
    }


    public void loadDefense(float time)
    {
        this.loadingStarted = Time.time;
        this.loadingTime = time;
        this.loading = true;
    }
}
