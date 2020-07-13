using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(CardImage))]
public class RewardFlipCard : MonoBehaviour
{
    public EventTrigger eventTrigger;
    public CardImage cardImage;
    public TextMeshProUGUI count_text; // how many cards of this type in pool

    public CardTypeCount reward;

    private bool has_fliped = false;

    // Start is called before the first frame update
    void Start()
    {
        // start fliped
        cardImage.image.uvRect = new Rect(0, 0, -1, 1);
        //transform.localEulerAngles = new Vector3(0, -180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PointerDown()
    {
        StartCoroutine(RevealReward());
    }

    public bool HasFliped()
    {
        return has_fliped;
    }

    IEnumerator RevealReward()
    {
        // deactivate trigger event
        eventTrigger.enabled = false;
        cardImage.image.uvRect = new Rect(0, 0, 1, 1);

        int frames = (int)(1f / Time.deltaTime);

        Vector3 angle = new Vector3(0, 180, 0);
        Vector3 angleStep = angle / frames;
        transform.localEulerAngles = -angle;

        for (int i = 0; i < frames / 2; ++i)
        {
            transform.localEulerAngles += angleStep;
            yield return new WaitForEndOfFrame();
        }

        // set type now
        cardImage.type = reward.type;
        count_text.transform.parent.gameObject.SetActive(true);
        count_text.text = reward.count.ToString();

        for (int i = 0; i < frames / 2; ++i)
        {
            transform.localEulerAngles += angleStep;
            yield return new WaitForEndOfFrame();
        }


        yield return new WaitForSecondsRealtime(2f);

        count_text.transform.parent.gameObject.SetActive(false);
        cardImage.image.enabled = false;
        has_fliped = true;
    }
}
