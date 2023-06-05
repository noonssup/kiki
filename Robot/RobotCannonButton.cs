using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotCannonButton : MonoBehaviour
{
    [SerializeField] RobotPlayerMoveController playerCtrl;
    [SerializeField] List<RobotWeaponFireController> cannonCtrl;

    [SerializeField] Image coolTimeIamge;


    
    private void OnEnable()
    {
        //if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
        playerCtrl = FindObjectOfType<RobotPlayerMoveController>();
        cannonCtrl = new List<RobotWeaponFireController>();
        ButtonSetup();
    }

    void ButtonSetup()
    {
        RobotWeaponFireController[] _allFireCtrl = FindObjectsOfType<RobotWeaponFireController>();
        foreach(RobotWeaponFireController c in _allFireCtrl)
        {
            if (c.gameObject.name == "Cannon(Clone)" && c.gameObject.layer == LayerMask.NameToLayer("PLAYER"))
            {
                if (cannonCtrl.Contains(c)) continue;
                else cannonCtrl.Add(c);
            }
        }

        if (cannonCtrl.Count <= 0) this.gameObject.SetActive(false);
        //coolTimeIamge = this.transform.GetComponentInChildren<Image>();
    }

    public void CannonFire()
    {
        foreach (RobotWeaponFireController c in cannonCtrl)
        {
            c.CannonFire();
        }
    }

    private void Update()
    {
        if(coolTimeIamge != null)
        {
            coolTimeIamge.fillAmount = 1 - (cannonCtrl[0].fireDelay / cannonCtrl[0].coolTime);
        }
    }
}
