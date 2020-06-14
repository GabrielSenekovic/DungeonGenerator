using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    // Update is called once per frame

    [SerializeField] Body player;

    [SerializeField] List<Sprite> headSprites; int headSpriteIndex = 0;

    [SerializeField] List<Sprite> eyeSprites; int eyeSpriteIndex = 0;

    [SerializeField] List<Sprite> torsoSprites; int torsoSpriteIndex = 0;

    [SerializeField] List<Sprite> legSprites; int legSpriteIndex = 0;

    private void Start()
    {
        player.head.sprite = headSprites[headSpriteIndex];

        player.eyes.eyes[0].sprite = eyeSprites[eyeSpriteIndex];
        player.eyes.eyes[1].sprite = eyeSprites[eyeSpriteIndex];

        player.torso.sprite = torsoSprites[torsoSpriteIndex];
        player.legs.sprite = legSprites[legSpriteIndex];
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            headSpriteIndex++; headSpriteIndex %= headSprites.Count;
            player.head.sprite = headSprites[headSpriteIndex];
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            eyeSpriteIndex++; eyeSpriteIndex %= eyeSprites.Count;
            player.eyes.eyes[0].sprite = eyeSprites[eyeSpriteIndex];
            player.eyes.eyes[1].sprite = eyeSprites[eyeSpriteIndex];
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            player.eyes.eyes[0].transform.position = new Vector2(player.eyes.eyes[0].transform.position.x - 0.0625f, player.eyes.eyes[0].transform.position.y);
            player.eyes.eyes[1].transform.position = new Vector2(player.eyes.eyes[1].transform.position.x + 0.0625f, player.eyes.eyes[1].transform.position.y);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            player.eyes.eyes[0].transform.position = new Vector2(player.eyes.eyes[0].transform.position.x + 0.0625f, player.eyes.eyes[0].transform.position.y);
            player.eyes.eyes[1].transform.position = new Vector2(player.eyes.eyes[1].transform.position.x - 0.0625f, player.eyes.eyes[1].transform.position.y);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            player.eyes.eyes[0].transform.position = new Vector2(player.eyes.eyes[0].transform.position.x, player.eyes.eyes[0].transform.position.y + 0.0625f);
            player.eyes.eyes[1].transform.position = new Vector2(player.eyes.eyes[1].transform.position.x, player.eyes.eyes[1].transform.position.y + 0.0625f);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.eyes.eyes[0].transform.position = new Vector2(player.eyes.eyes[0].transform.position.x, player.eyes.eyes[0].transform.position.y - 0.0625f);
            player.eyes.eyes[1].transform.position = new Vector2(player.eyes.eyes[1].transform.position.x, player.eyes.eyes[1].transform.position.y - 0.0625f);
        }
    }
}
