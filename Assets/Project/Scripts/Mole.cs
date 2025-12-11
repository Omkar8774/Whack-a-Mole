using UnityEngine;
using UnityEngine.UI;

public class Mole : MonoBehaviour
{
    public int moleIndex;
    private GameManager gm;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            gm.OnMoleTapped(moleIndex);
        });
    }
}
