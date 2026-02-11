using UnityEngine;

public class RandomCameraMovement : MonoBehaviour
{
    [SerializeField]
    GameObject dancer;

    private float timeElapsed = 0f;
    private Vector3 direction;
    private float speed;

    void Start()
    {
        updateRandomMovement();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed > 4f)
        {
            this.gameObject.transform.position = dancer.gameObject.transform.position + randomCoordinateNormalised() * Random.Range(2f, 5f);
            updateRandomMovement();
            this.transform.LookAt(dancer.transform);
            timeElapsed = 0f;
        }

        this.gameObject.transform.position += direction * speed;
    }

    void updateRandomMovement()
    {

        speed = Random.Range(0f, 0.01f);
        direction = randomCoordinateNormalised();
    }

    Vector3 randomCoordinateNormalised()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
