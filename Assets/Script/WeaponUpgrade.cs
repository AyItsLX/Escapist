using System.Collections;
using System.Collections.Generic;
using Luminosity.IO.Examples;
using UnityEngine;

public class WeaponUpgrade : MonoBehaviour {
    public int upgradeLimit = 10;

    public PlayerType playerType1;
    public PlayerType playerType2;

    [SerializeField] private int attackspeedTier1 = 1;
    [SerializeField] private int damageTier1 = 1;
    [SerializeField] private int staminaRegenTier1 = 1;
    [SerializeField] private int moveSpeedTier1 = 1;

    //[SerializeField] private int attackspeedTier2 = 1;
    //[SerializeField] private int damageTier2 = 1;
    //[SerializeField] private int staminaRegenTier2 = 1;
    //[SerializeField] private int moveSpeedTier2 = 1;

    void Start () { }

    public void AttackSpeed(int player) {
        if (player == 1) {
            if (attackspeedTier1 < upgradeLimit && GUIManager.GUIManager_Instance.goldCount >= 50) {
                if (playerType1.shootingRate > 0.02f) {
                    GUIManager.GUIManager_Instance.goldCount -= 50;
                    GUIManager.GUIManager_Instance.UpdateGold();
                    playerType1.shootingRate -= 0.01f;
                    attackspeedTier1 += 1;
                    playerType2.shootingRate -= 0.01f;
                }
            }
        }
        //else {
        //    if (attackspeedTier2 < upgradeLimit) {
        //        playerType2.shootingRate -= 0.01f;
        //        attackspeedTier2 += 1;
        //    }
        //}
    }

    public void Damage(int player) {
        if (player == 1) {
            if (damageTier1 < upgradeLimit && GUIManager.GUIManager_Instance.goldCount >= 50) {
                GUIManager.GUIManager_Instance.goldCount -= 50;
                GUIManager.GUIManager_Instance.UpdateGold();
                playerType1.damage += 25;
                damageTier1 += 1;
                playerType2.damage += 25;
            }
        }
        //else {
        //    if (damageTier2 < upgradeLimit) {
        //        playerType2.damage += 25;
        //        damageTier2 += 1;
        //    }
        //}
    }

    public void StaminaRegen(int player) {
        if (player == 1) {
            if (staminaRegenTier1 < upgradeLimit && GUIManager.GUIManager_Instance.experienceCount >= 50) {
                GUIManager.GUIManager_Instance.experienceCount -= 50;
                GUIManager.GUIManager_Instance.UpdateExperience();
                playerType1.staminaRegen += 1;
                staminaRegenTier1 += 1;
                playerType2.staminaRegen += 1;
            }
        }
        //else {
        //    if (staminaRegenTier2 < upgradeLimit) {
        //        playerType2.staminaRegen += 1;
        //        staminaRegenTier2 += 1;
        //    }
        //}
    }

    public void MoveSpeed(int player) {
        if (player == 1) {
            if (staminaRegenTier1 < upgradeLimit && GUIManager.GUIManager_Instance.experienceCount >= 50) {
                GUIManager.GUIManager_Instance.experienceCount -= 50;
                GUIManager.GUIManager_Instance.UpdateExperience();
                playerType1.sprintSpeed += 0.1f;
                staminaRegenTier1 += 1;
                playerType2.sprintSpeed += 0.1f;
            }
        }
        //else {
        //    if (moveSpeedTier2 < upgradeLimit) {
        //        playerType2.movementSpeed += 0.1f;
        //        moveSpeedTier2 += 1;
        //    }
        //}
    }
}
