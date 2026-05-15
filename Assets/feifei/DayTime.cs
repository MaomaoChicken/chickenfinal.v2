using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayTime : MonoBehaviour
{
    public Light2D globalLight;
    public enum TimeState { Morning, Night }
    public TimeState currentState = TimeState.Morning;

    [Header("Time Settings")]
    public float timeOfDay = 0f; // ตัวแปรนี้ยังไม่ใช้ แต่สามารถใช้คำนวณเวลาของวันได้ในอนาคต
    public TimeState state = TimeState.Morning;
    public float time = 0f;
    public float dayDuration = 20f; // ระยะเวลาของวันทั้งหมด (เช้า + กลางคืน) วินาที
    public float nightDuration = 10f; // ระยะเวลาของกลางคืน วินาที
    public float morningDuration = 10f; // ระยะเวลาของเช้า วินาที
    public float noonDuration = 0f; // ระยะเวลาของเที่ยง วินาที
    public float eveningDuration = 0f; // ระยะเวลาของเย็น วินาที
    public float transitionDuration = 10f; // ระยะเวลาที่ใช้ในการเปลี่ยนสีจากเช้าไปกลางคืน
    
    [Header("Color Settings")]
    public Color morningColor = new Color(1f, 0.8f, 0.6f);
    public Color nightColor = new Color(0.15f, 0.15f, 0.35f);
    public Color noonColor = new Color(1f, 1f, 0.8f);   
    public Color eveningColor = new Color(1f, 0.5f, 0.3f);
    private float stateTimer;
    private bool timerIsRunning = true; // ตัวเปิด-ปิดการนับเวลา
    private Coroutine transitionCoroutine;

    public IEnumerator ChangeToMorning()
    {
        currentState = TimeState.Morning;
        timerIsRunning = true; // เริ่มนับเวลาใหม่สำหรับเช้า
        stateTimer = nightDuration;
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(LerpLightColor(GetTargetColor(currentState)));
        Debug.Log("เข้าสู่ช่วงเช้า...");
        yield return null;
    }
    void Start()
    {
        if (globalLight != null)
        {
            globalLight.color = GetTargetColor(currentState);
        }
        stateTimer = morningDuration;
    }

    void Update()
    {
        // ถ้านับเวลาอยู่ ให้หักเวลาออกเรื่อยๆ
        if (timerIsRunning)
        {
            stateTimer -= Time.deltaTime;

            if (stateTimer <= 0)
            {
                ChangeToNight(); // เรียกฟังก์ชันเปลี่ยนเป็นกลางคืน
            }
        }
    }

    void ChangeToNight()
    {
        currentState = TimeState.Night;
        timerIsRunning = false; // สั่งหยุดนับเวลา (ทำให้กลางคืนยาวไปตลอด)
        stateTimer = nightDuration;
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(LerpLightColor(GetTargetColor(currentState)));
        StartCoroutine(WaitThenMorning());
        Debug.Log("หมดเวลาเช้าแล้ว เข้าสู่ช่วงกลางคืนตลอดกาล...");
    }

    IEnumerator WaitThenMorning()
    {
        yield return new WaitForSeconds(nightDuration);
        ChangeToMorning();
    }

    IEnumerator LerpLightColor(Color targetColor)
    {
        float timeElapsed = 0;
        Color startColor = globalLight.color;

        while (timeElapsed < transitionDuration)
        {
            globalLight.color = Color.Lerp(startColor, targetColor, timeElapsed / transitionDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        globalLight.color = targetColor;
    }

    Color GetTargetColor(TimeState state)
    {
        return (state == TimeState.Morning) ? morningColor : nightColor;
    }

}
