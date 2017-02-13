
namespace Assets.Scripts.Controllers
{
    using System.Collections.Generic;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// The unit controller class.
    /// This class handles functionality of the units click movement
    /// </summary>
    public class UnitController : MonoBehaviour
    {
        /// <summary>
        /// The instance of the class.
        /// </summary>
        private static UnitController instance;

        /// <summary>
        /// The drag screen.
        /// </summary>
        private static Rect dragscreen = new Rect(0, 0, 0, 0);

        /// <summary>
        /// The selection highlight is the texture.
        /// </summary>
        [SerializeField]
        private Texture2D selectionHighlight;

        /// <summary>
        /// The start click reference.
        /// </summary>
        private Vector3 startclick = -Vector3.one;

        public GameObject theselectedobject;

        /// <summary>
        /// The the clicked object reference.
        /// </summary>
        public GameObject theclickedactionobject;

        /// <summary>
        /// The unit that has been selected.
        /// </summary>
        [SerializeField]
        public IGather theUnit;

        /// <summary>
        /// The list of units selected by drag screen.
        /// </summary>
        [SerializeField]
        public List<GameObject> Units = new List<GameObject>();

        /// <summary>
        /// The click destination of where to send the unit.
        /// </summary>
        public Vector3 clickdestination;

        /// <summary>
        /// Gets the instance of the UnitController.
        /// </summary>
        public static UnitController Self
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets or sets the drag screen.
        /// </summary>
        public static Rect DragScreen
        {
            get
            {
                return dragscreen;
            }

            set
            {
                dragscreen = value;
            }
        }

        /// <summary>
        /// The invert y function.
        /// Inverts the y.
        /// </summary>
        /// <param name="y">
        /// The y value to pass in.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float InvertY(float y)
        {
            return Screen.height - y;
        }

        ///// <summary>
        ///// The spawn unit function.
        ///// This function spawns the passed in unit
        ///// </summary>
        ///// <param name="theUnit">
        ///// The Unit to spawn.
        ///// </param>
        //public void SpawnUnit(IUnit theUnit)
        //{
        //    GameObject player = Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
        //}

        /// <summary>
        /// The check if selected function.
        /// Checks if the current game object is under the drag screen
        /// </summary>
        /// <param name="theunit">
        /// The unit to check if its under the drag screen.
        /// </param>
        public void CheckIfSelected(GameObject theunit)
        {
            if (theunit.GetComponent<Renderer>().isVisible && Input.GetMouseButtonUp(0))
            {
                Vector3 camPos = Camera.main.WorldToScreenPoint(theunit.transform.position);
                camPos.y = InvertY(camPos.y);

                if (DragScreen.Contains(camPos) & !this.Units.Contains(theunit))
                {
                    this.Units.Add(theunit);
                }

            }
        }

        /// <summary>
        /// The start function.
        /// </summary>
        private void Start()
        {
            instance = this;
        }

        /// <summary>
        /// The update function.
        /// </summary>
        private void Update()
        {
            this.ActivateDragScreen();
            this.SelectUnits();
            this.CommandUnits();
        }

        /// <summary>
        /// The select units function.
        /// This function is used for unit selection
        /// </summary>
        private void SelectUnits()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                this.ClearSelectedUnits();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    if (hit.transform.GetComponent(typeof(IGather)))
                    {
                        this.theUnit = (IGather)hit.transform.GetComponent(typeof(IGather));
                        this.theselectedobject = hit.transform.gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// The activate drag screen function.
        /// This controls the drag screen
        /// </summary>
        private void ActivateDragScreen()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.theUnit = null;
                this.Units.Clear();
                this.startclick = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (dragscreen.width < 0)
                {
                    dragscreen.x += dragscreen.width;
                    dragscreen.width = -dragscreen.width;
                }
                if (dragscreen.height < 0)
                {
                    dragscreen.y += dragscreen.height;
                    dragscreen.height = -dragscreen.height;
                }

                this.startclick = -Vector3.one;
            }

            if (Input.GetMouseButton(0))
            {
                dragscreen = new Rect(
                    this.startclick.x,
                    InvertY(this.startclick.y),
                    Input.mousePosition.x - this.startclick.x,
                    InvertY(Input.mousePosition.y) - InvertY(this.startclick.y));
            }
        }

        /// <summary>
        /// The clear selected units function.
        /// This clears the list of selected units and the current selected unit
        /// </summary>
        private void ClearSelectedUnits()
        {
            this.Units.Clear();
            this.theUnit = null;
            this.theclickedactionobject = null;
        }

        /// <summary>
        /// The command units function.
        /// Gives the units proper commands.
        /// </summary>
        private void CommandUnits()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    this.CheckTheClickedObject(hit);

                    if (hit.transform.GetComponent<Enemy>())
                    {
                        this.CommandToAttack(hit);
                    }
                    else if (hit.transform.GetComponent(typeof(IResources)))
                    {
                        this.CommandToHarvest(hit);
                    }
                    else if (hit.transform.gameObject.name == "Silo")
                    {
                        this.CommandToStock();
                    }
                    else if (hit.transform.gameObject.name == "Decontamination")
                    {
                        this.CommandToDecontaminate(hit);
                    }
                    else
                    {
                        this.CommandToIdle();
                    }

                    this.CommandToMove();
                }
            }
        }

        /// <summary>
        /// The command to attack function.
        /// Sends units to attack.
        /// </summary>
        /// <param name="hit">
        /// The hit object.
        /// </param>
        private void CommandToAttack(RaycastHit hit)
        {
                if (this.theUnit != null)
                {
                    // Attack with a single unit
                    switch (this.theUnit.GetType().ToString())
                    {
                        case "Assets.Scripts.Extractor":
                            Extractor extractor = this.theUnit as Extractor;
                            extractor.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                            extractor.theEnemy = hit.transform.gameObject;
                            extractor.SetTheTargetPosition(hit.transform.position);
                            extractor.ChangeStates("Battle");
                            break;
                        case "Assets.Scripts.Miner":
                            Miner miner = this.theUnit as Miner;
                            miner.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                            miner.theEnemy = hit.transform.gameObject;
                            miner.SetTheTargetPosition(hit.transform.position);
                            miner.ChangeStates("Battle");
                            break;
                        case "Assets.Scripts.Harvester":
                            Harvester harvester = this.theUnit as Harvester;
                            harvester.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                            harvester.theEnemy = hit.transform.gameObject;
                            harvester.SetTheTargetPosition(hit.transform.position);
                            harvester.ChangeStates("Battle");
                            break;
                   }
                } 
                else if (this.Units.Count > 0)
                {
                    foreach (GameObject go in this.Units)
                    {
                        IGather unit = (IGather)go.GetComponent(typeof(IGather));

                        switch (unit.GetType().ToString())
                        {
                            case "Assets.Scripts.Extractor":
                                Extractor extractor = unit as Extractor;
                                extractor.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                                extractor.theEnemy = hit.transform.gameObject;
                                extractor.SetTheTargetPosition(hit.transform.position);
                                extractor.ChangeStates("Battle");
                                break;
                            case "Assets.Scripts.Miner":
                                Miner miner = unit as Miner;
                                miner.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                                miner.theEnemy = hit.transform.gameObject;
                                miner.SetTheTargetPosition(hit.transform.position);
                                miner.ChangeStates("Battle");
                                break;
                            case "Assets.Scripts.Harvester":
                                Harvester harvester = unit as Harvester;
                                harvester.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                                harvester.theEnemy = hit.transform.gameObject;
                                harvester.SetTheTargetPosition(hit.transform.position);
                                harvester.ChangeStates("Battle");
                                break;
                       }
                    }
                }
        }

        /// <summary>
        /// The command to harvest function.
        /// Sends units to harvest.
        /// </summary>
        /// <param name="hit">
        /// The hit object.
        /// </param>
        private void CommandToHarvest(RaycastHit hit)
        {
                if (this.theUnit != null)
                {
                    // Harvest with a single unit
                    switch (this.theUnit.GetType().ToString())
                    {
                        case "Assets.Scripts.Extractor":
                            Extractor extractor = this.theUnit as Extractor;
                            extractor.TargetResource = hit.transform.GetComponent<Gas>();
                            if (extractor.TargetResource != null)
                            {
                                extractor.theRecentGeyser = hit.transform.gameObject;
                                extractor.SetTheTargetPosition(hit.transform.position);
                                extractor.ChangeStates("Harvest");
                            }
                            break;
                        case "Assets.Scripts.Miner":
                            Miner miner = this.theUnit as Miner;
                            miner.TargetResource = hit.transform.GetComponent<Minerals>();
                            if (miner.TargetResource != null)
                            {
                                miner.theRecentMineralDeposit = hit.transform.gameObject;
                                miner.SetTheTargetPosition(hit.transform.position);
                                miner.ChangeStates("Harvest");
                            }
                            break;
                        case "Assets.Scripts.Harvester":
                            Harvester harvester = this.theUnit as Harvester;
                            harvester.TargetResource = hit.transform.GetComponent<Food>();
                            if (harvester.TargetResource != null)
                            {
                                harvester.theRecentTree = hit.transform.gameObject;
                                harvester.SetTheTargetPosition(hit.transform.position);
                                harvester.ChangeStates("Harvest");
                            }
                            break;
                    }
                }
                else if (this.Units.Count > 0)
                {
                    foreach (GameObject go in this.Units)
                    {
                        IGather unit = (IGather)go.GetComponent(typeof(IGather));

                        switch (unit.GetType().ToString())
                        {
                            case "Assets.Scripts.Extractor":
                                Extractor extractor = unit as Extractor;
                                extractor.TargetResource = hit.transform.GetComponent<Gas>();
                                if (extractor.TargetResource != null)
                                {
                                    extractor.theRecentGeyser = hit.transform.gameObject;
                                    extractor.SetTheTargetPosition(hit.transform.position);
                                    extractor.ChangeStates("Harvest");
                                }
                                break;
                            case "Assets.Scripts.Miner":
                                Miner miner = unit as Miner;
                                miner.TargetResource = hit.transform.GetComponent<Minerals>();
                                if (miner.TargetResource != null)
                                {
                                    miner.theRecentMineralDeposit = hit.transform.gameObject;
                                    miner.SetTheTargetPosition(hit.transform.position);
                                    miner.ChangeStates("Harvest");
                                }
                                break;
                            case "Assets.Scripts.Harvester":
                                Harvester harvester = unit as Harvester;
                                harvester.TargetResource = hit.transform.GetComponent<Food>();
                                if (harvester.TargetResource != null)
                                {
                                    harvester.theRecentTree = hit.transform.gameObject;
                                    harvester.SetTheTargetPosition(hit.transform.position);
                                    harvester.ChangeStates("Harvest");
                                }
                                break;
                        }
                    }
                }
        }

        /// <summary>
        /// The command to move function.
        /// Sends units to appropriate destinations.
        /// </summary>
        private void CommandToMove()
        {
            IResources theResource = (IResources)this.theclickedactionobject.GetComponent(typeof(IResources));

            if (this.Units.Count > 0)
            {
                if (theResource != null)
                {
                    foreach (GameObject p in this.Units)
                    {
                        IGather g = (IGather)p.GetComponent(typeof(IGather));
                        this.UnitToResource(theResource, g);
                    }
                }
                else
                {
                    // Circle formation
                    for (int i = 0; i < this.Units.Count; i++)
                    {
                        float angle = i * (2 * 3.14159f / this.Units.Count);
                        float x = Mathf.Cos(angle) * 1.5f;
                        float z = Mathf.Sin(angle) * 1.5f;

                        IGather g = (IGather)this.Units[i].GetComponent(typeof(IGather));
                        this.clickdestination = new Vector3(this.clickdestination.x + x, 0.5f, this.clickdestination.z + z);
                        g.SetTheTargetPosition(this.clickdestination);
                    }
                }
            }
        }

        /// <summary>
        /// The command to idle function.
        /// Send units to idle.
        /// </summary>
        private void CommandToIdle()
        {
            // If there is no destination or the destination clicked is another unit..just return and dont move
            if (this.theclickedactionobject == null || this.theclickedactionobject.GetComponent(typeof(IGather)))
            {
                return;
            }

            // Send unit back to idle
            if (this.theUnit != null)
            {
                switch (this.theUnit.GetType().ToString())
                {
                    case "Assets.Scripts.Extractor":
                        Extractor extractor = this.theUnit as Extractor;
                        extractor.Target = null;
                        extractor.theEnemy = null;
                        extractor.SetTheTargetPosition(this.clickdestination);
                        extractor.ChangeStates("Idle");
                        break;
                    case "Assets.Scripts.Miner":
                        Miner miner = this.theUnit as Miner;
                        miner.Target = null;
                        miner.theEnemy = null;
                        miner.SetTheTargetPosition(this.clickdestination);
                        miner.ChangeStates("Idle");
                        break;
                    case "Assets.Scripts.Harvester":
                        Harvester harvester = this.theUnit as Harvester;
                        harvester.Target = null;
                        harvester.theEnemy = null;
                        harvester.SetTheTargetPosition(this.clickdestination);
                        harvester.ChangeStates("Idle");
                        break;
                }
            }
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    IGather unit = (IGather)go.GetComponent(typeof(IGather));

                    switch (unit.GetType().ToString())
                    {
                        case "Assets.Scripts.Extractor":
                            Extractor extractor = unit as Extractor;
                            extractor.Target = null;
                            extractor.theEnemy = null;
                            extractor.ChangeStates("Idle");
                            break;
                        case "Assets.Scripts.Miner":
                            Miner miner = unit as Miner;
                            miner.Target = null;
                            miner.theEnemy = null;
                            miner.ChangeStates("Idle");
                            break;
                        case "Assets.Scripts.Harvester":
                            Harvester harvester = unit as Harvester;
                            harvester.Target = null;
                            harvester.theEnemy = null;
                            harvester.ChangeStates("Idle");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The command to stock function.
        /// Sends units to stock.
        /// </summary>
        private void CommandToStock()
        {
            if (this.theUnit != null)
            {
                // Stock with a single unit
                switch (this.theUnit.GetType().ToString())
                {
                    case "Assets.Scripts.Extractor":
                        Extractor extractor = this.theUnit as Extractor;
                        if (extractor.Resourcecount > 0)
                        {
                            extractor.SetTheTargetPosition(this.clickdestination);
                            extractor.ChangeStates("Stock");
                        }
                        break;
                    case "Assets.Scripts.Miner":
                        Miner miner = this.theUnit as Miner;
                        if (miner.Resourcecount > 0)
                        {
                            miner.SetTheTargetPosition(this.clickdestination);
                            miner.ChangeStates("Stock");
                        }
                        break;
                    case "Assets.Scripts.Harvester":
                        Harvester harvester = this.theUnit as Harvester;
                        if (harvester.Resourcecount > 0)
                        {
                            harvester.SetTheTargetPosition(this.clickdestination);
                            harvester.ChangeStates("Stock");
                        }
                        break;
                }
            }
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    IGather unit = (IGather)go.GetComponent(typeof(IGather));

                    switch (unit.GetType().ToString())
                    {
                        case "Assets.Scripts.Extractor":
                            Extractor extractor = unit as Extractor;
                            if (extractor.Resourcecount > 0)
                            {
                                extractor.SetTheTargetPosition(this.clickdestination);
                                extractor.ChangeStates("Stock");
                            }
                            break;
                        case "Assets.Scripts.Miner":
                            Miner miner = unit as Miner;
                            if (miner.Resourcecount > 0)
                            {
                                miner.SetTheTargetPosition(this.clickdestination);
                                miner.ChangeStates("Stock");
                            }
                            break;
                        case "Assets.Scripts.Harvester":
                            Harvester harvester = unit as Harvester;
                            if (harvester.Resourcecount > 0)
                            {
                                harvester.SetTheTargetPosition(this.clickdestination);
                                harvester.ChangeStates("Stock");
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The command to decontaminate function.
        /// Send units to decontaminate.
        /// </summary>
        private void CommandToDecontaminate(RaycastHit hit)
        {
            if (this.theUnit != null)
            {
                // Stock with a single unit
                switch (this.theUnit.GetType().ToString())
                {
                    case "Assets.Scripts.Miner":
                        Miner miner = this.theUnit as Miner;
                        Debug.Log("Decontamination clicked for miner");
                        if (miner.transform.GetChild(0).name == "MineralsTaint")
                        {
                            miner.ChangeStates("Decontaminate");
                            Transform thedoor = hit.transform.GetChild(1);
                            Debug.Log(thedoor.name);
                            Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                            miner.SetTheTargetPosition(destination);
                        }
                        break;
                    case "Assets.Scripts.Harvester":
                        Harvester harvester = this.theUnit as Harvester;
                        if (harvester.transform.GetChild(0).name == "PickupFoodTaint")
                        {
                            harvester.ChangeStates("Decontaminate");
                            Transform thedoor = hit.transform.GetChild(1);
                            Debug.Log(thedoor.name);
                            Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                            harvester.SetTheTargetPosition(destination);
                        }
                        break;
                }
            }
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    IGather unit = (IGather)go.GetComponent(typeof(IGather));

                    switch (unit.GetType().ToString())
                    {
                        case "Assets.Scripts.Miner":
                            Miner miner = unit as Miner;
                            if (miner.transform.GetChild(0).name == "MineralsTaint")
                            {
                                miner.ChangeStates("Decontaminate");
                                Transform thedoor = hit.transform.GetChild(1);
                                Debug.Log(thedoor.name);
                                Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                                miner.SetTheTargetPosition(destination);
                            }
                            break;
                        case "Assets.Scripts.Harvester":
                            Harvester harvester = unit as Harvester;
                            if (harvester.transform.GetChild(0).name == "PickupFoodTaint")
                            {
                                harvester.ChangeStates("Decontaminate");
                                Transform thedoor = hit.transform.GetChild(1);
                                Debug.Log(thedoor.name);
                                Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                                harvester.SetTheTargetPosition(destination);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The check clicked object function.
        /// This determines the type of object clicked.
        /// </summary>
        /// <param name="hit">
        /// The object hit by the ray cast.
        /// </param>
        private void CheckTheClickedObject(RaycastHit hit)
        {
            this.theclickedactionobject = hit.transform.gameObject;

            if (hit.transform.GetComponent<Enemy>() || hit.transform.GetComponent(typeof(IResources)))
            {
                this.clickdestination = new Vector3(
                    this.theclickedactionobject.transform.position.x,
                    0.5f,
                    this.theclickedactionobject.transform.position.z);
            }
            else
            {
                this.clickdestination = new Vector3(hit.point.x, 0.5f, hit.point.z);
            }
        }

        /// <summary>
        /// The check unit type function.
        /// This function allows movement but restricts movement of the unit going to the wrong resource.
        /// </summary>
        /// <param name="theResource">
        /// The the Resource.
        /// </param>
        /// <param name="gatheringunit">
        /// The gathering unit.
        /// </param>
        private void UnitToResource(IResources theResource, IGather gatheringunit)
        {
                switch (gatheringunit.GetType().ToString())
                {
                    case "Assets.Scripts.Extractor":
                        Gas thegas = theResource as Gas;
                        if (thegas != null)
                        {
                            gatheringunit.SetTheTargetPosition(this.clickdestination);
                        }
                        break;
                    case "Assets.Scripts.Miner":
                        Minerals themineral = theResource as Minerals;
                        if (themineral != null)
                        {
                            gatheringunit.SetTheTargetPosition(this.clickdestination);
                        }
                        break;
                    case "Assets.Scripts.Harvester":
                        Food thefood = theResource as Food;
                        if (thefood != null)
                        {
                            gatheringunit.SetTheTargetPosition(this.clickdestination);
                        }
                        break;
                }
        }

        /// <summary>
        /// The On GUI function.
        /// This draws the drag screen to the screen
        /// </summary>
        private void OnGUI()
        {
            if (this.startclick != -Vector3.one)
            {
                GUI.color = new Color(1, 1, 1, 0.5f);
                GUI.DrawTexture(dragscreen, this.selectionHighlight);
            }
        }
    }
}
