﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HunterPie.Memory;

namespace HunterPie.Core {
    class Player {
        // Player info
        private Int64 LEVEL_ADDRESS;
        private Int64 EQUIPMENT_ADDRESS;
        public int Slot = 0;
        public int Level { get; private set; }
        public string Name { get; private set; }
        public int ZoneID { get; private set; }
        public string ZoneName { get; private set; }
        public int LastZoneID { get; private set; }
        public int WeaponID { get; private set; }
        public string WeaponName { get; private set; }
        public string SessionID { get; private set; }
        public int HarvestedItemsCounter { get; private set; }
        public object[] HarvestBoxFertilizers = new object[4];
        public int PrimaryMantle { get; private set; }
        public float[] PrimaryMantleInfo = new float[4];
        public int SecondaryMantle { get; private set; }
        public float[] SecondaryMantleInfo = new float[4];

        // Threading
        private ThreadStart ScanPlayerInfoRef;
        private Thread ScanPlayerInfo;

        public void StartScanning() {
            ScanPlayerInfoRef = new ThreadStart(GetPlayerInfo);
            ScanPlayerInfo = new Thread(ScanPlayerInfoRef);
            ScanPlayerInfo.Name = "Scanner_Player";
            Debugger.Warn("Initializing Player memory scanner...");
            ScanPlayerInfo.Start();
        }

        private void GetPlayerInfo() {
            while (Scanner.GameIsRunning) {
                GetPlayerLevel();
                GetPlayerName();
                GetZoneId();
                GetWeaponId();
                GetSessionId();
                GetEquipmentAddress();
                GetPrimaryMantle();
                GetSecondaryMantle();
                GetPrimaryMantleTimers();
                GetSecondaryMantleTimers();
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
            GetPlayerInfo();
        }

        private void GetPlayerLevel() {
            Int64 Address = Memory.Address.BASE + Memory.Address.LEVEL_OFFSET;
            Int64[] Offset = new Int64[4] { 0x70, 0x68, 0x8, 0x20 };
            Int64 AddressValue = Scanner.READ_MULTILEVEL_PTR(Address, Offset);
            LEVEL_ADDRESS = AddressValue + 0x108;
            Level = Scanner.READ_INT(LEVEL_ADDRESS);
        }

        private void GetPlayerName() {
            Int64 Address = LEVEL_ADDRESS - 64;
            Name = Scanner.READ_STRING(Address, 32).Trim('\x00');
        }

        private void GetZoneId() {
            Int64 Address = Memory.Address.BASE + Memory.Address.ZONE_OFFSET;
            Int64[] Offset = new Int64[4] { 0x660, 0x28, 0x18, 0x440 };
            Int64 ZoneAddress = Scanner.READ_MULTILEVEL_PTR(Address, Offset);
            ZoneID = Scanner.READ_INT(ZoneAddress + 0x2B0);
        }

        private void GetWeaponId() {
            Int64 Address = Memory.Address.BASE + Memory.Address.WEAPON_OFFSET;
            Int64[] Offset = new Int64[4] { 0x70, 0x5A8, 0x310, 0x148 };
            Address = Scanner.READ_MULTILEVEL_PTR(Address, Offset);
            WeaponID = Scanner.READ_INT(Address+0x2B8);
            WeaponName = GStrings.WeaponName(WeaponID);
        }

        private void GetSessionId() {
            Int64 Address = Memory.Address.BASE + Memory.Address.SESSION_OFFSET;
            Int64[] Offset = new Int64[4] { 0xA0, 0x20, 0x80, 0x9C };
            Address = Scanner.READ_MULTILEVEL_PTR(Address, Offset);
            SessionID = Scanner.READ_STRING(Address+0x3C8, 12);
        }

        private void GetEquipmentAddress() {
            Int64 Address = Memory.Address.BASE + Memory.Address.EQUIPMENT_OFFSET;
            Int64[] Offset = new Int64[4] { 0x78, 0x50, 0x40, 0x450 };
            Address = Scanner.READ_MULTILEVEL_PTR(Address, Offset);
            EQUIPMENT_ADDRESS = Address;
        }

        private void GetPrimaryMantle() {
            Int64 Address = LEVEL_ADDRESS + 0x34;
            PrimaryMantle = Scanner.READ_INT(Address);
        }

        private void GetSecondaryMantle() {
            Int64 Address = LEVEL_ADDRESS + 0x34 + 0x4;
            SecondaryMantle = Scanner.READ_INT(Address);
        }

        private void GetPrimaryMantleTimers() {
            Int64 PrimaryMantleTimerFixed = (PrimaryMantle * 4) + Address.timerFixed;
            Int64 PrimaryMantleTimer = (PrimaryMantle * 4) + Address.timerDynamic;
            Int64 PrimaryMantleCdFixed = (PrimaryMantle * 4) + Address.cooldownFixed;
            Int64 PrimaryMantleCdDynamic = (PrimaryMantle * 4) + Address.cooldownDynamic;
            PrimaryMantleInfo[0] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + PrimaryMantleTimerFixed);
            PrimaryMantleInfo[1] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + PrimaryMantleTimer);
            PrimaryMantleInfo[2] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + PrimaryMantleCdFixed);
            PrimaryMantleInfo[3] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + PrimaryMantleCdDynamic);
        }

        private void GetSecondaryMantleTimers() {
            Int64 SecondaryMantleTimerFixed = (SecondaryMantle * 4) + Address.timerFixed;
            Int64 SecondaryMantleTimer = (SecondaryMantle * 4) + Address.timerDynamic;
            Int64 SecondaryMantleCdFixed = (SecondaryMantle * 4) + Address.cooldownFixed;
            Int64 SecondaryMantleCdDynamic = (SecondaryMantle * 4) + Address.cooldownDynamic;
            SecondaryMantleInfo[0] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + SecondaryMantleTimerFixed);
            SecondaryMantleInfo[1] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + SecondaryMantleTimer);
            SecondaryMantleInfo[2] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + SecondaryMantleCdFixed);
            SecondaryMantleInfo[3] = Scanner.READ_FLOAT(EQUIPMENT_ADDRESS + SecondaryMantleCdDynamic);
        }

    }
}
