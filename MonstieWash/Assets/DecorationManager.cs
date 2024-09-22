using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DecorationManager : MonoBehaviour
{
    public enum ManagerStatus
    {
        EmptyHand,
        Holding
    }


    [Tooltip("Populate with sprites you wish to become decorations.")]
    [SerializeField] private List<DecorationSprite> decorations;
    [Tooltip("Distance between objects on the deco bar.")]
    [SerializeField] private float decoBarBufferDist;
    [Tooltip("Speed items on deco bar move")]
    [SerializeField] private float decoBarItemSpeed;
    [Tooltip("Object that will be created to represent decoration on bar.")]
    [SerializeField] private GameObject referenceBarItem;
    public ManagerStatus managerStatus;


    private List<DecorationUi> m_barDecorations; //Decorations on the deco bar.    
    private List<DecorationUi> m_activeDecorations; //Decorations active in scene.
    private DecorationUi m_currentlyHeldDecoration; //Decoration hand is actively using.

    [Serializable]
    public class DecorationSprite
    {
        [Tooltip("The sprite to use as a decoration")] public Sprite sprite;
        [Tooltip("A scale to apply while on the Ui bar to shrink sprite")] public Vector3 relativeBarScale;
        [Tooltip("A scale to apply while in the scene to grow sprite")] public Vector3 relativeActualScale;
    }


    [Serializable]
    public class DecorationUi
    {
        public enum Status
        {
            onBar,
            beingHeld,
            activeInScene
        }

        public Status status;
        private float moveSpeed; //How fast object moves in the ui.

        public GameObject sceneObject; //Reference to decorations object in scene.
        private SpriteRenderer m_backingImage; //Reference to image showing its backing sprite while on the bar.
        private SpriteRenderer m_spriteImage; //Reference to image showing its actual representative sprite.
        public Vector3 desiredLocation; //Where the Ui object wants to be positioned in the scene.
        

        public DecorationUi(GameObject obj, SpriteRenderer back, SpriteRenderer sprite, Sprite InitialSprite, float speed)
        {
            status = Status.onBar;
            sceneObject = obj;
            m_backingImage = back;
            m_spriteImage = sprite;
            m_spriteImage.sprite = InitialSprite;
            moveSpeed = speed;
        }

        /// <summary>
        /// Moves towards desired location.
        /// </summary>
        public void MoveTowardDesiredLocation()
        {
            sceneObject.gameObject.transform.position = Vector3.MoveTowards(sceneObject.gameObject.transform.position, desiredLocation, moveSpeed * Time.deltaTime);
        }
    }

    public void OnEnable()
    {
        InputManager.Instance.OnSwitchTool += cycleOptions;
    }

    public void OnDisable()
    {
        InputManager.Instance.OnSwitchTool -= cycleOptions;
    }


    private void Awake()
    {
        m_barDecorations = new List<DecorationUi>();
        m_activeDecorations = new List<DecorationUi>();
        managerStatus = ManagerStatus.EmptyHand;

        //Generate new gameobject and populate an equivalent Decoration Ui
        foreach (DecorationSprite s in decorations)
        {
            
            var newObj = Instantiate(referenceBarItem,this.transform);
            var newDeco = new DecorationUi(newObj, newObj.transform.GetChild(0).GetComponent<SpriteRenderer>(), newObj.transform.GetChild(1).GetComponent<SpriteRenderer>(), s.sprite, decoBarItemSpeed);
            newObj.transform.GetChild(1).transform.localScale = s.relativeBarScale;

            m_barDecorations.Add(newDeco);
        }

        //Reset positions of newly generated objects.
        RefreshBarUI();

        //Instantly set them to the correct locations.
        foreach (DecorationUi dUI in m_barDecorations)
        {
            dUI.sceneObject.transform.position = dUI.desiredLocation;
        }
    }

    private void Update()
    {
        //Update positions of all decorations.
        moveAllDecorations();
    }


    /// <summary>
    /// Realign Decoration Objects place on the bar.
    /// </summary>
    private void RefreshBarUI()
    {
        if (m_barDecorations.Count != 0)
        {       
            for(int i = 0; i < m_barDecorations.Count; i++ )
            {
                //Check if object is in first or second half of list.
                var halfwayPoint = Mathf.Round(m_barDecorations.Count * 0.5f);

                //In first half
                if (i <= (halfwayPoint - 1)) m_barDecorations[i].desiredLocation = (transform.position - new Vector3(decoBarBufferDist * (halfwayPoint - i) - decoBarBufferDist * 0.5f, 0, 0));
                //In second half
                else m_barDecorations[i].desiredLocation = (transform.position + new Vector3(decoBarBufferDist * (i - halfwayPoint) + decoBarBufferDist * 0.5f, 0, 0));
            }
        }        
    }

    private void moveAllDecorations()
    {
        foreach (DecorationUi dUI in m_barDecorations)
        {
            //Only apply to objects still sitting on the ui bar.
            if (dUI.status != DecorationUi.Status.onBar) continue;
            dUI.MoveTowardDesiredLocation();
        }
    }


    private void cycleOptions(int dir)
    {
        if (dir == -1) cycleOptionsLeft();
        else if (dir == 1) cycleOptionsRight();       
    }

    private void cycleOptionsLeft()
    {
        //Only allow cycling while enough decorations exist to matter and not holding an item.
        if (m_barDecorations.Count <= 4) return;
        if (managerStatus == ManagerStatus.Holding) return;
        //Get first item and move to back
        var itemToMove = m_barDecorations[0];
        m_barDecorations.RemoveAt(0);
        m_barDecorations.Add(itemToMove);
        //Refresh UI
        RefreshBarUI();
        //Instantly update new last items position.
        m_barDecorations[m_barDecorations.Count - 1].sceneObject.transform.position = m_barDecorations[m_barDecorations.Count - 1].desiredLocation;
    }

    private void cycleOptionsRight()
    {
        //Only allow cycling while enough decorations exist to matter and not holding an item.
        if (m_barDecorations.Count <= 4) return;
        if (managerStatus == ManagerStatus.Holding) return;
        //Get last item and move to front
        var itemToMove = m_barDecorations[m_barDecorations.Count-1];
        m_barDecorations.RemoveAt(m_barDecorations.Count - 1);
        m_barDecorations.Insert(0, itemToMove);
        //Refresh Ui
        RefreshBarUI();
        //Instantly update new first items position.
        m_barDecorations[0].sceneObject.transform.position = m_barDecorations[0].desiredLocation;
    }
}
