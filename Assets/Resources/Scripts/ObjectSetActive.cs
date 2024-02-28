using UnityEngine;

public class ObjectSetActive : MonoBehaviour
{
    [SerializeField] private GameControl gcontrol;
    public void ObjectDisable()
    {
        if (gameObject.name == "AP")
        {
            for (int i = 1; i <= 10; i++)
            {
                GameObject areaArrow = GameObject.Find("Area Arrow/" + i);
                if (areaArrow == null || !areaArrow.activeSelf)
                {
                    continue;
                }
                else
                {
                    areaArrow.SetActive(false);
                    break;
                }
            }

        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void ObjectAnable()
    {
        gameObject.SetActive(true);
    }

    public void LevelObjectDisable()
    {
        GameObject area = GameObject.Find("Props/Level Lock/Level " + gcontrol.arealevels);
        area.SetActive(false);
    }

}
