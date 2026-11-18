using UnityEngine;

public class Personaje : MonoBehaviour
{

    public Animator ani;
    public int combo;
    public bool atacando;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void Start_Combo()
    {
        atacando = false;
        if(combo < 3)
        {
            combo++;
        }
    }

    public void Finsih_Ani()
    {
        atacando = false;
        combo = 0;
    }

    public void Combos_()
    {
        if (Input.GetKeyDown(KeyCode.A) && !atacando)
        {
            atacando = true;
            ani.SetTrigger(""+combo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Combos_();
    }

    public void makeDamage()
    {

    }
}
