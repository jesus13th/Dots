using System;
using System.Collections;
using UnityEngine;

public class Dot : MonoBehaviour {
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Color[] colors = { Color.clear, Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };
    public Color color;
    public enum EnumColor { Clear, Blue, Cyan, Green, Magenta, Red, Yellow }
    [SerializeField] private EnumColor myColor;
    public EnumColor MyColor { 
        get { return myColor; } 
        set { _spriteRenderer.color = color = colors[(int)(myColor = value)]; } 
    }
    [SerializeField] private Position position;
    public Position Position { get { return position; } set { position = value; GameManager.Instance.grid[value.x, value.y] = this; } }

    void Start() => MyColor = Enum.GetValues(typeof(EnumColor)).GetRandomValue<EnumColor>();
    public void Explote() => StartCoroutine(GetUpColor());
    public IEnumerator GetUpColor() {
        for (int deltaY = position.y; deltaY < GameManager.Instance.grid.GetLength(1); deltaY++) {
            yield return new WaitForSeconds(0.025f);
            GameManager.Instance.grid[position.x, deltaY].MyColor = EnumColor.Clear;
            GameManager.Instance.SoundBubble();
            yield return new WaitForSeconds(0.05f);
            GameManager.Instance.grid[position.x, deltaY].MyColor = deltaY + 1 < GameManager.Instance.grid.GetLength(1) ? GameManager.Instance.grid[position.x, deltaY + 1].MyColor : Enum.GetValues(typeof(EnumColor)).GetRandomValue<EnumColor>();
            if (deltaY + 1 < GameManager.Instance.grid.GetLength(1))
                GameManager.Instance.grid[position.x, deltaY + 1].MyColor = EnumColor.Clear;
            yield return new WaitForSeconds(0.025f);
        }
    }
    public override string ToString() => myColor.ToString();
}
[Serializable] public struct Position {
    public int x;
    public int y;
}