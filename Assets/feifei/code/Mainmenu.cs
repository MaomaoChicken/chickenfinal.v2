using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // แก้จาก System.Net.Mime.MediaTypeNames

public class Mainmenu : MonoBehaviour
{
    public bool mute = false;
    public Image sound;          // ลบ private ออก ให้ลาก assign ใน Inspector ได้
    public Sprite sound_off;
    public Sprite sound_on;
    public object Play;
    public void startgame()
    {
        SceneManager.LoadScene(1);  // โหลด Scene index 1 (Gameplay)
    }

    public void exitgame()
    {
        Application.Quit();
        Debug.Log("Exit Game");     // ดูผลใน Editor
    }

    public void sound_toggle()
    {
        mute = !mute;
        AudioListener.pause = mute; // หยุด/เปิดเสียงทั้งหมดในเกม
    }

    private void Update()
    {
        if (sound != null)
        {
            sound.sprite = mute ? sound_off : sound_on;
        }
    }
}