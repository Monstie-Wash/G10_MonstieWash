using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;


public class DecorationManager : MonoBehaviour
{
    public enum ManagerStatus
    {
        EmptyHand,
        Holding
    }

    public ManagerStatus managerStatus;

    [Header("Configurables")]
    [Tooltip("Populate with sprites you wish to become decorations.")]
    [SerializeField] private List<DecorationSprite> decorations;
    [Tooltip("Distance between objects on the deco bar.")]
    [SerializeField] private float decoBarBufferDist;
    [Tooltip("Speed items on deco bar move")]
    [SerializeField] private float decoBarItemSpeed;
    [Tooltip("Max size an item can be scaled to")]
    [SerializeField] private float maxItemScale;
    [Tooltip("Min size an item can be scaled to")]
    [SerializeField] private float minItemScale;
    [Tooltip("Mood during decoration")]
    [SerializeField] private MoodType decoMood;

    [Header("References")]
    [Tooltip("Object that will be created to represent decoration on bar.")]
    [SerializeField] private GameObject referenceBarItem;
    [Tooltip("FadingSprite to represent Camera Flash")]
    [SerializeField] private FadingSprite camFlash;
    [Tooltip("Area to capture screenshot for polaroid")]
    [SerializeField] private RectTransform screenshotArea;
    [Tooltip("Save Path ie; /Save.Png, will be stored in default application data path.")]
    [SerializeField] private string savePath;

    //Private
    private List<DecorationUi> m_barDecorations; //Decorations on the deco bar.    
    private List<DecorationUi> m_activeDecorations; //Decorations active in scene.
    private DecorationUi m_currentlyHeldDecoration; //Decoration hand is actively using.
    private Transform m_hand; //reference to players hand transform.
    private float m_pickupDistance = 1.5f; //How far to check for an item to pickup.
    private float m_rotatingValue = 0f; //Actively rotates held object with this.
    private float m_scaling = 0f; //Actively scales held object with this.

    #region Internal Classes
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
        public SpriteRenderer m_spriteImage; //Reference to image showing its actual representative sprite.
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
                m_spriteImage.gameObject.transform.localScale = spriteInfo.relativeActualScale;
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
                spriteInfo.relativeActualScale = m_spriteImage.gameObject.transform.localScale;
            }
            m_spriteImage.gameObject.transform.localScale = spriteInfo.relativeBarScale;

            status = Status.onBar;
            m_backingImage.gameObject.SetActive(true);

            //Reset spritemask to appear on mask.
            m_spriteImage.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            //Reset rotation.
            sceneObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void placeInScene()
        {
            status = Status.activeInScene;

            //Save scale changes.
            spriteInfo.relativeActualScale = m_spriteImage.gameObject.transform.localScale;
        }
    }

    #endregion

    public void OnEnable()
    {
        InputManager.Instance.OnSwitchTool += CycleOptions;
        InputManager.Instance.OnActivate_Started += Clicked;
        InputManager.Instance.OnSwitchTool_Ended += ResetRotation;
        InputManager.Instance.OnNavigate += DropHeldItem;
        InputManager.Instance.OnScroll += ScaleInput;
        InputManager.Instance.OnScroll_Ended += ResetScaling;
    }

    public void OnDisable()
    {
        InputManager.Instance.OnSwitchTool -= CycleOptions;
        InputManager.Instance.OnActivate_Started -= Clicked;
        InputManager.Instance.OnSwitchTool_Ended -= ResetRotation;
        InputManager.Instance.OnNavigate -= DropHeldItem;
        InputManager.Instance.OnScroll -= ScaleInput;
        InputManager.Instance.OnScroll_Ended -= ResetScaling;
    }


    private void Awake()
    {
        m_barDecorations = new List<DecorationUi>();
        m_activeDecorations = new List<DecorationUi>();
        managerStatus = ManagerStatus.EmptyHand;
        m_hand = FindFirstObjectByType<PlayerHand>().gameObject.transform;

        //Enable Finish button. Disable decorate button
        FindFirstObjectByType<DecorationNavigate>().gameObject.SetActive(false);
        FindFirstObjectByType<FinishLevelButton>(FindObjectsInactive.Include).gameObject.SetActive(true);

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

        LockMonsterMood();
    }

    private void Update()
    {
        //Update positions of all decorations.
        MoveAllDecorations();
        //Rotate held object if desired.
        if (m_rotatingValue != 0 && m_currentlyHeldDecoration != null)
        {
            m_currentlyHeldDecoration.sceneObject.transform.Rotate(0,0, 100 * m_rotatingValue * Time.deltaTime);
        }
        //Scales held object if desired.
        if (m_scaling != 0 && m_currentlyHeldDecoration != null)
        {
            var scaleToApply = new Vector3(5 * m_scaling * Time.deltaTime, 5 * m_scaling * Time.deltaTime, 0);
            var newScale = m_currentlyHeldDecoration.m_spriteImage.gameObject.transform.localScale + scaleToApply;

            //If new scale doesn't breach upper or lower limits then apply it.
            if (!(newScale.x > maxItemScale) && !(newScale.x < minItemScale)) m_currentlyHeldDecoration.m_spriteImage.gameObject.transform.localScale = newScale;
        }
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

    private void CycleOptions(int dir)
    {
        //While empty handed cycle through decoration options.
        if (managerStatus == ManagerStatus.EmptyHand)
        {
            if (dir == -1) CycleOptionsLeft();
            else if (dir == 1) CycleOptionsRight();
        }
        //If holding an item instead rotate it.
        else
        {
            m_rotatingValue = -dir;
        }    
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
        m_barDecorations[0].sceneObject.transform.position = m_barDecorations[0].desiredLocation - new Vector3(decoBarBufferDist , 0);
    }


    private void ResetRotation()
    {
        m_rotatingValue = 0f;
    }


    private void ScaleInput(int dir)
    {
        if (m_currentlyHeldDecoration == null) return;
        m_scaling = -dir;  
    }

    private void ResetScaling()
    {
        m_scaling = 0f;
    }


    private void DropHeldItem()
    {
        if (m_currentlyHeldDecoration == null) return;      
        m_activeDecorations.Remove(m_currentlyHeldDecoration);
        m_barDecorations.Add(m_currentlyHeldDecoration);
        m_currentlyHeldDecoration.returnToBar();
        RefreshBarUI();
        m_currentlyHeldDecoration.sceneObject.transform.position = m_currentlyHeldDecoration.desiredLocation;
        m_currentlyHeldDecoration = null;
        managerStatus = ManagerStatus.EmptyHand;
    }

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
            foreach (DecorationUi d in m_activeDecorations)
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

    private void LockMonsterMood()
    {
        StartCoroutine(HandleMonsterMoods());
    }

    private IEnumerator HandleMonsterMoods()
    {
        MonsterBrain mb = FindFirstObjectByType<MonsterBrain>();

        mb.UpdateMood(99999, decoMood);
        foreach (MoodType mt in mb.Moods)
        {
            if (mt != decoMood)
            {
                mb.UpdateMood(-99999, mt);
            }

        }
        

        //Wait for frame to finish so animation updates can take place based on new moods.
        yield return new WaitForEndOfFrame();

        mb.PauseBrain(true);
    }


    public void TakePolaroid()
    {
        StartCoroutine(PolaroidSnap());
    }

    private IEnumerator PolaroidSnap()
    {       
        //Screenshot area definition
        Vector3[] corners = new Vector3[4];
        screenshotArea.GetWorldCorners(corners);
        int width = ((int)corners[3].x - (int)corners[0].x) - 100;
        int height = (int)corners[1].y - (int)corners[0].y;
        var startX = corners[0].x;
        var startY = corners[0].y;

        //Turn off bar and button for photo.
        this.transform.GetChild(0).gameObject.SetActive(false);
        FindFirstObjectByType<FinishLevelButton>().gameObject.SetActive(false);
        m_hand.gameObject.SetActive(false);

        //Wait for seconds and camera flash + sound here.
        camFlash.FadeIn();
        yield return new WaitForSeconds(1f);
        

        //Wait for frame to fully render and update before taking screenshot.
        yield return new WaitForEndOfFrame();

        //Apply screen view to a texture (Take screenshot)
        Texture2D tempText = new Texture2D(width, height, TextureFormat.RGB24, false);
        tempText.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        tempText.Apply();

        //Apply a modifier to end of file based on count of objects stored.
        var fileCount = Directory.GetFiles(Application.persistentDataPath).Length;
        //Save to file.
        byte[] byteArray = tempText.EncodeToPNG();
        string saveLocation = Application.persistentDataPath + savePath + fileCount + ".Png";

        File.WriteAllBytes(saveLocation, byteArray);

        Debug.Log("Saved screenshot at: " + saveLocation);
        Destroy(tempText);



        GameSceneManager.Instance.FinishLevel();
    }

}
