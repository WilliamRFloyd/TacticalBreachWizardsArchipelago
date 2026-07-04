using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Wizards.Perks;
using Wizards.SaveSystem;
using TBWArch.Utils;
using System;
using System.Reflection;
using System.IO;
using Wizards.Stages;
using System.Runtime.CompilerServices;
using TBWArch.Archipelago;

namespace TBWArch.SaveSystem
{
    public static class SaveManagerExtensions
    {
        public static void CallSaveNamedSaveBehaviours(IEnumerable<NamedSaveBehaviour> _saveables, string _filename, bool _logAsError = false, bool _copyUnused = true)
        {
			string broadcastChannel = _logAsError ? Dev.errorChannel : "SaveSystemTrace";
			HashSet<string> hashSet = new HashSet<string>();
			string text = _filename + ".bak";
			bool flag = FileTools.MoveOverwrite(_filename, text);
			_saveables = from _s in _saveables
			orderby _s.SaveName
			select _s;
			using (FileWriter fileWriter = new FileWriter(_filename))
			{
				foreach (NamedSaveBehaviour namedSaveBehaviour in _saveables)
				{
					if (hashSet.Contains(namedSaveBehaviour.SaveName))
					{
						Dev.Log(broadcastChannel, new object[]
						{
							"Duplicate SaveName while saving, skipping name:",
							namedSaveBehaviour.SaveName,
							"attached to",
							namedSaveBehaviour.name
						});
					}
					else
					{
						hashSet.Add(namedSaveBehaviour.SaveName);
						namedSaveBehaviour.Save(fileWriter);
					}
				}
				if (_copyUnused && File.Exists(text) && flag)
				{
					using (FileReader fileReader = new FileReader(text))
					{
						while (!fileReader.endOfFile)
						{
							if (string.IsNullOrWhiteSpace(fileReader.currentBlock) || hashSet.Contains(fileReader.currentBlock))
							{
								fileReader.ReadLine();
							}
							else
							{
								Dev.Log(broadcastChannel, new object[]
								{
									"Unexpected SaveName",
									fileReader.currentBlock,
									" in file",
									fileReader.filename,
									"Copying to new file"
								});
								fileReader.CopyBlock(fileReader.currentBlock, fileWriter);
							}
						}
					}
				}
			}
        }
        
        public static bool CallLoadNamedSaveBehaviours(IEnumerable<NamedSaveBehaviour> _saveables, string _filename, bool _logAsError = false, Action<NamedSaveBehaviour> _onLoad = null)
        {
			string broadcastChannel = _logAsError ? Dev.errorChannel : "SaveSystemTrace";
			if (File.Exists(_filename))
			{
				Dictionary<string, NamedSaveBehaviour> dictionary = new Dictionary<string, NamedSaveBehaviour>();
				foreach (NamedSaveBehaviour namedSaveBehaviour in _saveables)
				{
					if (dictionary.ContainsKey(namedSaveBehaviour.SaveName))
					{
						Dev.Log(broadcastChannel, new object[]
						{
							"Duplicate SaveName while loading, skipping name:",
							namedSaveBehaviour.SaveName,
							"attached to",
							namedSaveBehaviour.name
						});
					}
					else
					{
						dictionary.Add(namedSaveBehaviour.SaveName, namedSaveBehaviour);
					}
				}
				using (FileReader fileReader = new FileReader(_filename))
				{
					while (!fileReader.endOfFile)
					{
						string currentBlock = fileReader.currentBlock;
						if (dictionary.ContainsKey(currentBlock))
						{
							DataBlock data = DataBlock.CreateFromFileReader(fileReader);
							dictionary[currentBlock].Load(data);
							if (_onLoad != null)
							{
								_onLoad(dictionary[currentBlock]);
							}
						}
						else
						{
							if (currentBlock != "")
							{
								Dev.Log(broadcastChannel, new object[]
								{
									"No NamedSaveBehaviour with SaveName",
									currentBlock,
									", skipping"
								});
							}
							fileReader.ReadLine();
						}
					}
				}
				foreach (NamedSaveBehaviour namedSaveBehaviour2 in _saveables)
				{
					namedSaveBehaviour2.FinishLoading();
				}
				return true;
			}
			return false;
        }

        extension(SaveManager s)
        {
            public static string archipleagoFile
            {
                get
                {
                    return SaveManager.GetSaveSlotArchipelagoFile();
                }
            }
    
            public static string GetSaveSlotArchipelagoFile(int _slot = 4)
            {
                return ModSaveSystem.GetArchipelagoSaveSlotDirectory() + "Archipelago.txt";
            }

            public void SaveArchipelagoData()
            {
                SaveManagerExtensions.CallSaveNamedSaveBehaviours(Dev.FindObjectsOfType<SaveArchipelagoBehavior>(), SaveManager.archipleagoFile, false, true);
            }

            public bool LoadArchipelagoData()
            {
                return SaveManagerExtensions.CallLoadNamedSaveBehaviours(Dev.FindObjectsOfType<SaveArchipelagoBehavior>(), SaveManager.archipleagoFile, false, null);
            }
        }
    }

    public abstract class SaveArchipelagoBehavior : NamedSaveBehaviour
    {
        public static ConnectionManager connectionManager;

		public static LevelSaveManager levelSaveManager;

        public abstract void ResetProgress();
    }

    public class ConnectionManager : SaveArchipelagoBehavior, ISaveableComponent
    {
        public string serverName { get; set; }

        public string slotName { get; set; }

        public int index { get; set; } = 0;

        public override string SaveName
        {
            get
            {
                return "ConnectionManager";
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (connectionManager == null)
            {
                connectionManager = this;
            }
        }

        public override void ResetProgress()
        {
            this.serverName = "";
            this.slotName = "";
        }

        public void SaveData(IDataBlockRecorder _writer)
        {
            _writer.WriteData("serverName", this.serverName);
            _writer.WriteData("slotName", this.slotName);
            _writer.WriteData("index", this.index);
        }

        public void LoadData(DataBlock _data)
        {
            this.ResetProgress();
            this.serverName = _data.GetValueOrDefault("serverName", "archipelago.gg:");
            this.slotName = _data.GetValueOrDefault("slotName", "");
            this.index = _data.GetIntValueOrDefault("index", 0);
        }

		public void FinishLoadingData()
        {
            ArchipelagoClient.ServerData.Uri = this.serverName;
            ArchipelagoClient.ServerData.SlotName = this.slotName;
            ArchipelagoClient.ServerData.Index = this.index;
        }
    }

	public class LevelSaveManager : SaveArchipelagoBehavior, ISaveableComponent
	{
		public HashSet<string> completedLevels;
        public override string SaveName
		{
			get
			{
				return "LevelSaveManager";
			}
		}

        protected override void Awake()
        {
            base.Awake();
			this.completedLevels = new HashSet<string>();
            if (levelSaveManager == null)
            {
                levelSaveManager = this;
            }
        }

        public override void ResetProgress()
        {
            this.completedLevels.Clear();
        }

        public void SaveData(IDataBlockRecorder _writer)
        {
			_writer.WriteComment("Completed Levels");
			foreach (string value in this.completedLevels)
			{
				_writer.WriteListData("completedLevel", value);
			}
        }

        public void LoadData(DataBlock _data)
        {
            this.ResetProgress();
            if (_data.ContainsKey("completedLevel"))
			{
				foreach (string item in _data.FindList("completedLevel"))
				{
					this.completedLevels.Add(item);
				}
			}
        }

		public void FinishLoadingData()
		{
			
		}
	}
}