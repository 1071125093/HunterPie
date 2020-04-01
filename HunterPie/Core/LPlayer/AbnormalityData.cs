﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HunterPie.Logger;
using HunterPie.Properties;

namespace HunterPie.Core {
    public class AbnormalityData {
        private static XmlDocument AbnormalitiesData;

        static public void LoadAbnormalityData() {
            AbnormalitiesData = new XmlDocument();
            AbnormalitiesData.LoadXml(Resources.AbnormalityData);
            Debugger.Warn(GStrings.GetLocalizationByXPath("/Console/String[@ID='MESSAGE_ABNORMALITIES_DATA_LOAD']"));
        }

        static public XmlNodeList GetHuntingHornAbnormalities() {
            XmlNodeList HuntingHornAbnormalitiesData = AbnormalitiesData.SelectNodes("//Abnormalities/HUNTINGHORN_Abnormalities/Abnormality");
            return HuntingHornAbnormalitiesData;
        }

        static public XmlNodeList GetPalicoAbnormalities() {
            XmlNodeList PalicoAbnormalitiesData = AbnormalitiesData.SelectNodes("//Abnormalities/PALICO_Abnormalities/Abnormality");
            return PalicoAbnormalitiesData;
        }

        static public XmlNodeList GetBlightAbnormalities() {
            XmlNodeList BlightAbnormalitiesData = AbnormalitiesData.SelectNodes("//Abnormalities/DEBUFF_Abnormalities/Abnormality");
            return BlightAbnormalitiesData;
        }

        static public XmlNodeList GetMiscAbnormalities() {
            XmlNodeList BlightAbnormalitiesData = AbnormalitiesData.SelectNodes("//Abnormalities/MISC_Abnormalities/Abnormality");
            return BlightAbnormalitiesData;
        }

        static public string GetAbnormalityIconByID(string Type, int ID) {
            XmlNode Abnormality = AbnormalitiesData.SelectSingleNode($"//Abnormalities/{Type}_Abnormalities/Abnormality[@ID='{ID}']");
            string IconName = Abnormality?.Attributes["Icon"].Value;
            IconName = string.IsNullOrEmpty(IconName) ? "ICON_MISSING" : IconName;
            return IconName;
        }

    }
}
