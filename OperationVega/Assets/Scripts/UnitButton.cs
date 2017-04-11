
namespace Assets.Scripts
{
    using UI;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// The unit button class.
    /// Handles functionality of the UnitButtons.
    /// </summary>
    public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// The unit reference.
        /// Reference to the specific units data to be tied into this button.
        /// </summary>
        [HideInInspector]
        public GameObject Unit;

        /// <summary>
        /// The on pointer enter function.
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.Unit == null)
            {
                Destroy(this.gameObject);
                return;
            }

            Color thecolor = new Color();

            GameObject selectionsquare = this.Unit.transform.FindChild("SelectionHighlight").gameObject;
            if (this.Unit.GetComponent<Miner>())
            {
                thecolor = Color.red;
            }
            else if (this.Unit.GetComponent<Harvester>())
            {
                thecolor = Color.green;
            }
            else if (this.Unit.GetComponent<Extractor>())
            {
                thecolor = Color.blue;
            }
            selectionsquare.GetComponent<MeshRenderer>().material.color = thecolor;
            UIManager.Self.unit = this.Unit;
            UIManager.Self.UpdateStatsPanel(UIManager.Self.tooltippanel);
            UIManager.Self.tooltippanel.SetActive(true);
        }

        /// <summary>
        /// The on pointer exit function.
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnPointerExit(PointerEventData eventData)
        {
            GameObject selectionsquare = this.Unit.transform.FindChild("SelectionHighlight").gameObject;
            selectionsquare.GetComponent<MeshRenderer>().material.color = Color.black;
            UIManager.Self.tooltippanel.SetActive(false);
        }

        /// <summary>
        /// The on pointer click function.
        /// This will open the upgrade panel. Allowing for Upgrades.
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Pause the game - to be implemented here
            UIManager.Self.upgradepanel.SetActive(true);
            UIManager.Self.UpdateStatsPanel(UIManager.Self.upgradepanel);
            UIManager.Self.unit = this.Unit;
        }
    }
}
