using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotGunButton : MonoBehaviour
{
    [SerializeField] RobotPlayerMoveController playerCtrl;
    [SerializeField] List<RobotWeaponFireController> autogunCtrl;

    [SerializeField] Image coolTimeIamge;



    private void OnEnable()
    {
        playerCtrl = FindObjectOfType<RobotPlayerMoveController>();
        autogunCtrl = new List<RobotWeaponFireController>();
        ButtonSetup();
    }

    void ButtonSetup()
    {
        RobotWeaponFireController[] _allFireCtrl = FindObjectsOfType<RobotWeaponFireController>();
        foreach (RobotWeaponFireController c in _allFireCtrl)
        {
            if (c.gameObject.name == "AutoGun(Clone)" && c.gameObject.layer == LayerMask.NameToLayer("PLAYER"))
            {
                if (autogunCtrl.Contains(c)) continue;
                else autogunCtrl.Add(c);
            }
        }

        if (autogunCtrl.Count <= 0) this.gameObject.SetActive(false);
        //coolTimeIamge = this.transform.GetComponentInChildren<Image>();
    }

    public void GunFire()
    {
        foreach (RobotWeaponFireController c in autogunCtrl)
        {
            c.GunFire();
        }
    }

    private void Update()
    {
        if (coolTimeIamge != null)
        {
            coolTimeIamge.fillAmount = 1 - (autogunCtrl[0].fireDelay / autogunCtrl[0].coolTime);
        }
    }
}
