
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

        /// <summary>
        /// The the clicked object reference.
        /// </summary>
        public GameObject theclickedobject;

        /// <summary>
        /// The unit that has been selected.
        /// </summary>
        [SerializeField]
        private IGather theUnit;

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
            this.theclickedobject = null;
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
                    this.theclickedobject = hit.transform.gameObject;
                    this.clickdestination = new Vector3(hit.point.x, 0.5f, hit.point.z);

                    this.CommandToAttack(hit);
                    this.CommandToMove();
                }
            }
        }

        /// <summary>
        /// The to attack function.
        /// </summary>
        /// <param name="hit">
        /// The hit.
        /// </param>
        private void CommandToAttack(RaycastHit hit)
        {
            if (hit.transform.GetComponent<Enemy>())
            {
                if (this.theUnit != null)
                {
                    // Attack with a single unit
                    switch (this.theUnit.GetType().ToString())
                    {
                        case "Assets.Scripts.Extractor":
                            Extractor extractor = this.theUnit as Extractor;
                            extractor.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                            extractor.TheExtractorFsm.Feed("IdleToBattle");
                            break;
                        case "Assets.Scripts.Miner":
                            Miner miner = this.theUnit as Miner;
                            miner.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                            miner.TheMinerFsm.Feed("IdleToBattle");
                            break;
                    }
                }

                // Attack with multiple units
                if (this.Units.Count > 0)
                {
                    foreach (GameObject go in this.Units)
                    {
                        IGather unit = (IGather)go.GetComponent(typeof(IGather));

                        switch (unit.GetType().ToString())
                        {
                            case "Assets.Scripts.Extractor":
                                Extractor extractor = unit as Extractor;
                                extractor.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                                extractor.TheExtractorFsm.Feed("IdleToBattle");
                                break;
                            case "Assets.Scripts.Miner":
                                Miner miner = unit as Miner;
                                miner.Target = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
                                miner.TheMinerFsm.Feed("IdleToBattle");
                                break;
                        }
                    }
                }
            }
            else
            {
                this.CommandToIdle();
            }
        }

        /// <summary>
        /// The command to move function.
        /// </summary>
        private void CommandToMove()
        {
            if (this.theUnit != null & this.theclickedobject != null)
            {
                this.theUnit.SetTheTargetPosition(this.clickdestination);
            }

            if (this.Units.Count > 0)
            {
                foreach (GameObject p in this.Units)
                {
                    IGather g = (IGather)p.GetComponent(typeof(IGather));
                    g.SetTheTargetPosition(this.clickdestination);
                }
            }
        }

        /// <summary>
        /// The command to idle function.
        /// </summary>
        private void CommandToIdle()
        {
            // Send unit back to idle
            if (this.theUnit != null)
            {
                switch (this.theUnit.GetType().ToString())
                {
                    case "Assets.Scripts.Extractor":
                        Extractor extractor = this.theUnit as Extractor;
                        extractor.Target = null;
                        extractor.TheExtractorFsm.Feed("BattleToIdle");
                        break;
                    case "Assets.Scripts.Miner":
                        Miner miner = this.theUnit as Miner;
                        miner.Target = null;
                        miner.TheMinerFsm.Feed("BattleToIdle");
                        break;
                }
            }

            // Send multiple units back to idle
            if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    IGather unit = (IGather)go.GetComponent(typeof(IGather));

                    switch (unit.GetType().ToString())
                    {
                        case "Assets.Scripts.Extractor":
                            Extractor extractor = unit as Extractor;
                            extractor.Target = null;
                            extractor.TheExtractorFsm.Feed("BattleToIdle");
                            break;
                        case "Assets.Scripts.Miner":
                            Miner miner = unit as Miner;
                            miner.Target = null;
                            miner.TheMinerFsm.Feed("BattleToIdle");
                            break;
                    }
                }
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
