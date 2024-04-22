using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicBrain : MonsterBrain
{
    // Tracks current mood
    private string m_currentMood;

    private RoomSaver m_RoomSaver;

    // Animations
    // Variables that hold the mimic object prefabs so that their animator components can be referenced. 
    [Tooltip("The prefab for the mimic's front viewing angle.")] [SerializeField] private GameObject mimicFrontPrefab;
    [Tooltip("The prefab for the mimic's left viewing angle.")] [SerializeField] private GameObject mimicLeftPrefab;
    [Tooltip("The prefab for the mimic's right viewing angle.")] [SerializeField] private GameObject mimicRightPrefab;
    [Tooltip("The prefab for the mimic's back viewing angle.")] [SerializeField] private GameObject mimicBackPrefab;
    // Animators are used to switch animations for each of the mimic's angles via scripting, based on the mimic's current mood
    private Animator m_mimicFrontAnimator;
    private Animator m_mimicLeftAnimator;
    private Animator m_mimicRightAnimator;
    private Animator m_mimicBackAnimator;

    private void Awake()
    {
        base.Awake();
        m_RoomSaver = FindFirstObjectByType<RoomSaver>();
    }

    private void OnEnable() 
    {
        m_RoomSaver.OnScenesLoaded += LoadMimic;
    }

    private void LoadMimic()
    {
            // Get mimic prefabs
            mimicFrontPrefab = GameObject.FindGameObjectWithTag("Front");
            mimicLeftPrefab = GameObject.FindGameObjectWithTag("Left");
            mimicRightPrefab = GameObject.FindGameObjectWithTag("Right");
            mimicBackPrefab = GameObject.FindGameObjectWithTag("Back");

            // Get Animator components from mimic prefabs
            m_mimicFrontAnimator = mimicFrontPrefab.GetComponent<Animator>();
            m_mimicLeftAnimator = mimicLeftPrefab.GetComponent<Animator>();
            m_mimicRightAnimator = mimicRightPrefab.GetComponent<Animator>();
            m_mimicBackAnimator = mimicBackPrefab.GetComponent<Animator>();
    }

    /// <summary>
    // State machine that applies animations to mimic GameObject prefabs based on the mimic's current mood. 
    /// Overridden from MonsterBrain
    /// </summary>
    protected override void UpdateAnimations()
    {   
        var highestMood = GetHighestMood();
        var highestMoodName = ReadMood(highestMood);

        if (highestMoodName == m_currentMood) return;

        print(highestMoodName);

        switch (highestMoodName)
        {
            case "Angry":
                m_mimicFrontAnimator.Play("Mimic_Angry_Front");
                m_mimicLeftAnimator.Play("Mimic_Angry_Left");
                m_mimicRightAnimator.Play("Mimic_Angry_Right");
                m_mimicBackAnimator.Play("Mimic_Angry_Back");
                m_currentMood = highestMoodName;
                break;
            case "Happy":
                m_mimicFrontAnimator.Play("Mimic_Happy_Front");
                m_mimicLeftAnimator.Play("Mimic_Happy_Left");
                m_mimicRightAnimator.Play("Mimic_Happy_Right");
                m_mimicBackAnimator.Play("Mimic_Happy_Back");
                m_currentMood = highestMoodName;
                break;
            case "Hungry":
                m_mimicFrontAnimator.Play("Mimic_Hungry_Front");
                m_mimicLeftAnimator.Play("Mimic_Hungry_Left");
                m_mimicRightAnimator.Play("Mimic_Hungry_Right");
                m_mimicBackAnimator.Play("Mimic_Hungry_Back");
                m_currentMood = highestMoodName;
                break;
            default:
                m_mimicFrontAnimator.Play("Mimic_Idle_Front");
                m_mimicLeftAnimator.Play("Mimic_Idle_Left");
                m_mimicRightAnimator.Play("Mimic_Idle_Right");
                m_mimicBackAnimator.Play("Mimic_Idle_Back");
                break;
        }
    }

}
