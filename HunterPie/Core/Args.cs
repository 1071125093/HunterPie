﻿using System;
using System.Collections.Generic;

namespace HunterPie.Core {
    /* Abnormalities */
    public class AbnormalityEventArgs : EventArgs {
        public Abnormality Abnormality;

        public AbnormalityEventArgs(Abnormality abnorm) {
            this.Abnormality = abnorm;
        }
    }
    /* Activities */
    public class SteamFuelEventArgs : EventArgs {
        public int Available;
        public int Max;

        public SteamFuelEventArgs(int available, int max) {
            this.Available = available;
            this.Max = max;
        }
    }
    public class DaysLeftEventArgs : EventArgs {
        public byte Days;
        // Generic name
        // Argosy 
        // Tailraiders means
        public bool Modifier; 

        public DaysLeftEventArgs(byte Days, bool Modifier = false) {
            this.Days = Days;
            this.Modifier = Modifier;
        }
    }
    /* Keyboard input */
    public class KeyboardInputEventArgs : EventArgs {
        public int Key { get; private set; }
        public KeyboardHookHelper.KeyboardMessage KeyMessage { get; private set; }
        
        public KeyboardInputEventArgs(int KeyCode, KeyboardHookHelper.KeyboardMessage Message) {
            this.Key = KeyCode;
            this.KeyMessage = Message;
        }

    }
    /* Party and Members */
    public class PartyMemberEventArgs : EventArgs {
        public string Name;
        public int Damage;
        public string Weapon;
        public bool IsInParty;

        public PartyMemberEventArgs(Member m) {
            this.Name = m.Name;
            this.Damage = m.Damage;
            this.Weapon = m.WeaponIconName;
            this.IsInParty = m.IsInParty;
        }
    }

    /* HB and Fertilizers */
    public class FertilizerEventArgs : EventArgs {
        public int ID;
        public string Name;
        public int Amount;

        public FertilizerEventArgs(Fertilizer m) {
            this.ID = m.ID;
            this.Name = m.Name;
            this.Amount = m.Amount;
        }
    }

    public class HarvestBoxEventArgs : EventArgs {
        public int Counter;
        public int Max;

        public HarvestBoxEventArgs(HarvestBox m) {
            this.Counter = m.Counter;
            this.Max = m.Max;
        }

    }

    /* Monster Events */

    public class MonsterPartEventArgs : EventArgs {
        public string Name;
        public int ID;
        public float Health;
        public float TotalHealth;
        public byte BrokenCounter;

        public MonsterPartEventArgs(Part mPart) {
            this.Name = mPart.Name;
            this.ID = mPart.ID;
            this.Health = mPart.Health;
            this.TotalHealth = mPart.TotalHealth;
            this.BrokenCounter = mPart.BrokenCounter;
        }

    }

    public class MonsterSpawnEventArgs : EventArgs {
        public string Name;
        public string ID;
        public string Crown;
        public float CurrentHP;
        public float TotalHP;
        public bool isTarget;
        public Dictionary<string, int> Weaknesses;

        public MonsterSpawnEventArgs(Monster m) {
            this.Name = m.Name;
            this.ID = m.ID;
            this.Crown = m.Crown;
            this.CurrentHP = m.CurrentHP;
            this.TotalHP = m.TotalHP;
            this.isTarget = m.isTarget;
            this.Weaknesses = m.Weaknesses;
        }
    }
    
    public class MonsterUpdateEventArgs : EventArgs {
        public float CurrentHP;
        public float TotalHP;
        public float Enrage;

        public MonsterUpdateEventArgs(Monster m) {
            this.CurrentHP = m.CurrentHP;
            this.TotalHP = m.TotalHP;
            this.Enrage = m.EnrageTimer;
        }
    }

}
