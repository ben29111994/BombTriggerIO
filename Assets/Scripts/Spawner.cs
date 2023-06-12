using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    public GameObject player;
    public EZObjectPool botSpawner;
    public EZObjectPool bombSpawner;
    public EZObjectPool dotSpawner;
    public List<Vector3> origin = new List<Vector3>();
    public List<Vector3> bombSpawnPoints = new List<Vector3>();
    public int[] checkExist;
    public List<GameObject> bombList = new List<GameObject>();
    int bombId = 0;
    public List<GameObject> leaderboardList = new List<GameObject>();
    public List<GameObject> rankList = new List<GameObject>();
    public Text rank1Text;
    public Text rank2Text;
    public Text rank3Text;
    public Text currentRankText;
    public List<GameObject> skinList = new List<GameObject>();
    public List<Sprite> flagList = new List<Sprite>();
    float mapLimit = 30;
    public Color[] colorList;
    public Color dotColor;
    public GameObject crown;
    public GameObject zones;
    public int zone1, zone2, zone3, zone4;

    void OnEnable()
    {
        instance = this;
        var randomColor = Random.Range(0, colorList.Length);
        dotColor = colorList[randomColor];

        bombSpawnPoints = origin.OrderBy(x => Random.value).ToList();
        checkExist = new int[bombSpawnPoints.Count];
        skinListUpdate(GameController.instance.randomSkin);
        for (int i = 0; i < botSpawner.ObjectList.Count; i++)
        {
            leaderboardList.Add(botSpawner.ObjectList[i]);
            var randomSkin = Random.Range(0, skinList.Count);
            var skin = Instantiate(skinList[randomSkin]);
            skin.transform.parent = botSpawner.ObjectList[i].transform;
            skin.transform.localPosition = Vector3.zero;
            skinList.RemoveAt(randomSkin);
            skinList.TrimExcess();
            var randomFlag = Random.Range(0, flagList.Count);
            botSpawner.ObjectList[i].GetComponent<Bot>().botFlag.sprite = flagList[randomFlag];
            GameObject bot;
            botSpawner.TryGetNextObject(GameController.instance.spawnPoints[i], Quaternion.Euler(new Vector3(0, 0, 0)), out bot);
            bot.transform.localScale = new Vector3(1, 1, 1);
        }
        InvokeRepeating("SpawnBot", 0, 2f);
        InvokeRepeating("SpawnBomb", 0, 1f);
        InvokeRepeating("SpawnDot", 0.5f, 0.25f);
        leaderboardList.Add(player);
        RefreshLeaderboard();
    }

    void SpawnBot()
    {
        GameObject bot;
        try
        { 
            botSpawner.TryGetNextObject(new Vector3(Random.Range(-mapLimit, mapLimit), 1.5f, Random.Range(-mapLimit, mapLimit)), Quaternion.Euler(new Vector3(0, 0, 0)), out bot);
            bot.transform.localScale = new Vector3(1, 1, 1);
        }
        catch {
            CancelInvoke("SpawnBot");
        }
    }

    public void StartSpawnBot()
    {
        InvokeRepeating("SpawnBot", 2, 2);
    }

    void SpawnBomb()
    {
        GameObject bomb;
        int id = -1;
        if(bombId < checkExist.Length)
        {
            if (checkExist[bombId] == 0)
            {
                id = bombId;
            }
            bombId++;
        }
        else
        {
            CancelInvoke("SpawnBomb");
            id = -1;
        }
        if (id != -1)
        {
            bombSpawner.TryGetNextObject(bombSpawnPoints[id], Quaternion.Euler(new Vector3(0, 0, 0)), out bomb);
            bombList.Add(bomb);
            try
            {
                var bombPos = bomb.transform.position;
                bombPos.y = 1.5f;
                bomb.transform.position = bombPos;
                bomb.name = id.ToString();
                checkExist[id] = 1;
            }
            catch { }
        }
    }

    void SpawnDot()
    {
        try
        {
            GameObject dot;
            dotSpawner.TryGetNextObject(new Vector3(Random.Range(-mapLimit, mapLimit), 1, Random.Range(-mapLimit, mapLimit)), Quaternion.Euler(new Vector3(0, 0, 0)), out dot);
            dot.GetComponent<Renderer>().material.color = dotColor;
            dot.GetComponent<Renderer>().material.SetColor("_EmissionColor", dotColor * 0.5f);
        }
        catch { }
    }

    public void Patch(int id)
    {
        StartCoroutine(delayPatch(id));
    }

    public void RefreshLeaderboard()
    {
        //rankList = leaderboardList.OrderByDescending(x => x.GetComponent<Bot>().botTotalScore).ToList();
        rankList = leaderboardList.ToList();
        GameObject temp;
        for (int j = 0; j <= rankList.Count - 2; j++)
        {
            for (int i = 0; i <= rankList.Count - 2; i++)
            {
                int a;
                int b;
                if(rankList[i].tag == "Bot")
                {
                    a = rankList[i].GetComponent<Bot>().botTotalScore;
                }
                else
                {
                    a = rankList[i].GetComponent<GameController>().totalScore;
                }
                if (rankList[i + 1].tag == "Bot")
                {
                    b = rankList[i + 1].GetComponent<Bot>().botTotalScore;
                }
                else
                {
                    b = rankList[i + 1].GetComponent<GameController>().totalScore;
                }
                if (a < b)
                {
                    temp = rankList[i + 1];
                    rankList[i + 1] = rankList[i];
                    rankList[i] = temp;
                }
            }
        }

        if (rankList[0].tag == "Bot")
        {
            rank1Text.text = "#1   " + rankList[0].GetComponent<Bot>().botTotalScore + "  " + rankList[0].name;
            rank1Text.color = rankList[0].GetComponent<Bot>().botColor;
            rank1Text.transform.parent.GetComponent<Image>().color = new Color32(0, 0, 0, 130);

        }
        else
        {
            rank1Text.text = "#1   " + rankList[0].GetComponent<GameController>().totalScore + "  " + rankList[0].name;
            rank1Text.color = Color.white;
            rank1Text.transform.parent.GetComponent<Image>().color = GameController.instance.playerColor;
        }
        if (rankList[1].tag == "Bot")
        {
            rank2Text.text = "#2   " + rankList[1].GetComponent<Bot>().botTotalScore + "  " + rankList[1].name;
            rank2Text.color = rankList[1].GetComponent<Bot>().botColor;
            rank2Text.transform.parent.GetComponent<Image>().color = new Color32(0, 0, 0, 130);
        }
        else
        {
            rank2Text.text = "#2   " + rankList[1].GetComponent<GameController>().totalScore + "  " + rankList[1].name;
            rank2Text.color = Color.white;
            rank2Text.transform.parent.GetComponent<Image>().color = GameController.instance.playerColor;
        }
        if (rankList[2].tag == "Bot")
        {
            rank3Text.text = "#3   " + rankList[2].GetComponent<Bot>().botTotalScore + "  " + rankList[2].name;
            rank3Text.color = rankList[2].GetComponent<Bot>().botColor;
            rank3Text.transform.parent.GetComponent<Image>().color = new Color32(0, 0, 0, 130);
        }
        else
        {
            rank3Text.text = "#3   " + rankList[2].GetComponent<GameController>().totalScore + "  " + rankList[2].name;
            rank3Text.color = Color.white;
            rank3Text.transform.parent.GetComponent<Image>().color = GameController.instance.playerColor;
        }
        for (int i = 0; i < rankList.Count; i++)
        {
            if (rankList[i].tag == "Bot")
            {
                rankList[i].GetComponent<bl_Hud>().HudInfo.m_Text = (i + 1).ToString();
            }
            else
            {
                currentRankText.text = "#" + (i + 1).ToString() + "   " + rankList[i].GetComponent<GameController>().totalScore + "  " + rankList[i].name;
            }
        }
        crown.transform.parent = rankList[0].transform;
        crown.transform.localPosition = new Vector3(0, 2, 0);
        crown.transform.localScale = rankList[0].transform.localScale;
    }

    IEnumerator delayPatch(int id)
    {
        bool isHold = true;
        var randomTime = Random.Range(3, 10);
        while (true)
        {
            //if (Mathf.Abs(bombSpawnPoints[id].x) == 18 || Mathf.Abs(bombSpawnPoints[id].y) == 18)
            //{
            //    yield return new WaitForSeconds(4);
            //}
            //else
            yield return new WaitForSeconds(randomTime);
            if (bombSpawnPoints[id].x > 0 && bombSpawnPoints[id].y > 0 && zone1 < 1)
            {
                zone1++;
                isHold = false;
                //Debug.Log("zone1:" + zone1);
            }
            else if (bombSpawnPoints[id].x < 0 && bombSpawnPoints[id].y > 0 && zone2 < 1)
            {
                zone2++;
                isHold = false;
                //Debug.Log("zone1:" + zone2);
            }
            else if (bombSpawnPoints[id].x < 0 && bombSpawnPoints[id].y < 0 && zone3 < 1)
            {
                zone3++;
                isHold = false;
                //Debug.Log("zone1:" + zone3);
            }
            else if (bombSpawnPoints[id].x > 0 && bombSpawnPoints[id].y < 0 && zone4 < 1)
            {
                zone4++;
                isHold = false;
                //Debug.Log("zone1:" + zone4);
            }
            if(!isHold)
            {
                GameObject bomb;
                bombSpawner.TryGetNextObject(bombSpawnPoints[id], Quaternion.Euler(new Vector3(0, 0, 0)), out bomb);
                try
                {
                    var bombPos = bomb.transform.position;
                    bombPos.y = 1.5f;
                    bomb.transform.position = bombPos;
                    bomb.name = id.ToString();
                    checkExist[id] = 1;
                }
                catch { }
                break;
            }
        }
    }

    public void skinListUpdate(int id)
    {
        skinList.RemoveAt(id);
        skinList.TrimExcess();
    }
}
