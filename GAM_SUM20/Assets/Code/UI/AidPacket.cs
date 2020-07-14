using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class AidPacket : MonoBehaviour
{
    [Serializable]
    public class TimeFormat
    {
        [HideInInspector]
        public int year;
        [HideInInspector]
        public int month;
        [HideInInspector]
        public int day;

        public int hours;
        public int mins;
        public int seconds;

        public TimeFormat(int h, int m, int s)
        {
            year = month = day = 0;
            hours = h; mins = m; seconds = s;
        }
        public TimeFormat(DateTime date)
        {
            year = date.Year; month = date.Month; day = date.Day;
            hours = date.Hour; mins = date.Minute; seconds = date.Second;
        }
        public TimeFormat(TimeSpan span)
        {
            day = span.Days;
            //year = day % 365;
            //month = (day - (year * 365)) % 12;
            //day = (day - (year * 365) - (month * 12));
            hours = span.Hours; mins = span.Minutes; seconds = span.Seconds;
        }
        public override string ToString()
        {
            return hours.ToString() + ":" + mins.ToString() + ":" + seconds.ToString();
        }
        public static explicit operator TimeSpan(TimeFormat time)
        {
            return new TimeSpan(time.hours, time.mins, time.seconds);
        }
        public static explicit operator DateTime(TimeFormat time)
        {
            return new DateTime(time.year, time.month, time.day, time.hours, time.mins, time.seconds);
        }
    }
    public TimeFormat dateInterval;
    private TimeSpan timeInterval;
    private DateTime lastDateUsed;
    private bool timeEnded = false;
    public TMPro.TextMeshProUGUI timerText;

    public float refreshTime = 1f;
    private float currentTime = 0f;

    public Transform watchPivot;
    public UnityEngine.UI.Button button;
    private DeckManager deckManager;
    Vector3 initScale;

    private string FILE_NAME = "AidPacketTimer";

    // Start is called before the first frame update
    void Start()
    {
        timeInterval = (TimeSpan)dateInterval;
        deckManager = FindObjectOfType<DeckManager>();
        Assert.IsTrue(deckManager != null);
        initScale = transform.localScale;

        if (!LoadTimer())
            lastDateUsed = DateTime.Now;

        timeEnded = button.interactable = false;

        DateTime now = DateTime.Now;
        TimeSpan dateDif = now.Subtract(lastDateUsed);
        TimeSpan timeLeft = timeInterval.Subtract(dateDif);
        TimeFormat timeLeftFormat = new TimeFormat(timeLeft);
        timerText.text = timeLeftFormat.ToString();
    }

    private void OnDestroy()
    {
        SaveTimer();
    }

    // Update is called once per frame
    void Update()
    {

        if (button.IsInteractable() == false)
        {
            if (timeEnded == false)
            {
                RotateWatchPivot();
                // update timer
                currentTime += Time.unscaledDeltaTime;
                if (currentTime >= refreshTime)
                {
                    currentTime = 0f;
                    DateTime now = DateTime.Now;
                    TimeSpan dateDif = now.Subtract(lastDateUsed);
                    TimeSpan timeLeft = timeInterval.Subtract(dateDif);
                    TimeFormat timeLeftFormat = new TimeFormat(timeLeft);
                    timerText.text = timeLeftFormat.ToString();
                    if (TimeSpan.Compare(timeInterval, dateDif) < 0)
                    {
                        // time passed
                        TimeFormat zeroTime = new TimeFormat(0, 0, 0);
                        timerText.text = zeroTime.ToString();

                        timeEnded = true;
                        button.interactable = true;

                        return;
                    }
                    
                }

            }
        }
        else
        {
            Pulse();
        }



    }
    void RotateWatchPivot()
    {
        watchPivot.Rotate(new Vector3(0, 0, 10));
    }
    void Pulse()
    {
        float t = Mathf.PingPong(Time.time, 0.5f);
        transform.localScale = initScale + initScale * t * 0.5f;
    }
    public void OpenAidPacket(int cardCount)
    {
        button.interactable = false;
        transform.localScale = initScale;

        CardType[] raw_deck = new CardType[cardCount];
        for (int i = 0; i < cardCount; ++i)
        {
            raw_deck[i] = (CardType)GameSettings.INSTANCE.randomizer.Next(0, (int)CardType.CardType_Count);
        }
        CardTypeCount[] deck = deckManager.CollapseDeck(raw_deck.ToList());

        StartCoroutine(RewardAnimation(deck));

    }

    IEnumerator RewardAnimation(CardTypeCount[] reward)
    {
        button.interactable = false;

        RewardFlipCard rewardCardPrefab = deckManager.rewardCardPrefab;
        Transform rewardPanel = deckManager.rewardPanel;
        yield return null;

        Vector3 initScale = rewardPanel.parent.localScale;
        rewardPanel.parent.localScale = Vector3.zero;
        rewardPanel.parent.gameObject.SetActive(true);

        List<RewardFlipCard> rewardFlipCards = new List<RewardFlipCard>();

        // add cards to reward
        for (int i = 0; i < reward.Length; ++i)
        {
            if (reward[i].count == 0)
                continue;
            RewardFlipCard card = Instantiate(rewardCardPrefab, rewardPanel) as RewardFlipCard;
            card.reward = reward[i];

            rewardFlipCards.Add(card);
        }

        int frames = (int)(0.5f / Time.deltaTime);
        for (int i = 0; i < frames; ++i)
        {
            rewardPanel.parent.localScale = Vector3.Lerp(Vector3.zero, initScale, (float)(i + 1) / frames);
            yield return null;
        }

        // wait for every card to be flipped
        while (!deckManager.IsEveryCardFliped(rewardFlipCards))
        {
            yield return new WaitForSecondsRealtime(1f);
        }
        // add reward & remove
        foreach (RewardFlipCard c in rewardFlipCards)
        {
            deckManager.AddToPool(c.reward);
            Destroy(c.gameObject);
        }

        // reverse scale
        for (int i = 0; i < frames; ++i)
        {
            rewardPanel.parent.localScale = Vector3.Lerp(initScale, Vector3.zero, (float)(i + 1) / frames);
            yield return null;
        }
        rewardPanel.parent.localScale = initScale;
        rewardPanel.parent.gameObject.SetActive(false);

        timeEnded = false;
        lastDateUsed = DateTime.Now;
    }

    bool LoadTimer()
    {
        string path = Application.persistentDataPath + "/" + FILE_NAME;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            TimeFormat data = formatter.Deserialize(stream) as TimeFormat;
            Assert.IsTrue(data != null);
            lastDateUsed = (DateTime)data;

            stream.Close();
            return true;
        }
        return false;

    }

    void SaveTimer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + FILE_NAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        TimeFormat data = new TimeFormat(lastDateUsed);
        formatter.Serialize(stream, data);
        stream.Close();
    }

}
