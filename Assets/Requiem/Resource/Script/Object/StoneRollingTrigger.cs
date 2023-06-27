using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StoneRollingTrigger : MonoBehaviour
{
    [SerializeField] private Switch m_switch;
    [SerializeField] Switch platformSwich1;
    [SerializeField] Switch platformSwich2;
    [SerializeField] Object_Trigger rollingStoneTrigger;
    [SerializeField] RollingStone rollingStone;
    [SerializeField] CinemachineVirtualCamera mainCM;

    [SerializeField] bool isActive = false;
    private bool prevIsActive = false;

    //[CustomEditor(typeof(StoneRollingTrigger))]
    //public class StoneRollingTriggerEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        StoneRollingTrigger trigger = (StoneRollingTrigger)target;

    //        EditorGUILayout.Space();

    //        if (GUILayout.Button("Reset Trigger"))
    //        {
    //            trigger.ResetTrigger();
    //        }
    //    }
    //}

    void Start()
    {
        mainCM = DataController.MainCM;
    }

    void Update()
    {
        if (isActive && !prevIsActive)
        {
            DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, 10f, 4f);
            DivAreaManager.Instance.DivAreaActive(false);
            Invoke("PlatformActive", 2f);
            Invoke("StoneActive", 4f);
            mainCM.GetComponent<CinemachineConfiner2D>().enabled = false;
            mainCM.Follow = rollingStone.transform;
        }

        if (isActive)
        {
            DOTween.To(() => mainCM.m_Lens.OrthographicSize, x => mainCM.m_Lens.OrthographicSize = x, 10f, 4f);
        }

        prevIsActive = isActive;
        if (isActive != m_switch.isActive)
        {
            isActive = m_switch.isActive;
        }
    }

    void ResetTrigger()
    {
        platformSwich1.isActive = false;
        platformSwich2.isActive = false;
        isActive = false;
        rollingStoneTrigger.ResetTrigger();
        rollingStone.resetPosition();
        mainCM.Follow = PlayerData.PlayerObj.transform;
        mainCM.GetComponent<CinemachineConfiner2D>().enabled = true;
        prevIsActive = false;
    }

    void PlatformActive()
    {
        platformSwich1.isActive = true;
        platformSwich2.isActive = true;
    }

    void StoneActive()
    {
        rollingStoneTrigger.ActiveTrigger();
    }
}
