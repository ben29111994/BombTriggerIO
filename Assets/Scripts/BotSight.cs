using System.Collections.Generic;
using UnityEngine;

public class BotSight : MonoBehaviour
{
    public GameObject bot;
    public List<GameObject> opponents = new List<GameObject>();
    public List<GameObject> dots = new List<GameObject>();
    bool isComeToBomb = false;
    GameObject currentBombTarget;
    float countDetectRate = 0;
    public Spawner spawner;

    private void OnEnable()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();
    }

    private void Update()
    {
        if(countDetectRate > 0)
        {
            countDetectRate -= 0.02f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Bot")
        {
            opponents.Add(other.gameObject);
            if(isComeToBomb)
            {
                try
                {
                    float decision = 0;
                    foreach (var item in opponents)
                    {
                        if (Vector3.Distance(item.transform.position, currentBombTarget.transform.position) >= Vector3.Distance(bot.transform.position, currentBombTarget.transform.position) /*|| item.transform.localScale.x / bot.transform.localScale.x <= 2*/)
                        {
                            decision = 1;
                            break;
                        }
                    }

                    if (decision > 0)
                    {
                        bot.GetComponent<Bot>().StopInvoke();
                        bot.GetComponent<Bot>().ChangeDir(currentBombTarget.transform.position);
                        isComeToBomb = true;
                    }
                    else
                    {
                        var runAway = transform.InverseTransformDirection(currentBombTarget.transform.position * 100);
                        runAway.y = bot.transform.position.y;
                        bot.GetComponent<Bot>().StopInvoke();
                        bot.GetComponent<Bot>().ChangeDir(runAway);
                    }
                }
                catch { }
            }
            else if(other.tag == "Player")
            {
                bot.GetComponent<Bot>().StopInvoke();
                bot.GetComponent<Bot>().ChangeDir(other.transform.position);
            }
        }

        //if (other.tag == "Dot")
        //{
        //    dots.Add(other.gameObject);
        //    if (isComeToBomb)
        //    {
        //        try
        //        {
        //            float decision = 0;
        //            foreach (var item in dots)
        //            {
        //                if (Vector3.Distance(item.transform.position, currentBombTarget.transform.position) < 5)
        //                {
        //                    decision = 1;
        //                    break;
        //                }
        //            }

        //            if (decision > 0)
        //            {
        //                bot.GetComponent<Bot>().StopInvoke();
        //                bot.GetComponent<Bot>().ChangeDir(currentBombTarget.transform.position);
        //                isComeToBomb = true;
        //            }
        //            else
        //            {
        //                var runAway = transform.InverseTransformDirection(currentBombTarget.transform.position * 100);
        //                runAway.y = bot.transform.position.y;
        //                bot.GetComponent<Bot>().StopInvoke();
        //                bot.GetComponent<Bot>().ChangeDir(runAway);
        //            }
        //        }
        //        catch { }
        //    }
        //}

        if (other.tag == "Wall")
        {
            bot.GetComponent<Bot>().StopInvoke();
            bot.GetComponent<Bot>().CheckWall(true);
            bot.GetComponent<Bot>().ChangeDir(new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10)));
            //bot.GetComponent<Bot>().StartInvoke(1f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && countDetectRate <= 0 && ((other == spawner.rankList[0] || other == spawner.rankList[1] || other == spawner.rankList[2])))
        {
            bot.GetComponent<Bot>().StopInvoke();
            bot.GetComponent<Bot>().ChangeDir(other.transform.position);
            countDetectRate += 0.5f;
        }

        if (other.tag == "MovingBomb" && countDetectRate <= 0)
        {
            //if (dots.Count > 0 && opponents.Count == 0 && other.GetComponent<Bomb>().exceptionCount > 0)
            //{
            //    float decision = 0;
            //    foreach (var item in dots)
            //    {
            //        if (Vector3.Distance(item.transform.position, other.transform.position) < 5)
            //        {
            //            decision = 1;
            //            break;
            //        }
            //    }

            //    if (decision > 0)
            //    {
            //        bot.GetComponent<Bot>().StopInvoke();
            //        bot.GetComponent<Bot>().ChangeDir(other.transform.position);
            //        isComeToBomb = true;
            //        currentBombTarget = other.gameObject;
            //    }
            //    else
            //    {
            //        var runAway = transform.InverseTransformDirection(other.transform.position * 100);
            //        runAway.y = bot.transform.position.y;
            //        bot.GetComponent<Bot>().StopInvoke();
            //        bot.GetComponent<Bot>().ChangeDir(runAway);
            //        bot.GetComponent<Bot>().StartInvoke(0.5f);
            //    }
            //}
            if (other.GetComponent<Bomb>().exceptionCount <= 0)
            {
                var runAway = transform.InverseTransformDirection(other.transform.position * 100);
                runAway.y = bot.transform.position.y;
                bot.GetComponent<Bot>().StopInvoke();
                bot.GetComponent<Bot>().ChangeDir(runAway);
                bot.GetComponent<Bot>().StartInvoke(1f);
            }
            else if (opponents.Count > 0 && other.GetComponent<Bomb>().exceptionCount > 0)
            {
                float decision = 0;
                foreach (var item in opponents)
                {
                    if ((item == spawner.rankList[0] || item == spawner.rankList[1] || item == spawner.rankList[2]) && item.tag == "Bot")
                    {
                        break;
                    }
                    else if (Vector3.Distance(item.transform.position, other.transform.position) >= Vector3.Distance(bot.transform.position, other.transform.position) /*|| item.transform.localScale.x / bot.transform.localScale.x <= 2*/)
                    {
                        decision = 1;
                        break;
                    }
                }

                if (decision > 0)
                {
                    bot.GetComponent<Bot>().StopInvoke();
                    bot.GetComponent<Bot>().ChangeDir(other.transform.position);
                    isComeToBomb = true;
                    currentBombTarget = other.gameObject;
                }
                else
                {
                    var runAway = transform.InverseTransformDirection(other.transform.position * 100);
                    runAway.y = bot.transform.position.y;
                    bot.GetComponent<Bot>().StopInvoke();
                    bot.GetComponent<Bot>().ChangeDir(runAway);
                    bot.GetComponent<Bot>().StartInvoke(1f);
                }
            }
            else
            {
                bot.GetComponent<Bot>().StopInvoke();
                bot.GetComponent<Bot>().ChangeDir(other.transform.position);
                isComeToBomb = true;
                currentBombTarget = other.gameObject;
            }
            countDetectRate += 0.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Bot")
        {
            float decision = 0;
            if (opponents.Count > 0)
            {
                try
                {
                    foreach (var item in opponents)
                    {
                        if (Vector3.Distance(item.transform.position, currentBombTarget.transform.position) >= Vector3.Distance(bot.transform.position, currentBombTarget.transform.position) /*|| item.transform.localScale.x / bot.transform.localScale.x <= 2*/)
                        {
                            decision = 1;
                            break;
                        }
                    }

                    if (decision > 0)
                    {
                        bot.GetComponent<Bot>().StopInvoke();
                        bot.GetComponent<Bot>().ChangeDir(currentBombTarget.transform.position);
                        isComeToBomb = true;
                    }
                    else
                    {
                        var runAway = transform.InverseTransformDirection(currentBombTarget.transform.position * 100);
                        runAway.y = bot.transform.position.y;
                        bot.GetComponent<Bot>().StopInvoke();
                        bot.GetComponent<Bot>().ChangeDir(runAway);
                        bot.GetComponent<Bot>().StartInvoke(1f);
                    }
                }
                catch { }
            }

            opponents.Remove(other.gameObject);
            opponents.TrimExcess();
        }

        //if (other.tag == "Dot")
        //{
        //    float decision = 0;
        //    if (dots.Count > 0)
        //    {
        //        try
        //        {
        //            foreach (var item in dots)
        //            {

        //                if (Vector3.Distance(item.transform.position, currentBombTarget.transform.position) < 5)
        //                {
        //                    decision = 1;
        //                    break;
        //                }
        //            }

        //            if (decision > 0)
        //            {
        //                bot.GetComponent<Bot>().StopInvoke();
        //                bot.GetComponent<Bot>().ChangeDir(currentBombTarget.transform.position);
        //                isComeToBomb = true;
        //            }
        //            else
        //            {
        //                var runAway = transform.InverseTransformDirection(currentBombTarget.transform.position * 100);
        //                runAway.y = bot.transform.position.y;
        //                bot.GetComponent<Bot>().StopInvoke();
        //                bot.GetComponent<Bot>().ChangeDir(runAway);
        //                bot.GetComponent<Bot>().StartInvoke(1f);
        //            }
        //        }
        //        catch { }

        //        dots.Remove(other.gameObject);
        //        dots.TrimExcess();
        //    }
        //}

        if (other.tag == "Wall")
        {
            bot.GetComponent<Bot>().CheckWall(false);
            bot.GetComponent<Bot>().StartInvoke(1f);
        }

        //if (other.tag == "MovingBomb")
        //{
        //    isComeToBomb = false;

        //    if (other.GetComponent<Bomb>().exceptionCount > 0)
        //    {
        //        bot.GetComponent<Bot>().StopInvoke();
        //        bot.GetComponent<Bot>().ChangeDir(other.transform.position);
        //        isComeToBomb = true;
        //    }
        //}
    }

    public void ChangeStatus(bool isBombHeading)
    {
        bot.GetComponent<Bot>().MeetBomb(isBombHeading);
        isComeToBomb = isBombHeading;
    }
}
