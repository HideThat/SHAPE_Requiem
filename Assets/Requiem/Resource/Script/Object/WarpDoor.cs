using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Cainos.PixelArtPlatformer_Dungeon;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WarpDoor))]
public class WarpDoorEditor : Editor
{
    private SerializedProperty otherDoorProp;
    private SerializedProperty triggerDoorsProp;
    private SerializedProperty fadeOutAndInTimeProp;
    private SerializedProperty fadeOutAndInWaitTimeProp;
    private SerializedProperty isOpenedProp;
    private SerializedProperty playerInProp;
    private SerializedProperty showObjectsProp;
    private SerializedProperty keyBlockProp;
    private SerializedProperty doorProp;
    private SerializedProperty lampLightProp;
    private SerializedProperty lampGlowProp;
    private SerializedProperty lampDustProp;
    private SerializedProperty lampLitProp;

    private void OnEnable()
    {
        otherDoorProp = serializedObject.FindProperty("otherDoor");
        triggerDoorsProp = serializedObject.FindProperty("triggerDoors");
        fadeOutAndInTimeProp = serializedObject.FindProperty("fadeOutAndInTime");
        fadeOutAndInWaitTimeProp = serializedObject.FindProperty("fadeOutAndInWaitTime");
        isOpenedProp = serializedObject.FindProperty("isOpened");
        playerInProp = serializedObject.FindProperty("playerIn");
        showObjectsProp = serializedObject.FindProperty("showObjects");
        keyBlockProp = serializedObject.FindProperty("keyBlock");
        doorProp = serializedObject.FindProperty("door");
        lampLightProp = serializedObject.FindProperty("lampLight");
        lampGlowProp = serializedObject.FindProperty("lampGlow");
        lampDustProp = serializedObject.FindProperty("lampDust");
        lampLitProp = serializedObject.FindProperty("lampLit");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(otherDoorProp);
        EditorGUILayout.PropertyField(fadeOutAndInTimeProp);
        EditorGUILayout.PropertyField(fadeOutAndInWaitTimeProp);
        EditorGUILayout.PropertyField(isOpenedProp);
        EditorGUILayout.PropertyField(playerInProp);
        EditorGUILayout.PropertyField(showObjectsProp);
        EditorGUILayout.PropertyField(triggerDoorsProp);

        WarpDoor warpDoor = (WarpDoor)target;

        if (warpDoor.showObjects)
        {
            EditorGUILayout.PropertyField(keyBlockProp);
            EditorGUILayout.PropertyField(doorProp);
            EditorGUILayout.PropertyField(lampLightProp);
            EditorGUILayout.PropertyField(lampGlowProp);
            EditorGUILayout.PropertyField(lampDustProp);
            EditorGUILayout.PropertyField(lampLitProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif


public class WarpDoor : MonoBehaviour
{
    [SerializeField] private WarpDoor otherDoor;
    [SerializeField] private WarpDoor[] triggerDoors;
    [SerializeField] float fadeOutAndInTime = 0.2f;
    [SerializeField] float fadeOutAndInWaitTime = 0.2f;
    [SerializeField] public bool isOpened = false;
    [SerializeField] bool playerIn;
    [Header("프로그래머")]
    [SerializeField] public bool showObjects = false;
    [SerializeField] GameObject keyBlock;
    [SerializeField] Door door;
    [SerializeField] Light2D lampLight;
    [SerializeField] GameObject lampGlow;
    [SerializeField] GameObject lampDust;
    [SerializeField] GameObject lampLit;

    Transform player;



    private void Start()
    {
        door = GetComponent<Door>();
        door.IsOpened = isOpened;
        keyBlock.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (playerIn && Input.GetKey(KeyCode.F) && !PlayerData.Instance.m_playerObj.GetComponent<PlayerControllerGPT>().m_isWarp)
        {
            Invoke("MoveOtherDoor", fadeOutAndInTime);
            
            StartCoroutine(PlayerWarpDelay());
            FadeManager.Instance.FadeOutAndIn(fadeOutAndInTime, fadeOutAndInWaitTime);
        }

        if (playerIn)
        {
            TurnLight();
        }
        else
        {
            Invoke("TurnLight", 1f);
        }
        door.IsOpened = isOpened;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOpened)
        {
            playerIn = true;
            player = collision.transform;
            keyBlock.SetActive(true);

            Debug.Log("Player In");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOpened)
        {
            playerIn = false;
            keyBlock.SetActive(false);
            Debug.Log("Player Out");
        }
    }

    void MoveOtherDoor()
    {
        player.position = new Vector2(otherDoor.transform.position.x, otherDoor.transform.position.y + 1f);
    }

    void TurnLight()
    {
        if (playerIn)
        {
            DOTween.To(() => lampLight.pointLightOuterRadius, x => lampLight.pointLightOuterRadius = x, 3f, 1f);
        }
        else
        {
            DOTween.To(() => lampLight.pointLightOuterRadius, x => lampLight.pointLightOuterRadius = x, 0f, 2f);
        }
        lampDust.SetActive(playerIn);
        lampGlow.SetActive(playerIn);
        lampLit.SetActive(playerIn);
    }

    public void DoorOpen()
    {
        foreach (var item in triggerDoors)
        {
            item.isOpened = true;
        }
    }

    IEnumerator PlayerWarpDelay()
    {
        PlayerData.Instance.m_playerObj.GetComponent<PlayerControllerGPT>().m_isWarp = true;

        yield return new WaitForSeconds(0.5f);

        PlayerData.Instance.m_playerObj.GetComponent<PlayerControllerGPT>().m_isWarp = false;
    }
}