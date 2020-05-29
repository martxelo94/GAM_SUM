using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraMovement : MonoBehaviour
{
    private Camera mainCamera;

    public float minHeight = 0.0f;
    public float maxHeight = 1.0f;
    public float speed = 1f;
    public bool invertY = true;
    public Transform target;    // to move by dragging

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        // find player deck and position camera
        Deck[] decks = FindObjectsOfType<Deck>();
        Deck player_deck = null;
        foreach (Deck d in decks)
            if (d.team == TeamType.Player) {
                player_deck = d;
                break;
            }
        Vector3 p = player_deck.transform.position;
        p.y = Mathf.Clamp(p.y, minHeight, maxHeight);
        p.x = transform.position.x;
        p.z = transform.position.z;
        transform.position = p;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDragging()) {
            float dy = GetDeltaPosYAtTouch();
            MoveY(invertY? -dy : dy);
        }
    }

    bool IsTouchDown()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
    }

    bool IsDragging()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        return Input.touchCount > 0;
#endif
    }
    public float GetPosYAtTouch()
    {
#if UNITY_EDITORz
        float h = Input.GetAxisRaw("Mouse Y");
        //Debug.Log("Mouse Y = " + h);
        Vector3 screenPos = Input.mousePosition;
#else
        Vector3 screenPos = Input.touches[0].position;
#endif
        screenPos.z = target.position.z;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        //Debug.Log("World Delta = " + worldDeltaPos);
        return worldPos.y;
    }
    public float GetDeltaPosYAtTouch()
    {
#if UNITY_EDITOR
        float h = Input.GetAxis("Mouse Y");
        //Debug.Log("Mouse Y = " + h);
        Vector3 screenDeltaPos = new Vector3(0, h, 0);
#else
        Vector3 screenDeltaPos = Input.touches[0].deltaPosition;
#endif
        screenDeltaPos.z = target.position.z;
        screenDeltaPos.x = Screen.width / 2;
        screenDeltaPos.y += Screen.height / 2;

        Vector3 worldDeltaPos = mainCamera.ScreenToWorldPoint(screenDeltaPos);
        worldDeltaPos -= transform.position;
        //Debug.Log("World Delta = " + worldDeltaPos);
        return worldDeltaPos.y;
    }


    void MoveY(float deltaY)
    {
        Vector3 p = transform.position;
        p.y += deltaY * speed;
        p.y = Mathf.Clamp(p.y, minHeight, maxHeight);
        transform.position = p;
        //Debug.Log("Moved deltaY = " + deltaY);
        //Debug.Log("New Height = " + p.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(
            new Vector3(0, minHeight - Screen.height, target.position.z),
            new Vector3(500, 1, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(
            new Vector3(0, maxHeight + Screen.height, target.position.z),
            new Vector3(500, 1, 1));

    }
}
