// ˆÓ}Fˆê’è‹——£‚Å’âŽ~‚µA‚»‚ÌŒã‰Á‘¬‚·‚é“G

using UnityEngine;

public class EnemyAccele : EnemyBase
{
    private Vector3 startPos;
    private bool stopped = false;
    private float timer = 0f;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    void Update()
    {
        if (!stopped)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 2f);

            if (Vector3.Distance(startPos, transform.position) >= 2f)
            {
                stopped = true;
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= 1f)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * 4f);
            }
        }
    }
}