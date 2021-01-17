using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public GameObject character, regular, jump, drill;
    private int activeMode;
    
    // Start is called before the first frame update
    void Start()
    {
        activeMode = 0;

        regular.SetActive(true);
        jump.SetActive(false);
        drill.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    activeMode = (activeMode + 1) % 3;
        //    SwitchMode();
        //}
        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    RotateCharacter();
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    TranslateCharacter();
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    ScaleCharacter();
        //}
    }

    public void SwitchMode()
    {
        switch(activeMode)
        {
            case 0:
                regular.SetActive(true);
                jump.SetActive(false);
                drill.SetActive(false);
                break;
            case 1:
                regular.SetActive(false);
                jump.SetActive(true);
                drill.SetActive(false);
                break;
            case 2:
                regular.SetActive(false);
                jump.SetActive(false);
                drill.SetActive(true);
                break;
        }
    }

    private void RotateCharacter()
    {
        switch (activeMode)
        {
            case 0:
                character.transform.Rotate(character.transform.rotation.x + 15f, character.transform.rotation.y, character.transform.rotation.z);
                break;
            case 1:
                character.transform.Rotate(character.transform.rotation.x, character.transform.rotation.y + 15f, character.transform.rotation.z); ;
                break;
            case 2:
                character.transform.Rotate(character.transform.rotation.x, character.transform.rotation.y, character.transform.rotation.z + 15f);
                break;
        }
    }

    private void TranslateCharacter()
    {
        switch (activeMode)
        {
            case 0:
                character.transform.position = new Vector3(character.transform.position.x + 0.15f, character.transform.position.y, character.transform.position.z);
                break;
            case 1:
                character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y + 0.15f, character.transform.position.z); ;
                break;
            case 2:
                character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z + 0.15f);
                break;
        }
    }
    private void ScaleCharacter()
    {
        switch (activeMode)
        {
            case 0:
                character.transform.localScale = new Vector3(character.transform.localScale.x + 0.15f, character.transform.localScale.y, character.transform.localScale.z);
                break;
            case 1:
                character.transform.localScale = new Vector3(character.transform.localScale.x, character.transform.localScale.y + 0.15f, character.transform.localScale.z); ;
                break;
            case 2:
                character.transform.localScale = new Vector3(character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z + 0.15f);
                break;
        }
    }
}
