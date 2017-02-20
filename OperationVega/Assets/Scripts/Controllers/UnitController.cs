
namespace Assets.Scripts.Controllers
{
    using System.Collections.Generic;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// The unit controller class.
    /// This class handles functionality of the units click movement and actions.
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
        /// The selected object reference.
        /// This is the object that is left clicked on.
        /// </summary>
        [HideInInspector]
        public GameObject theselectedobject;

        /// <summary>
        /// The clicked object reference.
        /// This is the object that was right clicked on to perform an action.
        /// </summary>
        [HideInInspector]
        public GameObject theclickedactionobject;

        /// <summary>
        /// The unit that has been selected.
        /// </summary>
        public IUnit theUnit;

        /// <summary>
        /// The list of units selected by the drag screen.
        /// </summary>
        [HideInInspector]
        public List<GameObject> Units = new List<GameObject>();

        /// <summary>
        /// The click destination of where to send the unit.
        /// </summary>
        [HideInInspector]
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
        /// Inverts the y so the drag screen will drag accordingly.
        /// <para></para>
        /// <remarks><paramref name="y"></paramref> -The number to subtract from the screen height.</remarks>
        /// </summary>
        public static float InvertY(float y)
        {
            return Screen.height - y;
        }

        /// <summary>
        /// The check if selected function.
        /// Checks if the current game object is under the drag screen.
        /// <para></para>
        /// <remarks><paramref name="theunit"></paramref> -The object to check if it is under the drag screen.</remarks>
        /// </summary>
        public void CheckIfSelected(GameObject theunit)
        {
            if (theunit.GetComponent<Renderer>().isVisible && Input.GetMouseButtonUp(0))
            {
                Vector3 camPos = Camera.main.WorldToScreenPoint(theunit.transform.position);
                camPos.y = InvertY(camPos.y);

                if (DragScreen.Contains(camPos) & !this.Units.Contains(theunit))
                {
                    GameObject selectionsquare = theunit.transform.FindChild("SelectionHighlight").gameObject;
                    selectionsquare.GetComponent<MeshRenderer>().enabled = true;
                    selectionsquare.GetComponent<MeshRenderer>().material.color = Color.black;
                    Debug.DrawLine(theunit.transform.position, new Vector3(theunit.transform.position.x, 5.0f, theunit.transform.position.z), Color.black);
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
        /// This function is used for unit selection.
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
                    if (hit.transform.GetComponent(typeof(IUnit)))
                    {
                        this.theUnit = (IUnit)hit.transform.GetComponent(typeof(IUnit));
                        this.theselectedobject = hit.transform.gameObject;
                        GameObject selectionsquare = this.theselectedobject.transform.FindChild("SelectionHighlight").gameObject;
                        selectionsquare.GetComponent<MeshRenderer>().enabled = true;
                        selectionsquare.GetComponent<MeshRenderer>().material.color = Color.black;
                        Debug.DrawLine(this.theselectedobject.transform.position, new Vector3(this.theselectedobject.transform.position.x, 5.0f, this.theselectedobject.transform.position.z), Color.black);
                    }
                }
            }
        }

        /// <summary>
        /// The activate drag screen function.
        /// This controls the drag screen.
        /// </summary>
        private void ActivateDragScreen()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.theUnit = null;
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
        /// This clears the list of selected units and the current selected unit.
        /// </summary>
        private void ClearSelectedUnits()
        {
            if (this.Units.Count > 0)
            { 
                foreach (GameObject go in this.Units)
                {
                    GameObject selectionsquare = go.transform.FindChild("SelectionHighlight").gameObject;
                    selectionsquare.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            this.Units.Clear();
            this.theUnit = null;
            this.theclickedactionobject = null;
            if (this.theselectedobject != null)
            {
                GameObject selectionsquare = this.theselectedobject.transform.FindChild("SelectionHighlight").gameObject;
                selectionsquare.GetComponent<MeshRenderer>().enabled = false;
            }
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
                    this.theclickedactionobject = hit.transform.gameObject;
                    this.clickdestination = new Vector3(hit.point.x, 0.5f, hit.point.z);

                    if (hit.transform.GetComponent<Enemy>())
                    {
                        this.CommandToAttack(hit);
                    }
                    else if (hit.transform.GetComponent(typeof(IResources)))
                    {
                        this.CommandToHarvest(hit);
                    }
                    else if (hit.transform.tag == "PickUp")
                    {
                        this.CommandToPickUp(hit);
                    }
                    else if (hit.transform.gameObject.name == "Silo")
                    {
                        this.CommandToStock(hit);
                    }
                    else if (hit.transform.gameObject.name == "Decontamination")
                    {
                        this.CommandToDecontaminate(hit);
                    }
                    else
                    {
                        this.CommandToIdle();
                    }
                }
            }
        }

        /// <summary>
        /// The command to attack function.
        /// Sends unit(s) to attack.
        /// <para></para>
        /// <remarks><paramref name="hit"></paramref> -The object that was hit by the ray cast.</remarks>
        /// </summary>
        private void CommandToAttack(RaycastHit hit)
        {
            // single check only one clicked
            if (this.theUnit != null)
            {
                this.theUnit.SetTheMovePosition(hit.transform.position);
                this.theUnit.SetTarget(hit.transform.gameObject);
                this.theUnit.ChangeStates("Battle");
            } 
            // multiple selected
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    if (!go.GetComponent(typeof(IUnit)))
                    {
                        Debug.LogWarning(string.Format("hey, no component on {0}", go.name));
                    }
                    else
                    {
                        IUnit unit = (IUnit)go.GetComponent(typeof(IUnit));

                        unit.SetTheMovePosition(hit.transform.position);
                        unit.SetTarget(hit.transform.gameObject);
                        unit.ChangeStates("Battle");
                    }
                }
            }
        }

        /// <summary>
        /// The command to harvest function.
        /// Sends unit(s) to harvest a resource.
        /// <para></para>
        /// <remarks><paramref name="hit"></paramref> -The object that was hit by the ray cast.</remarks>
        /// </summary>
        private void CommandToHarvest(RaycastHit hit)
        {
            if (this.theUnit != null)
            {
               this.theUnit.SetTargetResource(hit.transform.gameObject);
            }
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    if (!go.GetComponent(typeof(IUnit)))
                    {
                        Debug.LogWarning(string.Format("hey, no component on {0}", go.name));
                    }
                    else
                    {
                        IUnit unit = (IUnit)go.GetComponent(typeof(IUnit));
                        unit.SetTargetResource(hit.transform.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// The command to idle function.
        /// Send unit(s) to idle.
        /// </summary>
        private void CommandToIdle()
        {
            // If there is no destination or the destination clicked is another unit..just return and dont move
            if (this.theclickedactionobject == null || this.theclickedactionobject.GetComponent(typeof(IUnit)))
            {
                return;
            }

            // Send unit back to idle
            if (this.theUnit != null)
            {
                this.theUnit.SetTarget(null);
                this.theUnit.SetTheMovePosition(this.clickdestination);
                this.theUnit.ChangeStates("Idle");
            }
            else if (this.Units.Count > 0)
            {
                for (int i = 0; i < this.Units.Count; i++)
                {
                    float angle = i * (2 * 3.14159f / this.Units.Count);
                    float x = Mathf.Cos(angle) * 1.5f;
                    float z = Mathf.Sin(angle) * 1.5f;

                    this.clickdestination = new Vector3(this.clickdestination.x + x, 0.5f, this.clickdestination.z + z);

                    if (!this.Units[i].GetComponent(typeof(IUnit)))
                    {
                        Debug.LogWarning(string.Format("hey, no component on {0}", this.Units[i].name));
                    }
                    else
                    {
                        IUnit unit = (IUnit)this.Units[i].GetComponent(typeof(IUnit));

                        unit.SetTarget(null);
                        unit.SetTheMovePosition(this.clickdestination);
                        unit.ChangeStates("Idle");
                    }
                }
            }
        }

        /// <summary>
        /// The command to pick up function.
        /// This function sends unit(s) to the pickup item.
        /// <para></para>
        /// <remarks><paramref name="hit"></paramref> -The object that was hit by the ray cast.</remarks>
        /// </summary>
        private void CommandToPickUp(RaycastHit hit)
        {
            if (this.theUnit != null)
            {
                this.theUnit.GoToPickup(hit.transform.gameObject);
            }
        }

        /// <summary>
        /// The command to stock function.
        /// Sends unit(s) to stock.
        /// <para></para>
        /// <remarks><paramref name="hit"></paramref> -The object that was hit by the ray cast.</remarks>
        /// </summary>
        private void CommandToStock(RaycastHit hit)
        {
            if (this.theUnit != null)
            {
                this.theUnit.SetTheMovePosition(this.clickdestination);
                this.theUnit.ChangeStates("Stock");
            }
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    if (!go.GetComponent(typeof(IUnit)))
                    {
                        Debug.LogWarning(string.Format("hey, no component on {0}", go.name));
                    }
                    else
                    {
                        IUnit unit = (IUnit)go.GetComponent(typeof(IUnit));

                        unit.SetTheMovePosition(this.clickdestination);
                        unit.ChangeStates("Stock");
                    }
                }
            }
        }

        /// <summary>
        /// The command to decontaminate function.
        /// Send unit(s) to decontaminate.
        /// <para></para>
        /// <remarks><paramref name="hit"></paramref> -The object that was hit by the ray cast.</remarks>
        /// </summary>
        private void CommandToDecontaminate(RaycastHit hit)
        {
            if (this.theUnit != null)
            {
                if (this.theselectedobject.transform.Find("MineralsTainted") || this.theselectedobject.transform.Find("FoodTainted"))
                {
                    this.theUnit.ChangeStates("Decontaminate");
                    Transform thedoor = hit.transform.GetChild(1);
                    Debug.Log(thedoor.name);
                    Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                    this.theUnit.SetTheMovePosition(destination);
                }
            }
            else if (this.Units.Count > 0)
            {
                foreach (GameObject go in this.Units)
                {
                    if (!go.GetComponent(typeof(IUnit)))
                    {
                        Debug.LogWarning(string.Format("hey, no component on {0}", go.name));
                    }
                    else
                    {
                        IUnit unit = (IUnit)go.GetComponent(typeof(IUnit));

                        if (go.transform.Find("MineralsTainted") || go.transform.Find("FoodTainted"))
                        {
                            unit.ChangeStates("Decontaminate");
                            Transform thedoor = hit.transform.GetChild(1);
                            Debug.Log(thedoor.name);
                            Vector3 destination = new Vector3(thedoor.position.x, 0.5f, thedoor.position.z);
                            unit.SetTheMovePosition(destination);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The On GUI function.
        /// This draws the drag screen to the screen.
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
