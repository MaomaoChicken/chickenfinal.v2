using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeElapsed = 0f;
    private bool isRunning = true;
    private int minutes = 10;

    public GameTimer(int minutes)
    {
        this.minutes = minutes;
    }

    void Update()
    {
        if (!isRunning) return;

        timeElapsed += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void Start() => timerText.text = "08:00";
    public void StopTimer() => isRunning = false;
    public void ResetTimer() => timeElapsed = 0f;
}
