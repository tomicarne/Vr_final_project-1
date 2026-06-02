using UnityEngine;
public class ManagerPaginas : MonoBehaviour
{
        public int paginaCount = 0;
        public GameObject textoPuzzle;

        void Update()
        {
                if(paginaCount == 5)
                {
                        textoPuzzle.SetActive(true);
                        gameObject.SetActive(false);
                }   
        }
        
}
