using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "NPC/CharacterData")]
public class DATAChar : ScriptableObject
{
    public Sprite idleSprite;
    public Sprite[] walkSprites; // ใส่ 2 ใบ
    public float walkAnimSpeed = 0.15f;
}