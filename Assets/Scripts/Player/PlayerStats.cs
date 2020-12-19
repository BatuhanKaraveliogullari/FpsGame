using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public override void Die()
    {
        base.Die();

        if(GameManager.instance != null)
        {
            GameManager.instance.isDied = true;

            GameManager.instance.isStarted = false;

            GameManager.instance.EndGameControl(GameManager.instance.timer, GameManager.instance.killedEnemy);
        }
    }
}
