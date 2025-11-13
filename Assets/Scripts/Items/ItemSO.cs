using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Rarity Rarity;
    public string ItemName;
    public Weight Weight;
    public float TemperatureC;
    [Range(0, 100)] public float Organic;
    [Range(0, 5)] public int Magnetism;
    public string[] Compounds;
    public Sorting Sorting;
    public int Value;
    public Sprite[] Sprite;
    
}
