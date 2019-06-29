using UnityEngine;
using UnityEngine.EventSystems;

namespace Luminosity.IO.Examples
{
	public class MainMenu : MonoBehaviour 
	{
        public static MainMenu mainMenu;


		[SerializeField]
		private MenuPage m_startPage = null;
		[SerializeField]
		private MenuPage[] m_pages = null;

		private MenuPage m_currentPage;

		private void Start()
		{
            if (mainMenu == null) {
                mainMenu = this;
            }

            ChangePage(m_startPage.ID);
		}

		public void ChangePage(string id)
		{
            //if (m_currentPage != null) {
            //    m_currentPage.gameObject.SetActive(false);
            //}

            m_currentPage = FindPage(id);
			if(m_currentPage != null)
			{
                if (gameObject.name == "Canvas1" || gameObject.name == "Canvas2" || gameObject.name == "Canvas3")
                    m_currentPage.gameObject.SetActive(false);
                else
                    m_currentPage.gameObject.SetActive(true);

                EventSystem.current.SetSelectedGameObject(m_currentPage.FirstSelected);
			}
		}

        public void GameOverUI() {
            m_currentPage = FindPage("Over");

            if (m_currentPage != null) {
                GameManager.Instance.OnResumePressed();
                m_currentPage.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(m_currentPage.FirstSelected);
            }
        }

		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		private MenuPage FindPage(string id)
		{
			foreach(MenuPage page in m_pages)
			{
				if(page.ID == id)
					return page;
			}

			Debug.LogError("Unable to find menu page with id: " + id);
			return null;
		}
	}
}
