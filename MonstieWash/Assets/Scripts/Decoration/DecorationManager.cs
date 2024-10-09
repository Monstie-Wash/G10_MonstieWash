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
    [Tooltip("Save Name ie; /Polaroid, will be stored in default application data path.")]
    [SerializeField] private string savePath;

    //Private
    private List<DecorationUi> m_barDecorations; //Decorations on the deco bar.    
    private List<DecorationUi> m_activeDecorations; //Decorations active in scene.
    private DecorationUi m_currentlyHeldDecoration; //Decoration hand is actively using.
    private Transform m_hand; //reference to players hand transform.
    private float m_pickupDistance = 1.5f; //How far to check for an item to pickup.
    private float m_rotatingDirection = 0f; //Actively rotates held object with this.
    private float m_scaling = 0f; //Actively scales held object with this.

    #region Internal Classes
    [Serializable]

    //Holds data about sprites to be used as decorations. Relative bar scale is size when shown on the deco bar, actual scale is size shown when dragged into the scene.
    private class DecorationSprite
    {
        [Tooltip("The sprite to use as a decoration")] public Sprite sprite;
        [Tooltip("A scale to apply while on the Ui bar to shrink sprite")] public Vector3 relativeBarScale;
        [Tooltip("A scale to apply while in the scene to grow sprite")] public Vector3 relativeActualScale;
    }

    //Object that will hold live data about a decoration, generated from list of decoration sprites ^.
    [Serializable]
    private class DecorationUi
    {
        //Where the decoration currently is.
        public enum Status
        {
            OnBar,
            BeingHeld,
            ActiveInScene
        }

        //Public
        public Status status;
        public Vector3 desiredLocation; //Where the Ui object wants to be positioned in the scene.
        public GameObject sceneObject; //Reference to decorations object in scene.
        public SpriteRenderer m_spriteImage; //Reference to image showing its actual representative sprite.

        //Private
        private SpriteRenderer m_backingImage; //Reference to image showing its backing sprite while on the bar.
        private float m_moveSpeed; //How fast object moves in the ui.
        private DecorationSprite m_spriteInfo; //Original sprite sizing data.                            

        public DecorationUi(GameObject obj, SpriteRenderer back, SpriteRenderer sprite, float speed, DecorationSprite spr)
        {
            status = Status.OnBar;
            sceneObject = obj;
            m_backingImage = back;
            m_spriteImage = sprite;
            m_spriteImage.sprite = spr.sprite;
            m_moveSpeed = speed;
            m_spriteInfo = spr;
        }

        /// <summary>
        /// Moves towards desired location.
        /// </summary>
        public void MoveTowardDesiredLocation()
        {
            sceneObject.gameObject.transform.position = Vector3.MoveTowards(sceneObject.gameObject.transform.position, desiredLocation, m_moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Object will follow player hand.
        /// </summary>
        public void PickUp()
        {
            //Change to relative scene scale if picked up off the bar.
            if (status == Status.OnBar)
            {
                m_spriteImage.gameObject.transform.localScale = m_spriteInfo.relativeActualScale;
            }

            status = Status.BeingHeld;
            m_backingImage.gameObject.SetActive(false);

            //Change sprite mask to appear anywhere.
            m_spriteImage.maskInteraction = SpriteMaskInteraction.None;   
        }

        /// <summary>
        /// Object will become a part of the deco bar list again.
        /// </summary>
        public void ReturnToBar()
        {
            //Change back to local bar scale and save scale changes if was in scene.
            if (status == Status.ActiveInScene)
            {
                m_spriteInfo.relativeActualScale = m_spriteImage.gameObject.transform.localScale;
            }
            m_spriteImage.gameObject.transform.localScale = m_spriteInfo.relativeBarScale;

            status = Status.OnBar;
            m_backingImage.gameObject.SetActive(true);

            //Reset spritemask to appear on mask.
            m_spriteImage.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            //Reset rotation.
            sceneObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        /// <summary>
        /// Object will be fixed to a location, rotation and size in the scene, unmoving.
        /// </summary>
        public void PlaceInScene()
        {
            status = Status.ActiveInScene;

            //Save scale changes.
            m_spriteInfo.relativeActualScale = m_spriteImage.gameObject.transform.localScale;
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
        //Create empty lists.
        m_barDecorations = new List<DecorationUi>();
        m_activeDecorations = new List<DecorationUi>();

        //Set initial status.
        managerStatus = ManagerStatus.EmptyHand;

        //Find player hand.
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

        //Set monster to the chosen decoration mood.
        LockMonsterMood();
    }

    private void Update()
    {
        //Update positions of all decorations.
        MoveAllDecorations();

        //Rotate held object if desired.
        if (m_rotatingDirection != 0 && m_currentlyHeldDecoration != null)
        {
            m_currentlyHeldDecoration.sceneObject.transform.Rotate(0,0, 100 * m_rotatingDirection * Time.deltaTime);
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

                //In first half move via buffer to the left.
                if (i <= (halfwayPoint - 1)) m_barDecorations[i].desiredLocation = (transform.position - new Vector3(decoBarBufferDist * (halfwayPoint - i) - decoBarBufferDist * 0.5f, 0, 0));
                //In second half move via buffer to the right.
                else m_barDecorations[i].desiredLocation = (transform.position + new Vector3(decoBarBufferDist * (i - halfwayPoint) + decoBarBufferDist * 0.5f, 0, 0));
            }
        }        
    }

    /// <summary>
    /// Handles how each decoration should currently be moving.
    /// </summary>
    private void MoveAllDecorations()
    {
        foreach (DecorationUi dUI in m_barDecorations)
        {
            //Decorations on the bar will always seek their resting spot.
            if (dUI.status == DecorationUi.Status.OnBar) dUI.MoveTowardDesiredLocation();
        }

        foreach (DecorationUi aUI in m_activeDecorations)
        {
            switch (aUI.status)
            {
                case DecorationUi.Status.ActiveInScene:
                    //Empty case as decorations in the scene should remain stationary.

                    break;
                case DecorationUi.Status.BeingHeld:
                    //Objects being held should move with the player hand.
                    aUI.sceneObject.transform.position = m_hand.position;
                    break;
            }
        }
    }

    /// <summary>
    /// Takes an input direction from input manager and calls correct cycle method.
    /// </summary>
    /// <param name="dir">-1 for left 1 for right.</param>
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
            m_rotatingDirection = -dir;
        }    
    }

    /// <summary>
    /// Moves bar items to the left.
    /// </summary>
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

    /// <summary>
    /// Moves bar items to the right.
    /// </summary>
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

    /// <summary>
    /// Called by releasing rotation input.
    /// </summary>
    private void ResetRotation()
    {
        m_rotatingDirection = 0f;
    }

    /// <summary>
    /// Takes a direction and updates scaling value via input.
    /// </summary>
    /// <param name="dir">-1 will rotate left, 1 will rotate right.</param>
    private void ScaleInput(int dir)
    {
        if (m_currentlyHeldDecoration == null) return;
        m_scaling = -dir;  
    }

    /// <summary>
    /// Resets scaling value when input is cancelled.
    /// </summary>
    private void ResetScaling()
    {
        m_scaling = 0f;
    }

    /// <summary>
    /// Correctly manages items position in status lists after being released.
    /// </summary>
    private void DropHeldItem()
    {
        if (m_currentlyHeldDecoration == null) return;      
        m_activeDecorations.Remove(m_currentlyHeldDecoration);
        m_barDecorations.Add(m_currentlyHeldDecoration);
        m_currentlyHeldDecoration.ReturnToBar();
        RefreshBarUI();
        m_currentlyHeldDecoration.sceneObject.transform.position = m_currentlyHeldDecoration.desiredLocation;
        m_currentlyHeldDecoration = null;
        managerStatus = ManagerStatus.EmptyHand;
    }

    /// <summary>
    /// Called whenever player inputs activate. Handles cases differently based on what player hand is closest to and if holding an object already.
    /// </summary>
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
                    PickupObject(d);
                    return;
                }
            }
            //Then check scene if none were found.
            foreach (DecorationUi d in m_activeDecorations)
            {
                if (Vector2.Distance(m_hand.transform.position, d.sceneObject.transform.position) <= m_pickupDistance)
                {
                    PickupObject(d);                    
                    return;
                }
            }
        }
        else //When holding something
        {            
            m_currentlyHeldDecoration.PlaceInScene();
            RefreshBarUI();

            managerStatus = ManagerStatus.EmptyHand;
            m_currentlyHeldDecoration = null;
        }
    }


    private void PickupObject(DecorationUi d)
    {
        m_currentlyHeldDecoration = d;
        m_barDecorations.Remove(d);
        m_activeDecorations.Add(d);
        d.PickUp();
        RefreshBarUI();
        managerStatus = ManagerStatus.Holding;
    }

    /// <summary>
    /// Starts coroutine that handles monster mood management.
    /// </summary>
    private void LockMonsterMood()
    {
        StartCoroutine(HandleMonsterMoods());
    }

    /// <summary>
    /// Locks monster to a single chosen mood for the duration of decorating.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Calls screenshot coroutine.
    /// </summary>
    public void TakePolaroid()
    {
        StartCoroutine(PolaroidSnap());
    }

    /// <summary>
    /// Takes a screenshot in an area and saves it to file. Default path will be User/AppData/LocalLow/MagicalCleaningAgency/MonstieWash/variable name chosen.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PolaroidSnap()
    {
        //Screenshot area definition
        var corners = new Vector3[4];
        screenshotArea.GetWorldCorners(corners);
        var width = (int)corners[3].x - (int)corners[0].x;
        var height = (int)corners[1].y - (int)corners[0].y;
        var startX = corners[0].x;
        var startY = corners[0].y;

        //Turn off bar and button for photo.
        this.transform.GetChild(0).gameObject.SetActive(false);
        FindFirstObjectByType<DecorationNavigate>().gameObject.SetActive(false);
        m_hand.gameObject.SetActive(false);

        //Wait for seconds and camera flash + sound here.
        camFlash.FadeIn();
        yield return new WaitForSeconds(1f);
        

        //Wait for frame to fully render and update before taking screenshot.
        yield return new WaitForEndOfFrame();

        //Apply screen view to a texture (Take screenshot)
        var tempText = new Texture2D(width, height, TextureFormat.RGB24, false);
        tempText.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        tempText.Apply();

        //Apply a modifier to end of file based on count of objects stored.
        var saveLocation = Application.persistentDataPath + "/PolaroidSaving" + savePath;
        Directory.CreateDirectory(saveLocation);
        var fileCount = Directory.GetFiles(saveLocation).Length;

        //Save to file.
        var byteArray = tempText.EncodeToPNG();
        var saveFile = $"{saveLocation}/Polaroid_{fileCount}.Png";

        File.WriteAllBytes(saveFile, byteArray);

        Debug.Log("Saved screenshot at: " + saveFile);
        Destroy(tempText);

        m_hand.gameObject.SetActive(true);
        GameSceneManager.Instance.GoToGallery();
    }

}
