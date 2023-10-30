using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<GameObject> PlayerHand_;
    public List<GameObject> PlayerCoins_;
    Vector3 PlayerPos_;
    public int PlayerNum_;
    public bool isFolded = false;

    public Player(Vector3 pPos, int pNum)
    {
        PlayerPos_ = pPos;
        PlayerNum_ = pNum;
        PlayerHand_ = new List<GameObject>();
        PlayerCoins_ = new List<GameObject>();
    }

    private Vector3 firstCardSlot;
    private float startOffset;
    public void DealHand(int handSize, float handSpread)
    {
        // current offset, no cards on top of each other ideally.
        float currOffset = 0.0f - ((handSpread * handSize) / 2);
        startOffset = currOffset;
        // for (i to numcards)
        for (int ii = 0; ii < handSize; ++ii)
        {
            Vector3 cardPos = PlayerPos_;
            cardPos.x = cardPos.x + currOffset + Random.Range(-UIManager.Instance.Drift, UIManager.Instance.Drift);
            cardPos.z += ii;
            cardPos.y += Random.Range(-UIManager.Instance.Drift, UIManager.Instance.Drift);
            firstCardSlot = cardPos;
            
            currOffset += handSpread;
            UIManager.Instance.UIList.MoveIt(UIManager.Instance.Deck[0], cardPos, Random.Range(1.0f, 2.0f), 0.0f, UIManager.Instance.Easing, ii + 1);
            // UIManager.Instance.UIList.RotateIt(UIManager.Instance.Deck[0], Random.Range(-UIManager.Instance.Twist, UIManager.Instance.Twist), Random.Range(1.0f, 2.0f), 0.0f, UIManager.Instance.Easing, ii + 1);

            if (PlayerNum_ == 1)
                UIManager.Instance.UIList.FlipIt(UIManager.Instance.Deck[0], 1.0f, 0.0f, UIManager.Instance.Easing, ii + 1);
            PlayerHand_.Add(UIManager.Instance.Deck[0]);
            UIManager.Instance.Deck.RemoveAt(0);
        }
        isFolded = false;
    }

    public void DealCoins(int countAmount = 10)
    {
        Vector3 chipPos = firstCardSlot;
        float chipOff = startOffset;
        chipPos.y -= 1.6f;
        for (int jj = 0; jj < 10; ++jj)
        {
            chipPos.x = firstCardSlot.x + Random.Range(-UIManager.Instance.Drift, UIManager.Instance.Drift);
            chipPos.y = chipPos.y + Random.Range(-UIManager.Instance.Drift, UIManager.Instance.Drift);
            chipPos.z += jj * 0.10f;
            chipOff += 0.15f;
            UIManager.Instance.UIList.MoveIt(UIManager.Instance.Chips[0], chipPos, Random.Range(1.0f, 2.0f), 0.0f, UIManager.Instance.Easing, jj + 1);
            PlayerCoins_.Add(UIManager.Instance.Chips[0]);
            UIManager.Instance.Chips.RemoveAt(0);
        }
    }

    public void ClearHand()
    {
        int handCount = PlayerHand_.Count;
        for (int ii = 0; ii < handCount; ++ii)
        {
            if (PlayerNum_ == 1)
                UIManager.Instance.UIList.FlipIt(PlayerHand_[0], 1.0f, 0.0f, UIManager.Instance.Easing, ii + 1);
            Vector3 cardPos = new Vector3(-9.0f, -3.5f, PlayerHand_[0].transform.localPosition.z);
            UIManager.Instance.UIList.MoveIt(PlayerHand_[0], cardPos, Random.Range(1.0f, 2.0f), 0.0f, UIManager.Instance.Easing, ii + 1);
            // UIManager.Instance.UIList.RotateIt(PlayerHand_[0], 0, Random.Range(1.0f, 2.0f), 0.0f, UIManager.Instance.Easing, ii + 1);
            UIManager.Instance.Deck.Add(PlayerHand_[0]);
            PlayerHand_.RemoveAt(0);
        }
    }

    public void ClearCoins()
    {
        int jj = 0;
        while (PlayerCoins_.Count > 0)
        {
            UIManager.Instance.UIList.MoveIt(PlayerCoins_[0], new Vector3(9.0f, -3.5f, PlayerCoins_[0].transform.localPosition.z), Random.Range(1.0f, 2.0f), 0.0f, UIManager.Instance.Easing, jj + 1);
            jj++;
            UIManager.Instance.Chips.Add(PlayerCoins_[0]);
            PlayerCoins_.RemoveAt(0);
        }
    }
    public void DoThing()
    {
        if (Random.Range(1, 101) <= 25 || PlayerCoins_.Count <= 0)
        {
            // fold;
            ClearHand();
            int notFoldCount = 0;
            foreach (Player currPlayer in UIManager.Instance.Players)
            {
                if (currPlayer.isFolded == false)
                {
                    notFoldCount++;
                }
            }
            if (notFoldCount > 1)
            {
                isFolded = true;
            }
        }
        else
        {
            if (PlayerCoins_.Count <= 0)
                return;
            int randRan = Random.Range(1, (PlayerCoins_.Count < 5) ? PlayerHand_.Count : 5);
            for (int ii = 0; ii < randRan; ++ii)
            {
                float drift = UIManager.Instance.Drift;
                if (PlayerCoins_.Count <= 0)
                {
                    isFolded = true;
                    break;
                }
                UIManager.Instance.UIList.MoveIt(PlayerCoins_[0], new Vector3(0.0f + (Random.Range(-drift, drift) * 2), 0.25f + (Random.Range(-drift, drift) * 2), PlayerCoins_[0].transform.localPosition.z), 1.0f, 0.0f, UIManager.Instance.Easing, ii + 1);
                UIManager.Instance.Pot.Add(PlayerCoins_[0]);
                PlayerCoins_.RemoveAt(0);
            }
        }
    }
}
