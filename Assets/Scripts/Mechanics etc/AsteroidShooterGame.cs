
using UnityEngine;

public class AsteroidShooterGame : MonoBehaviour {

    [SerializeField] private GameAsteroidUI gameUI;

    [SerializeField] private Transform asteroidPrefab;
    
    [SerializeField] private float spawnTimerMax = 1f;
    [SerializeField] private float changeDirectionTimer = 1f;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject asteroidShooter;

    [SerializeField] private Transform movingPoint1;
    [SerializeField] private Transform movingPoint2;

    [SerializeField] private GameObject teleportToNextLevel;

    private bool IsGameStarted = false;

    private float spawnTimer = 0f;

    private float angle = 0f;
    private float shootingForce;
    private float randomAngle;



    private void Start()
    {
        MiniGameManager.Instance.OnGameStarted += MiniGameManager_OnAsteroidGameStarted;
        gameUI.OnAsteroidGameFinished += GameUI_OnAsteroidGameFinished;

        teleportToNextLevel.SetActive(false);
      
    }
    private void OnDestroy()
    {
        gameUI.OnAsteroidGameFinished -= GameUI_OnAsteroidGameFinished;
        MiniGameManager.Instance.OnGameStarted -= MiniGameManager_OnAsteroidGameStarted;
    }

    private void GameUI_OnAsteroidGameFinished(object sender, System.EventArgs e)
    {
        ShowTeleport();
        gameObject.SetActive(false);
    }

    private void MiniGameManager_OnAsteroidGameStarted(object sender, System.EventArgs e)
    {
        IsGameStarted = true;
        MoveShooter();
    }

    private void Update()
    {
        RunAsteroidGame();
    }

    private void RunAsteroidGame()
    {
        if (IsGameStarted)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnTimerMax)
            {
                ShootAsteroid();
                spawnTimer = 0f;
            }
        }
    }

    private void ShootAsteroid()
    {
        angle = shootingPoint.GetComponent<Transform>().rotation.eulerAngles.z;

        angle = RandomAngle();

        Transform asteroid = Instantiate(asteroidPrefab, shootingPoint.GetComponent<Transform>().position, shootingPoint.GetComponent<Transform>().rotation);
        float radAngle = angle * Mathf.Deg2Rad;
        float x1 = Mathf.Cos(radAngle);
        float y1 = Mathf.Sin(radAngle);

        asteroid.GetComponent<Rigidbody2D>().AddForce(new Vector2(x1, y1) * RandomShootingForce());
    }

    private float RandomAngle()
    {
        randomAngle = Random.Range(shootingPoint.rotation.eulerAngles.z - 20f, shootingPoint.rotation.eulerAngles.z + 20f);
        return randomAngle;      
    }

    private float RandomShootingForce()
    {
        shootingForce = Random.Range(400f, 1500f);
        return shootingForce;
    }


    private void MoveShooter()
    {
        LeanTween.moveY(asteroidShooter, -5, changeDirectionTimer).setEaseLinear().setLoopPingPong();
    }

    private void ShowTeleport()
    {
        teleportToNextLevel.gameObject.SetActive(true);
    }

}
