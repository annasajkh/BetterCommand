using IL.RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BetterCommand.Source.DataStructures
{
    public class PlayerHealthData(GameObject player, float currentHealth)
    {
        public GameObject Player { get; private set; } = player;
        public float CurrentHealth { get; private set; } = currentHealth;
    }
}