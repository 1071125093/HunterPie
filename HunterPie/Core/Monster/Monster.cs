﻿using System;
using System.Threading;
using HunterPie.Memory;
using HunterPie.Logger;
using System.Collections.Generic;

namespace HunterPie.Core {
    public class Monster {
        // Private vars
        private string _name;
        private float _currentHP;
        private bool _isTarget;
        private float _enrageTimer;
        private float _enrageStaticTimer;

        // Monster basic info
        private int MonsterNumber;
        public string Name {
            get { return _name; }
            set {
                if (value != null && _name != value) {
                    if (CurrentHP > 0) {
                        _name = value;
                        // Only call this if monster is actually alive
                        GetMonsterWeaknesses();
                        _onMonsterSpawn();
                    }
                } else if (value == null && _name != value) {
                    _name = value;
                    _onMonsterDespawn();
                }
            }
        }
        public string ID { get; private set; }
        public float TotalHP { get; private set; }
        public float CurrentHP {
            get { return _currentHP; }
            set {
                if (value != _currentHP) {
                    _currentHP = value;
                    _onHPUpdate();
                    if (value <= 0) {
                        _onMonsterDeath();
                    }
                }
            }
        }
        public Dictionary<string, int> Weaknesses;
        public float HPPercentage { get; private set; } = 1;
        public bool isTarget {
            get { return _isTarget; }
            set {
                if (value != _isTarget) {
                    _isTarget = value;
                    _onTargetted();
                }
            }
        }
        public float EnrageTimer {
            get { return _enrageTimer; }
            set {
                if (value != _enrageTimer) {
                    if (value > 0 && _enrageTimer == 0) {
                        _onEnrage();
                    } else if (value == 0 && _enrageTimer > 0) {
                        _onUnenrage();
                    }
                    _enrageTimer = value;
                }
            }
        }
        public bool isEnraged {
            get { return _enrageTimer > 0; }
        }
        private Int64 MonsterAddress;

        // Threading
        ThreadStart MonsterInfoScanRef;
        Thread MonsterInfoScan;

        // Game events
        public delegate void MonsterEnrageEvents(object source, EventArgs args);
        public delegate void MonsterEvents(object source, MonsterEventArgs args);
        public event MonsterEvents OnMonsterSpawn;
        public event MonsterEvents OnMonsterDespawn;
        public event MonsterEvents OnMonsterDeath;
        public event MonsterEvents OnHPUpdate;
        public event MonsterEvents OnTargetted;
        public event MonsterEnrageEvents OnEnrage;
        public event MonsterEnrageEvents OnUnenrage;
        

        protected virtual void _onMonsterSpawn() {
            MonsterEventArgs args = new MonsterEventArgs(this);
            OnMonsterSpawn?.Invoke(this, args);
        }

        protected virtual void _onMonsterDespawn() {
            MonsterEventArgs args = new MonsterEventArgs(this);
            OnMonsterDespawn?.Invoke(this, args);
        }

        protected virtual void _onMonsterDeath() {
            MonsterEventArgs args = new MonsterEventArgs(this);
            OnMonsterDeath?.Invoke(this, args);
        }

        protected virtual void _onHPUpdate() {
            MonsterEventArgs args = new MonsterEventArgs(this);
            OnHPUpdate?.Invoke(this, args);
        }

        protected virtual void _onTargetted() {
            MonsterEventArgs args = new MonsterEventArgs(this);
            OnTargetted?.Invoke(this, args);
        }

        protected virtual void _onEnrage() {
            OnEnrage?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void _onUnenrage() {
            OnUnenrage?.Invoke(this, EventArgs.Empty);
        }

        public Monster(int initMonsterNumber) {
            MonsterNumber = initMonsterNumber;
        }

        public void StartThreadingScan() {
            MonsterInfoScanRef = new ThreadStart(ScanMonsterInfo);
            MonsterInfoScan = new Thread(MonsterInfoScanRef);
            MonsterInfoScan.Name = $"Scanner_Monster.{MonsterNumber}";
            Debugger.Warn($"Initializing monster({MonsterNumber}) scanner...");
            MonsterInfoScan.Start();
        }

        public void StopThread() {
            MonsterInfoScan.Abort();
        }

        /* Debugging purposes
        private void SimulateMonster() {
            this.ID = "em002_00";
            this.Name = "Rathalos";
            this.CurrentHP = 19008.5f;
            this.TotalHP = 19008.5f;
            int x = 0;
            while (true) {
                this.ID = "em002_00";
                this.Name = "Rathalos";
                _onMonsterSpawn();
                CurrentHP -= 11;
                x++;
                if (x == 100) EnrageTimer = 120;
                Thread.Sleep(200);
            }
        }*/

        private void ScanMonsterInfo() {
            //SimulateMonster(); // Debugging purposes
            while (Scanner.GameIsRunning) {
                GetMonsterAddress();
                GetMonsterIDAndName();
                GetMonsterHp();
                GetMonsterEnrageTimer();
                Thread.Sleep(150);
            }
            Thread.Sleep(1000);
            ScanMonsterInfo();
        }

        private void GetMonsterAddress() {
            Int64 Address = Memory.Address.BASE + Memory.Address.MONSTER_OFFSET;
            // This will give us the third monster's address, so we can find the second and first monster with it
            Int64 ThirdMonsterAddress = Scanner.READ_MULTILEVEL_PTR(Address, Memory.Address.Offsets.MonsterOffsets);
            ThirdMonsterAddress = Scanner.READ_LONGLONG(ThirdMonsterAddress);
            if (MonsterNumber == 3) {
                //if (ThirdMonsterAddress != MonsterAddress) Debugger.Log($"Found 3rd Monster address -> 0x{ThirdMonsterAddress:X}");
                MonsterAddress = ThirdMonsterAddress;
            } else if (MonsterNumber == 2) {
                Int64 SecondMonsterAddress = Scanner.READ_LONGLONG(ThirdMonsterAddress + Memory.Address.Offsets.NextMonsterPtr);
                //if (SecondMonsterAddress != MonsterAddress) Debugger.Log($"Found 2nd Monster address -> 0x{SecondMonsterAddress:X}");
                MonsterAddress = SecondMonsterAddress;
            } else {
                Int64 FirstMonsterAddress = Scanner.READ_LONGLONG(Scanner.READ_LONGLONG(ThirdMonsterAddress + Memory.Address.Offsets.NextMonsterPtr) + Memory.Address.Offsets.NextMonsterPtr);
                //if (FirstMonsterAddress != MonsterAddress) Debugger.Log($"Found 1st Monster address -> 0x{FirstMonsterAddress:X}");
                MonsterAddress = FirstMonsterAddress;
            }
        }

        private void GetMonsterHp() {
            Int64 MonsterHPComponent = Scanner.READ_LONGLONG(this.MonsterAddress + Memory.Address.Offsets.MonsterHPComponentOffset);
            Int64 MonsterTotalHPAddress = MonsterHPComponent + 0x60;
            Int64 MonsterCurrentHPAddress = MonsterTotalHPAddress + 0x4;
            float f_TotalHP = Scanner.READ_FLOAT(MonsterTotalHPAddress);
            float f_CurrentHP = Scanner.READ_FLOAT(MonsterCurrentHPAddress);

            if ((this.ID != null) && f_CurrentHP <= f_TotalHP && f_CurrentHP > 0 && !this.ID.StartsWith("ems")) {
                this.TotalHP = f_TotalHP;
                this.CurrentHP = f_CurrentHP;
                this.HPPercentage = f_CurrentHP / f_TotalHP == 0 ? 1 : f_CurrentHP / f_TotalHP;
            } else {
                this.TotalHP = 0.0f;
                this.CurrentHP = 0.0f;
                this.HPPercentage = 1f;
            }
        }

        private void GetMonsterIDAndName() {
            Int64 NamePtr = Scanner.READ_LONGLONG(this.MonsterAddress + Address.Offsets.MonsterNamePtr);
            string MonsterId = Scanner.READ_STRING(NamePtr + 0x0c, 64).Replace("\x00", "");
            if (MonsterId != "") {
                try {
                    string ActualID = MonsterId.Split('\\')[4];
                    if (GStrings.GetMonsterNameByID(ActualID) != null) {
                        if (ActualID != this.ID) Debugger.Log($"Found new monster #{MonsterNumber} address -> 0x{MonsterAddress:X}");
                        this.ID = ActualID;
                        this.Name = GStrings.GetMonsterNameByID(this.ID);
                    } else {
                        this.ID = null;
                        this.Name = null;
                    }
                } catch {
                    this.ID = null;
                    this.Name = null;
                }
            } else {
                this.ID = null;
                this.Name = null;
            }
        }

        private void GetMonsterWeaknesses() {
            Weaknesses = MonsterData.GetMonsterWeaknessById(this.ID);
        }

        private void GetMonsterEnrageTimer() {
            EnrageTimer = Scanner.READ_FLOAT(MonsterAddress + 0x1BE2C);
        }

    }
}
