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


    [SerializeField] private List<DecorationUi> m_barDecorations; //Decorations on the deco bar.    
    [SerializeField] private List<DecorationUi> m_activeDecorations; //Decorations active in scene.
    [SerializeField] private DecorationUi m_currentlyHeldDecoration; //Decoration hand is actively using.
    private Transform m_hand; //reference to players hand transform.
    private float m_pickupDistance = 1; //How far to check for an item to pickup.

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

        [SerializeField] public Status status;
        private float moveSpeed; //How fast object moves in the ui.

        public GameObject sceneObject; //Reference to decorations object in scene.
        private SpriteRenderer m_backingImage; //Reference to image showing its backing sprite while on the bar.
        private SpriteRenderer m_spriteImage; //Reference to image showing its actual representative sprite.
        public Vector3 desiredLocation; //Where the Ui object wants to be positioned in the scene.
        private DecorationSprite spriteInfo; //Original sprite sizing data.
        

        public DecorationUi(GameObject obj, SpriteRenderer back, SpriteRenderer sprite, float speed, DecorationSprite spr)
        {
            status = Status.onBar;
            sceneObject = obj;
            m_backingImage = back;
            m_spriteImage = sprite;
            m_spriteImage.sprite = spr.sprite;
            moveSpeed = speed;
            spriteInfo = spr;
        }

        /// <summary>
        /// Moves towards desired location.
        /// </summary>
        public void MoveTowardDesiredLocation()
        {
            sceneObject.gameObject.transform.position = Vector3.MoveTowards(sceneObject.gameObject.transform.position, desiredLocation, moveSpeed * Time.deltaTime);
        }

        public void pickUp()
        {
            //Change to relative scene scale if picked up off the bar.
            if (status == Status.onBar)
            {
                sceneObject.transform.localScale = spriteInfo.relativeActualScale;
            }

            status = Status.beingHeld;
            m_backingImage.gameObject.SetActive(false);

            //Change sprite mask to appear anywhere.
            m_spriteImage.maskInteraction = SpriteMaskInteraction.None;   
        }

        public void returnToBar()
        {
            //Change back to local bar scale and save scale changes if was in scene.
            if (status == Status.activeInScene)
            {
                spriteInfo.relativeActualScale = sceneObject.transform.localScale;
            }
            sceneObject.transform.localScale = spriteInfo.relativeBarScale;

            status = Status.onBar;
            m_backingImage.gameObject.SetActive(true);

            //Reset spritemask to appear on mask.
            m_spriteImage.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        public void placeInScene()
        {
            status = Status.activeInScene;
        }
    }

    public void OnEnable()
    {
        InputManager.Instance.OnSwitchTool += CycleOptions;
        InputManager.Instance.OnActivate_Started += Clicked;
    }

    public void OnDisable()
    {
        InputManager.Instance.OnSwitchTool -= CycleOptions;
        InputManager.Instance.OnActivate_Started -= Clicked;
    }


    private void Awake()
    {
        m_barDecorations = new List<DecorationUi>();
        m_activeDecorations = new List<DecorationUi>();
        managerStatus = ManagerStatus.EmptyHand;
        m_hand = FindFirstObjectByType<PlayerHand>().gameObject.transform;

        //Generate new gameobject and populate an equivalent Decoration Ui
        foreach (DecorationSprite s in decorations)
        {
            
            var newObj = Instantiate(referenceBarItem,this.transform);
            var newDeco = new DecorationUi(newObj, newObj.transform.GetChild(0).GetComponent<SpriteRenderer>(), newObj.transform.GetChild(1).GetComponent<SpriteRenderer>(), decoBarItemSpeed, s);
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
        MoveAllDecorations();
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

    private void MoveAllDecorations()
    {
        foreach (DecorationUi dUI in m_barDecorations)
        {
            if (dUI.status == DecorationUi.Status.onBar) dUI.MoveTowardDesiredLocation();
        }

        foreach (DecorationUi aUI in m_activeDecorations)
        {
            switch (aUI.status)
            {
                case DecorationUi.Status.activeInScene:
                    //Don't move while actively placed in scene.
                    break;
                case DecorationUi.Status.beingHeld:
                    aUI.sceneObject.transform.position = m_hand.position;
                    break;
            }
        }
    }

    #region Cycling
    private void CycleOptions(int dir)
    {
        if (dir == -1) CycleOptionsLeft();
        else if (dir == 1) CycleOptionsRight();       
    }

    private void CycleOptionsLeft()
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

    private void CycleOptionsRight()
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

    #endregion

    private void Clicked()
    {
        //Check you aren't already holding an item.
        if (managerStatus == ManagerStatus.EmptyHand)
        { 
            //Find if any decorations in bar are near player hand first.
            foreach (DecorationUi d in m_barDecorations)
            {
                if (Vector2.Distance(m_hand.transform.position, d.sceneObject.transform.position) <= m_pickupDistance)
                {
                    m_currentlyHeldDecoration = d;
                    m_barDecorations.Remove(d);
                    m_activeDecorations.Add(d);
                    d.pickUp();
                    RefreshBarUI();

                    managerStatus = ManagerStatus.Holding;
                    return;
                }
            }
            //Then check scene if none were found.
            foreach (DecorationUi d in m_barDecorations)
            {
                if (Vector2.Distance(m_hand.transform.position, d.sceneObject.transform.position) <= m_pickupDistance)
                {
                    m_currentlyHeldDecoration = d;
                    m_barDecorations.Remove(d);
                    m_activeDecorations.Add(d);
                    d.pickUp();
                    RefreshBarUI();

                    managerStatus = ManagerStatus.Holding;
                    return;
                }
            }
        }
        else //When holding something
        {            
            m_currentlyHeldDecoration.placeInScene();
            RefreshBarUI();

            managerStatus = ManagerStatus.EmptyHand;
            m_currentlyHeldDecoration = null;
        }
    }
}
