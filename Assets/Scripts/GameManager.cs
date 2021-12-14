using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    [Header("Grid"), SerializeField, Range(0, 1)] private float gridSize;
    [SerializeField] private Vector2 gridOffset;
    public Dot[,] grid = new Dot[6, 12];
    [SerializeField] private Dot pDot;
    [SerializeField] private List<Dot> dots = new List<Dot>();
    private readonly Vector2 offset = new Vector2(-2.5f, -1.5f);
    [SerializeField] private GameObject pFinger;
    [SerializeField] private LineRenderer fingerLineRenderer;
    [SerializeField] private int maxConnections = 0;
    public int MaxConnections { get { return maxConnections; } set { maxConnections = value > maxConnections ? value : maxConnections; } }
    [SerializeField] private int score;
    public int Score { get { return score; } set { scoreText.text = $"Score: {(score = value)} \nMax Connections: {maxConnections}"; } }
    [SerializeField] private UnityEngine.UI.Text scoreText;
    [SerializeField] private AudioSource _audiosource;
    [SerializeField] private AudioClip bubbleClip;


    void Start() {
        Instance = this;
        Score = 0;
        StartCoroutine(CreateDot());
    }
    private void Update() => TouchMovement();
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(gridOffset, Quaternion.identity, Vector3.one * gridSize);
        for (int y = 0; y <= grid.GetLength(1); y++) {
            Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(grid.GetLength(0), y, 0));
        }
        for (int x = 0; x <= grid.GetLength(0); x++) {
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, grid.GetLength(1), 0));
        }
    }
    private IEnumerator CreateDot() {
        for (int y = 0; y < grid.GetLength(1); y++) {
            for (int x = 0; x < grid.GetLength(0); x++) {
                Instantiate(pDot, new Vector2(x, y) + gridOffset + Vector2.one * 0.5f, Quaternion.identity).Position = new Position() { x = x, y = y };
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    private void TouchMovement() {
        if (Input.touchCount == 0) return;
        Touch t = Input.GetTouch(0);
        switch (t.phase) {
            case TouchPhase.Moved:
                pFinger.transform.position = ScreenToWorldPoint(t.position);
                AddDot(Physics2D.OverlapCircle(pFinger.transform.position, 0.2f, 1 << LayerMask.NameToLayer("Dot"))?.GetComponent<Dot>());
                break;
            case TouchPhase.Ended:
                fingerLineRenderer.positionCount = 0;
                ExploteDots();
                break;
        }
    }
    private void AddDot(Dot dot) {
        if (dots.AddIfNotExist(dot)) {
            if (dots.Count == 1)
                fingerLineRenderer.material.color = dots.First().color;
            fingerLineRenderer.positionCount = dots.Count;
            fingerLineRenderer.SetPosition(dots.IndexOf(dot), dot.transform.position);
        }
    }
    private Vector3 ScreenToWorldPoint(Vector3 position) {
        var pos = Camera.main.ScreenToWorldPoint(position);
        pos.z = -0.1f;
        return pos;
    }
    private void ExploteDots() {
        if (dots.Count > 2) {
            AddScore(dots.Count);
            dots.OrderByDescending(d => d.Position.y).ToList().ForEach(d => d.Explote());
        }
        dots.Clear();
    }
    public void SoundBubble() {
        _audiosource.PlayOneShot(bubbleClip, 0.1f);
    }
    private void AddScore(int count) {
        MaxConnections = count;
        Score = score + count + (count - 3) * count;
    }
    public void Restart() {
        SceneManager.LoadScene(0);
    }
}

public static class Extensions {
    public const float maxMagnitud = 1.5f; 
    public static T GetRandomValue<T>(this IList list) => (T)list[Random.Range(1, list.Count)];
    public static bool AddIfNotExist(this List<Dot> list, Dot item) {
        if (!list.Contains(item) && item != null) {
            if (list.Count > 0 && (list.First().MyColor != item.MyColor || Vector2.Distance(list.Last().transform.position, item.transform.position) > maxMagnitud))
                return false;
            list.Add(item);
            return true;
        }
        return false;
    }
}