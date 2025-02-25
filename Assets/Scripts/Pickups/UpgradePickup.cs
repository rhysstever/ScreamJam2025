using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UpgradeTier {
    Weak,
    Average,
    Strong
}

public class UpgradePickup : MonoBehaviour {
    [SerializeField]
    private TMP_Text soulsCostText;
    [SerializeField]
    private SpriteRenderer buffedStatSprite, nerfedStatSprite;
    [SerializeField]
    private GameObject activeArrowSet, weakArrowSetPrefab, averageArrowSetPrefab, strongArrowSetPrefab;
    [SerializeField]
    private UpgradeTier tier;
    [SerializeField]
    private Stat buffedStat, nerfedStat;

    private float buffedStatAmount, nerfedStatAmount;
    private int cost;

    /// <summary>
    /// Randomize the effected stats from picking up this upgrade
    /// </summary>
    /// <param name="tier">The intensity of the upgrade</param>
    public void RandomizeStats(UpgradeTier tier) {
        // Get a random stat to buff
        string[] statNames = Enum.GetNames(typeof(Stat));
        Stat randomStat = (Stat)UnityEngine.Random.Range(0, statNames.Length);

        // Get a second, unique stat to nerf
        List<Stat> remainingStats = new List<Stat>();
        for(int i = 0; i < statNames.Length; i++) {
            Stat stat = (Stat)i;
            if(stat != randomStat) {
                remainingStats.Add(stat);
            }
        }
        Stat randomStat2 = remainingStats[UnityEngine.Random.Range(0, remainingStats.Count)];

        // Set this component's values based on the tier and generated stats
        this.tier = tier;
        cost = RoomManager.instance.TierSoulCostMap[tier];
        soulsCostText.text = string.Format("x {0}", cost);

        switch(tier) {
            case UpgradeTier.Weak:
                Instantiate(weakArrowSetPrefab, activeArrowSet.transform);
                break;
            case UpgradeTier.Average:
                Instantiate(averageArrowSetPrefab, activeArrowSet.transform);
                break;
            case UpgradeTier.Strong:
                Instantiate(strongArrowSetPrefab, activeArrowSet.transform);
                break;
        }

        buffedStat = randomStat;
        buffedStatAmount = RoomManager.instance.TierStatAmountMap[tier].Item1;
        buffedStatSprite.sprite = RoomManager.instance.StatSpriteMap[buffedStat];
        nerfedStat = randomStat2;
        nerfedStatAmount = RoomManager.instance.TierStatAmountMap[tier].Item2;
        nerfedStatSprite.sprite = RoomManager.instance.StatSpriteMap[nerfedStat];
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider != null) {
            if(collision.gameObject.tag == "Player") {
                Player player = collision.gameObject.GetComponent<Player>();
                if(player.SoulsCount >= cost) {
                    // If the player collides with the upgrade, buff and nerf the player in the corresponding stats
                    collision.gameObject.GetComponent<Player>().Buff(buffedStat, buffedStatAmount);
                    collision.gameObject.GetComponent<Player>().Buff(nerfedStat, nerfedStatAmount);
                    player.SpendSouls(cost);
                    Destroy(gameObject);
                } else {
                    Debug.Log("Not Enough Souls");
                }
            }
        }
    }
}
