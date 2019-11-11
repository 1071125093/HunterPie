﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace HunterPie.Core {
    class AutoUpdate {
        private string BranchURI = "https://bitbucket.org/Haato/hunterpie/raw/";
        private string LocalUpdateHash;
        private string OnlineUpdateHash;

        public AutoUpdate(string branch) {
            BranchURI = $"{BranchURI}{branch}/";
        }

        public void checkAutoUpdate() {
            CheckLocalHash();
            CheckOnlineHash();
            if (LocalUpdateHash == OnlineUpdateHash) return;
            DownloadNewUpdater();
        }

        private void CheckLocalHash() {
            if (!File.Exists("Update.exe")) {
                LocalUpdateHash = "";
                return;
            }
            var _file = File.OpenRead("Update.exe");
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] bytes = sha256.ComputeHash(_file);

                StringBuilder builder = new StringBuilder();
                for (int c = 0; c < bytes.Length; c++) {
                    builder.Append(bytes[c].ToString("x2"));
                }
                _file.Close();
                LocalUpdateHash = builder.ToString();
            }
        }

        private void CheckOnlineHash() {
            WebRequest request = WebRequest.Create($"{BranchURI}Update.exe");
            WebResponse r_response = request.GetResponse();
            using (Stream r_content = r_response.GetResponseStream()) {
                using (SHA256 sha256 = SHA256.Create()) {
                    byte[] bytes = sha256.ComputeHash(r_content);

                    StringBuilder builder = new StringBuilder();
                    for (int c = 0; c < bytes.Length; c++) {
                        builder.Append(bytes[c].ToString("x2"));
                    }
                    OnlineUpdateHash = builder.ToString();
                }
            }
        }
        
        private void DownloadNewUpdater() {
            WebRequest xrequest = WebRequest.Create($"{BranchURI}Update.exe");
            WebResponse xr_response = xrequest.GetResponse();
            using (Stream r_content = xr_response.GetResponseStream()) {
                StreamReader content = new StreamReader(r_content);
                MemoryStream ms = new MemoryStream();
                content.BaseStream.CopyTo(ms);
                byte[] contentBytes = ms.ToArray();
                var _file = File.OpenWrite("Update.exe");
                _file.Write(contentBytes, 0, contentBytes.Length);
                _file.Close();
                
            }
        }
    }
}
