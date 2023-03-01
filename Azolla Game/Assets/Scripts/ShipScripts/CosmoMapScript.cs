using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CosmoMapScript : MonoBehaviour
{
    [SerializeField]
    private Button closeButt;

    [SerializeField]
    private Button goButt;

    // Start is called before the first frame update
    void Start()
    {
        // Set Up Button Click Events
        Button cButt = closeButt.GetComponent<Button>();
        cButt.onClick.AddListener(cButtPress);

        Button gButt = goButt.GetComponent<Button>();
        gButt.onClick.AddListener(gButtPress);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void cButtPress()
    {
        this.gameObject.SetActive(false);
    }

    void gButtPress()
    {
        SceneManager.LoadScene(1);
    }

}
