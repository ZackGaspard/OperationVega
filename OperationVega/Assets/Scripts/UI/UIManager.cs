using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;



namespace UI
{
    public class UIManager : MonoBehaviour
    {
        #region -- VARIABLES --
        private Button m_NewGame;
        private Button m_LoadGame;
        private Button m_QuitGame;
        private Button m_Instructions;
        private Button m_Options;
        #endregion

        #region -- VOID FUNCTIONS --
        void OnRallyClick()
        {
            //Function will be use to rally upon click.
        }
        void OnHarvestClick()
        {
            //Function will be use to harvest upon click.
        }
        void OnRecallClick()
        {
            //Function will be use to recall upon click.
        }
        void OnCancelActionClick()
        {
            //Function will cancel previous action upon click.
        }
        void OnQuitGameClick()
        {
            //Function will quit game upon click.
        }
        #endregion
    }
}
